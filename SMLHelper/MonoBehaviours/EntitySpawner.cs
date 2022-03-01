namespace SMLHelper.V2.MonoBehaviours
{
    using System.Collections;
    using Handlers;
    using Patchers;
    using UnityEngine;
    using UWE;
    using Logger = Logger;

    internal class EntitySpawner : MonoBehaviour
    {
        internal SpawnInfo spawnInfo;

        void Start()
        {
            StartCoroutine(SpawnAsync());
        }

        IEnumerator SpawnAsync()
        {
            var stringToLog = spawnInfo.Type switch
            {
                SpawnInfo.SpawnType.ClassId => spawnInfo.ClassId,
                _ => spawnInfo.TechType.AsString()
            };

            var task = new TaskResult<GameObject>();
            yield return GetPrefabAsync(task);

            var prefab = task.Get();

            if (prefab == null)
            {
                Logger.Error($"no prefab found for {stringToLog}; process for Coordinated Spawn canceled.");
                Destroy(gameObject);
            }


            var obj = Utils.InstantiateDeactivated(prefab, spawnInfo.SpawnPosition, spawnInfo.Rotation);

            var lwe = obj.GetComponent<LargeWorldEntity>();
            
            var lws = LargeWorldStreamer.main;
            yield return new WaitUntil(() => lws != null && lws.IsReady()); // first we make sure the world streamer is initialized

            // non-global objects cannot be spawned in unloaded terrain so we need to wait
            if (lwe is {cellLevel: not (LargeWorldEntity.CellLevel.Batch or LargeWorldEntity.CellLevel.Global)})
            {
                var batch = lws.GetContainingBatch(spawnInfo.SpawnPosition);
                yield return new WaitUntil(() => lws.IsBatchReadyToCompile(batch)); // then we wait until the terrain is fully loaded (must be checked on each frame for faster spawns)
            }

            var lw = LargeWorld.main;

            yield return new WaitUntil(() => lw != null && lw.streamer.globalRoot != null); // need to make sure global root is ready too for global spawns.
            
            lw.streamer.cellManager.RegisterEntity(obj);

            obj.SetActive(true);

            LargeWorldStreamerPatcher.savedSpawnInfos.Add(spawnInfo);

            Destroy(gameObject);
        }

        IEnumerator GetPrefabAsync(IOut<GameObject> gameObject)
        {
            GameObject obj;

            if (spawnInfo.Type == SpawnInfo.SpawnType.ClassId) // Spawn is via ClassID.
            {
                var request = PrefabDatabase.GetPrefabAsync(spawnInfo.ClassId);
                yield return request;

                request.TryGetPrefab(out obj);
            }
            else // spawn is via TechType.
            {
                var task = CraftData.GetPrefabForTechTypeAsync(spawnInfo.TechType);
                yield return task;

                obj = task.GetResult();
            }

            gameObject.Set(obj);
        }
    }
}

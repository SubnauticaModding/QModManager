namespace SMLHelper.V2.Assets
{
    using System.Collections;
    using UnityEngine;
    using UWE;

    // request for getting ModPrefab asynchronously
    internal class ModPrefabRequest: IPrefabRequest, IEnumerator
    {
        private readonly ModPrefab modPrefab;

        private int state = 0;
        private CoroutineTask<GameObject> task;
        private TaskResult<GameObject> taskResult;

        public ModPrefabRequest(ModPrefab modPrefab)
        {
            this.modPrefab = modPrefab;
        }

        private void Init()
        {
            if (task != null)
                return;

            taskResult = new TaskResult<GameObject>();
            task = new CoroutineTask<GameObject>(modPrefab.GetGameObjectInternalAsync(taskResult), taskResult);
        }

        public object Current
        {
            get
            {
                Init();
                return task;
            }
        }

        public bool TryGetPrefab(out GameObject result)
        {
            result = taskResult.Get();
            return result != null;
        }

        public bool MoveNext()
        {
            Init();
            return state++ == 0;
        }

        public void Reset() {}

#if BELOWZERO || SUBNAUTICA_EXP
        public void Release()
        {
        }
#endif
    }
}

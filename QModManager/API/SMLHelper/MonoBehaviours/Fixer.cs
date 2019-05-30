namespace QModManager.API.SMLHelper.MonoBehaviours
{
    using Harmony;
    using System.Reflection;
    using UnityEngine;
    using Logger = QModManager.Utility.Logger;

    internal class Fixer : MonoBehaviour, IProtoEventListener
    {
        [SerializeField]
        public TechType techType;

        [SerializeField]
        public string ClassId;

        private float time;
        private bool initalized;

        private static FieldInfo BuilderPrefab = AccessTools.Field(typeof(Builder), "prefab");

        private void Update()
        {
            if (!initalized)
            {
                time = Time.time + 1f;
                initalized = true;
            }

            GameObject prefab = (GameObject)BuilderPrefab.GetValue(null);

            if (transform.position == new Vector3(-5000, -5000, -5000) && gameObject != prefab && Time.time > time)
            {
                Logger.Debug("Destroying object: " + gameObject);
                Destroy(gameObject);
            }
        }

        public void OnProtoSerialize(ProtobufSerializer serializer) { }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            Constructable constructable = GetComponent<Constructable>();
            if (constructable != null)
            {
                constructable.techType = techType;
            }

            TechTag techTag = GetComponent<TechTag>();
            if (techTag != null)
            {
                techTag.type = techType;
            }
        }
    }
}

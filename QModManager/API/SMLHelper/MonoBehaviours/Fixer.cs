namespace SMLHelper.V2.MonoBehaviours
{
    using UnityEngine;
    using System.Reflection;
    using Logger = V2.Logger;

    public class Fixer : MonoBehaviour, IProtoEventListener
    {
        [SerializeField]
        public TechType techType;

        [SerializeField]
        public string ClassId;

        private float time;
        private bool initalized;

        private static FieldInfo BuilderPrefab = typeof(Builder).GetField("prefab", BindingFlags.NonPublic | BindingFlags.Static);

        private void Update()
        {
            if (!initalized)
            {
                time = Time.time + 1f;
                initalized = true;
            }

            var prefab = (GameObject)BuilderPrefab.GetValue(null);

            if (transform.position == new Vector3(-5000, -5000, -5000) && gameObject != prefab && Time.time > time)
            {
                Logger.Debug("Destroying object: " + gameObject);
                Destroy(gameObject);
            }
        }

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
        }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            var constructable = GetComponent<Constructable>();
            if (constructable != null)
            {
                constructable.techType = techType;
            }

            var techTag = GetComponent<TechTag>();
            if (techTag != null)
            {
                techTag.type = techType;
            }
        }
    }
}

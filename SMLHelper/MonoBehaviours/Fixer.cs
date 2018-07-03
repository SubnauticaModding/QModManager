namespace SMLHelper.V2.MonoBehaviours
{
    using UnityEngine;

    public class Fixer : MonoBehaviour, IProtoEventListener
    {
        [SerializeField]
        public TechType techType;

        [SerializeField]
        public string ClassId;

        void Update()
        {
            if (Utils.NearlyEqual(transform.position.x, 0f) || Utils.NearlyEqual(transform.position.y, 0f))
            {
                SetActiveRecursively(gameObject, false);
            }
        }

        void SetActiveRecursively(GameObject go, bool active)
        {
            go.SetActive(active);
            foreach(Transform transform in go.transform)
            {
                SetActiveRecursively(transform.gameObject, active);
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

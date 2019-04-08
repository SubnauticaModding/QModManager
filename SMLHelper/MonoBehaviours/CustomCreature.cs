namespace SMLHelper.V2.MonoBehaviours
{
    using UnityEngine;

    internal class CustomCreature : MonoBehaviour
    {
        public float scale;

        void Update()
        {
            Vector3 desiredScale = new Vector3(scale, scale, scale);
            if (transform.localScale != desiredScale)
            {
                transform.localScale = desiredScale;
            }
        }
    }
}

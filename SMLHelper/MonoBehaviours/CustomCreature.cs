namespace SMLHelper.V2.MonoBehaviours
{
    using UnityEngine;

    public class CustomCreature : MonoBehaviour
    {
        public float scale;

        void Update()
        {
	     // At the moment all this does is fix the scale, it will likely have more functionality later
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}

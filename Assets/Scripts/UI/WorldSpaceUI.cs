using UnityEngine;

namespace Assets.Scripts.UI
{
    public class WorldSpaceUI : MonoBehaviour
    {
        void Update()
        {
            Camera camera = Camera.main;
            transform.LookAt(transform.position + camera.transform.rotation * Vector3.back, camera.transform.rotation * Vector3.up);
        }
    }
}
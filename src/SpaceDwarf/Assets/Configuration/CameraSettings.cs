using UnityEngine;

namespace Assets.Configuration
{
    // this shouldnt be a mono behaviour, but I want access in the editor....
    public class CameraSettings : MonoBehaviour
    {
        // camera
        public bool SmoothZoom = false;
        public float ZoomSpeed = 40;
        public float MaxOrthoSize = 16;
        public float MinOrthoSize = 2;
    }
}

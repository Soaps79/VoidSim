using UnityEngine;

namespace Assets.Controllers.Cameras
{
    public abstract class CameraControl : ScriptableObject
    {
        public abstract string Name { get; }

        private bool _isEnabled = true;
        public bool IsEnabled { get { return _isEnabled; } }
        public void Enable() { _isEnabled = true; }
        public void Disable() { _isEnabled = false; }
        
        public abstract void Execute(ICamera camera, float timeDelta);
    }
}
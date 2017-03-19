namespace Assets.Controllers.Cameras
{
    // todo: make viewable in editor
    public abstract class CameraControl
    {
        private bool _isEnabled = true;
        public bool IsEnabled { get { return _isEnabled; } }

        public abstract string Name { get; }
        public abstract void Execute(ICamera camera, float timeDelta);

        public void Enable() { _isEnabled = true; }
        public void Disable() { _isEnabled = false; }
    }
}
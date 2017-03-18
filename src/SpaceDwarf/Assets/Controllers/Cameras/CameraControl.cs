namespace Assets.Controllers.Cameras
{
    // todo: make viewable in editor
    public abstract class CameraControl<T> where T : ICamera
    {
        public abstract string Name { get; }
        public abstract void Execute(T camera, float timeDelta);
    }
}
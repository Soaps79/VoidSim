namespace Assets.Controllers.Cameras
{
    public class FreeCamera : CameraBase
    {
        public FreeCamera()
        {
            IsPhysicsBased = false;
        }

        protected override void OnStart()
        {
            base.OnStart();

            AddCameraControl(new PanAndScanControl());
        }
    }
}

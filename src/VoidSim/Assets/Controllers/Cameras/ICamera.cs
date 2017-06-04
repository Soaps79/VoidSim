using UnityEngine;

namespace Assets.Controllers.Cameras
{
    public interface ICamera
    {
        Camera CameraComponent { get; }
        Vector3 Position { get; }
        Vector3 Forward { get; }
        Vector3 Up { get; }
        Vector3 Right { get; }
        bool IsActive { get; set; }
    }
}

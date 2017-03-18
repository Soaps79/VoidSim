﻿using Assets.Configuration;
using UnityEngine;

namespace Assets.Controllers.Cameras
{
    public interface ICamera
    {
        Vector3 Position { get; }
        Vector3 Forward { get; }
        Vector3 Up { get; }
        Vector3 Right { get; }
        CameraSettings CameraSettings { get; }
    }
}

﻿using QGame;
using UnityEngine;

namespace Assets.Controllers.Cameras
{
	public class RotatingCamera : QScript
	{
		public Camera Camera;
		public Vector3 Rotation;

		void Start()
		{
			OnEveryUpdate += RotateCamera;
		}

		private void RotateCamera()
		{
			Camera.transform.Rotate(Rotation);
		}
	}
}
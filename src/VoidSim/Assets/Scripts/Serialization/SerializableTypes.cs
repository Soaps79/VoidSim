using UnityEngine;

namespace Assets.Scripts.Serialization
{
	public class Vector2Data
	{
		public float X;
		public float Y;

		public static implicit operator Vector2(Vector2Data rValue)
		{
			return new Vector2(rValue.X, rValue.Y);
		}

		public static implicit operator Vector2Data(Vector2 rValue)
		{
			return new Vector2Data { X = rValue.x, Y = rValue.y };
		}
	}

	public class Vector3Data
	{
		public float X;
		public float Y;
		public float Z;

		public static implicit operator Vector3(Vector3Data rValue)
		{
			return new Vector3(rValue.X, rValue.Y, rValue.Z);
		}

		public static implicit operator Vector3Data(Vector3 rValue)
		{
			return new Vector3Data { X = rValue.x, Y = rValue.y, Z = rValue.z };
		}
	}

	public class QuaternionData
	{
		public float X;
		public float Y;
		public float Z;
		public float W;

		public static implicit operator Quaternion(QuaternionData rValue)
		{
			return new Quaternion(rValue.X, rValue.Y, rValue.Z, rValue.W);
		}

		public static implicit operator QuaternionData(Quaternion rValue)
		{
			return new QuaternionData { X = rValue.x, Y = rValue.y, Z = rValue.z, W = rValue.w };
		}
	}
}
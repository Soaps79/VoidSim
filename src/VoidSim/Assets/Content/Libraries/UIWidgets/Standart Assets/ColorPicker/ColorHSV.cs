using UnityEngine;
using System;

namespace UIWidgets
{
	/// <summary>
	/// Color in HSV format.
	/// </summary>
	public struct ColorHSV
	{
		/// <summary>
		/// Hue.
		/// </summary>
		public int H;

		/// <summary>
		/// Saturation.
		/// </summary>
		public int S;

		/// <summary>
		/// Value.
		/// </summary>
		public int V;

		/// <summary>
		/// Alpha.
		/// </summary>
		public byte A;

		/// <summary>
		/// Initializes a new instance of the <see cref="UIWidgets.ColorHSV"/> struct.
		/// </summary>
		/// <param name="h">Hue.</param>
		/// <param name="s">Saturation.</param>
		/// <param name="v">Value.</param>
		/// <param name="a">Alpha.</param>
		public ColorHSV(int h, int s, int v, byte a)
		{
			H = h;
			S = s;
			V = v;
			A = a;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UIWidgets.ColorHSV"/> struct.
		/// </summary>
		/// <param name="color">Color.</param>
		public ColorHSV(Color color)
		{
			var colors = new float[]{color.r, color.g, color.b};
			float max = Mathf.Max(colors);
			float min = Mathf.Min(colors);
			var delta = max - min;

			H = 0;
			if (delta==0)
			{
				H = 0;
			}
			else if (max==color.r && color.g >= color.b)
			{
				H = Mathf.RoundToInt(60 * ( (color.g - color.b) / delta ));
			}
			else if (max==color.r && color.g < color.b)
			{
				H = Mathf.RoundToInt(60 * ( (color.g - color.b) / delta )) + 360;
			}
			else if (max==color.g)
			{
				H = Mathf.RoundToInt(60 * ( (color.b - color.r) / delta )) + 120;
			}
			else if (max==color.b)
			{
				H = Mathf.RoundToInt(60 * ( (color.r - color.g) / delta )) + 240;
			}
			if (H < 0)
			{
				H += 360;
			}

			S = (max==0f) ? 0 : Mathf.RoundToInt((1 - (min / max)) * 255);
			V = Mathf.RoundToInt(max * 255);
			A = (byte)Mathf.RoundToInt(color.a * 255);
		}

		/// <summary>
		/// ColorHSV can be implicitly converted to Color32.
		/// </summary>
		/// <param name="color">Color.</param>
		public static implicit operator Color32(ColorHSV color)
		{
			return (Color)color;
		}

		/// <summary>
		/// ColorHSV can be implicitly converted to Color.
		/// </summary>
		/// <param name="color">Color.</param>
		public static implicit operator Color(ColorHSV color)
		{
			var hue = Mathf.Abs((color.H / 360f) % 1f);
			var saturation = Mathf.Abs((color.S / 256f) % 1f);
			var value = Mathf.Abs((color.V / 256f) % 1f);

			hue = Mathf.Clamp(hue, 0.001f, 0.999f);
			saturation = Mathf.Clamp(saturation, 0.001f, 0.999f);
			value = Mathf.Clamp(value, 0.001f, 0.999f);

			float h6 = hue * 6f;
			if (h6 == 6f) { h6 = 0f; }
			int ihue = (int)h6;
			float p = value * (1f - saturation);
			float q = value * (1f - (saturation * (h6 - (float)ihue)));
			float t = value * (1f - (saturation * (1f - (h6 - (float)ihue))));
			switch (ihue)
			{
				case 0:
					return new Color(value, t, p, color.A / 255f);
				case 1:
					return new Color(q, value, p, color.A / 255f);
				case 2:
					return new Color(p, value, t, color.A / 255f);
				case 3:
					return new Color(p, q, value, color.A / 255f);
				case 4:
					return new Color(t, p, value, color.A / 255f);
				default:
					return new Color(value, p, q, color.A / 255f);
			}
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (obj is ColorHSV)
			{
				return Equals((ColorHSV)obj);
			}
			return false;
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="other">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public bool Equals(ColorHSV other)
		{
			return H == other.H && S == other.S && V == other.V && A == other.A;
		}

		/// <summary>
		/// Hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Compare specified colors.
		/// </summary>
		/// <param name="color1">First color.</param>
		/// <param name="color2">Second color.</param>
		/// <returns>true if the colors are equal; otherwise, false.</returns>
		public static bool operator ==(ColorHSV color1, ColorHSV color2)
		{
			return Equals(color1, color2);
		}

		/// <summary>
		/// Compare specified colors.
		/// </summary>
		/// <param name="color1">First color.</param>
		/// <param name="color2">Second color.</param>
		/// <returns>true if the colors not equal; otherwise, false.</returns>
		public static bool operator !=(ColorHSV color1, ColorHSV color2)
		{
			return !Equals(color1, color2);
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return String.Format("HSVA({0}, {1}, {2}, {3})", new object[] {
				H,
				S,
				V,
				A
			});
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <param name="format">Format.</param>
		public string ToString(string format)
		{
			return String.Format("HSVA({0}, {1}, {2}, {3})", new object[] {
				H.ToString(format),
				S.ToString(format),
				V.ToString(format),
				A.ToString(format)
			});
		}
	}
}
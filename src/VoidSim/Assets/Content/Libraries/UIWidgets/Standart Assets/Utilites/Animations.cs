using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace UIWidgets
{
	/// <summary>
	/// Animations.
	/// </summary>
	public static class Animations
	{
		/// <summary>
		/// Rotate animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="time">Time.</param>
		/// <param name="startAngle">Start rotation angle.</param>
		/// <param name="endAngle">End rotation angle.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		static public IEnumerator Rotate(RectTransform rectTransform, float time=0.5f, float startAngle = 0, float endAngle = 90, bool unscaledTime = false, Action actionAfter = null)
		{
			var start_rotarion = rectTransform.localRotation.eulerAngles;

			var end_time = (unscaledTime ? Time.unscaledTime : Time.time) + time;
			
			while ((unscaledTime ? Time.unscaledTime : Time.time) <= end_time)
			{
				var rotation_x = Mathf.Lerp(startAngle, endAngle, 1 - (end_time - (unscaledTime ? Time.unscaledTime : Time.time)) / time);
				
				rectTransform.localRotation = Quaternion.Euler(rotation_x, start_rotarion.y, start_rotarion.z);
				yield return null;
			}
			
			//return rotation back for future use
			rectTransform.localRotation = Quaternion.Euler(start_rotarion);

			if (actionAfter!=null)
			{
				actionAfter();
			}
		}

		/// <summary>
		/// Rotate animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="time">Time.</param>
		/// <param name="startAngle">Start rotation angle.</param>
		/// <param name="endAngle">End rotation angle.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		static public IEnumerator RotateZ(RectTransform rectTransform, float time=0.5f, float startAngle = 0, float endAngle = 90, bool unscaledTime = false, Action actionAfter = null)
		{
			var start_rotarion = rectTransform.localRotation.eulerAngles;
			
			var end_time = (unscaledTime ? Time.unscaledTime : Time.time) + time;
			
			while ((unscaledTime ? Time.unscaledTime : Time.time) <= end_time)
			{
				var rotation_z = Mathf.Lerp(startAngle, endAngle, 1 - (end_time - (unscaledTime ? Time.unscaledTime : Time.time)) / time);
				
				rectTransform.localRotation = Quaternion.Euler(start_rotarion.x, start_rotarion.y, rotation_z);
				yield return null;
			}
			
			//return rotation back for future use
			rectTransform.localRotation = Quaternion.Euler(start_rotarion);

			if (actionAfter!=null)
			{
				actionAfter();
			}
		}

		/// <summary>
		/// Collapse animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="time">Time.</param>
		/// <param name="isHorizontal">Is Horizontal animated width changes; otherwise height.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		static public IEnumerator Collapse(RectTransform rectTransform, float time=0.5f, bool isHorizontal = false, bool unscaledTime = false, Action actionAfter = null)
		{
			var size = rectTransform.rect.size;
			var layoutElement = Utilites.GetOrAddComponent<LayoutElement>(rectTransform);
			if (size.x==0 || size.y==0)
			{
				size = new Vector2(layoutElement.preferredWidth, layoutElement.preferredHeight);
			}

			var end_time = (unscaledTime ? Time.unscaledTime : Time.time) + time;
			while ((unscaledTime ? Time.unscaledTime : Time.time) <= end_time)
			{
				if (isHorizontal)
				{
					var width = Mathf.Lerp(size.x, 0, 1 - (end_time - (unscaledTime ? Time.unscaledTime : Time.time)) / time);
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
					layoutElement.preferredWidth = width;
				}
				else
				{
					var height = Mathf.Lerp(size.y, 0, 1 - (end_time - (unscaledTime ? Time.unscaledTime : Time.time)) / time);
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
					layoutElement.preferredHeight = height;
				}

				yield return null;
			}
			
			//return size back for future use
			if (isHorizontal)
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
				layoutElement.preferredWidth = size.x;
			}
			else
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
				layoutElement.preferredHeight = size.y;
			}

			if (actionAfter!=null)
			{
				actionAfter();
			}
		}

		/// <summary>
		/// Collapse animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="time">Time.</param>
		/// <param name="isHorizontal">Is Horizontal animated width changes; otherwise height.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		static public IEnumerator CollapseFlexible(RectTransform rectTransform, float time=0.5f, bool isHorizontal = false, bool unscaledTime = false, Action actionAfter = null)
		{
			var layoutElement = Utilites.GetOrAddComponent<LayoutElement>(rectTransform);
			if (isHorizontal)
			{
				layoutElement.preferredWidth = -1f;
			}
			else
			{
				layoutElement.preferredHeight = -1f;
			}

			var end_time = (unscaledTime ? Time.unscaledTime : Time.time) + time;
			while ((unscaledTime ? Time.unscaledTime : Time.time) <= end_time)
			{
				var size = Mathf.Lerp(1f, 0f, 1 - (end_time - (unscaledTime ? Time.unscaledTime : Time.time)) / time);
				if (isHorizontal)
				{
					layoutElement.flexibleWidth = size;
				}
				else
				{
					layoutElement.flexibleHeight = size;
				}

				yield return null;
			}
			
			if (isHorizontal)
			{
				layoutElement.flexibleWidth = 0f;
			}
			else
			{
				layoutElement.flexibleHeight = 0f;
			}

			if (actionAfter!=null)
			{
				actionAfter();
			}
		}

		/// <summary>
		/// Open animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="time">Time.</param>
		/// <param name="isHorizontal">Is Horizontal animated width changes; otherwise height.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		static public IEnumerator Open(RectTransform rectTransform, float time=0.5f, bool isHorizontal = false, bool unscaledTime = false, Action actionAfter = null)
		{
			var size = rectTransform.rect.size;
			var layoutElement = Utilites.GetOrAddComponent<LayoutElement>(rectTransform);
			if (size.x==0 || size.y==0)
			{
				size = new Vector2(layoutElement.preferredWidth, layoutElement.preferredHeight);
			}

			var end_time = (unscaledTime ? Time.unscaledTime : Time.time) + time;
			while ((unscaledTime ? Time.unscaledTime : Time.time) <= end_time)
			{
				if (isHorizontal)
				{
					var width = Mathf.Lerp(0, size.x, 1 - (end_time - (unscaledTime ? Time.unscaledTime : Time.time)) / time);
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
					layoutElement.preferredWidth = width;
				}
				else
				{
					var height = Mathf.Lerp(0, size.y, 1 - (end_time - (unscaledTime ? Time.unscaledTime : Time.time)) / time);
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
					layoutElement.preferredHeight = height;
				}

				yield return null;
			}
			
			//return size back for future use
			if (isHorizontal)
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
				layoutElement.preferredWidth = size.x;
			}
			else
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
				layoutElement.preferredHeight = size.y;
			}

			if (actionAfter!=null)
			{
				actionAfter();
			}
		}

		/// <summary>
		/// Open animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="time">Time.</param>
		/// <param name="isHorizontal">Is Horizontal animated width changes; otherwise height.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		static public IEnumerator OpenFlexible(RectTransform rectTransform, float time=0.5f, bool isHorizontal = false, bool unscaledTime = false, Action actionAfter = null)
		{
			var layoutElement = Utilites.GetOrAddComponent<LayoutElement>(rectTransform);
			if (isHorizontal)
			{
				layoutElement.preferredWidth = -1f;
			}
			else
			{
				layoutElement.preferredHeight = -1f;
			}

			var end_time = (unscaledTime ? Time.unscaledTime : Time.time) + time;
			while ((unscaledTime ? Time.unscaledTime : Time.time) <= end_time)
			{
				var size = Mathf.Lerp(0f, 1f, 1 - (end_time - (unscaledTime ? Time.unscaledTime : Time.time)) / time);
				if (isHorizontal)
				{
					layoutElement.flexibleWidth = size;
				}
				else
				{
					layoutElement.flexibleHeight = size;
				}

				yield return null;
			}
			
			//return size back for future use
			if (isHorizontal)
			{
				layoutElement.flexibleWidth = 1f;
			}
			else
			{
				layoutElement.flexibleHeight = 1f;
			}

			if (actionAfter!=null)
			{
				actionAfter();
			}
		}
	}
}
#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1
#else
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

namespace UIWidgets
{
	/// <summary>
	/// InputFieldProxy.
	/// </summary>
	public class InputFieldTMProProxy : IInputFieldProxy
	{

		/// <summary>
		/// The InputField.
		/// </summary>
		TMP_InputField inputField;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="UIWidgets.InputFieldProxy"/> class.
		/// </summary>
		/// <param name="input">Input.</param>
		public InputFieldTMProProxy(TMP_InputField input)
		{
			inputField = input;
		}
		
		#region IInputFieldProxy implementation
		/// <summary>
		/// The current value of the input field.
		/// </summary>
		/// <value>The text.</value>
		public string text {
			get {
				return inputField.text;
			}
			set {
				inputField.text = value;
			}
		}
		
		/// <summary>
		/// Accessor to the OnChangeEvent.
		/// </summary>
		/// <value>The OnValueChange.</value>
		public UnityEvent<string> onValueChange {
			get {
				return inputField.onValueChanged;
			}
		}
		
		/// <summary>
		/// Accessor to the OnChangeEvent.
		/// </summary>
		/// <value>The OnValueChange.</value>
		public UnityEvent<string> onValueChanged {
			get {
				return inputField.onValueChanged;
			}
		}
		
		/// <summary>
		/// The Unity Event to call when editing has ended.
		/// </summary>
		/// <value>The OnEndEdit.</value>
		public UnityEvent<string> onEndEdit {
			get {
				return inputField.onEndEdit;
			}
		}
		
		/// <summary>
		/// Current InputField caret position (also selection tail).
		/// </summary>
		/// <value>The caret position.</value>
		public int caretPosition {
			get {
				return inputField.caretPosition;
			}
			set {
				inputField.caretPosition = value;
			}
		}

		/// <summary>
		/// Is the InputField eligable for interaction (excludes canvas groups).
		/// </summary>
		/// <value>true</value>
		/// <c>false</c>
		public bool interactable {
			get {
				return inputField.interactable;
			}
			set {
				inputField.interactable = value;
			}
		}

		/// <summary>
		/// Determines whether InputField instance is null.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public bool IsNull()
		{
			return inputField==null;
		}

		/// <summary>
		/// Determines whether this lineType is LineType.MultiLineNewline.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public bool IsMultiLineNewline ()
		{
			return inputField.lineType==TMP_InputField.LineType.MultiLineNewline;
		}

		/// <summary>
		/// Function to activate the InputField to begin processing Events.
		/// </summary>
		public void ActivateInputField ()
		{
			inputField.ActivateInputField();
		}

		/// <summary>
		/// Gets the gameobject.
		/// </summary>
		/// <value>The gameobject.</value>
		public GameObject gameObject {
			get {
				return inputField.gameObject;
			}
		}
		#endregion
	}
}
#endif
using Assets.Scripts.Initialization;
using Assets.WorldMaterials.Products;
using DG.Tweening;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Scripts
{
	public static class LogChannels
	{
		public const string Trade = "Trade";
		public const string Serialization = "Serial";
	}

	public class GameMessages
	{
		public const string GameSpeedChange = "GameSpeedChange";
		public const string PreSave = "PreSave";
	}

	public class GameManager : QScript, IMessageListener
	{
		[SerializeField]
		private GameObject KVDUIGameObject;
		private UnityEngine.UI.Text _textGameObject;

		public static Bounds ScreenBounds;
		public Camera MainCamera;

		private MessageHub _messageHub;
		private KeyValueDisplay _keyValueDisplay;

		[SerializeField]
		// Enabling setting in editor and exposing a static interface. there may be a better solution
		// add to locator once it is generic
		private ProductLookup _lkp;
		private static ProductLookup _lkpActual;
		// This has since been made into a singleton, assess if this ref is still needed
		public static IProductLookup ProductLookup { get { return _lkpActual; } }

		public static string KeyValueTextName = "KeyValueText";

		public GameManager()
		{
			ServiceInitializer.Initialize();

			var messageHub = Locator.MessageHub as MessageHub;
			if(messageHub == null)
				throw new UnityException("MessageHub could not be found");
			OnEveryUpdate += delta => messageHub.Update();
		}

		void Start()
		{
			UberDebug.LogChannel(LogChannels.Trade, "Initializing");
			UberDebug.LogChannel(LogChannels.Serialization, "Initializing");
			DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
			InititalizeKeyValueDisplay();
			BindMouseMovementToKvd();
			InitializeScreenBounds();
			InitializeProductLookup();
			Locator.MessageHub.AddListener(this, GameMessages.GameSpeedChange);
		}

		private void InitializeProductLookup()
		{
			if(_lkp == null)
				throw new UnityException("ProductLookup not found");

			_lkpActual = _lkp;
		}

		private void BindMouseMovementToKvd()
		{
			KeyValueDisplay.Instance.Add("MousePos", () => Input.mousePosition.ToString());
		}

		private void InititalizeKeyValueDisplay()
		{
			if (KVDUIGameObject == null)
			{
				Debug.Log("GameManager's KVD UI object is null");
			}
			else
			{
				_textGameObject = KVDUIGameObject.GetComponent<UnityEngine.UI.Text>() as UnityEngine.UI.Text;
				if (_textGameObject == null)
				{
					Debug.Log("GameManager's KVD UI object is null");
				}
				else
				{
					Debug.Log("KVD Initialized");
					OnEveryUpdate += UpdateKeyValueDisplayText;
				}
			}
		}

		private void InitializeScreenBounds()
		{
			if (MainCamera == null)
			{
				throw new UnityException("GameManager does not have Main Camera");
			}

			float screenAspect = (float)Screen.width / (float)Screen.height;
			float cameraHeight = MainCamera.orthographicSize * 2;
			ScreenBounds = new Bounds(
				MainCamera.transform.position,
				new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
		}

		// Update is called once per frame
		private void UpdateKeyValueDisplayText(float delta)
		{
			_textGameObject.text = KeyValueDisplay.Instance.CurrentDisplayString();
		}

		public void HandleMessage(string type, MessageArgs args)
		{
			if (type == GameMessages.GameSpeedChange && args != null)
				HandleTimeChange(args as GameSpeedMessageArgs);
		}

		private void HandleTimeChange(GameSpeedMessageArgs args)
		{
			if (args == null) return;
			if (args.PreviousSpeedTimeScale != args.NewSpeedTimeScale)
			{
				DOTween.timeScale = args.NewSpeedTimeScale;
				QScript.TimeModifier = args.NewSpeedTimeScale;
			}
		}

		public string Name { get { return "GameManager"; } }
	}
}
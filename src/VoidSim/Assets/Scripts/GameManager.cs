using UnityEngine;
using System.Collections;

using QGame;
using Messaging;

public class GameMessages
{
    public const string GameSpeedChange = "GameSpeedChange";
    public const string ItemPlaced = "ItemPlaced";
}

public class GameManager : QScript
{
    [SerializeField]
    private GameObject KVDUIGameObject;
    private UnityEngine.UI.Text _textGameObject;

    public static Bounds ScreenBounds;
    public Camera MainCamera;

    private MessageHub _messageHub;
    private KeyValueDisplay _keyValueDisplay; // not yet used

    public static string KeyValueTextName = "KeyValueText";

    public GameManager()
    {
        _messageHub = new MessageHub();
        _keyValueDisplay = new KeyValueDisplay();

        Locator.Initialize(_messageHub, _keyValueDisplay);
    }

    void Start()
    {
        InititalizeKeyValueDisplay();
        BindMouseMovementtoKVD();
        InitializeScreenBounds();
    }

    private void BindMouseMovementtoKVD()
    {
        _keyValueDisplay.Add("MousePos", () => Input.mousePosition.ToString());
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
    public override void OnUpdate(float delta)
    {
        _messageHub.Update();
    }

    private void UpdateKeyValueDisplayText(float delta)
    {
        _textGameObject.text = _keyValueDisplay.CurrentDisplayString();
    }
}

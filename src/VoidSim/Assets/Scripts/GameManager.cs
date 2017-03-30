using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.WorldMaterials;
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
    private KeyValueDisplay _keyValueDisplay;

    [SerializeField]
    // Enabling setting in editor and exposing a static interface. there may be a better solution
    // add to locator once it is generic
    private ProductLookup _lkp;
    private static ProductLookup _lkpActual;
    public static IProductLookup ProductLookup { get { return _lkpActual; } }

    public static string KeyValueTextName = "KeyValueText";

    public GameManager()
    {
        OnEveryUpdate += (delta) => MessageHub.Instance.Update();
    }

    void Start()
    {
        InititalizeKeyValueDisplay();
        BindMouseMovementToKvd();
        InitializeScreenBounds();
        InitializeProductLookup();
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
}

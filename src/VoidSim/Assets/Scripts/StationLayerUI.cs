using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StationLayerUI : MonoBehaviour
{
    [SerializeField]
    private Station _parentStation;

    [SerializeField]
    private CanvasGroup _displayPanel;

    [SerializeField]
    private Text _displayText;

    public LayerType Type;
    private StationLayer _layerInfo;
    

	// Use this for initialization
	void Start ()
    {
	    if(_parentStation == null)
            throw new UnityException("StationLayerUI not given parent Station");

	    _layerInfo = _parentStation.GetLayer(Type);
	    if (_displayPanel != null && _displayText != null)
	        InitializeDisplayCanvas();
    }

    private void InitializeDisplayCanvas()
    {
        var text = string.Format("Layer: {0}\n", _layerInfo.Type);
        text += string.Format("Building Grid Capacity: {0}", _layerInfo.BuildingGridCapacity);
        _displayText.text = text;
    }

    // Update is called once per frame
	void Update () {
		
	}

    void OnMouseEnter()
    {
        var sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.color = new Color(.84f,1f,.84f,1);
        }

        if (_displayPanel != null)
            _displayPanel.alpha = 1;
    }

    void OnMouseExit()
    {
        var sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.color = Color.white;
        }

        if (_displayPanel != null)
            _displayPanel.alpha = 0;
    }
}

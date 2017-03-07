using System;
using System.Collections;
using System.Collections.Generic;
using QGame;
using UnityEngine;

public enum LayerType
{
    Top, Middle, Bottom
}

[Serializable]
public class StationLayer
{
    public LayerType Type;
    public int BuildingGridCapacity;
}

public class Station : QScript
{
    [SerializeField]
    private StationLayer[] _initialLayers;

    private Dictionary<LayerType, StationLayer> _layers =  new Dictionary<LayerType, StationLayer>();

    void Awake()
    {
        if (_initialLayers != null)
        {
            foreach (var initialLayer in _initialLayers)
            {
                _layers.Add(initialLayer.Type, initialLayer);
            }
        }
    }

	// Use this for initialization
	void Start ()
	{

	}

    public StationLayer GetLayer(LayerType type)
    {
        StationLayer toReturn = null;
        _layers.TryGetValue(type, out toReturn);
        return toReturn;
    }
}

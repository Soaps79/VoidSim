using System;
using Assets.Placeables;
using Assets.Placeables.Nodes;
using Assets.WorldMaterials.Population;
using Assets.WorldMaterials.UI;
using QGame;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public interface IInfoPanelManager
    {
        void AddPanel(Person person, Vector3 clickLocation);
    }

    public class InfoPanelManager : QScript, IInfoPanelManager
    {
        [SerializeField] private PersonViewModel _personPrefab;
        [SerializeField] private Vector3 _personOffset;
        private PersonViewModel _personInstance;
        private Canvas _personCanvas;

        void Start()
        {
            _personCanvas = Locator.CanvasManager.GetCanvas(CanvasType.ConstantUpdate);
            _personInstance = Instantiate(_personPrefab, _personCanvas.transform, false);
            _personInstance.name = "PersonViewModel";
            _personInstance.gameObject.SetActive(false);
        }

        public void AddPanel(Person person, Vector3 clickLocation)
        {
            _personInstance.SetData(person);
            _personInstance.transform.position = clickLocation + _personOffset;
            _personInstance.gameObject.SetActive(true);
        }
    }
}
using Assets.WorldMaterials.Population;
using Assets.WorldMaterials.UI;
using QGame;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class PersonPanelManager : QScript
    {
        [SerializeField] private PersonViewModel _personPrefab;
        [SerializeField] private Vector3 _personOffset;
        private PersonViewModel _personInstance;
        private Canvas _personCanvas;

        void Start()
        {
            _personCanvas = Locator.CanvasManager.GetCanvas(CanvasType.ConstantUpdate);
            _personInstance = Instantiate(_personPrefab, _personCanvas.transform, false);
            _personInstance.name = "PersonPanel";
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
using Assets.Placeables;
using Assets.WorldMaterials.Population;
using QGame;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public interface IInfoPanelManager
    {
        void AddPanel(Person person, Vector3 clickLocation);
        void AddPanel(Placeable placeable);
    }

    /// <summary>
    /// Serves as an interface exposed to the project for manipulating information panels.
    /// </summary>
    public class InfoPanelManager : QScript, IInfoPanelManager
    {
        private PersonPanelManager _personManager;
        private PlaceablePanelManager _placeableManager;

        void Start()
        {
            _personManager = transform.GetComponentInChildren<PersonPanelManager>();
            if(_personManager == null)
                throw new UnityException("InfoPanelManager could not find its PersonPanel");

            _placeableManager = transform.GetComponentInChildren<PlaceablePanelManager>();
            if (_placeableManager == null)
                throw new UnityException("InfoPanelManager could not find its PlaceablePanel");
        }

        public void AddPanel(Person person, Vector3 clickLocation)
        {
            _personManager.AddPanel(person, clickLocation);
        }

        public void AddPanel(Placeable placeable)
        {
            _placeableManager.AddPanel(placeable);
        }
    }
}
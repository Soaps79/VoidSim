using Assets.WorldMaterials.Population;
using QGame;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public interface IInfoPanelManager
    {
        void AddPanel(Person person, Vector3 clickLocation);
    }

    /// <summary>
    /// Serves as an interface exposed to the project for manipulating information panels.
    /// </summary>
    public class InfoPanelManager : QScript, IInfoPanelManager
    {
        private PersonPanelManager _personManager;

        void Start()
        {
            _personManager = transform.GetComponentInChildren<PersonPanelManager>();
            if(_personManager == null)
                throw new UnityException("InfoPanelManager could not find its PersonPanel");
        }

        public void AddPanel(Person person, Vector3 clickLocation)
        {
            _personManager.AddPanel(person, clickLocation);
        }
    }
}
using System.Collections.Generic;
using Assets.Scripts;
using Assets.WorldMaterials.Population;
using UIWidgets;

namespace Assets.Station.UI
{
    public class PopulationListViewModel : ListViewCustom<PersonViewModel, Person>
    {
        void Start()
        {
            gameObject.RegisterSystemPanel(gameObject);
            gameObject.SetActive(false);
        }

        public void UpdateList(List<Person> allPopulation)
        {
            DataSource = allPopulation.ToObservableList();
        }
    }
}
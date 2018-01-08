using System.Collections.Generic;
using Assets.Scripts;
using Assets.WorldMaterials.Population;
using UIWidgets;

namespace Assets.Station.UI
{
    public class PopulationListViewModel : ListViewCustom<PersonViewModel, Person>
    {
        public void UpdateList(List<Person> allPopulation)
        {
            DataSource = allPopulation.ToObservableList();
        }

        public void Initialize()
        {
            gameObject.RegisterSystemPanel(gameObject);
        }
    }
}
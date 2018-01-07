using System.Collections.Generic;
using Assets.WorldMaterials.Population;
using UIWidgets;

namespace Assets.Station.UI
{
    public class PopulationListViewModel : ListViewCustom<PersonViewModel, Person>
    {
        public void Initialize(PopulationControl control)
        {
            
        }

        public void UpdateList(List<Person> allPopulation)
        {
            DataSource = allPopulation.ToObservableList();
        }
    }
}
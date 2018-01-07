using Assets.WorldMaterials.Population;
using TMPro;
using UIWidgets;

namespace Assets.Station.UI
{
    public class PersonViewModel : ListViewItem, IViewData<Person>
    {
        public TMP_Text GenderText;
        public TMP_Text NameText;
        public TMP_Text LocationText;

        private Person _person;

        public void SetData(Person item)
        {
            _person = item;
            item.OnUpdate += SetPersonData;
            SetPersonData(item);
        }

        private void SetPersonData(Person item)
        {
            GenderText.text = item.IsMale ? "M" : "F";
            NameText.text = item.FirstName + " " + item.LastName;
            LocationText.text = item.CurrentActivity;
        }

        protected override void OnDisable()
        {
            if(_person != null)
                _person.OnUpdate -= SetPersonData;
        }

        protected override void OnEnable()
        {
            if(_person != null)
                _person.OnUpdate += SetPersonData;
        }
    }
}
using Assets.WorldMaterials.Population;
using TMPro;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Station.UI
{
    public class PersonViewModel : ListViewItem, IViewData<Person>
    {
        public TMP_Text GenderText;
        public TMP_Text NameText;
        public TMP_Text LocationText;

        private Person _person;

        public override Graphic[] GraphicsBackground
        {
            get
            {
                return new Graphic[] { Background, };
            }
        }

        public void SetData(Person item)
        {
            ClearPerson();
            _person = item;
            _person.OnUpdate += SetPersonData;
            SetPersonData(item);
        }

        private void ClearPerson()
        {
            if (_person != null)
            {
                _person.OnUpdate -= SetPersonData;
                _person = null;
            }
        }

        private void SetPersonData(Person item)
        {
            if(item.Id != _person.Id)
                throw new UnityException("List PersonViewModel UI object being populated by not its owner");
            GenderText.text = item.IsMale ? "M" : "F";
            NameText.text = item.FirstName + " " + item.LastName;
            LocationText.text = item.CurrentActivity;
        }

        public override void MovedToCache()
        {
            ClearPerson();
        }
    }
}
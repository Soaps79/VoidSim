using Assets.Controllers.GUI;
using Assets.WorldMaterials.Population;
using QGame;
using TMPro;
using UnityEngine;

namespace Assets.WorldMaterials.UI
{
    public class PersonViewModel : QScript
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _activityText;

        [SerializeField] private SliderBinding _foodSlider;
        [SerializeField] private SliderBinding _entSlider;
        [SerializeField] private SliderBinding _restSlider;

        private Person _person;

        public void SetData(Person person)
        {
            _person = person;
            SetFromData(_person);
            _person.OnUpdate += SetFromData;
        }

        private void SetFromData(Person person)
        {
            if(person != _person)
                throw new UnityException("PersonViewModel set data callback called by not its Person");

            _nameText.text = _person.FullName;
            _activityText.text = _person.CurrentActivity;

            _foodSlider.Initialize(() => 1);
            _entSlider.Initialize(_person.Wants.GetEntertainment);
            _restSlider.Initialize(_person.Wants.GetRest);
            //_foodSlider.Initialize();
        }
    }
}
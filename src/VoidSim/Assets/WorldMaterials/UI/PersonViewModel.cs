using Assets.Controllers.GUI;
using Assets.WorldMaterials.Population;
using QGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 649

namespace Assets.WorldMaterials.UI
{
    public class PersonViewModel : QScript
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _activityText;
        [SerializeField] private TMP_Text _employerName;
        [SerializeField] private Image _portrait;

        [SerializeField] private SliderBinding _foodSlider;
        [SerializeField] private SliderBinding _entSlider;
        [SerializeField] private SliderBinding _restSlider;

        private Person _person;

        public void SetData(Person person)
        {
            if (_person != null)
            {
                _person.OnUpdate -= SetFromData;
            }

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
            _employerName.text = string.IsNullOrEmpty(_person.Employer) ? "Unemployed" : _person.Employer;
            
            _foodSlider.Initialize(() => 1);
            _entSlider.Initialize(_person.Wants.GetEntertainment);
            _restSlider.Initialize(_person.Wants.GetRest);

            if (_person.PortraitSprite != null)
                _portrait.sprite = _person.PortraitSprite;
        }
    }
}
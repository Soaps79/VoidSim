using System;
using QGame;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Controllers.GUI
{
    /// <summary>
    /// Place on same GameObject as a Slider.
    /// Call Initialize to bind its value to a func
    /// </summary>
    public class SliderBinding : QScript
    {
        private Func<float> _func;
        private Slider _slider;

        public void Initialize(Func<float> func)
        {
            _func = func;
            _slider = GetComponent<Slider>();

            if(_func == null || _slider == null)
                throw new UnityException("SliderBinding initialized with bad things");

            OnEveryUpdate = UpdateSlider;
        }

        private void UpdateSlider(float obj)
        {
            _slider.value = _func();
        }
    }
}
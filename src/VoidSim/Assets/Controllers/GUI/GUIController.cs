using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Controllers.GUI
{
    public class GUIController : MonoBehaviour
    {
        public void ClosePanel(GameObject panel)
        {
            panel.SetActive(false);
        }

        public void OpenPanel(GameObject panel)
        {
            panel.SetActive(true);
        }

        public void TogglePanel(GameObject panel)
        {
            panel.SetActive(!panel.activeSelf);
        }
    }
}

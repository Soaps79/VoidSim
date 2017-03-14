using UnityEngine;

namespace Assets.Controllers.GUI
{
    public class ClosePanelButton : MonoBehaviour
    {
        public void ClosePanel(GameObject panel)
        {
            panel.SetActive(false);
        }


    }
}

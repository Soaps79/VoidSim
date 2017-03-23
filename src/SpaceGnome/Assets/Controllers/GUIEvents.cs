using UnityEngine;

namespace Assets.Controllers
{
    public class GUIEvents : MonoBehaviour {

        public void ClosePanel(GameObject panel)
        {
            panel.SetActive(false);
        }
    }
}

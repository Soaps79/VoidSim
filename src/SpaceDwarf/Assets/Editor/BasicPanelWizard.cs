using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public class BasicPanelWizard : ScriptableWizard
    {
        public string PanelLabel = "Default Label";
        public int Width = 420;
        public int Height = 640;

        public bool IsDraggable = true;
        public bool IsResizable = false;

        public GameObject InnerPanelContent = null;

        [MenuItem("TacticLib/GUI/Create Basic Panel")]
        static void CreateWizard()
        {
            DisplayWizard<BasicPanelWizard>("Create Basic Panel...", "Create");
        }

        void OnWizardCreate()
        {
            // create the panel game object

            // add it to scene, or serialize it directly, whatever
        }

        void OnWizardUpdate()
        {
            helpString = "Enter panel properties";
        }
    }
}

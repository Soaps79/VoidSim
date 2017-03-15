using System;
using System.IO;
using Assets.Controllers.GUI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Editor
{
    public class BasicPanelWizard : ScriptableWizard
    {
        private const string PreFabName = "Panel_Basic";
        private const string CanvasName = "Canvas";
        private const string ResizeZoneName = "ResizeZone";

        public string PanelTitle = "Default Title";
        public int Width = 420;
        public int Height = 560;

        public int PosX = 0;
        public int PosY = 0;

        public bool IsDraggable = true;
        public bool IsResizable = true;
        public bool IsFocusable = true;

        public GameObject InnerPanelContent = null;

        [MenuItem("TacticLib/GUI/Create Basic Panel")]
        static void CreateWizard()
        {
            DisplayWizard<BasicPanelWizard>("Create Basic Panel...", "Create");
        }

        void OnWizardCreate()
        {
            // create the panel game object
            var panel = Instantiate(Resources.Load(PreFabName)) as GameObject;
            if (panel == null)
            {
                throw new FileLoadException(string.Format("Could not load {0} prefab.", PreFabName));
            }

            panel.name = PanelTitle;
            ConfigurePanel(panel);

            // attach content to inner panel
            if (InnerPanelContent != null)
            {
                var holder = panel.transform.FindChild("ContentHolder");
                var content = Instantiate(InnerPanelContent, panel.transform);
                content.transform.parent = holder;
            }

            // add it to scene, or serialize it directly, whatever
            var canvas = GameObject.Find(CanvasName);
            if (canvas == null)
            {
                throw new ApplicationException("Must have a canvas to add a UI panel.");
            }
            panel.transform.SetParent(canvas.transform, worldPositionStays:false);
        }

        private void ConfigurePanel(GameObject panel)
        {
            // size and position
            var transform = panel.GetComponent<RectTransform>();

            //todo: check bounds
            transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Width);
            transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Height);

            // todo: check bounds
            transform.position = new Vector3(PosX, PosY, 0);

            // panel text
            var panelTitle = panel.GetComponentInChildren<Text>();
            panelTitle.text = PanelTitle;

            // behaviors
            var dragScript = panel.GetComponentInChildren<DragPanel>();
            dragScript.enabled = IsDraggable;

            var resizeScript = panel.GetComponentInChildren<ResizePanel>();
            var resizeButton = panel.transform.FindChild(ResizeZoneName);
            resizeScript.enabled = IsResizable;
            resizeButton.gameObject.SetActive(IsResizable);

            var focusScript = panel.GetComponent<FocusPanel>();
            focusScript.enabled = IsFocusable;
        }

        void OnWizardUpdate()
        {
            helpString = "Enter panel properties";
        }
    }
}

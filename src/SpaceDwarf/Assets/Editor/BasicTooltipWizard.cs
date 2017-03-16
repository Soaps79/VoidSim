﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Controllers.GUI;
using Assets.Framework;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public class BasicTooltipWizard : ScriptableWizard
    {
        public string Text1 = "Tooltip Text1";
        public string Text2 = "Tooltip Text2";

        [MenuItem("TacticLib/GUI/Add basic tooltip")]
        static void CreateWizard()
        {
            DisplayWizard<BasicTooltipWizard>("Add basic tooltip...", "Apply to selection");
        }

        void OnWizardCreate()
        {
            var selection = Selection.activeGameObject;
            // make sure we have an object selected
            if (selection == null)
            {
                Debug.LogWarning("No object was selected to add a tooltip to!");
                return;
            }

            var tooltip = selection.GetOrAddComponent<TooltipBehavior>();
            tooltip.TooltipText1 = Text1;
            tooltip.TooltipText2 = Text2;

            var collider = selection.AddComponent<BoxCollider>();

            Debug.Log(string.Format("Applied tooltip to {0}.", selection.name));

        }
    }
}

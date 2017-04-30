﻿using System;
using UnityEditor;
using UnityEngine;

namespace Assets.WorldMaterials.Editor
{
    // This is the result of tutorial at: http://catlikecoding.com/unity/tutorials/editor/custom-list/

    [Flags]
    public enum EditorListOption
    {
        None = 0,
        ListSize = 1,
        ListLabel = 2,
        ElementLabels = 4,
        Buttons = 8,
        Default = ListSize | ListLabel | ElementLabels,
        NoElementLabels = ListSize | ListLabel,
        All = Default | Buttons
    }

    public static class EditorList
    {

        public static void Show(SerializedProperty list, EditorListOption options = EditorListOption.Default)
        {
            if (!list.isArray)
            {
                EditorGUILayout.HelpBox(list.name + " is neither an array nor a list!", MessageType.Error);
                return;
            }

            var showListLabel = (options & EditorListOption.ListLabel) != 0;
            var showListSize = (options & EditorListOption.ListSize) != 0;

            if (showListLabel)
            {
                EditorGUILayout.PropertyField(list);
                EditorGUI.indentLevel += 1;
            }
            if (!showListLabel || list.isExpanded)
            {
                if (showListSize)
                {
                    EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
                }
                ShowElements(list, options);
            }
            if (showListLabel)
            {
                EditorGUI.indentLevel -= 1;
            }
        }

        private static void ShowElements(SerializedProperty list, EditorListOption options)
        {
            var showElementLabels = (options & EditorListOption.ElementLabels) != 0;
            var showButtons = (options & EditorListOption.Buttons) != 0;

            for (int i = 0; i < list.arraySize; i++)
            {
                if (showButtons)
                {
                    EditorGUILayout.BeginHorizontal();
                }

                if (showElementLabels)
                {
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
                }
                else {
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), GUIContent.none);
                }
                if (showButtons)
                {
                    ShowButtons(list, i);
                    EditorGUILayout.EndHorizontal();
                }
            }

            if (showButtons && list.arraySize == 0 && GUILayout.Button(addButtonContent, EditorStyles.miniButton))
            {
                list.arraySize += 1;
            }
        }
        
        private static GUILayoutOption miniButtonWidth = GUILayout.Width(20f);
        private static GUIContent moveButtonContent = new GUIContent("\u21b4", "move down");
        private static GUIContent duplicateButtonContent = new GUIContent("+", "duplicate");
        private static GUIContent deleteButtonContent = new GUIContent("-", "delete");
        private static GUIContent addButtonContent = new GUIContent("+", "add element");

        private static void ShowButtons(SerializedProperty list, int index)
        {
            if (GUILayout.Button(moveButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth))
            {
                list.MoveArrayElement(index, index + 1);
            }
            if (GUILayout.Button(duplicateButtonContent, EditorStyles.miniButtonMid, miniButtonWidth))
            {
                list.InsertArrayElementAtIndex(index);
            }
            if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight, miniButtonWidth))
            {
                int oldSize = list.arraySize;
                list.DeleteArrayElementAtIndex(index);
                if (list.arraySize == oldSize)
                {
                    list.DeleteArrayElementAtIndex(index);
                }
            }
        }
    }
}
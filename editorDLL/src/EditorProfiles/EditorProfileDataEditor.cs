using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

/// <summary>
/// License:
/// The MIT License (MIT)
/// 
/// Copyright (c) 2017 René Bühling, www.gamedev-profi.de
/// 
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
/// 
/// The above copyright notice and this permission notice shall be included in all
/// copies or substantial portions of the Software.
/// 
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
/// SOFTWARE.
/// </summary>

namespace GameDevProfi.EditorProfiles
{
    [InitializeOnLoad]
    [CustomEditor(typeof(EditorProfileData))]
    class EditorProfileDataEditor:Editor
    {

        static EditorProfileDataEditor()
        {
            /// Delaying until first editor tick so that the menu
            /// will be populated before setting check state, and
            /// re-apply correct action.
            /// The same goes for getting the preference from EditorPrefs
            /// as it might not be initialized during the constructor call time
            EditorApplication.delayCall += () => {
                syncMenus();
            };
        }

        private const string showRawMenuItem = "CONTEXT/EditorProfileData/Show Raw Data";
        [MenuItem(showRawMenuItem)]
        static void mnuShowRawData(MenuCommand command)
        {
            EditorPrefs.SetBool("GDP-EditorProfiles-inspect", !EditorPrefs.GetBool("GDP-EditorProfiles-inspect", false));
            syncMenus();   
        }

        private static void syncMenus()
        {
            Menu.SetChecked(showRawMenuItem, EditorPrefs.GetBool("GDP-EditorProfiles-inspect", false));
        }



        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            serializedObject.Update();

            EditorProfileData data = target as EditorProfileData;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Editor Profile", EditorStyles.largeLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Activate", "Use this profile on the current machine. Your selection is saved in Editor Preferences."))) data.activate();
            //if (GUILayout.Button(new GUIContent("Test", ",,,"))) Debug.Log("/"+EditorProfileData.active);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("This file stores various data values inside the project, but supports different files to be used on different machines.", MessageType.None);
            EditorGUILayout.EndHorizontal();

            /* //No need for name field as the name is shown in the header anyways:
            GUI.enabled = false;
            EditorGUILayout.TextField(new GUIContent("Name:", "Rename the file to change the name."), data.name);
            GUI.enabled = true;
            */

            EditorGUILayout.PrefixLabel(new GUIContent("Notes:","Type any text description. It has no meaning other than annotating the file for human readers."));
            data.notes=EditorGUILayout.TextArea(data.notes);

            bool showInspector = EditorPrefs.GetBool("GDP-EditorProfiles-inspect", false);
            /* following switch moved into via gear-menu
            showInspector = EditorGUILayout.ToggleLeft(new GUIContent("Inspect raw data","when set you can see raw data saved in the file. Only recommended for debugging."), showInspector);
            if (GUI.changed)
            {
                EditorPrefs.SetBool("GDP-EditorProfiles-inspect", showInspector);
            }*/
            if (showInspector)
            {
                EditorGUILayout.LabelField("TODO: show raw"); //TODO(RB): implement raw data listing
            }
            else
                drawSubEditors();
        }


        public delegate void DrawSubEditorCallback(EditorProfileData data);

        public static event DrawSubEditorCallback onDrawSubEditor;

        protected void drawSubEditors()
        {
            EditorProfileData data = target as EditorProfileData;
            if (data == null) return;
            else if (onDrawSubEditor != null) onDrawSubEditor(data); //call all listeners registered to onDrawSubEditor, if any.
            else EditorGUILayout.HelpBox("No data editors available.", MessageType.None);
        }

    }
}

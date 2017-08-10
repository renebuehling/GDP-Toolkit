using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using GameDevProfi.EditorProfiles;

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
namespace GameDevProfi.FileManagement
{
    [InitializeOnLoad]
    class FilePatcher
    {

        static FilePatcher()
        {
            EditorProfileDataEditor.onDrawSubEditor += EditorProfileDataEditor_onDrawSubEditor;
        }


        [MenuItem("Assets/"+Main.MenuFolderLabel+"/Add to FilePatcher in Active Editor Profile...")]
        public static void AddSelectedAssetToActiveProfile()
        {
            if (EditorProfileData.active == null)
            {
                EditorUtility.DisplayDialog("No Profile Selected", "Select an active profile first: Select an Editor Profile Asset and click 'Activate' in inspector.", "Ok");
                return;
            }

            string[] guids = Selection.assetGUIDs;
            if (EditorUtility.DisplayDialog("Add to "+EditorProfileData.active.name+"?","Do you want to add " + guids.Length + " files to file patching in active profile "+EditorProfileData.active.name+"?\n\n(Note: This is uncritical, there will no files changed unless you edit the mapping for each file in inspector.)", "Add", "Cancel"))
            {
                Undo.RecordObject(EditorProfileData.active, "Add entries to FilePatcher "+EditorProfileData.active.name);
                EditorProfileData.KeyString ks = EditorProfileData.active.getString("filePatcher.default",true, "");
                if (ks.value.Length > 0 && !ks.value.EndsWith("|")) ks.value += "|";
                foreach(string guid in guids)
                {
                    ks.value+=AssetDatabase.GUIDToAssetPath(guid)+">|";
                }
            }
        }

        //public enum ProcessingFlags {CheckVersion=1};

        private class StringFlags
        {
            private string pattern = "";
            public StringFlags(string pattern)
            {
                this.pattern = pattern;
            }

            public override string ToString()
            {
                return pattern;
            }

            private bool get(string symbol)
            {
                return pattern.Contains(symbol);
            }
            private void set(string symbol, bool value)
            { 
                if (value == get(symbol)) return;
                else if (value) pattern += symbol;
                else pattern = pattern.Replace(symbol, ""); 
            }

            public bool active
            {
                get { return get("A"); }
                set { set("A",value); }
            }

            public bool checkVersion
            {
                get{ return get("C"); }
                set { set("C", value); }
            }
            
        }

        private static bool open
        {
            get { return EditorPrefs.GetBool("GDP-EditorProfiles-FilePatcher-open", false); }
            set { EditorPrefs.SetBool("GDP-EditorProfiles-FilePatcher-open", value); }
        }
        private static void EditorProfileDataEditor_onDrawSubEditor(EditorProfileData data)
        {
            open = EditorGUILayout.Foldout(open, "File Patcher");
            if (open)
            {
                EditorProfileData.KeyString src = data.getString("filePatcher.default");
                if (src==null)
                {
                    EditorGUILayout.HelpBox("No entries.",MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("Patching is executed when you select the patch command from menu "+Main.MenuFolderLabel+".", MessageType.Info);
                    string[] lines  = src.value.Split(new char[] { '|' });
                    EditorGUILayout.BeginVertical();
                    for(int i=0; i<lines.Length; i++)
                    {
                        String line = lines[i];
                        if (line.Length == 0 && i==lines.Length-1) continue; //skip empty last one

                        string[] elements = line.Split(new char[] { '>' }); //syntax: fromFile>toFile>settingsInt
                        //Fill up any missing elements:
                        if (elements.Length == 0) elements = new string[] {"","","0"};
                        else if (elements.Length == 1) elements = new string[] { elements[0], "", "0"};
                        else if (elements.Length == 2) elements = new string[] { elements[0], elements[1],"0" };

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                            StringFlags f = new StringFlags(elements[2]);
                            GUI.changed = false;
                            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                                f.active = EditorGUILayout.ToggleLeft("Replace file", f.active,GUILayout.ExpandWidth(true));
                                //EditorGUILayout.LabelField("Replace file "+(f.active ? "" : "[Disabled]"), (f.active?EditorStyles.boldLabel:EditorStyles.label), GUILayout.ExpandWidth(true));
                                if (GUILayout.Button(new GUIContent("×","Delete this entry"),GUILayout.Width(20)))
                                {
                                    Undo.RecordObject(data, "Delete FilePatcher entry from "+data.name);
                                    List<String> l = lines.ToList();
                                    l.RemoveAt(i);
                                    lines=l.ToArray();
                                    src.value = string.Join("|", lines);
                                    break;
                                }
                            EditorGUILayout.EndHorizontal();
                            GUIStyle padded = new GUIStyle();padded.padding = new RectOffset(16, 0, 0, 0);
                            EditorGUILayout.BeginVertical(padded);
                                elements[0]=EditorGUILayout.TextField(elements[0]);
                                EditorGUILayout.LabelField("with file", EditorStyles.miniLabel);
                                EditorGUILayout.BeginHorizontal();
                                    elements[1]=EditorGUILayout.TextField(elements[1]);
                                    if (GUILayout.Button(new GUIContent("...","Browse file system"),EditorStyles.miniButton,GUILayout.Width(22)))
                                    {
                                        string path = "";
                                        try { path = Path.GetDirectoryName(elements[1]); } catch{ path = ""; } //if path holds a filename, we try to open the file picker dialog in the file's folder.
                                        path = EditorUtility.OpenFilePanel("Select File Path", path, "");
                                        if (path.Length != 0) elements[1] = path;
                                    }
                                EditorGUILayout.EndHorizontal();
                                f.checkVersion = EditorGUILayout.ToggleLeft(new GUIContent("Check Version", "For DLLs version numbers can be compared. If this is checked, then the file will only be copied when the version numbers differ."), f.checkVersion);
                                //EditorGUILayout.LabelField(f.ToString());
                            EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                        if (GUI.changed)
                        {
                            elements[2] = f.ToString();
                            lines[i] = string.Join(">", elements);
                            src.value = string.Join("|", lines);
                            break;
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.HelpBox("To add a new entry select the file(s) in Project View and add it via context menu to the active profile.", MessageType.Info);

                /*UnityEngine.Object newObj = null;
                newObj=EditorGUILayout.ObjectField("Add entry for asset:", newObj, typeof(UnityEngine.Object), false);
                if (newObj!=null)
                {
                    Debug.Log("Adding new: "+newObj);
                }*/
            }
        }

        [MenuItem(Main.MenuFolderLabel + "/Patch Files")]
        public static void PerfomCopies()
        {
            EditorProfileData data = EditorProfileData.active;
            if (data==null)
            {
                EditorUtility.DisplayDialog("No Profile Selected", "Select an active profile first: Select an Editor Profile Asset and click 'Activate' in inspector.", "Ok");
                return;
            }
            EditorProfileData.KeyString src = data.getString("filePatcher.default");

            int changed = 0;
            string[] lines = src.value.Split(new char[] { '|' });
            for (int i = 0; i < lines.Length; i++)
            {
                String line = lines[i];
                if (line.Length == 0) continue; //skip any empty 

                string[] elements = line.Split(new char[] { '>' }); //syntax: fromFile>toFile>settingsInt
                //Skip incomplete
                if (elements.Length == 0) continue;
                else if (elements.Length == 1) continue; 
                else if (elements.Length == 2) elements = new string[] { elements[0], elements[1], "" }; //if only flags are missing, allow processing with default flags
                StringFlags f = new StringFlags(elements[2]);

                if (!f.active) { Debug.Log("Skip (disabled): " + elements[0]); continue; }
                if (!File.Exists(elements[0])) { Debug.LogWarning("File not found: " + elements[0]); continue; }
                if (!File.Exists(elements[1])) { Debug.LogWarning("File not found: "+elements[1]); continue; }

                try
                {
                    EditorUtility.DisplayCancelableProgressBar("FilePatcher", elements[1]+" to "+elements[0], i / lines.Length);
                    File.Copy(elements[1], elements[0],true);
                    Debug.Log("Copied " + elements[1] + " to " + elements[0]);
                    changed += 1;
                }
                catch(Exception ex)
                {
                    Debug.LogError("Could not copy " + elements[1] + " to " + elements[0]+": "+ex.Message);
                }
            }
            EditorUtility.ClearProgressBar();
            if (src.value.Length==0)
                EditorUtility.DisplayDialog("Entries missing", "There are no File Patcher entries in the active profile. Select files in Assets and add them via context menu to the current profile.", "Ok");
            else if (changed==0)
                EditorUtility.DisplayDialog("Nothing happened", "No files where changed. Check console for reasons.", "Ok");
            else
                AssetDatabase.Refresh();
        }
    }
}

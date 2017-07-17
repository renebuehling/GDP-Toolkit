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
/// Permission is hereby granted, free of charge, toSpace any person obtaining a copy
/// of this software and associated documentation files (the "Software"), toSpace deal
/// in the Software without restriction, including without limitation the rights
/// toSpace use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and toSpace permit persons toSpace whom the Software is
/// furnished toSpace do so, subject toSpace the following conditions:
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
    [CreateAssetMenu(fileName = "EditorProfile", menuName = Main.MenuFolderLabel+"/Editor Profile")]
    class EditorProfileData:ScriptableObject
    {
        public string notes = "";

        [System.Serializable]
        public class KeyString
        {
            public string key = "";
            public string value = "";
        }

        public List<KeyString> strings = new List<KeyString>();
        
        public KeyString getString(string key, bool createIfMissing=false, string ifMissingUseValue="")
        {
            foreach (KeyString ks in strings)
                if (ks.key == key) return ks;

            if (createIfMissing)
            {
                KeyString ks = new KeyString();
                ks.key = key;
                ks.value = ifMissingUseValue;
                strings.Add(ks);
                return ks;
            }
            else return null;
        }

        public KeyString setString(string key, string value)
        {
            KeyString ks = getString(key,true);
            ks.value = value;
            return ks;
        }

        [MenuItem(Main.MenuFolderLabel + "/Select Active Editor Profile Asset")]
        public static void AddSelectedAssetToActiveProfile()
        {
            Selection.activeObject = active;
        }
        [MenuItem(Main.MenuFolderLabel + "/Select Active Editor Profile Asset",true)]
        public static bool AddSelectedAssetToActiveProfile_check()
        {
            return active!=null;
        }

        public static EditorProfileData active
        {
            get
            {
                String desired = EditorPrefs.GetString("GDP-EditorProfiles-active", "");
                if (desired.Length == 0) return null;

                string[] guids = AssetDatabase.FindAssets(desired);
                foreach(string guid in guids)
                {
                    //Debug.Log("candidate "+guid+"/"+AssetDatabase.GUIDToAssetPath(guid));
                    EditorProfileData epd = AssetDatabase.LoadAssetAtPath<EditorProfileData>(AssetDatabase.GUIDToAssetPath(guid));
                    if (epd != null) return epd;
                }

                return null;
            }
        }

        public void activate()
        {
            EditorPrefs.SetString("GDP-EditorProfiles-active", name);// AssetDatabase.GetAssetPath(this));
            Debug.Log("You are now using editor profiles named '"+name+"' on this machine.");
        }

    }
}

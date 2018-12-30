using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using GameDevProfi.Utils;
using UnityEditor.Build;


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
namespace GameDevProfi.ProblemFinder
{
    /// <summary>
    /// Provides a window and some managment functionality for quick and easy
    /// implementation of game specific / custom problem scanners.
    /// Users can simply add a problem scanner to check for custom conditions
    /// and requirements inside Unity editor.
    /// </summary>
    public class ProblemFinderWindow: EditorWindow, IPreprocessBuild, IPostprocessBuild
    {
        
        /// <summary>
        /// Shows the window, used as menu hook.
        /// </summary>
        [MenuItem("Window/"+Main.MenuFolderLabel+"/Problem Finder")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(ProblemFinderWindow)); //Show existing window instance. If one doesn't exist, make one.
        }

        private static Texture2D icoBulb;
        private static Texture2D icoProblem;
        private static Texture2D icoOk;

        private void loadIcons()
        {
            Debug.Log("Resources in problemfinder:\n"+DLL.listAllResources());
            if (icoBulb == null) icoBulb = DLL.LoadImageResource("GameDevProfi.Resources.bulb.png",16,16,Color.magenta,GetType().Assembly);
            if (icoProblem == null) icoProblem = DLL.LoadImageResource("GameDevProfi.Resources.exclam.png", 16, 16,Color.magenta, GetType().Assembly);
            if (icoOk == null) icoOk = DLL.LoadImageResource("GameDevProfi.Resources.bullet-ok.png", 16, 16,Color.magenta, GetType().Assembly);
        }


        private void OnEnable()
        {
            loadIcons();
            titleContent.text = "Problems";
            titleContent.image = icoOk;
            //minSize = new Vector2 (500, 400);

            findProblems();
        }

        private void OnDisable()
        {
            //UnityEngine.Object.DestroyImmediate(icon);
            //icon = null;
        }

        #region Build Events Hooks

        /// <summary>
        /// Will be true while Player builds.
        /// </summary>
        private bool isBuilding = false;

        /// <summary>
        /// Required by IPreprocessBuild and IPostprocessBuild.
        /// </summary>
        public int callbackOrder { get { return 0; } }

        /// <summary>
        /// Callback used by IPreprocessBuild.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="path"></param>
        public void OnPreprocessBuild(BuildTarget target, string path)
        {
            isBuilding = true;
        }
        /// <summary>
        /// Callback used by IPostprocessBuild.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="path"></param>
        public void OnPostprocessBuild(BuildTarget target, string path)
        {
            isBuilding = false;
        }
        #endregion

        /// <summary>
        /// Base class for one found problem, shown in the ProblemFinder window.
        /// Create an instance of a problem for each problem found and add them
        /// to <see cref="ProblemScanner.problems"/> of your ProblemScanner instance.
        /// </summary>
        public abstract class Problem
        {
            /// <summary>
            /// Provides a short description of the problem. 
            /// This is the text directly visible in the window list.
            /// </summary>
            /// <returns>A short summary of the problem.</returns>
            public abstract string getLabel();
            /// <summary>
            /// A detailed description of how to fix the problem. 
            /// This text is shown when the user clicks the item in the list.
            /// </summary>
            /// <returns>Detailed problem description.</returns>
            public virtual string getFixDescription() { return "No fix or suggestion description available."; }
            /// <summary>
            /// Return true if you implemented an automatic solution of the
            /// problem within <see cref="autoFix()"/>.
            /// If you do not override this method it will automatically 
            /// return if <see cref="autoFix()"/> is implemented/overriden.
            /// </summary>
            /// <returns>True if <see cref="autoFix()"/> is implemented.</returns>
            public virtual bool canAutoFix() { return GetType().GetMethod("autoFix").DeclaringType != typeof(Problem); } //default: true, if any subclass overrides/implements autoFix().
            /// <summary>
            /// Subclasses should implement the automated solution
            /// of the problem, if possible. This method is executed,
            /// if the 'Fix now' button in the fix description dialog
            /// is clicked or if a list item is ctrl+clicked.
            /// </summary>
            public virtual void autoFix() { }
            /// <summary>
            /// Subclasses should implement the selection of the relevant object
            /// in this method. This is usually the object that has the
            /// problem. Use <code>Selection.ActiveGameObject</code> or similar
            /// to perform the selection change.
            /// </summary>
            public virtual void select() { }
        }

        /// <summary>
        /// Convenience implementation of <see cref="Problem"/> which simply
        /// adds a textual description without fix to the Problem finder window.
        /// </summary>
        public class GeneralProblemInfo:Problem
        {
            private string shortinfo = "";
            private  string longinfo = "";
            /// <summary>
            /// Use instance of this class to quickly add text-only info to 
            /// the Problem finder window.
            /// </summary>
            /// <param name="shortinfo">Label text shown in the window.</param>
            /// <param name="longinfo">Long description text of the problem.</param>
            public GeneralProblemInfo(string shortinfo, string longinfo)
            {
                this.shortinfo = shortinfo;
                this.longinfo = longinfo;
            }

#pragma warning disable CS1591 // Missing xml comment
            public override string getLabel()
            {
                return shortinfo; 
            }
            public override string getFixDescription()
            {
                return longinfo;
            }
            public override bool canAutoFix()
            {
                return false;
            }
#pragma warning restore CS1591 // Missing xml comment
        }

        /// <summary>
        /// Storage of Problem scanner GUI status.
        /// </summary>
        public class ProblemScannerGUI
        {
            /// <summary>
            /// Is this problem category currently expanded?
            /// </summary>
            public bool expand = true;
        }

        /// <summary>
        /// Base class for any module that adds problem scanning
        /// functionality to the ProblemScanner window.
        /// 
        /// Create a subclass, annotate it with <code>[UnityEditor.InitializeOnLoad]</code> 
        /// and within the constructor of your ProblemScanner class add a new instance to <see cref="ProblemFinderWindow.scanners"/>.
        /// </summary>
        public abstract class ProblemScanner
        {
            /// <summary>
            /// List of found problems. Add instances of <see cref="Problem"/> 
            /// to this list inside <see cref="scan()"/>. Use <code>problems.Clear()</code>
            /// to empty this list.
            /// </summary>
            public List<Problem> problems = new List<Problem>();
            /// <summary>
            /// Cache of the current painting status. Just keep this untouched.
            /// </summary>
            public ProblemScannerGUI gui = new ProblemScannerGUI();
            /// <summary>
            /// Subclasses can override this to provide a label for the scanner.
            /// This is essentially the text of the foldable item group.
            /// </summary>
            /// <returns>Text of group, usually describing the problem category this scanner cares for, i.e. 'Character Setup', 'Level Consistency', etc.</returns>
            public abstract string getLabel();
            /// <summary>
            /// Subclasses should implement the actual problem scanning here. 
            /// This usually starts with <code>problems.Clear()</code>. Then
            /// perform the testings and use <code>problems.Add(new Problem())</code>
            /// to record problems found. (Use your subclasses of Problem).
            /// </summary>
            public abstract void scan();

            /// <summary>
            /// Auto called after scan(), implement Problem.Equals() for custom Problem classes.
            /// </summary>
            public virtual void removeDoubles() 
            {
                for (int i = problems.Count - 1; i >= 0; i--)
                {
                    for (int j = i - 1; j >= 0; j--)
                        if (problems[j].Equals(problems[i])) { problems.RemoveAt(i); break; }
                }
            }
        }

        /// <summary>
        /// List of registered problem scanners.
        /// To add your own, subclass ProblemScanner
        /// and add it to this list on init. 
        /// Use [InitializeOnLoad] and a static constructor
        /// on your class to register when the dll loads.
        /// </summary>
        public static List<ProblemScanner> scanners = new List<ProblemScanner>();

        private void OnInspectorUpdate() { if (autoRefresh) findProblems(); Repaint(); }    /*Repaint();*/
        private void OnProjectChange() { if (autoRefresh) findProblems(); }
        private void OnHierarchyChange() { if (autoRefresh) findProblems(); }

        /// <summary>
        /// Moment when last scanning occured. 
        /// Used to limit the scanning workload.
        /// </summary>
        private float lastScan = 0f;

        //	private int prevProbs=-1;
        private void findProblems()
        {
            if (Application.isPlaying || isBuilding) return;

            if (Mathf.Abs(Time.time - lastScan) < 1f) { /*Debug.Log("Not scanning, "+Time.time+" last:"+lastScan+" delta:"+Mathf.Abs(Time.time-lastScan));*/ return; } //scanned within 1 sec ago

            int problems = 0;
            foreach (ProblemScanner s in scanners)
            {
                s.scan();
                s.removeDoubles();
                problems += s.problems.Count;
            }

            titleContent.text = "Problems: " + problems;
            if (problems == 0) titleContent.image = icoOk;
            else titleContent.image = icoProblem;
            //if (problems!=prevProbs) Repaint();
            //prevProbs=problems;
            lastScan = Time.time;
        }

        private bool autoRefresh = true;

        private Vector2 rootScrollpos = new Vector2();


        private void OnGUI()
        {
            if (Application.isPlaying)
            {
                GUILayout.Label("Panel not available in PlayMode.");
                return;
            }
            else if (isBuilding)
            {
                GUILayout.Label("Panel not available during Build.");
                return;
            }

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button(new GUIContent("Scan", "Check for problems (again)."), EditorStyles.toolbarButton)) findProblems();
            autoRefresh = GUILayout.Toggle(autoRefresh, new GUIContent("Auto", "Automatically perform a re-scan when project changes."), EditorStyles.toolbarButton);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("++", "Expand all."), EditorStyles.toolbarButton)) foreach (ProblemScanner s in scanners) s.gui.expand = true;
            if (GUILayout.Button(new GUIContent("--", "Collapse all."), EditorStyles.toolbarButton)) foreach (ProblemScanner s in scanners) s.gui.expand = false;
            EditorGUILayout.EndHorizontal();

            rootScrollpos = EditorGUILayout.BeginScrollView(rootScrollpos); //,GUILayout.Width(100)); // .ExpandWidth(true));
            GUIStyle buttonStyle = new GUIStyle(EditorStyles.label); buttonStyle.richText = true;
            bool abort = false;
            foreach (ProblemScanner s in scanners)
            {
                GUIStyle fos = new GUIStyle(EditorStyles.foldout);
                if (s.problems.Count > 0) fos.fontStyle = FontStyle.Bold;
                s.gui.expand = EditorGUILayout.Foldout(s.gui.expand, s.getLabel() + " (" + s.problems.Count + ")", fos);
                if (s.gui.expand)
                {
                    foreach (Problem p in s.problems)
                    {
                        EditorGUILayout.BeginHorizontal();
                        try
                        {
                            if (fixButton(p)) { findProblems(); abort = true;}
                            else if (GUILayout.Button(new GUIContent(p.getLabel(), "Click to select gameobject."), buttonStyle)) p.select();
                            //EditorGUILayout.SelectableLabel(p.getLabel(),GUILayout.ExpandWidth(true));
                            //GUILayout.FlexibleSpace();
                        }
                        catch { }
                        EditorGUILayout.EndHorizontal();
                        if (abort) break;
                    }
                }
                if (abort) break;
            }
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Draw the fix bulb icon.
        /// </summary>
        /// <param name="p">Problem to display.</param>
        /// <returns><c>true</c>, if fix was applied, requiring a rescan of problems.</returns>
        private bool fixButton(Problem p)
        {
            GUIStyle fixStyle = new GUIStyle(GUI.skin.label);
            fixStyle.padding = new RectOffset();
            fixStyle.fixedWidth = 20;
            if (GUILayout.Button(new GUIContent(p.canAutoFix() ? icoBulb : icoProblem, "Get suggestion for Fix"), fixStyle))
            {
                if (!p.canAutoFix()) //no auto-fix, just show text
                    EditorUtility.DisplayDialog("Problem Solver", p.getFixDescription(), "Ok");
                else if (Event.current.control || //ctrl+click on bulb -> execute fix immediately
                         EditorUtility.DisplayDialog("Problem Solver", p.getFixDescription(), "Fix now", "Cancel"))
                {
                    p.autoFix();
                    return true;
                }
            }
            return false;
        }

        
    }

}

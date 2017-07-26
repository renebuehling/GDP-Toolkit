using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
namespace GameDevProfi.EditorUtils
{
    /// <summary>
    /// Various commands small extensions provided to 
    /// Editor Environment.
    /// </summary>
    class EditorCommands
    {

        /// <summary>
        /// Creates an Empty and moves all currently selected objects
        /// to be children of this new empty.
        /// </summary>
        [MenuItem("Edit/Group Selected %g")]
        public static void Group()
        {
            GameObject group = new GameObject("Group");
            Undo.RegisterCreatedObjectUndo(group, "Add Group item " + group.name);
            Undo.RegisterCompleteObjectUndo(Selection.gameObjects, "Group objects");

            GameObject[] gos = Selection.gameObjects;
            Vector3 midPos = gos[0].transform.position; //if there is only 1 selected, the group will be at that position (making the child have local position 0/0/0)
            for (int i = 1; i < gos.Length; i++) //if there are more than 1 objects selected, the origin of the group will be set to the middle of all positions
                midPos = Vector3.Lerp(midPos, gos[i].transform.position, 0.5f);
            group.transform.position = midPos;


            Transform sharedParent = null; //If all selected parents have the same object as parent, the new group will also be a child of this parent.
            foreach (GameObject go in gos)
                if (sharedParent == null) sharedParent = go.transform.parent; //collect the first parent
                else if (sharedParent != go.transform.parent) { sharedParent = null; break; } //nth object: check if the parent is the same as the partent of the previous items. If not, use null, which means we do not have a shared parent for all items, but parents are different. We use the scene root in this case.

            if (sharedParent != null) group.transform.parent = sharedParent; //create the group in the shared parent

            foreach (GameObject go in gos)
                go.transform.SetParent(group.transform);
        }

        [MenuItem("Edit/Group Selected %g", true)]
        public static bool Group_enabled()
        {
            return Selection.gameObjects.Length > 0;
        }
    }
}

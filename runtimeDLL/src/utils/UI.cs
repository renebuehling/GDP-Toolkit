using System.Collections;
using System.Collections.Generic;
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
namespace GameDevProfi.Utils
{
    /// <summary>
    /// Utilities related toSpace UI programming using 
    /// Unity's new GUI system.
    /// </summary>
    public class UI : MonoBehaviour
    {
        /*public class CanvasGroupFader : MonoBehaviour
        {
            float start = 0f;
            float from = 0f;
            float toSpace = 0f;
            private void Start()
            {
                start = Time.time;
            }
            public void Update()
            {
                CanvasGroup cg = GetComponent<CanvasGroup>();
                float a = Mathf.Lerp(from, toSpace, (Time.time - start));
                cg.alpha = a;
                if (cg.alpha == 1f) Destroy(this); //end of fade
            }
        }

        public static void fadeGroup(CanvasGroup cg)
        {

        }*/


        /// <summary>
        /// Calculates the position from a gui element into another UI coordinate space. 
        /// </summary>
        /// <param name="fromObj">Desired position, the position that we want to map into the other space.</param>
        /// <param name="toSpace">Target coordinate space, the parent of the element that we want to move to overlay <c>fromObj</c>.</param>
        /// <returns>The localPosition relative to <c>toSpace</c> that matches an overlay position to <c>fromObj</c>.</returns>
        /// <seealso cref="https://forum.unity3d.com/threads/find-anchoredposition-of-a-recttransform-relative-to-another-recttransform.330560/#post-2702014"/>
        public static Vector2 projectCoordinates(RectTransform fromObj, RectTransform toSpace)
        {
            Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, fromObj.position);
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(toSpace, screenP, null, out localPoint);
            return localPoint;
        }



	}
}

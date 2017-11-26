using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
namespace GameDevProfi.Utils
{
    /// <summary>
    /// Utilities related to UI programming using 
    /// Unity's new GUI system.
    /// </summary>
    public class UI : MonoBehaviour
    {
        /*public class CanvasGroupFader : MonoBehaviour
        {
            float start = 0f;
            float from = 0f;
            float to = 0f;
            private void Start()
            {
                start = Time.time;
            }
            public void Update()
            {
                CanvasGroup cg = GetComponent<CanvasGroup>();
                float a = Mathf.Lerp(from, to, (Time.time - start));
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
        /// <param name="to">Target coordinate space, the parent of the element that we want to move to overlay <c>fromObj</c>.</param>
        /// <returns>The localPosition relative to <c>to</c> that matches an overlay position to <c>fromObj</c>.</returns>
        /// <seealso cref="https://forum.unity3d.com/threads/find-anchoredposition-of-a-recttransform-relative-to-another-recttransform.330560/#post-2702014"/>
        public static Vector2 projectCoordinates(RectTransform fromObj, RectTransform to)
        {
            Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, fromObj.position);
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, null, out localPoint);
            return localPoint;
        }


        /// <summary>
        /// Scrolls immediately to a given object in the scrollview.
        /// 
        /// <b>Important:</b> 
        /// Unity UI's scrolling component and UI setup is complex and full of variations. Therefore this
        /// function may not always give the expected results. Simply try it. If it does not work, you may copy the
        /// implementation of this function into your own code and customize as needed.
        /// </summary>
        /// <param name="scrollRect">ScrollView that should perform the scrolling.</param>
        /// <param name="focusObject">Object to scroll to. Must be a child of scrollRect's content object.</param>
        /// <param name="keepX">If true, scrolling does not apply to X.</param>
        /// <param name="keepY">If true, scrolling does not apply to Y.</param>
        /// <seealso cref="https://stackoverflow.com/questions/30766020/how-to-scroll-to-a-specific-element-in-scrollrect-with-unity-ui"/>
        public static void scrollJumpTo(UnityEngine.UI.ScrollRect scrollRect, GameObject focusObject, bool keepX = false, bool keepY = false)
        {
            RectTransform contentPanel = scrollRect.content;
            RectTransform target = focusObject.GetComponent<RectTransform>();

            Canvas.ForceUpdateCanvases();

            Vector2 result =
                (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position) //current scrollpos
                - (
                    (Vector2)scrollRect.transform.InverseTransformPoint((Vector2)target.position) //... pos of object
                );

            result.x += (scrollRect.GetComponent<RectTransform>().rect.width / 2); //try to scroll object into center
            result.y -= (scrollRect.GetComponent<RectTransform>().rect.height / 2); //try to scroll object into center
                                                                                    //Debug.Log("scrollRect.rect="+(scrollRect.GetComponent<RectTransform>().rect)+", result="+result);

            result.x = Mathf.Clamp(result.x, 0, contentPanel.rect.width);
            result.y = Mathf.Clamp(result.y, -contentPanel.rect.height, 0);

            if (keepX) result.x = contentPanel.anchoredPosition.x;
            if (keepY) result.y = contentPanel.anchoredPosition.y;

            //scrollRect.inertia = false;
            contentPanel.anchoredPosition = result;
            scrollRect.StopMovement(); //Important: Stop any flow that may come from inertia, because it will falsify the programmatic scrolling results here (as we want a hard but reliable jump to a fixed position)
            //Debug.Log("-> result=" + result +", c.ap="+ contentPanel.anchoredPosition+"c.rs="+contentPanel.rect.height); 
        }

        /// <summary>
        /// Changes the alpha value of a Graphic's color.
        /// This is a shortcut to apply <see cref="Colors.changeAlpha(Color, float, bool)"/>
        /// to the Graphic's color.
        /// </summary>
        /// <param name="g">Graphic to change, i.e. a Button, Image, etc.</param>
        /// <param name="value">New alpha value or change to apply to the alpha value (see incremental parameter).</param>
        /// <param name="incremental">If true, value will be added to the current alpha of the color. If false, value will be assigned as new alpha value of the color.</param>
        /// <see cref="Colors.changeAlpha(Color, float, bool)"/>
        public static void changeAlpha(Graphic g, float value, bool incremental = false)
        {
            if (g!=null) g.color = Colors.changeAlpha(g.color, value, incremental);
        }

    }
}

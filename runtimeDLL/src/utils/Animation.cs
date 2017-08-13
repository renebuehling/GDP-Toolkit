using UnityEngine;

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
    /// Contains utilities for animation calculation.
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// Maps <code>Time.time</code> to a scale of [0..1] when a interval 
        /// of <code>duration</code> seconds is intended.
        /// 
        /// This is a reminder/shortcut helper, i.e. for using AnimationCurves:
        /// - Define an AnimationCurve with keyframes with from 0 to 1 (horizontal axis).
        /// - If you want to get the animation curve value with having one animation cycle be mapped to 2 seconds, use:
        ///  <code>animationCurve.Evaluate(Utils.timeScale(2f));</code>
        /// 
        /// </summary>
        /// <param name="duration">Desired animation duration in seconds.</param>
        /// <returns>A value between 0 and 1.</returns>
        public static float timeScale01(float duration)
        {
            return timeScale01(Time.time, duration);
        }

        /// <summary>
        /// Maps the given <code>time</code> to a scale of [0..1] when a interval 
        /// of <code>duration</code> seconds is intended.
        /// 
        /// This is a reminder/shortcut helper, i.e. for using AnimationCurves:
        /// - Define an AnimationCurve with keyframes with from 0 to 1 (horizontal axis).
        /// - If you want to get the animation curve value with having one animation cycle be mapped to 2 seconds, use:
        ///  <code>animationCurve.Evaluate(Utils.timeScale(Time.time,2f));</code>
        /// 
        /// </summary>
        /// <param name="time">Time stamp, usually something like <code>Time.time</code>.</param>
        /// <param name="duration">Desired animation duration in seconds.</param>
        /// <returns>A value between 0 and 1.</returns>
        public static float timeScale01(float time, float duration)
        {
            return Mathf.Clamp01((time % duration) / duration);
        }

        /// <summary>
        /// Returns the y value from the given curve if the curve
        /// should be mapped to a total length of <code>duration</code> seconds.
        /// Uses <code>timeScale01(Time.time,duration)</code>.
        /// </summary>
        /// <param name="curve">Curve.</param>
        /// <param name="duration">Duration.</param>
        /// <returns>Current curve value.</returns>
        public static float eval(AnimationCurve curve, float duration)
        {
            return curve.Evaluate(timeScale01(Time.time, duration));
        }
    }
}

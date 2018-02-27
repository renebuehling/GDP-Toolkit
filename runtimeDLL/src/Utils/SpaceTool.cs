using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// License:
/// The MIT License (MIT)
/// 
/// Copyright (c) 2018 René Bühling, www.gamedev-profi.de
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
    /// Utility for spatial calculations.
    /// This class can be used in a static way by calling the static methods
    /// or in an instance way by creating a Space instance and calling the methods on this object.
    /// Main difference is only that the object can store the self parameter so that 
    /// there is no need to pass it as parameter to every method. 
    /// 
    /// Decision help: 
    /// Use static methods when you only want to make a few measures, especially when these are for different objects.
    /// Create an object when you need multiple calculations on the same object.
    /// 
    /// </summary>
    public class SpaceTool
    {
        /// <summary>
        /// Cache to an object which will be passed as <c>self</c> parameter to the static functions.
        /// This is usually the object the measure relates to, i.e. the player character when you want to check if a player is near something.
        /// </summary>
        public Transform self=null;

        /// <summary>
        /// Create a new Space measurement tool and store the given element 
        /// as starting point for measures.
        /// </summary>
        /// <param name="self">The first object to be passed to the measurements methods as <c>self</c> parameter.</param>
        public SpaceTool(Transform self)
        {
            this.self = self;
        }

        /// <summary>
        /// Create a new Space measurement tool and store the given element's <c>transform</c> property
        /// as starting point for measures.
        /// </summary>
        /// <param name="self">The first object to be passed to the measurements methods as <c>self</c> parameter.</param>
        public SpaceTool(GameObject self)
        {
            this.self = self.transform;
        }

        /// <summary>
        /// Create a new Space measurement tool and store the given element's <c>transform</c> property
        /// as starting point for measures.
        /// </summary>
        /// <param name="self">The first object to be passed to the measurements methods as <c>self</c> parameter.</param>
        public SpaceTool(MonoBehaviour self)
        {
            this.self = self.transform;
        }


        /// <summary>
        /// Shortcut for measuring the distance between two objects.
        /// </summary>
        /// <param name="self">First object.</param>
        /// <param name="other">Another object.</param>
        /// <returns>The distance between the two objects, by measuring distance of transform origins.</returns>
        /// <see cref="distanceTo(Transform)"/>
        /// <see cref="distanceTo(MonoBehaviour)"/>
        public static float distanceTo(Transform self, Transform other)
        {
            return Vector3.Distance(self.position, other.transform.position);
        }

        /// <summary>
        /// Shortcut for measuring the distance between <see cref="self"/> and another object.
        /// </summary>
        /// <param name="other">Other object. </param>
        /// <returns>Distance between <c>self</c> and <c>other</c>.</returns>
        /// <see cref="distanceTo(Transform, Transform)"/>
        public float distanceTo(Transform other)
        {
            return distanceTo(self, other);
        }

        /// <summary>
        /// Shortcut for measuring the distance between <see cref="self"/> and another object.
        /// </summary>
        /// <param name="other">Other object. </param>
        /// <returns>Distance between <c>self</c> and <c>other</c>.</returns>
        /// <see cref="distanceTo(Transform, Transform)"/>
        public float distanceTo(MonoBehaviour other)
        {
            return distanceTo(self, other.transform);
        }



        /// <summary>
        /// Shortcut to test if an object is close to another object.
        /// </summary>
        /// <param name="self">First object.</param>
        /// <param name="other">The other object.</param>
        /// <param name="range">Maximum distance of the other object to make this function return true.</param>
        /// <returns>True if <c>other</c> is no more than <c>range</c> units away from <c>self</c>.</returns>
        /// <see cref="isDistanceTo(Transform, float)"/>
        public static bool isDistanceTo(Transform self, Transform other, float range)
        {
            return distanceTo(self, other) <= range;
        }

        /// <summary>
        /// Shortcut to test if an object is close to another object.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <param name="range">Maximum distance of the other object to make this function return true.</param>
        /// <returns>True if <c>other</c> is no more than <c>range</c> units away from <c>self</c>.</returns>
        /// <see cref="isDistanceTo(Transform, Transform, float)"/>
        public bool isDistanceTo(Transform other, float range)
        {
            return distanceTo(self, other) <= range;
        }



        /// <summary>
        /// Calculates the direction vector from a given object towards the cow.
        /// </summary>
        /// <param name="other">Object to look from.</param>
        /// <param name="self">Object to look at.</param>
        /// <returns>Vector describing the direction from <c>other</c> towards <c>self</c>.</returns>
        /// <see cref="directionFrom(Transform)"/>
        public static Vector3 directionFrom(Transform other, Transform self)
        {
            //point towards target:
            Vector3 dir = self.transform.position - other.transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            return Quaternion.AngleAxis(angle, Vector3.forward) * other.transform.right;
        }


        /// <summary>
        /// Calculates the direction vector from a given object towards the cow.
        /// </summary>
        /// <param name="other">Object to look from.</param>
        /// <param name="self">Object to look at.</param>
        /// <returns>Vector describing the direction from <c>other</c> towards <c>self</c>.</returns>
        /// <see cref="directionFrom(Transform, Transform)"/>
        public Vector3 directionFrom(Transform other)
        {
            return directionFrom(other, self);
        }
    }
}

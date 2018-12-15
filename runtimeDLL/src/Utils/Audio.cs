using System.Collections;
using System.Collections.Generic;
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
    /// Utilities related to sound, music and audio processing.
    /// </summary>
    public class Audio
    {

        /// <summary>
        /// Helper class that destroys the gameobject as soon as
        /// <see cref="audioSource"/> is no longer playing. 
        /// Call <see cref="PlayClip2D(AudioClip)"/> to use this component.
        /// </summary>
        public class AutoCleanup : MonoBehaviour
        {
            /// <summary>
            /// Audio source to monitor. 
            /// </summary>
            public AudioSource audioSource;

            /// <summary>
            /// Tries to find the audio source automatically if not set.
            /// </summary>
            protected virtual void Start()
            {
                if (audioSource == null) audioSource = GetComponent<AudioSource>();

                if (audioSource == null)
                {
                    Debug.LogWarning("AutoCleanup has not audioSource and could not find any. This script has no effect, disabling.");
                    enabled = false;
                }
            }

            /// <summary>
            /// Destroys the game object when audioSource is no longer playing.
            /// </summary>
            protected virtual void Update()
            {
                if (!audioSource.isPlaying) Destroy(this.gameObject);
            }
        }

        /// <summary>
        /// Creates an audio player that removes itself after the given clip
        /// finished playing. Similar to <see cref="AudioSource.PlayClipAtPoint(AudioClip, Vector3)"/>, 
        /// but with preset for 2D sound and the ability to change the sound parameters
        /// through the return parameter.
        /// </summary>
        /// <param name="clip">Clip to play.</param>
        /// <returns>The generated audio source which could be used to adjust audio source settings.</returns>
        public static AudioSource PlayClip2D(AudioClip clip)
        {
            GameObject go = new GameObject();
            AudioSource aus = go.AddComponent<AudioSource>();
            aus.spatialBlend = 0f;
            aus.clip = clip;

            AutoCleanup r = go.AddComponent<AutoCleanup>();
            r.audioSource = aus;
            aus.Play();

            return aus;
        }

    }
}
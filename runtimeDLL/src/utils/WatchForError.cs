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
    /// Utility to track Unity's errors which cannot be caught by exception catching. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// Example Use Case:  <br/>
    /// When loading a scene which is not included in the build, Unity will just add an
    /// error message to output saying <c>Scene '...' couldn't be loaded because it has not been added to the build settings or the AssetBundle has not been loaded.</c>
    /// </para>
    /// <para>
    /// A way to react on scene loading failure is to attach an log message output recorder
    /// to the application and see if an error occurs while the scene loading attemp is running.
    /// This is what this class does.
    /// </para>
    /// <para>
    /// Usage:
    /// <list type="bullets">
    /// <item>Create and start an instance of <c>WatchForError</c> right before the critical line of code. Use <c>WatchForError.startNew()</c> for this.</item>
    /// <item>Call <c>stop()</c> right after the critical line.</item>
    /// <item>Look at the number of <c>errors</c> and react accordingly.</item>
    /// </list>
    /// </para>
    /// <para>
    /// Example:<br/>
    /// <code>
    /// WatchForError watch = WatchForError.startNew();
    /// SceneManager.LoadScene("NonExistingLevel"); 
    /// if (watch.stop().errors>0) Debug.Log("Scene could not be loaded.");
    /// </code>
    /// </para>
    /// </remarks>
    public class WatchForError
    {
        /// <summary>
        /// Creates a new instance which is started automatically.
        /// </summary>
        /// <returns>New Watcher.</returns>
        public static WatchForError startNew()
        {
            return new WatchForError();
        }
        
        /// <summary>Number of errors recorded since start/creation of object.</summary>
        public int errors = 0;
        /// <summary>Number of warnings recorded since start/creation of object.</summary>
        public int warnings = 0;

        /// <summary>Creates a new instance and calls start().</summary>
        public WatchForError()
        {
            start();
        }
        /// <summary>Unregisters any listeners by calling stop() on destruction.</summary>
        ~WatchForError()
        {
            stop();
        }

        /// <summary>
        /// Starts recording of messages. It is <b>not</b> necessary to call
        /// this command usually, as it is automatically called on object creation.
        /// Use <c>start()</c> only to resume an observation after calling <c>stop()</c>.
        /// </summary>
        /// <returns>This object.</returns>
        public WatchForError start() 
        {
            Application.logMessageReceivedThreaded += Application_logMessageReceived;
            return this;
        }
        /// <summary>
        /// Stops recording of messages. Automatically called on destruction. 
        /// </summary>
        /// <returns>This object.</returns>
        public WatchForError stop()
        { 
            Application.logMessageReceivedThreaded -= Application_logMessageReceived;
            return this;
        }

        /// <summary>
        /// Implements the recording.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            switch(type)
            {
                case LogType.Error:
                    errors += 1;
                    break;
                case LogType.Warning:
                    warnings += 1;
                    break;
            }
        }

    }
}

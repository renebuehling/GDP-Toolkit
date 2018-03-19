using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
    /// Encapsulates simple http submit requests and adds some basic error handling.
    /// </summary>
    public class WebForm
    {
        /// <summary>
        /// Cache of the form data collected with Add methods.
        /// </summary>
        private List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        /// <summary>
        /// Cache of the URL to which the data is submitted.
        /// Defined in constructor.
        /// </summary>
        private string URL = "";

        /// <summary>
        /// Creates a new helper to post data to a www URL, i.e. a php script.
        /// </summary>
        /// <param name="URL">Address where the data is submitted to, something like http://www.example.com/file.php </param>
        public WebForm(string URL)
        {
            this.URL = URL;
        }

        /// <summary>
        /// Safely adds data to be submitted to the URL.
        /// This method skips the parameter if value is an empty string.
        /// When directly adding the data with a new MultipartFormDataSection object, 
        /// adding an empty parameter causes an error.
        /// </summary>
        /// <param name="key">Name of the parameter</param>
        /// <param name="value">Value to submit</param>
        public void Add(string key, string value)
        {
            if (value.Length < 1)
            {
                Debug.Log("Webform skipping empty parameter " + key);
            }
            else
                formData.Add(new MultipartFormDataSection(key, value));
        }
        /// <summary>
        /// Integer version of <see cref="Add(string, string)"/>.
        /// </summary>
        /// <param name="key">Name of the parameter</param>
        /// <param name="value">Value to submit</param>
        public void Add(string key, int value)
        {
            Add(key, value.ToString());
        }

        /// <summary>
        /// Any object version of <see cref="Add(string, string)"/>.
        /// </summary>
        /// <param name="key">Name of the parameter</param>
        /// <param name="value">Value to submit. If null, nothing happens, else result of toString() will be added.</param>
        public void Add(string key, object value)
        {
            if (value!=null) Add(key, value.ToString());
        }

        /// <summary>
        /// List version of <see cref="Add(string, object)"/>.
        /// </summary>
        /// <param name="items">Key-Value pairs to submit.</param>
        public void Add(IDictionary<string,object> items)
        {
            foreach(string key in items.Keys)
                Add(key, items[key]);
        }

        /// <summary>
        /// Contains the submission results. This field should always carry
        /// an object an never be null. You can use <code>result.isDone</code>
        /// to test if the submission was already completed.
        /// </summary>
        public DownloadHandlerBuffer result = new DownloadHandlerBuffer();

        /// <summary>
        /// Tiny helper that will be true if the submission was successful
        /// and false when there was an error. Note: This makes sense only
        /// after the submission was completed.
        /// </summary>
        public bool successful = false;

        /// <summary>
        /// Contains the recently recorded error, i.e. problems that caused
        /// <see cref="successful"/> to be <c>false</c>.
        /// Note that this might be updated late: It may contain old information
        /// while submission is still running.
        /// </summary>
        public string recentError = "";

        /// <summary>
        /// Text version of the content received. 
        /// This may be the html code of a site etc.
        /// In case of an error this might give additional insights on the problem.
        /// </summary>
        public string recentResult = "";

        /// <summary>
        /// Performs the request, waits for it and updates <see cref="successful"/>
        /// and <see cref="result"/>.
        /// </summary>
        /// <returns>Enumerator for Unity coroutine.</returns>
        public IEnumerator submit()
        {
            UnityWebRequest www = UnityWebRequest.Post(URL, formData);
            www.downloadHandler = result;
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                successful = false;
                recentError = www.responseCode+" "+www.error;
                Debug.Log(www.error);
            }
            else
            {
                successful = true;
                recentError = "";
            }
            try{
                recentResult = www.downloadHandler.text;
            }catch (System.Exception ex) { recentResult = "ERR: "+ex.Message;}
        }

        /// <summary>
        /// Tries to parse the result that the server sent back after <see cref="submit"/>
        /// from a JSON structure into an object.
        /// </summary>
        /// <typeparam name="T">Class that should be instantiated and populated with the data from received JSON structure.</typeparam>
        /// <param name="fallBackToNew">If true, a new object of class T will be created and returned in case of an error. Otherewise T's default will be returned (this is probably null).</param>
        /// <returns>A new instance of T which fields are initialized with the data from JSON or a fallback object in case of error (see parameter fallBackToNew).</returns>
        public T resultAsJSON<T>(bool fallBackToNew = false) where T : new()
        {
            T o = default(T);
            try
            {
                o = JsonUtility.FromJson<T>(result.text);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Could not interpret server answer: " + (result == null ? "(Result is null)" : result.text) + "\nError: " + ex.Message);
                if (fallBackToNew) o = new T();
            }
            return o;
        }
    }
}

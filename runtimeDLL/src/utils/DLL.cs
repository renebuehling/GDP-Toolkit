using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace GameDevProfi.Utils
{
    /// <summary>
    /// Contains helpers for working with a custom C# DLL.
    /// </summary>
    public class DLL
    {
        /// <summary>
        /// Creates a list of all resources like images that are embedded in the DLL.
        /// Use this method for debugging and find valid resourceName parameters
        /// for <see cref="LoadImageResource(string, int, int)"/>.
        /// </summary>
        /// <returns>A dump of the loadable embedded resources.</returns>
        public static string listAllResources()
        {
            Assembly a = Assembly.GetCallingAssembly(); // Assembly.GetExecutingAssembly();
            string[] names = a.GetManifestResourceNames();
            return string.Join("\n", names);            
        }

        /// <summary>
        /// Loads an image embedded in the DLL into a Unity texture.
        /// Use listAllResources() to find the resources available for load by this method. 
        /// Note: The method uses the assembly of the calling script, which usually is
        /// the DLL that you want resources to load from.
        /// </summary>
        /// <param name="resourceName">Name of the image to load. This is usally made of default project namespace (see project properties) plus folders (separated by dot) plus the actual filename.</param>
        /// <param name="width">Width of the result image. This is usually the same as the size of the resource itself.</param>
        /// <param name="height">Height of the result image. This is usually the same as the size of the resource itself.</param>
        /// <returns>Image as texture or null if not loadable.</returns>
        public static Texture2D LoadImageResource(string resourceName, int width, int height)
        {
            Assembly a = Assembly.GetCallingAssembly();
            Stream stream = a.GetManifestResourceStream(resourceName);
            Texture2D texture = null;
            if (stream == null)
            {
                Debug.LogWarning("DLL Resource not found: "+resourceName);
            }
            else
            {
                texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
                texture.hideFlags = HideFlags.HideAndDontSave;
                texture.LoadImage(ReadToEnd(stream));
                //if (texture == null)  Debug.LogError("Missing Dll resource: " + resourceName);
            }
            return texture;
        }

        /// <summary>
        /// Helper to read a the stream.
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Contents as byte array.</returns>
        private static byte[] ReadToEnd(Stream stream)
        {
            long originalPosition = stream.Position;
            stream.Position = 0;
            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;
                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }
    }
}

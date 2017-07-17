using UnityEngine;
using UnityEditor;

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
namespace GameDevProfi
{
    [InitializeOnLoad]
    class Main
    {
        public const string MenuFolderLabel = "GameDev-Profi";

        static Main()
        {
            Debug.Log("GDP-Toolkit-Editor loaded.");
        }
    }

}

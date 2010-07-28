﻿#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2010, DoLittle Studios
//
// Licensed under the Microsoft Permissive License (Ms-PL), Version 1.1 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the license at 
//
//   http://balder.codeplex.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion
#if(XNA)
using System;
using Balder.Imaging;

namespace Balder.Rendering.Xna
{
    public class ImageContext : IImageContext
    {
        public void SetFrame(byte[] frameBytes)
        {
            throw new NotImplementedException();
        }

        public void SetFrame(ImageFormat format, byte[] frameBytes)
        {
            throw new NotImplementedException();
        }

        public void SetFrame(ImageFormat format, byte[] frameBytes, ImagePalette palette)
        {
            throw new NotImplementedException();
        }

        public int[] GetPixelsAs32BppARGB()
        {
            throw new NotImplementedException();
        }

        public ImageFormat[] SupportedImageFormats
        {
            get { throw new NotImplementedException(); }
        }
    }
}
#endif
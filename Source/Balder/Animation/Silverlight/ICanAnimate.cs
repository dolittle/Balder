#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Author: Ionescu Marius <yonescu_marius@yahoo.com>
// Copyright (c) 2007-2011, DoLittle Studios
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
using System;
using System.Collections.Generic;
using System.Windows.Media.Animation;
using Balder.Math;
using System.Windows;

namespace Balder.Animation.Silverlight
{
    /// <summary>
    /// Defines an asset
    /// </summary>
    /// 

    public interface ICanAnimate
    {
        /// <summary>
        /// Start Animation get; set;
        /// </summary>
        bool StartAnimation { get; set; }

        /// <summary>
        /// Stop Animation get; set;
        /// </summary>
        bool StopAnimation { get; set; }

        /// <summary>
        /// Gets or sets the duration of the animation
        /// </summary>
        public Duration Duration { get; set; }

        /// <summary>
        /// Gets or sets the target mesh name of the object
        /// </summary>
        public string TargetMeshAnimation { get; set; }

        /// <summary>
        /// Gets or sets the keyframe <see cref="keyframe"/> to animate
        /// </summary>
        public string[] keysFrame { get; set; }
    }
}
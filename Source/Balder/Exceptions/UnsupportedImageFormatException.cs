#region License
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
using System;
using Balder.Core.Imaging;

namespace Balder.Core.Exceptions
{
	/// <summary>
	/// The exception that is thrown when a specific <see cref="ImageFormat"/> is not supported
	/// </summary>
	public class UnsupportedImageFormatException : ArgumentException
	{
		/// <summary>
		/// Initializes a new instance of <see cref="UnsupportedImageFormatException"/>
		/// </summary>
		/// <param name="format">Format that was not supported</param>
		public UnsupportedImageFormatException(ImageFormat format)
			: base("Unsupported ImageFormat ("+format.ToString()+")")
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="UnsupportedImageFormatException"/>
		/// </summary>
		/// <param name="format">Format that was not supported</param>
		/// <param name="message">Message to show</param>
		public UnsupportedImageFormatException(ImageFormat format, string message)
			: base("Unsupported ImageFormat (" + format.ToString() + ") - Message: "+message)
		{
		}
	}
}

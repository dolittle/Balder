#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
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
#if(SILVERLIGHT)
using System;
using System.ComponentModel;
using System.Globalization;
using Balder.Content;
using Balder.Execution;
using Balder.Imaging;
using Ninject;

namespace Balder.Silverlight.TypeConverters
{
	public class UriToImageTypeConverter : TypeConverter
	{
		private readonly IContentManager _contentManager;

		public UriToImageTypeConverter()
			: this(Runtime.Instance.Kernel.Get<IContentManager>())
		{
		}

		public UriToImageTypeConverter(IContentManager contentManager)
		{
			_contentManager = contentManager;
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType.Equals(typeof(string));
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if( value is string )
			{
				var uri = value as string;
				var images = _contentManager.Load<Image>(uri);
				return images;
			}
			return null;
		}
	}
}
#endif
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Balder.Execution;
using Balder.Math;

namespace Balder.Rendering.Silverlight
{
	[Singleton]
	public class GeometryDebugInfo : IGeometryDebugInfo
	{
		private readonly Canvas[] _displayElements;
		private Canvas _frontDisplayElement;
		private Canvas _backDisplayElement;
		private int _currentFrame;

		private static readonly Brush TextBrush = new SolidColorBrush(System.Windows.Media.Colors.Green);

		public GeometryDebugInfo()
		{
			_displayElements = new Canvas[2];
			_displayElements[0] = new Canvas();
			_displayElements[1] = new Canvas();

			NewFrame();
		}

		public void NewFrame()
		{
			_currentFrame ^= 1;
			_frontDisplayElement = _displayElements[_currentFrame];
			_backDisplayElement = _displayElements[_currentFrame^1];
			_backDisplayElement.Children.Clear();
		}

		public void AddVertex(Vector transformedVector, Vector projectedVector)
		{
			var textBlock = new TextBlock();
			textBlock.Foreground = TextBrush;
			textBlock.Inlines.Add(string.Format("Transformed : {0}", transformedVector));
			textBlock.Inlines.Add(new LineBreak());
			textBlock.Inlines.Add(string.Format("Projected : {0}", projectedVector));
			Canvas.SetLeft(textBlock, projectedVector.X);
			Canvas.SetTop(textBlock, projectedVector.Y);
			_backDisplayElement.Children.Add(textBlock);
		}

		public UIElement DisplayElement { get { return _frontDisplayElement; }}
	}
}

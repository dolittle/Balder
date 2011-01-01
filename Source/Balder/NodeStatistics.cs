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
using System.Linq.Expressions;
using Balder.Extensions.Silverlight;
#endif
using Balder.Execution;
using Balder.Diagnostics;

namespace Balder
{
	public class NodeStatistics
#if(SILVERLIGHT)
			: INotifyPropertyChanged
#endif
	{
#if(SILVERLIGHT)
		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { }; 
#endif
		private long _nodeStartTime;
		private long _childrenStartTime;

		protected IStopwatch Stopwatch
		{
			get { return Runtime.Instance.Stopwatch; }
		}


		private long _timeSpentInNode;
		public long TimeSpentInNode
		{
			get { return _timeSpentInNode; }
			set
			{
				_timeSpentInNode = value;
#if(SILVERLIGHT)
				OnPropertyChanged("TimeSpentInNode");
#endif
			}
		}

		private long _timeSpentInChildren;
		public long TimeSpentInChildren
		{
			get { return _timeSpentInChildren; }
			set
			{
				_timeSpentInChildren = value;
#if(SILVERLIGHT)
				OnPropertyChanged("TimeSpentInChildren");
#endif
			}
		}

		public void BeginNodeTiming()
		{
			_nodeStartTime = Stopwatch.ElapsedMilliseconds;
		}

		public void EndNodeTiming()
		{
			TimeSpentInNode = Stopwatch.ElapsedMilliseconds - _nodeStartTime;
		}

		public void BeginChildrenTiming()
		{
			_childrenStartTime = Stopwatch.ElapsedMilliseconds;
		}

		public void EndChildrenTiming()
		{
			TimeSpentInChildren = Stopwatch.ElapsedMilliseconds - _childrenStartTime;
		}


#if(SILVERLIGHT)
		protected void OnPropertyChanged(string propertyName)
		{
			if( null != PropertyChanged )
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
#endif
	}

	
}
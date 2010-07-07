using System;
using System.IO;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace Balder.Build.Tasks
{
	[TaskName("ReadLastBuildDate")]
	public class ReadLastBuildDate : Task
	{
		protected override void ExecuteTask()
		{
			var dateString = new DateTime(2010,2,1).ToString(Constants.DateFormat);
			if( File.Exists(Constants.LastBuildFile))
			{
				dateString = File.ReadAllText(Constants.LastBuildFile);
			}
			Project.Properties[Constants.LastBuildVariable] = dateString;
		}
	}
}

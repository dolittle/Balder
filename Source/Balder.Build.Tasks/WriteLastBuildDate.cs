using System;
using System.IO;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace Balder.Build.Tasks
{
	[TaskName("WriteCurrentBuildDate")]
	public class WriteCurrentBuildDate : Task
	{
		protected override void ExecuteTask()
		{
			var dateString = new DateTime(2010, 2, 1).ToString(Constants.DateFormat);
			if( Project.Properties.Contains(Constants.CurrentDate) )
			{
				dateString = Project.Properties[Constants.CurrentDate];
			}
			File.WriteAllText(Constants.LastBuildFile,dateString);
		}
	}
}
using System;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace Balder.Build.Tasks
{
	[TaskName("GetCurrentDate")]
	public class GetCurrentDate : Task
	{
		protected override void ExecuteTask()
		{
			var dateString = DateTime.Now.ToString(Constants.DateFormat);
			Project.Properties[Constants.CurrentDate] = dateString;
		}
	}
}
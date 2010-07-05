using System;
using System.IO;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace Balder.Build.Tasks
{
	[TaskName("GetVersion")]
	public class GetVersion : Task
	{
		protected override void ExecuteTask()
		{
			var version = "0.0.0.0";
			if( File.Exists(Constants.VersionFile) )
			{
				version = File.ReadAllText(Constants.VersionFile);
			} else
			{
				File.WriteAllText(Constants.VersionFile, version);
			}

			var v = new Version(version);
			if( v.Revision == 0 )
			{
				Project.Properties[Constants.ShouldDeployVariable] = true.ToString();
			} else
			{
				Project.Properties[Constants.ShouldDeployVariable] = false.ToString();
			}

			Project.Properties[Constants.VersionVariable] = version;
		}
	}
}

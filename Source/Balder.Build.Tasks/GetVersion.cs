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
			string version = "0.0.0.0";
			if( File.Exists(Constants.VersionFile) )
			{
				version = File.ReadAllText(Constants.VersionFile);
			} else
			{
				File.WriteAllText(Constants.VersionFile, version);
			}
			Project.Properties[Constants.VersionVariable] = version;
		}
	}
}

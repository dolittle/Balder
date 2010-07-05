using System;
using System.IO;
using System.Linq;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace Balder.Build.Tasks
{
	[TaskName("IncreaseVersion")]
	public class IncreaseVersion : Task
	{
		protected override void ExecuteTask()
		{
			if( File.Exists(Constants.VersionFile))
			{
				var version = File.ReadAllText(Constants.VersionFile);

				Console.WriteLine("Current : "+version);
				var numbers = version.Split('.');

				var numbersAsIntegers = new int[numbers.Length];

				for( var numberIndex=0; numberIndex<numbers.Length; numberIndex++ )
				{
					numbersAsIntegers[numberIndex] = Convert.ToInt32(numbers[numberIndex]);
				}

				numbersAsIntegers[numbersAsIntegers.Length - 1]++;
				
				var newVersion = string.Empty;
				foreach( var number in numbersAsIntegers )
				{
					if( !string.IsNullOrEmpty(newVersion) )
					{
						newVersion += ".";
					}
					newVersion += number.ToString();
				}

				File.WriteAllText(Constants.VersionFile,newVersion);
			}
		}
	}
}

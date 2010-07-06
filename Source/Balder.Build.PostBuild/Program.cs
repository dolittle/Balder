using System;
using System.IO;

namespace Balder.Build.PostBuild
{
	public class Program
	{
		public const string CurrentVersionFile = "current_version.txt";
		public const string DropBoxPath = "c:\\DropBox\\My DropBox\\Public\\Balder\\Builds";
		static int Main(string[] args)
		{
			Console.WriteLine("*** Copy ChangeLog file ***");
			if( File.Exists(CurrentVersionFile) && args.Length > 0 &&
				File.Exists(args[0]))
			{
				var version = File.ReadAllText(CurrentVersionFile);
				Console.WriteLine("  Current version : " + version);
				Console.WriteLine("  Copying : "+args[0]);
				var destination = DropBoxPath + "\\" + version+"\\changelog.xml";
				Console.WriteLine("  Destination : "+destination);
				try
				{
					File.Copy(args[0], destination);
				} catch(Exception ex)
				{
					Console.WriteLine("  Error : "+ex.Message);
					return -1;
				}
			} else
			{
				Console.WriteLine("  - Error : File doesn't exist");
				return -1;
			}

			return 0;
		}
	}
}

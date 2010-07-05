using System;
using NAnt.Core.Attributes;
using NAnt.Core.Types;

namespace Balder.Build.Tasks
{
	[Serializable, ElementName("MergedFileSet")]
	public class MergedFileSet : FileSet
	{
		[BuildElement("fileset")]
		public void AddFileSetRef(FileSet fileSetRef)
		{
			foreach( var include in fileSetRef.Includes )
			{
				Includes.Add(include);
			}
		}
	}
}
using NUnit.Framework;

namespace Balder.Build.Tasks.Tests
{
	[TestFixture]
	public class CreateWorkItemsReportTests
	{
		[Test]
		public void Test()
		{
			var task = new CreateWorkItemsReport();
			task.ExecuteTask();
			
		}
	}
}

using System;
using System.Data;
using System.Net;
using System.Security.Principal;
using Balder.Build.Tasks.WorkItems;

namespace Balder.Build.Tasks
{
	public class CreateWorkItemsReport //: Task
	{
		public void ExecuteTask()
		{
			var proxy = new ClientServiceSoapClient();
			proxy.ClientCredentials.Windows.ClientCredential = new NetworkCredential("adept_cp", "0512ageh", "snd");
			proxy.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Identification;
			
			var header = new RequestHeader();

			var dataSet = new DataSet();
			var date = string.Empty;
			
			
			
			


			var result = proxy.GetWorkItem(header, 13596, 1, 0, DateTime.Now, true, new MetadataTableHaveEntry[] {}, out date, out dataSet);
			
		}
	}
}

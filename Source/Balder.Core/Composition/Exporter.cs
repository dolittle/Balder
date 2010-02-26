using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using Balder.Core.Execution;

namespace Balder.Core.Composition
{
	public class Exporter : ExportProvider
	{
		private readonly IObjectFactory _objectFactory;

		public Exporter(IObjectFactory objectFactory)
		{
			_objectFactory = objectFactory;
		}

		

		protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
		{
			throw new NotImplementedException();
		}
	}
}
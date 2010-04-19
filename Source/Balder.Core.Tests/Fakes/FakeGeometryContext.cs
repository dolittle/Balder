using System;
using Balder.Core.Display;
using Balder.Core.Materials;
using Balder.Core.Objects.Geometries;
using Balder.Core.Rendering;

namespace Balder.Core.Tests.Fakes
{
	public class FakeGeometryContext : IGeometryContext
	{
		public void SetMaterialForAllFaces(Material material)
		{
			throw new NotImplementedException();
		}

		public void GenerateDetailLevel(DetailLevel targetLevel, DetailLevel sourceLevel)
		{
			throw new NotImplementedException();
		}

		public IGeometryDetailLevel GetDetailLevel(DetailLevel level)
		{
			throw new NotImplementedException();
		}

		public void Render(Viewport viewport, INode node, DetailLevel detailLevel)
		{
			throw new NotImplementedException();
		}

		public bool HasDetailLevel(DetailLevel level)
		{
			throw new NotImplementedException();
		}
	}
}

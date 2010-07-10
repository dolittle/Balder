using System;
using Balder.Core.Display;
using Balder.Core.Materials;
using Balder.Core.Objects.Geometries;
using Balder.Core.Rendering;

namespace Balder.Core.Tests.Fakes
{
	public class FakeGeometryContext : IGeometryContext
	{
		private FakeGeometryDetailLevel dl = new FakeGeometryDetailLevel();


		public void SetMaterialForAllFaces(Material material)
		{
			
		}

		public void GenerateDetailLevel(DetailLevel targetLevel, DetailLevel sourceLevel)
		{
			
		}

		public IGeometryDetailLevel GetDetailLevel(DetailLevel level)
		{
			return dl;
		}

		public void Render(Viewport viewport, INode node, DetailLevel detailLevel)
		{
			
		}

		public bool HasDetailLevel(DetailLevel level)
		{
			return true;
		}
	}
}

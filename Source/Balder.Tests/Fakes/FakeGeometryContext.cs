using System;
using Balder.Display;
using Balder.Materials;
using Balder.Objects.Geometries;
using Balder.Rendering;

namespace Balder.Tests.Fakes
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

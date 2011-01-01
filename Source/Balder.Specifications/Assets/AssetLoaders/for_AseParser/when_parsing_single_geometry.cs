using Machine.Specifications;

namespace Balder.Specifications.Assets.AssetLoaders.for_AseParser
{
	[Subject(Subjects.Parsing)]
	public class when_parsing_single_and_simple_geometry : given.an_ase_parser
	{
		Because of = () => { };

		It should_return_one_geometry = () => { };
		It should_load_vertices = () => { };
		It should_load_faces = () => { };
		It should_load_transform = () => { };
		It should_load_name = () => { };
	}

	[Subject(Subjects.Parsing)]
	public class when_parsing_geometry_with_texture_coordinates : given.an_ase_parser
	{
		Because of = () => { };

		It should_load_texture_coordinates = () => { };
	}

	[Subject(Subjects.Parsing)]
	public class when_parsing_geometry_with_normals : given.an_ase_parser
	{
		Because of = () => { };

		It should_load_vertex_normals = () => { };
		It should_load_face_normals = () => { };
	}

	[Subject(Subjects.Parsing)]
	public class when_parsing_two_geometries : given.an_ase_parser
	{
		Because of = () => { };

		It should_return_two_geometries = () => { };
		It should_set_position_correct_for_both_geometries = () => { };
	}
}

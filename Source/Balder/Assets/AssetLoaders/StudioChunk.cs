namespace Balder.Assets.AssetLoaders
{
	// Based on code from http://helixtoolkit.codeplex.com/

	public enum StudioChunk
	{
		// Primary chunk

		MAIN3DS = 0x4D4D,

		// Main Chunks

		EDIT3DS = 0x3D3D, // this is the start of the editor config
		KEYF3DS = 0xB000, // this is the start of the keyframer config
		VERSION = 0x0002,
		MESHVERSION = 0x3D3E,

		// sub defines of EDIT3DS

		EDIT_MATERIAL = 0xAFFF,
		EDIT_CONFIG1 = 0x0100,
		EDIT_CONFIG2 = 0x3E3D,
		EDIT_VIEW_P1 = 0x7012,
		EDIT_VIEW_P2 = 0x7011,
		EDIT_VIEW_P3 = 0x7020,
		EDIT_VIEW1 = 0x7001,
		EDIT_BACKGR = 0x1200,
		EDIT_AMBIENT = 0x2100,
		EDIT_OBJECT = 0x4000,

		EDIT_UNKNW01 = 0x1100,
		EDIT_UNKNW02 = 0x1201,
		EDIT_UNKNW03 = 0x1300,
		EDIT_UNKNW04 = 0x1400,
		EDIT_UNKNW05 = 0x1420,
		EDIT_UNKNW06 = 0x1450,
		EDIT_UNKNW07 = 0x1500,
		EDIT_UNKNW08 = 0x2200,
		EDIT_UNKNW09 = 0x2201,
		EDIT_UNKNW10 = 0x2210,
		EDIT_UNKNW11 = 0x2300,
		EDIT_UNKNW12 = 0x2302,
		EDIT_UNKNW13 = 0x3000,
		EDIT_UNKNW14 = 0xAFFF,

		// sub defines of EDIT_MATERIAL
		MAT_NAME01 = 0xA000,
		MAT_LUMINANCE = 0xA010,
		MAT_DIFFUSE = 0xA020,
		MAT_SPECULAR = 0xA030,
		MAT_SHININESS = 0xA040,
		MAT_MAP = 0xA200,
		MAT_MAPFILE = 0xA300,

		// sub defines of EDIT_OBJECT
		OBJ_TRIMESH = 0x4100,
		OBJ_LIGHT = 0x4600,
		OBJ_CAMERA = 0x4700,

		OBJ_UNKNWN01 = 0x4010,
		OBJ_UNKNWN02 = 0x4012, // Could be shadow

		// sub defines of OBJ_CAMERA
		CAM_UNKNWN01 = 0x4710,
		CAM_UNKNWN02 = 0x4720,

		// sub defines of OBJ_LIGHT
		LIT_OFF = 0x4620,
		LIT_SPOT = 0x4610,
		LIT_UNKNWN01 = 0x465A,

		// sub defines of OBJ_TRIMESH
		TRI_VERTEXL = 0x4110,
		TRI_FACEL2 = 0x4111,
		TRI_FACEL1 = 0x4120,
		TRI_FACEMAT = 0x4130,
		TRI_TEXCOORD = 0x4140,
		TRI_SMOOTH = 0x4150,
		TRI_LOCAL = 0x4160,
		TRI_VISIBLE = 0x4165,

		// sub defs of KEYF3DS

		KEYF_UNKNWN01 = 0xB009,
		KEYF_UNKNWN02 = 0xB00A,
		KEYF_FRAMES = 0xB008,
		KEYF_OBJDES = 0xB002,
		KEYF_HIERARCHY = 0xB030,
		KFNAME = 0xB010,

		//  these define the different color chunk types
		COL_RGB = 0x0010,
		COL_TRU = 0x0011, // RGB24
		COL_UNK = 0x0013,

		// defines for viewport chunks

		TOP = 0x0001,
		BOTTOM = 0x0002,
		LEFT = 0x0003,
		RIGHT = 0x0004,
		FRONT = 0x0005,
		BACK = 0x0006,
		USER = 0x0007,
		CAMERA = 0x0008, // = 0xFFFF is the actual code read from file
		LIGHT = 0x0009,
		DISABLED = 0x0010,
		BOGUS = 0x0011,
	}
}
#region License

//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2010, DoLittle Studios
//
// Licensed under the Microsoft Permissive License (Ms-PL), Version 1.1 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the license at 
//
//   http://balder.codeplex.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

#endregion

using System;

namespace Balder.Core
{
	public static class ColorOperations
	{
		private static byte[,] AdditiveTable;

		private const int DimensionSize = 256;

		static ColorOperations()
		{
			InitializeAdditive();
		}

		private static void InitializeAdditive()
		{
			AdditiveTable = new byte[DimensionSize, DimensionSize];
			for( var x=0; x<DimensionSize; x++ )
			{
				for( var y=0; y<DimensionSize; y++ )
				{
					var added = x + y;
					if( added > byte.MaxValue )
					{
						added = byte.MaxValue;
					}
					AdditiveTable[x, y] = (byte) added;
				}
			}
		}


		public static byte AddValues(byte value1, byte value2)
		{
			return AdditiveTable[value1,value2];
		}

		public static Color AddColors(Color color1, Color color2)
		{
			var result = new Color
			             	{

			             	};
			return result;
			
			
		}
	}
}

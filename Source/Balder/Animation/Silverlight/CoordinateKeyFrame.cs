#region License

//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Author: Ionescu M. Marius <yonescu_marius@yahoo.com>
// Copyright (c) 2007-2011, DoLittle Studios, Informaticha Studios
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
#if(XAML)
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Balder.Math;
using System.Collections;
using System.Globalization;

namespace Balder.Animation.Silverlight
{
	/// <summary>
	/// Represents keyframe coordinate <see cref="Coordinate"/>
	/// </summary>
	public class CoordinateKeyFrame
	{
		/// <summary>
		/// Gets or sets the keyframe <see cref="keyframe"/> to animate
		/// </summary>
		public string[] keysFrame { get; set; }

        public string[] taggedKeys { get; set; }

        public string[] storedFrame { get; set; }

        public string[][] keysVisited { get; set; }

        public string[][] unusualKeys { get; set; }

        /// <summary>
        /// Search string in array of strings.
        /// </summary>
        /// <param name="a">Vector of search.</param>
        /// <param name="x">Value searched.</param>
        /// <returns>Return position of item searched.</returns>
        public static int ArrayIndexOf(string[] a, string x)
        {
            if (a == null)
                return -1;
            for (int i = 0; i < a.Length; ++i)
                if (a[i] == x)
                    return i;
            return -1;
        }

        /// <summary>
        /// Cauta un string intr-o coloana a unei matrice de string-uri.
        /// </summary>
        /// <param name="a">Matricea in care se va efectua cautarea.</param>
        /// <param name="x">Valoarea cautata.</param>
        /// <param name="p">Indicele coloanei pe care se va efectua cautarea.</param>
        /// <returns>Pozitia in care a fost gasita valoarea cautata (0-based), sau -1 daca x nu apare in a[][p].</returns>
        public static int ArrayIndexOfPos(string[][] a, string x, int p)
        {
            if (a == null)
                return -1;
            for (int i = 0; i < a.Length; ++i)
                if (a[i].Length > p && a[i][p] == x)
                    return i;
            return -1;
        }

        /// <summary>
        /// Number format digits.
        /// </summary>
        /// <param name="cc">N/A -yet.</param>
        /// <returns>N/A -yet</returns>
        public static string GetNumberWithDigits(object o, int NumberDecimalDigits, string NumberDecimalSeparator, string NumberGroupSeparator)
        {
            NumberFormatInfo nfi = new CultureInfo("en-US").NumberFormat;
            nfi.NumberDecimalDigits = NumberDecimalDigits;
            nfi.NumberDecimalSeparator = NumberDecimalSeparator;
            nfi.NumberGroupSeparator = NumberGroupSeparator;

            return Convert.ToDecimal(o).ToString("N", nfi);
        }
	}
}
#endif
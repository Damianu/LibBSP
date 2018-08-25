using System;
using System.Collections.Generic;

namespace LibBSP {

	/// <summary>
	/// Holds the data used by the brush side structures of all formats of BSP.
	/// </summary>
	public struct BrushSide {

		public int plane { get; private set; }
		public float dist { get; private set; }
		public int texture { get; private set; } // -1 is a valid texture index in Quake 2. However it means "unused" there
		public int face { get; private set; }
		public int displacement { get; private set; } // In theory, this should always point to the side's displacement info. In practice, displacement brushes are removed on compile, leaving only the faces.
		public bool bevel { get; private set; }

		/// <summary>
		/// Creates a new <see cref="BrushSide"/> object from a <c>byte</c> array.
		/// </summary>
		/// <param name="data"><c>byte</c> array to parse.</param>
		/// <param name="type">The map type.</param>
		/// <param name="version">The version of this lump.</param>
		/// <exception cref="ArgumentNullException"><paramref name="data" /> was <c>null</c>.</exception>
		/// <exception cref="ArgumentException">This structure is not implemented for the given maptype.</exception>
		public BrushSide(byte[] data, MapType type, int version = 0) : this() {
			if (data == null) {
				throw new ArgumentNullException();
			}
			plane = -1;
			dist = -1;
			texture = -1;
			face = -1;
			displacement = -1;
			bevel = false;
			switch (type) {
				case MapType.CoD:
				case MapType.CoD2:
				case MapType.CoD4: {
					// Call of Duty's format sucks. The first field is either a float or an int
					// depending on whether or not it's one of the first six sides in a brush.
					// Store both possibilities, and the algorithm will determine which to use.
					dist = BitConverter.ToSingle(data, 0);
					goto case MapType.Quake3;
				}
				case MapType.Quake3:
				case MapType.FAKK:
				case MapType.STEF2Demo:
				case MapType.MOHAA: {
					plane = BitConverter.ToInt32(data, 0);
					texture = BitConverter.ToInt32(data, 4);
					break;
				}
				case MapType.STEF2: {
					texture = BitConverter.ToInt32(data, 0);
					plane = BitConverter.ToInt32(data, 4);
					break;
				}
				case MapType.Raven: {
					plane = BitConverter.ToInt32(data, 0);
					texture = BitConverter.ToInt32(data, 4);
					face = BitConverter.ToInt32(data, 8);
					break;
				}
				case MapType.Source17:
				case MapType.Source18:
				case MapType.Source19:
				case MapType.Source20:
				case MapType.Source21:
				case MapType.Source22:
				case MapType.Source23:
				case MapType.Source27:
				case MapType.L4D2:
				case MapType.TacticalInterventionEncrypted:
				case MapType.DMoMaM: {
					this.displacement = BitConverter.ToInt16(data, 4);
					// In little endian format, this byte takes the least significant bits of a short
					// and can therefore be used for all Source engine formats, including Portal 2.
					this.bevel = data[6] > 0;
					goto case MapType.SiN;
				}
				case MapType.SiN:
				case MapType.Quake2:
				case MapType.Daikatana:
				case MapType.SoF: {
					plane = BitConverter.ToUInt16(data, 0);
					texture = BitConverter.ToInt16(data, 2);
					break;
				}
				case MapType.Vindictus: {
					plane = BitConverter.ToInt32(data, 0);
					texture = BitConverter.ToInt32(data, 4);
					displacement = BitConverter.ToInt32(data, 8);
					bevel = data[12] > 0;
					break;
				}
				case MapType.Nightfire: {
					face = BitConverter.ToInt32(data, 0);
					plane = BitConverter.ToInt32(data, 4);
					break;
				}
				default: {
					throw new ArgumentException("Map type " + type + " isn't supported by the BrushSide class.");
				}
			}
		}

		/// <summary>
		/// Factory method to parse a <c>byte</c> array into a <c>List</c> of <see cref="BrushSide"/> objects.
		/// </summary>
		/// <param name="data">The data to parse.</param>
		/// <param name="type">The map type.</param>
		/// <param name="version">The version of this lump.</param>
		/// <returns>A <c>List</c> of <see cref="BrushSide"/> objects.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="data" /> was <c>null</c>.</exception>
		/// <exception cref="ArgumentException">This structure is not implemented for the given maptype.</exception>
		public static List<BrushSide> LumpFactory(byte[] data, MapType type, int version = 0) {
			if (data == null) {
				throw new ArgumentNullException();
			}
			int structLength = 0;
			switch (type) {
				case MapType.Quake2:
				case MapType.Daikatana:
				case MapType.SoF: {
					structLength = 4;
					break;
				}
				case MapType.CoD:
				case MapType.CoD2:
				case MapType.CoD4:
				case MapType.SiN:
				case MapType.Nightfire:
				case MapType.Quake3:
				case MapType.STEF2:
				case MapType.STEF2Demo:
				case MapType.Source17:
				case MapType.Source18:
				case MapType.Source19:
				case MapType.Source20:
				case MapType.Source21:
				case MapType.Source22:
				case MapType.Source23:
				case MapType.Source27:
				case MapType.L4D2:
				case MapType.TacticalInterventionEncrypted:
				case MapType.DMoMaM:
				case MapType.FAKK: {
					structLength = 8;
					break;
				}
				case MapType.MOHAA:
				case MapType.Raven: {
					structLength = 12;
					break;
				}
				case MapType.Vindictus: {
					structLength = 16;
					break;
				}
				default: {
					throw new ArgumentException("Map type " + type + " isn't supported by the BrushSide lump factory.");
				}
			}
			int numObjects = data.Length / structLength;
			List<BrushSide> lump = new List<BrushSide>(numObjects);
			byte[] bytes = new byte[structLength];
			for (int i = 0; i < numObjects; i++) {
				Array.Copy(data, (i * structLength), bytes, 0, structLength);
				lump.Add(new BrushSide(bytes, type, version));
			}
			return lump;
		}

		/// <summary>
		/// Gets the index for this lump in the BSP file for a specific map format.
		/// </summary>
		/// <param name="type">The map type.</param>
		/// <returns>Index for this lump, or -1 if the format doesn't have this lump.</returns>
		public static int GetIndexForLump(MapType type) {
			switch (type) {
				case MapType.CoD: {
					return 3;
				}
				case MapType.CoD4:
				case MapType.CoD2: {
					return 5;
				}
				case MapType.Raven:
				case MapType.Quake3: {
					return 9;
				}
				case MapType.FAKK: {
					return 10;
				}
				case MapType.MOHAA: {
					return 11;
				}
				case MapType.STEF2:
				case MapType.STEF2Demo: {
					return 12;
				}
				case MapType.Quake2:
				case MapType.SiN:
				case MapType.Daikatana:
				case MapType.SoF: {
					return 15;
				}
				case MapType.Nightfire: {
					return 16;
				}
				case MapType.Vindictus:
				case MapType.TacticalInterventionEncrypted:
				case MapType.Source17:
				case MapType.Source18:
				case MapType.Source19:
				case MapType.Source20:
				case MapType.Source21:
				case MapType.Source22:
				case MapType.Source23:
				case MapType.Source27:
				case MapType.L4D2:
				case MapType.DMoMaM: {
					return 19;
				}
				default: {
					return -1;
				}
			}
		}
	}
}

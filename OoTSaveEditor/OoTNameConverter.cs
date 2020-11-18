using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.N64.OoTSaveEditor {
	public static class OoTNameConverter {
		private static Dictionary<byte, char> InternalToChar = BuildInternalToChar();
		private static Dictionary<byte, char> BuildInternalToChar() {
			var d = new Dictionary<byte, char>();
			for (int i = 0; i < 26; ++i) {
				d.Add((byte)(i + 0xAB), (char)('A' + i));
			}
			for (int i = 0; i < 26; ++i) {
				d.Add((byte)(i + 0xC5), (char)('a' + i));
			}
			for (int i = 0; i < 10; ++i) {
				d.Add((byte)i, (char)('0' + i));
			}
			d.Add((byte)0xea, '.');
			d.Add((byte)0xe4, '-');
			d.Add((byte)0xdf, ' ');
			return d;
		}

		public static string StringFromInternal(uint p1, uint p2) {
			byte[] bytes = new byte[8];
			bytes[0] = (byte)((p1 & 0xff000000u) >> 24);
			bytes[1] = (byte)((p1 & 0x00ff0000u) >> 16);
			bytes[2] = (byte)((p1 & 0x0000ff00u) >> 8);
			bytes[3] = (byte)((p1 & 0x000000ffu));
			bytes[4] = (byte)((p2 & 0xff000000u) >> 24);
			bytes[5] = (byte)((p2 & 0x00ff0000u) >> 16);
			bytes[6] = (byte)((p2 & 0x0000ff00u) >> 8);
			bytes[7] = (byte)((p2 & 0x000000ffu));

			StringBuilder sb = new StringBuilder();
			foreach (byte b in bytes) {
				char v;
				if (InternalToChar.TryGetValue(b, out v)) {
					sb.Append(v);
				} else {
					sb.AppendFormat("\\{x2}", b);
				}
			}

			return sb.ToString().TrimEnd();
		}

		public static (uint p1, uint p2) StringToInternal(string s) {
			if (s.Length > 8) {
				throw new Exception("Name too long, max 8 chars.");
			}

			byte[] bytes = new byte[8] { 0xdf, 0xdf, 0xdf, 0xdf, 0xdf, 0xdf, 0xdf, 0xdf };
			int idx = 0;
			foreach (byte b in ConvertToInternalBytes(s)) {
				bytes[idx] = b;
				++idx;
			}


			uint p1 = (uint)(bytes[0] << 24) | (uint)(bytes[1] << 16) | (uint)(bytes[2] << 8) | (uint)(bytes[3]);
			uint p2 = (uint)(bytes[4] << 24) | (uint)(bytes[5] << 16) | (uint)(bytes[6] << 8) | (uint)(bytes[7]);
			return (p1, p2);
		}

		private static IEnumerable<byte> ConvertToInternalBytes(string s) {
			for (int i = 0; i < s.Length; ++i) {
				if (s[i] == '\\') {
					char hex0 = s[i + 1];
					char hex1 = s[i + 2];
					byte b = 0;
					switch (hex0) {
						case '0': b = 0x00; break;
						case '1': b = 0x10; break;
						case '2': b = 0x20; break;
						case '3': b = 0x30; break;
						case '4': b = 0x40; break;
						case '5': b = 0x50; break;
						case '6': b = 0x60; break;
						case '7': b = 0x70; break;
						case '8': b = 0x80; break;
						case '9': b = 0x90; break;
						case 'A': case 'a': b = 0xa0; break;
						case 'B': case 'b': b = 0xb0; break;
						case 'C': case 'c': b = 0xc0; break;
						case 'D': case 'd': b = 0xd0; break;
						case 'E': case 'e': b = 0xe0; break;
						case 'F': case 'f': b = 0xf0; break;
					}
					switch (hex1) {
						case '0': b += 0x00; break;
						case '1': b += 0x01; break;
						case '2': b += 0x02; break;
						case '3': b += 0x03; break;
						case '4': b += 0x04; break;
						case '5': b += 0x05; break;
						case '6': b += 0x06; break;
						case '7': b += 0x07; break;
						case '8': b += 0x08; break;
						case '9': b += 0x09; break;
						case 'A': case 'a': b += 0x0a; break;
						case 'B': case 'b': b += 0x0b; break;
						case 'C': case 'c': b += 0x0c; break;
						case 'D': case 'd': b += 0x0d; break;
						case 'E': case 'e': b += 0x0e; break;
						case 'F': case 'f': b += 0x0f; break;
					}
					yield return b;

					i += 2;
				} else {
					// this is very inefficient but for 8 chars whatever...
					bool found = false;
					foreach (var kvp in InternalToChar) {
						if (kvp.Value == s[i]) {
							yield return kvp.Key;
							found = true;
							break;
						}
					}
					if (!found) {
						throw new Exception("Illegal char.");
					}
				}
			}
		}
	}
}

/* Based on the Base 64 VLQ implementation in Closure Compiler:
 * https://github.com/google/closure-compiler/blob/master/src/com/google/debugging/sourcemap/Base64VLQ.java
 *
 * Copyright 2011 The Closure Compiler Authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System.Text;

namespace SourcemapToolkit.SourcemapParser
{
	/// <summary>
	/// This class provides a mechanism for converting an interger to Base64 Variable-length quantity (VLQ)
	/// </summary>
	internal static class Base64VlqEncoder
	{
		public static void Encode(StringBuilder output, int value)
		{
			var vlq = ToVlqSigned(value);

			do
			{
				var maskResult = vlq & Base64VlqConstants.VlqBaseMask;
				vlq >>= Base64VlqConstants.VlqBaseShift;
				if (vlq > 0)
				{
					maskResult |= Base64VlqConstants.VlqContinuationBit;
				}
				output.Append(Base64Converter.ToBase64(maskResult));
			} while (vlq > 0);
		}

		private static int ToVlqSigned(int value)
		{
			return value < 0 ? ((-value << 1) + 1) : (value << 1) + 0;
		}
	}
}

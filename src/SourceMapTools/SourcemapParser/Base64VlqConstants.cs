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

namespace SourcemapToolkit.SourcemapParser
{
	/// <summary>
	/// Constants used in Base64 VLQ encode/decode
	/// </summary>
	internal static class Base64VlqConstants
	{
		// A Base64 VLQ digit can represent 5 bits, so it is base-32.
		public const int VlqBaseShift = 5;
		public const int VlqBase = 1 << VlqBaseShift;

		// A mask of bits for a VLQ digit (11111), 31 decimal.
		public const int VlqBaseMask = VlqBase - 1;

		// The continuation bit is the 6th bit.
		public const int VlqContinuationBit = VlqBase;
	}
}

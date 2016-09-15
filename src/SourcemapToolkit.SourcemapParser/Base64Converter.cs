using System;
using System.Collections.Generic;

namespace SourcemapToolkit.SourcemapParser
{
	/// <summary>
	/// Used to convert Base64 characters values into integers
	/// </summary>
	internal class Base64Converter
	{
		private const string Base64Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
		private readonly Dictionary<char, int> _base64DecodeMap = new Dictionary<char, int>();

		public Base64Converter()
		{
			for (int i = 0; i < Base64Alphabet.Length; i += 1)
			{
				_base64DecodeMap[Base64Alphabet[i]] = i;
			}
		}

		/// <summary>
		/// Converts a base64 value to an integer.
		/// </summary>
		internal int FromBase64(char base64Value)
		{
			if (!_base64DecodeMap.ContainsKey(base64Value))
			{
				throw new ArgumentOutOfRangeException(nameof(base64Value), "Tried to convert an invalid base64 value");
			}

			return _base64DecodeMap[base64Value];
		}
	}
}

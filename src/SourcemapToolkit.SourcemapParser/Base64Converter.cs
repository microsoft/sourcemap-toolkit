using System;
using System.Collections.Generic;

namespace SourcemapToolkit.SourcemapParser
{
	/// <summary>
	/// Used to convert Base64 characters values into integers
	/// </summary>
	internal static class Base64Converter
	{
		private const string Base64Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
		private static readonly Dictionary<char, int> _base64DecodeMap = new Dictionary<char, int>();

		static Base64Converter()
		{
			for (int i = 0; i < Base64Alphabet.Length; i += 1)
			{
				_base64DecodeMap[Base64Alphabet[i]] = i;
			}
		}

		/// <summary>
		/// Converts a base64 value to an integer.
		/// </summary>
		internal static int FromBase64(char base64Value)
		{
			int result;
			if (!_base64DecodeMap.TryGetValue(base64Value, out result))
			{
				throw new ArgumentOutOfRangeException(nameof(base64Value), "Tried to convert an invalid base64 value");
			}

			return result;
		}

		/// <summary>
		/// Converts a integer to base64 value
		/// </summary>
		internal static char ToBase64(int value)
		{
			if (value < 0 || value >= Base64Alphabet.Length)
			{
				throw new ArgumentOutOfRangeException(nameof(value));
			}

			return Base64Alphabet[value];
		}
	}
}

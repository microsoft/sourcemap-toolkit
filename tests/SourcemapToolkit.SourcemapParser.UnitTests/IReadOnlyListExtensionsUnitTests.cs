using System;
using System.Collections.Generic;
using Xunit;

namespace SourcemapToolkit.SourcemapParser.UnitTests;

public class IReadOnlyListExtensionsUnitTests
{
	[Fact]
	public void IndexOf_NullList_ThrowsNullReferenceException()
	{
		// Arrange, Act, and Assert
		Assert.Throws<NullReferenceException>(() => IReadOnlyListExtensions.IndexOf(null, 1));
	}

	[Fact]
	public void IndexOf_ValueInList_CorrectlyReturnsIndex()
	{
		// Arrange
		IReadOnlyList<int> list = new[] { 6, 4, 1, 8 };

		// Act
		var index = IReadOnlyListExtensions.IndexOf(list, 1);

		// Assert
		Assert.Equal(2, index);
	}

	[Fact]
	public void IndexOf_ValueNotInList_CorrectlyReturnsNegativeOne()
	{
		// Arrange
		IReadOnlyList<int> list = new[] { 2, 4, 6, 8 };

		// Act
		var index = IReadOnlyListExtensions.IndexOf(list, 1);

		// Assert
		Assert.Equal(-1, index);
	}

	[Fact]
	public void IndexOf_ValueAppearsMultipleTimes_CorrectlyReturnsFirstInstance()
	{
		// Arrange
		IReadOnlyList<int> list = new[] { 2, 4, 6, 8, 4 };

		// Act
		var index = IReadOnlyListExtensions.IndexOf(list, 4);

		// Assert
		Assert.Equal(1, index);
	}

	[Fact]
	public void BinarySearch_NullList_ThrowsNullReferenceException()
	{
		// Arrange
		var comparer = Comparer<int>.Default;

		// Act & Assert
		Assert.Throws<NullReferenceException>(() => IReadOnlyListExtensions.BinarySearch(null, 1, comparer));
	}

	[Fact]
	public void BinarySearch_EvenNumberOfElements_CorrectlyMatchesListImplementation()
	{
		// Arrange
		// 6 elements total
		const int minFillIndexInclusive = 1;
		const int maxFillIndexInclusive = 4;

		var comparer = Comparer<int>.Default;
		var list = new List<int>();

		for (var i = minFillIndexInclusive; i <= maxFillIndexInclusive; i++)
		{
			list.Add(2 * i); // multiplying each entry by 2 to make sure there are gaps
		}

		// Act & Assert
		for (var i = minFillIndexInclusive - 1; i <= maxFillIndexInclusive + 1; i++)
		{
			Assert.Equal(list.BinarySearch(i, comparer), IReadOnlyListExtensions.BinarySearch(list, i, comparer));
			Assert.Equal(list.BinarySearch(i + 1, comparer), IReadOnlyListExtensions.BinarySearch(list, i + 1, comparer));
		}
	}

	[Fact]
	public void BinarySearch_OddNumberOfElements_CorrectlyMatchesListImplementation()
	{
		// Arrange
		// 6 elements total
		const int minFillIndexInclusive = 1;
		const int maxFillIndexInclusive = 5;

		var comparer = Comparer<int>.Default;
		var list = new List<int>();

		for (var i = minFillIndexInclusive; i <= maxFillIndexInclusive; i++)
		{
			list.Add(2 * i); // multiplying each entry by 2 to make sure there are gaps
		}

		// Act & Assert
		for (var i = minFillIndexInclusive - 1; i <= maxFillIndexInclusive + 1; i++)
		{
			Assert.Equal(list.BinarySearch(i, comparer), IReadOnlyListExtensions.BinarySearch(list, i, comparer));
			Assert.Equal(list.BinarySearch(i + 1, comparer), IReadOnlyListExtensions.BinarySearch(list, i + 1, comparer));
		}
	}
}
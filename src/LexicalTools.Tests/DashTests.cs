using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using WeSay.LexicalTools.Dashboard;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class DashTests
	{
		[Test]
		public void ComputeSmallestPossibleButtonSizes_Empty_Empty()
		{
			Assert.IsEmpty(Dash.ComputeSmallestPossibleButtonSizes(new List<IEnumerable<Size>>()));
		}

		[Test]
		public void ComputeSmallestPossibleButtonSizes_ListOfEmpty_Empty()
		{
			List<IEnumerable<Size>> sizesListList = new List<IEnumerable<Size>>();
			sizesListList.Add(new List<Size>());
			sizesListList.Add(new List<Size>());
			sizesListList.Add(new List<Size>());
			Assert.IsEmpty(Dash.ComputeSmallestPossibleButtonSizes(sizesListList));
		}

		[Test]
		public void ComputeSmallestPossibleButtonSizes_ListOfEmptyAfterNonEmpty_NonEmpty()
		{
			List<IEnumerable<Size>> sizesListList = new List<IEnumerable<Size>>();
			List<Size> sizes = new List<Size>();
			sizes.Add(new Size(30, 30));
			sizesListList.Add(sizes);
			sizesListList.Add(new List<Size>());
			sizes = Dash.ComputeSmallestPossibleButtonSizes(sizesListList);
			Assert.AreEqual(1, sizes.Count);
			Assert.Contains(new Size(30, 30), sizes);
		}

		[Test]
		public void ComputeSmallestPossibleButtonSizes_OneList_OneItem_SameItem()
		{
			List<IEnumerable<Size>> sizesListList = new List<IEnumerable<Size>>();
			List<Size> sizes = new List<Size>();
			sizes.Add(new Size(34, 48));
			sizesListList.Add(sizes);
			sizes = Dash.ComputeSmallestPossibleButtonSizes(sizesListList);
			Assert.AreEqual(1, sizes.Count);
			Assert.AreEqual(new Size(34, 48), sizes[0]);
		}

		[Test]
		public void ComputeSmallestPossibleButtonSizes_OneList_ItemsWithSameWidths_SmallestItem()
		{
			List<IEnumerable<Size>> sizesListList = new List<IEnumerable<Size>>();
			List<Size> sizes = new List<Size>();
			sizes.Add(new Size(34, 60));
			sizes.Add(new Size(34, 30));
			sizes.Add(new Size(34, 45));
			sizesListList.Add(sizes);
			sizes = Dash.ComputeSmallestPossibleButtonSizes(sizesListList);
			Assert.AreEqual(1, sizes.Count);
			Assert.AreEqual(new Size(34, 30), sizes[0]);
		}

		[Test]
		public void ComputeSmallestPossibleButtonSizes_OneList_ItemsWithSameHeights_SmallestItem()
		{
			List<IEnumerable<Size>> sizesListList = new List<IEnumerable<Size>>();
			List<Size> sizes = new List<Size>();
			sizes.Add(new Size(60, 34));
			sizes.Add(new Size(30, 34));
			sizes.Add(new Size(45, 34));
			sizesListList.Add(sizes);
			sizes = Dash.ComputeSmallestPossibleButtonSizes(sizesListList);
			Assert.AreEqual(1, sizes.Count);
			Assert.AreEqual(new Size(30, 34), sizes[0]);
		}

		[Test]
		public void
			ComputeSmallestPossibleButtonSizes_OneList_MultipleSizes_SameListOrderUnspecified()
		{
			List<IEnumerable<Size>> sizesListList = new List<IEnumerable<Size>>();
			List<Size> sizes = new List<Size>();
			sizes.Add(new Size(60, 34));
			sizes.Add(new Size(30, 50));
			sizes.Add(new Size(45, 45));
			sizesListList.Add(sizes);
			sizes = Dash.ComputeSmallestPossibleButtonSizes(sizesListList);
			Assert.AreEqual(3, sizes.Count);
			Assert.Contains(new Size(60, 34), sizes);
			Assert.Contains(new Size(30, 50), sizes);
			Assert.Contains(new Size(45, 45), sizes);
		}

		[Test]
		public void ComputeSmallestPossibleButtonSizes_TwoLists_SameItem_OneItemSame()
		{
			List<IEnumerable<Size>> sizesListList = new List<IEnumerable<Size>>();
			List<Size> sizes = new List<Size>();
			sizes.Add(new Size(60, 34));
			sizesListList.Add(sizes);
			sizesListList.Add(sizes);
			sizes = Dash.ComputeSmallestPossibleButtonSizes(sizesListList);
			Assert.AreEqual(1, sizes.Count);
			Assert.Contains(new Size(60, 34), sizes);
		}

		[Test]
		public void ComputeSmallestPossibleButtonSizes_TwoLists_SameHeights_OneItemLargestWidth()
		{
			List<IEnumerable<Size>> sizesListList = new List<IEnumerable<Size>>();
			List<Size> sizes = new List<Size>();
			sizes.Add(new Size(60, 34));
			sizesListList.Add(sizes);
			sizes = new List<Size>();
			sizes.Add(new Size(40, 34));
			sizesListList.Add(sizes);
			sizes = Dash.ComputeSmallestPossibleButtonSizes(sizesListList);
			Assert.AreEqual(1, sizes.Count);
			Assert.Contains(new Size(60, 34), sizes);
		}

		[Test]
		public void ComputeSmallestPossibleButtonSizes_TwoLists_SameWidths_OneItemLargestHeight()
		{
			List<IEnumerable<Size>> sizesListList = new List<IEnumerable<Size>>();
			List<Size> sizes = new List<Size>();
			sizes.Add(new Size(60, 30));
			sizesListList.Add(sizes);
			sizes = new List<Size>();
			sizes.Add(new Size(60, 40));
			sizesListList.Add(sizes);
			sizes = Dash.ComputeSmallestPossibleButtonSizes(sizesListList);
			Assert.AreEqual(1, sizes.Count);
			Assert.Contains(new Size(60, 40), sizes);
		}

		[Test]
		public void
			ComputeSmallestPossibleButtonSizes_TwoLists_DifferentSizes_OneItemLargestDimensions()
		{
			List<IEnumerable<Size>> sizesListList = new List<IEnumerable<Size>>();
			List<Size> sizes = new List<Size>();
			sizes.Add(new Size(80, 30));
			sizesListList.Add(sizes);
			sizes = new List<Size>();
			sizes.Add(new Size(60, 40));
			sizesListList.Add(sizes);
			sizes = Dash.ComputeSmallestPossibleButtonSizes(sizesListList);
			Assert.AreEqual(1, sizes.Count);
			Assert.Contains(new Size(80, 40), sizes);
		}

		[Test]
		public void
			ComputeSmallestPossibleButtonSizes_MultipleLists_LotsOfSizes_CoalescedWidthsAndHeights
			()
		{
			List<IEnumerable<Size>> sizesListList = new List<IEnumerable<Size>>();
			List<Size> sizes = new List<Size>();
			sizes.Add(new Size(30, 12));
			sizesListList.Add(sizes);
			sizes = new List<Size>();
			sizes.Add(new Size(10, 36));
			sizes.Add(new Size(20, 24));
			sizes.Add(new Size(45, 12));
			sizesListList.Add(sizes);
			sizes = new List<Size>();
			sizes.Add(new Size(10, 96));
			sizes.Add(new Size(15, 48));
			sizes.Add(new Size(33, 24));
			sizes.Add(new Size(60, 12));
			sizesListList.Add(sizes);
			sizes = Dash.ComputeSmallestPossibleButtonSizes(sizesListList);
			Assert.AreEqual(3, sizes.Count);
			Assert.Contains(new Size(60, 12), sizes);
			Assert.Contains(new Size(33, 24), sizes);
			Assert.Contains(new Size(30, 48), sizes);
		}

		[Test]
		public void ComputeBestButtonSize_NullPossibleSizes_EmptySize()
		{
			Size spaceForButtons = new Size(500, 300);
			Assert.AreEqual(Size.Empty,
							Dash.ComputeBestButtonSize(null, spaceForButtons, new List<int>()));
		}

		[Test]
		public void ComputeBestButtonSize_NoPossibleSizes_EmptySize()
		{
			Size spaceForButtons = new Size(500, 300);
			Assert.AreEqual(Size.Empty,
							Dash.ComputeBestButtonSize(new List<Size>(),
													   spaceForButtons,
													   new List<int>()));
		}

		[Test]
		public void ComputeBestButtonSize_OneSize_SameSize()
		{
			Size spaceForButtons = new Size(500, 300);
			List<Size> sizes = new List<Size>();
			sizes.Add(new Size(100, 50));
			List<int> buttonsPerGroup = new List<int>();
			buttonsPerGroup.Add(1);
			Size result = Dash.ComputeBestButtonSize(sizes, spaceForButtons, buttonsPerGroup);
			Assert.AreEqual(new Size(100, 50), result);
		}

		[Test]
		public void ComputeBestButtonSize_OneSizeLargerThanAvailableSpace_SameSize()
		{
			Size spaceForButtons = new Size(500, 300);
			List<Size> sizes = new List<Size>();
			sizes.Add(new Size(1000, 5000));
			List<int> buttonsPerGroup = new List<int>();
			buttonsPerGroup.Add(1);
			Size result = Dash.ComputeBestButtonSize(sizes, spaceForButtons, buttonsPerGroup);
			Assert.AreEqual(new Size(1000, 5000), result);
		}

		[Test]
		public void ComputeBestButtonSize_OneSizeNoAvailableSpace_SameSize()
		{
			Size spaceForButtons = Size.Empty;
			List<Size> sizes = new List<Size>();
			sizes.Add(new Size(1000, 5000));
			List<int> buttonsPerGroup = new List<int>();
			buttonsPerGroup.Add(1);
			Size result = Dash.ComputeBestButtonSize(sizes, spaceForButtons, buttonsPerGroup);
			Assert.AreEqual(new Size(1000, 5000), result);
		}

		[Test]
		public void ComputeBestButtonSize_TwoSizesBothClipped_LeastClippedSize()
		{
			Size spaceForButtons = new Size(50, 300);
			List<Size> sizes = new List<Size>();
			sizes.Add(new Size(60, 50));
			sizes.Add(new Size(70, 30));
			List<int> buttonsPerGroup = new List<int>();
			buttonsPerGroup.Add(1);
			Size result = Dash.ComputeBestButtonSize(sizes, spaceForButtons, buttonsPerGroup);
			Assert.AreEqual(new Size(60, 50), result);
		}

		[Test]
		public void ComputeBestButtonSize_TwoSizesOneClipped_NonClippedSize()
		{
			Size spaceForButtons = new Size(50, 300);
			List<Size> sizes = new List<Size>();
			sizes.Add(new Size(60, 30));
			sizes.Add(new Size(40, 50));
			List<int> buttonsPerGroup = new List<int>();
			buttonsPerGroup.Add(1);
			Size result = Dash.ComputeBestButtonSize(sizes, spaceForButtons, buttonsPerGroup);
			Assert.AreEqual(new Size(40, 50), result);
		}

		[Test]
		public void ComputeBestButtonSize_TwoSizesBothScrolled_LeastScrolledSize()
		{
			Size spaceForButtons = new Size(500, 4);
			List<Size> sizes = new List<Size>();
			sizes.Add(new Size(60, 5));
			sizes.Add(new Size(40, 10));
			List<int> buttonsPerGroup = new List<int>();
			buttonsPerGroup.Add(1);
			Size result = Dash.ComputeBestButtonSize(sizes, spaceForButtons, buttonsPerGroup);
			Assert.AreEqual(new Size(60, 5), result);
		}

		[Test]
		public void ComputeBestButtonSize_TwoSizesOneScrolled_NonScrolledSize()
		{
			Size spaceForButtons = new Size(700, 80);
			List<Size> sizes = new List<Size>();
			sizes.Add(new Size(600, 50));
			sizes.Add(new Size(400, 100));
			List<int> buttonsPerGroup = new List<int>();
			buttonsPerGroup.Add(1);
			Size result = Dash.ComputeBestButtonSize(sizes, spaceForButtons, buttonsPerGroup);
			Assert.AreEqual(new Size(600, 50), result);
		}

		[Test]
		public void ComputeBestButtonSize_TwoSizesOneScrolledOneClipped_ScrolledSize()
		{
			Size spaceForButtons = new Size(500, 300);
			List<Size> sizes = new List<Size>();
			sizes.Add(new Size(600, 250));
			sizes.Add(new Size(40, 500));
			List<int> buttonsPerGroup = new List<int>();
			buttonsPerGroup.Add(1);
			Size result = Dash.ComputeBestButtonSize(sizes, spaceForButtons, buttonsPerGroup);
			Assert.AreEqual(new Size(40, 500), result);
		}

		[Test]
		public void ComputeBestButtonSize_TwoSizesBothFit_SizeClosestToRatio()
		{
			Size spaceForButtons = new Size(500, 300);
			List<Size> sizes = new List<Size>();
			sizes.Add(new Size(100, 25));
			sizes.Add(new Size(101, 24));
			List<int> buttonsPerGroup = new List<int>();
			buttonsPerGroup.Add(1);
			Size result = Dash.ComputeBestButtonSize(sizes, spaceForButtons, buttonsPerGroup);
			Assert.AreEqual(new Size(100, 25), result);
		}

		[Test]
		public void ComputeBestButtonSize_TwoSizesOneScrolledMultipleButtons_NonScrolledSize()
		{
			Size spaceForButtons = new Size(500, 149);
			List<Size> sizes = new List<Size>();
			sizes.Add(new Size(105, 50));
			sizes.Add(new Size(50, 55));
			List<int> buttonsPerGroup = new List<int>();
			buttonsPerGroup.Add(5);
			buttonsPerGroup.Add(2);
			Size result = Dash.ComputeBestButtonSize(sizes, spaceForButtons, buttonsPerGroup);
			Assert.AreEqual(new Size(50, 55), result);
		}

		[Test]
		public void
			ComputeBestButtonSize_MultipleSizesContainingClippedScrolledNonscrolled_BestSizeByRatioAndNotClippedAndNotScrolled
			()
		{
			Size spaceForButtons = new Size(500, 300);
			List<Size> sizes = new List<Size>();
			sizes.Add(new Size(105, 50));
			sizes.Add(new Size(60, 70));
			sizes.Add(new Size(50, 55));
			sizes.Add(new Size(260, 49));

			List<int> buttonsPerGroup = new List<int>();
			buttonsPerGroup.Add(5);
			buttonsPerGroup.Add(2);
			Size result = Dash.ComputeBestButtonSize(sizes, spaceForButtons, buttonsPerGroup);
			Assert.AreEqual(new Size(105, 50), result);
		}
	}
}
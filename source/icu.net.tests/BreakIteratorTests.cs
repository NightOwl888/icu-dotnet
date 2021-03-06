// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System.Linq;
using NUnit.Framework;

namespace Icu.Tests
{
	[TestFixture]
	[Category("Full ICU")]
	public class BreakIteratorTests
	{
		[Test]
		public void Split_Character()
		{
			var parts = BreakIterator.Split(BreakIterator.UBreakIteratorType.CHARACTER, "en-US", "abc");

			Assert.That(parts.Count(), Is.EqualTo(3));
			Assert.That(parts.ToArray(), Is.EquivalentTo(new[] { "a", "b", "c"}));
		}

		[Test]
		public void Split_Word()
		{
			var parts = BreakIterator.Split(BreakIterator.UBreakIteratorType.WORD, "en-US", "Aa Bb. Cc");
			Assert.That(parts.Count(), Is.EqualTo(3));
			Assert.That(parts.ToArray(), Is.EquivalentTo(new[] { "Aa", "Bb", "Cc"}));
		}

		[Test]
		public void Split_Line()
		{
			var parts = BreakIterator.Split(BreakIterator.UBreakIteratorType.LINE, "en-US", "Aa Bb. Cc");
			Assert.That(parts.Count(), Is.EqualTo(3));
			Assert.That(parts.ToArray(), Is.EquivalentTo(new[] { "Aa ", "Bb. ", "Cc"}));
		}

		[Test]
		public void Split_Sentence()
		{
			var parts = BreakIterator.Split(BreakIterator.UBreakIteratorType.SENTENCE, "en-US", "Aa bb. Cc 3.5 x? Y?x! Z");
			Assert.That(parts.ToArray(), Is.EquivalentTo(new[] { "Aa bb. ", "Cc 3.5 x? ", "Y?", "x! ","Z"}));
			Assert.That(parts.Count(), Is.EqualTo(5));
		}

		[Test]
		public void GetBoundaries_Character()
		{
			var text = "abc? 1";
			var expected = new[] {
				new Boundary(0, 1), new Boundary(1, 2), new Boundary(2, 3), new Boundary(3, 4), new Boundary(4, 5), new Boundary(5, 6)
			};

			var parts = BreakIterator.GetBoundaries(BreakIterator.UBreakIteratorType.CHARACTER, new Locale("en-US"), text);

			Assert.That(parts.Count(), Is.EqualTo(expected.Length));
			Assert.That(parts.ToArray(), Is.EquivalentTo(expected));
		}

		[Test]
		public void GetBoundaries_Word()
		{
			var parts = BreakIterator.GetBoundaries(BreakIterator.UBreakIteratorType.WORD, new Locale("en-US"), WordBoundaryTestData.Text);

			Assert.That(parts.Count(), Is.EqualTo(WordBoundaryTestData.ExpectedOnlyWords.Length));
			Assert.That(parts.ToArray(), Is.EquivalentTo(WordBoundaryTestData.ExpectedOnlyWords));
		}

		[Test]
		public void GetBoundaries_Line()
		{
			var text = "Aa bb. Ccdef 3.5 x? Y?x! Z";
			var expected = new[] {
				new Boundary(0, 3), new Boundary(3, 7), new Boundary(7, 13), new Boundary(13, 17), new Boundary(17, 20),
				new Boundary(20, 22), new Boundary(22, 25), new Boundary(25, 26)
			};

			var parts = BreakIterator.GetBoundaries(BreakIterator.UBreakIteratorType.LINE, new Locale("en-US"), text);

			Assert.That(parts.Count(), Is.EqualTo(expected.Length));
			Assert.That(parts.ToArray(), Is.EquivalentTo(expected));
		}

		[Test]
		public void GetBoundaries_Sentence()
		{
			var text = "Aa bb. Ccdef 3.5 x? Y?x! Z";
			var expected = new[] {
				new Boundary(0, 7), new Boundary(7, 20), new Boundary(20, 22), new Boundary(22, 25), new Boundary(25, 26)
			};

			var parts = BreakIterator.GetBoundaries(BreakIterator.UBreakIteratorType.SENTENCE, new Locale("en-US"), text);
			
			Assert.That(parts.Count(), Is.EqualTo(expected.Length));
			Assert.That(parts.ToArray(), Is.EquivalentTo(expected));
		}

		[Test]
		public void GetWordBoundaries_IgnoreSpacesAndPunctuation()
		{
			var onlyWords = BreakIterator.GetWordBoundaries(new Locale("en-US"), WordBoundaryTestData.Text, false);

			Assert.That(onlyWords.Count(), Is.EqualTo(WordBoundaryTestData.ExpectedOnlyWords.Length));
			Assert.That(onlyWords.ToArray(), Is.EquivalentTo(WordBoundaryTestData.ExpectedOnlyWords));
		}

		[Test]
		public void GetWordBoundaries_IncludeSpacesAndPunctuation()
		{
			var allBoundaries = BreakIterator.GetWordBoundaries(new Locale("en-US"), WordBoundaryTestData.Text, true);

			Assert.That(allBoundaries.Count(), Is.EqualTo(WordBoundaryTestData.ExpectedAllBoundaries.Length));
			Assert.That(allBoundaries.ToArray(), Is.EquivalentTo(WordBoundaryTestData.ExpectedAllBoundaries));
		}

		/// <summary>
		/// The hypenated text case tests the difference between Word and Line
		/// breaks described in:
		/// http://userguide.icu-project.org/boundaryanalysis#TOC-Line-break-Boundary
		/// </summary>
		[Test]
		public void GetWordAndLineBoundariesWithHyphenatedText()
		{
			var text = "Good-day, kind sir !";
			var expectedWords = new[] {
				new Boundary(0, 4), new Boundary(5, 8), new Boundary(10, 14), new Boundary(15, 18)
			};
			var expectedLines = new[] {
				new Boundary(0, 5), new Boundary(5, 10), new Boundary(10, 15), new Boundary(15, 20)
			};

			var wordBoundaries = BreakIterator.GetBoundaries(BreakIterator.UBreakIteratorType.WORD, new Locale("en-US"), text);
			var lineBoundaries = BreakIterator.GetBoundaries(BreakIterator.UBreakIteratorType.LINE, new Locale("en-US"), text);

			Assert.That(wordBoundaries.Count(), Is.EqualTo(expectedWords.Length));
			Assert.That(wordBoundaries.ToArray(), Is.EquivalentTo(expectedWords));

			Assert.That(lineBoundaries.Count(), Is.EqualTo(expectedLines.Length));
			Assert.That(lineBoundaries.ToArray(), Is.EquivalentTo(expectedLines));
		}

		/// <summary>
		/// Test data for GetBoundaries_Word and GetWordBoundaries  tests
		/// </summary>
		internal static class WordBoundaryTestData
		{
			public const string Text = "Aa bb. Ccdef 3.5 x? Y?x! Z";

			public static readonly Boundary[] ExpectedOnlyWords = new[] {
				new Boundary(0, 2), new Boundary(3, 5), new Boundary(7, 12), new Boundary(13, 16),
				new Boundary(17, 18), new Boundary(20, 21), new Boundary(22, 23), new Boundary(25, 26)
			};

			public static readonly Boundary[] ExpectedAllBoundaries = new[] {
				new Boundary(0, 2), new Boundary(2, 3), new Boundary(3, 5), new Boundary(5, 6),
				new Boundary(6, 7), new Boundary(7, 12), new Boundary(12, 13), new Boundary(13, 16),
				new Boundary(16, 17), new Boundary(17, 18), new Boundary(18, 19), new Boundary(19, 20),
				new Boundary(20, 21), new Boundary(21, 22), new Boundary(22, 23), new Boundary(23, 24),
				new Boundary(24, 25), new Boundary(25, 26)
			};
		}
	}
}

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace TagsCloudVisualization
{
    public static class EnumerableExtensions
    {
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public static IEnumerable<T[]> Ngramms<T>(this IEnumerable<T> enumerable, int n)
        {
            var passedItemsNumber = 0;
            var items = new T[n];
            var length = new Integer {Value = -1};
            foreach (var lastItem in enumerable.Cycle(length))
            {
                passedItemsNumber++;
                for (var i = 0; i < n - 1; i++)
                    items[i] = items[i + 1];
                items[n - 1] = lastItem;

                if (passedItemsNumber < n) continue;
                var res = new T[n];
                items.CopyTo(res, 0);
                yield return res;

                if (passedItemsNumber == length.Value + n - 1)
                    yield break;
            }
        }

        private class Integer
        {
            public int Value;
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private static IEnumerable<T> Cycle<T>(this IEnumerable<T> enumerable, Integer length = null)
        {
            var isCounted = false;
            var cnt = 0;
            while (true)
            {
                foreach (var item in enumerable)
                {
                    cnt++;
                    yield return item;
                }
                if (length == null || isCounted) continue;
                length.Value = cnt;
                isCounted = true;
            }
        }
    }

    [TestFixture]
    public class Ngramms_Should
    {
        private readonly int[] array = {1, 2};

        [Test]
        public void ReturnAsManyElenentsAs_WasGiven()
        {
            array.Ngramms(5).Count().Should().Be(3);
        }

        [Test]
        public void ReturnCoorectNgramms()
        {
            var res = array.Ngramms(5).ToArray();
            res[0].ShouldAllBeEquivalentTo(new[]
            {
                1, 2, 1, 2, 1
            }, o => o.WithStrictOrdering());
            res[1].ShouldAllBeEquivalentTo(new[]
            {
                2, 1, 2, 1, 2
            }, o => o.WithStrictOrdering());
        }
    }
}
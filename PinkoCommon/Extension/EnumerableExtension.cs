using System;
using System.Collections.Generic;
using System.Linq;

namespace PinkoCommon.Extension
{
    public static class EnumerableExtension
    {
        /// <summary>
        /// ForEach
        /// </summary>
        public static IEnumerable<TItem> ForEach<TItem>(this IEnumerable<TItem> sequence, System.Action<TItem> action)
        {
            var forEach = sequence as TItem[] ?? sequence.ToArray();
            foreach (var item in forEach)
                action(item);

            return forEach;
        }

        /// <summary>
        /// Do() one time operation
        /// </summary>
        public static IEnumerable<TItem> Do<TItem>(this IEnumerable<TItem> sequence, Action<IEnumerable<TItem>> action)
        {
            action(sequence);

            return sequence;
        }
    }
}

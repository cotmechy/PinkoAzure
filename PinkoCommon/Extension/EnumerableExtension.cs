using System.Collections.Generic;

namespace PinkoCommon.Extension
{
    public static class EnumerableExtension
    {

        public static void ForEach<TItem>(this IEnumerable<TItem> sequence, System.Action<TItem> action)
        {
            foreach (var item in sequence)
                action(item);
        }
    }
}

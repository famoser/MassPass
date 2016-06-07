using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Famoser.MassPass.Business.Extensions
{
    public static class ObservaleCollectionExtension
    {
        public static void OrderByInside<T, TKey>(this ObservableCollection<T> collection, Func<T, TKey> selector)
        {
            var sortableList = new List<T>(collection);
            sortableList = sortableList.OrderBy(selector).ToList();

            for (int i = 0; i < sortableList.Count; i++)
            {
                collection.Move(collection.IndexOf(sortableList[i]), i);
            }
        }
        public static void OrderByDescendingInside<T, TKey>(this ObservableCollection<T> collection, Func<T, TKey> selector)
        {
            var sortableList = new List<T>(collection);
            sortableList = sortableList.OrderByDescending(selector).ToList();

            for (int i = 0; i < sortableList.Count; i++)
            {
                collection.Move(collection.IndexOf(sortableList[i]), i);
            }
        }
    }
}

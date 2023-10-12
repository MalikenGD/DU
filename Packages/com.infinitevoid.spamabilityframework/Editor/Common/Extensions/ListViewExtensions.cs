using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace InfiniteVoid.SpamFramework.Editor.Common.Extensions
{
    public static class ListViewExtensions
    {
        /// <summary>
        /// Sets on selection changed event with Unity version-specific syntax
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="onSelectedChanged"></param>
        public static void SetOnSelectionChanged(this ListView listView, Action<IEnumerable<object>> onSelectedChanged)
        {
#if UNITY_2022
            listView.selectionChanged += onSelectedChanged;
#else
            listView.onSelectionChange += onSelectedChanged;
#endif

        }

        /// <summary>
        /// Sets the height with Unity version-specific syntax
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="numItems"></param>
        public static void SetHeight(this ListView listView, int numItems)
        {
#if UNITY_2021_3_OR_NEWER
            listView.style.minHeight = listView.fixedItemHeight * numItems;
#else
            listView.style.minHeight = listView.itemHeight * numItems;
#endif
        }
        
        /// <summary>
        /// Reloads the ListView with Unity version-specific syntax
        /// </summary>
        /// <param name="listView"></param>
        public static void Reload(this ListView listView)
        {
#if UNITY_2021_3_OR_NEWER
            listView.Rebuild();
#else
            listView.Refresh();
#endif
        }
    }
}
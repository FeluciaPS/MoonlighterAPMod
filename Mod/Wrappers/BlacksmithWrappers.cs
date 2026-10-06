using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace MoonlighterAPMod.Mod.Wrappers
{
    internal class BlacksmithWrappers
    {
        internal sealed class TabInfoWrapper
        {
            private static readonly Type _type = typeof(TreeItemSelector).GetNestedType(
                "TabInfo",
                BindingFlags.NonPublic);

            private static readonly FieldInfo _pagesField = _type.GetField(
                "pages",
                BindingFlags.Public | BindingFlags.Instance);
            private readonly object _instance;

            internal TabInfoWrapper(object instance) => _instance = instance;

            internal IList Pages => (IList)_pagesField.GetValue(_instance);
        }

        internal sealed class PageInfoWrapper
        {
            private static readonly Type _type = typeof(TreeItemSelector).GetNestedType(
                "PageInfo",
                BindingFlags.NonPublic);

            private static readonly FieldInfo _itemsField = _type.GetField(
                "items",
                BindingFlags.Public | BindingFlags.Instance);

            private static readonly FieldInfo _unlockedField = _type.GetField(
                "unlocked",
                BindingFlags.Public | BindingFlags.Instance);

            private static readonly FieldInfo _seenField = _type.GetField(
                "seen",
                BindingFlags.Public | BindingFlags.Instance);

            private static readonly FieldInfo _isWeaponFamilyField = _type.GetField(
                "isWeaponFamily",
                BindingFlags.Public | BindingFlags.Instance);

            private readonly object _instance;

            internal PageInfoWrapper(object instance) => _instance = instance;

            internal List<ItemMaster> Items => (List<ItemMaster>)_itemsField.GetValue(_instance);

            internal List<bool> Unlocked => (List<bool>)_unlockedField.GetValue(_instance);

            internal List<bool> Seen => (List<bool>)_seenField.GetValue(_instance);

            internal bool IsWeaponFamily => (bool)_isWeaponFamilyField.GetValue(_instance);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;

namespace MoonlighterAPMod.Mod.Wrappers
{
    internal sealed class DecorativePanelWrapper
    {
        private static readonly Type _type = typeof(DecorativePanel);

        private static readonly FieldInfo _decorativeItemsField = _type.GetField(
            "_decorativeItems",
            BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo _validItemsField = _type.GetField(
            "_validItems",
            BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo _maxPageField = _type.GetField(
            "_maxPage",
            BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo _decorativeSlotField = _type.GetField(
            "_decorativeSlot",
            BindingFlags.NonPublic | BindingFlags.Instance);


        private readonly object _instance;

        internal DecorativePanelWrapper(object instance) => _instance = instance;

        internal List<DecorativeItemMaster> DecorativeItems 
        {
            get { return (List<DecorativeItemMaster>)_decorativeItemsField.GetValue(_instance); }
            set { _decorativeItemsField.SetValue(_instance, value);  }
        } 

        internal List<DecorativeItemMaster> ValidItems
        {
            get { return (List<DecorativeItemMaster>)_validItemsField.GetValue(_instance); }
            set { _validItemsField.SetValue(_instance, value); }
        }

        internal int MaxPage
        {
            get { return (int)_maxPageField.GetValue(_instance); }
            set { _maxPageField.SetValue(_instance, value); }
        }

        internal DecorativeSlot DecorativeSlot => (DecorativeSlot)_decorativeSlotField.GetValue(_instance);
    }
}

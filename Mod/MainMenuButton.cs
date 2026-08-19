using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoonlighterAPMod.Mod
{
    /**
     * TODO: WIP unused.
     * Moonlighter code is a demon and in fact sets a billion flags whenever a panel opens and I haven't found them all.
     */
    internal class MainMenuButton : MonoBehaviour
    {
        private Selectable previous;
        private Selectable next;
        private Button button;
        internal event Action OnClick;
        private void Start()
        {
            button = gameObject.GetComponent<Button>();

            button.onClick.RemoveAllListeners();
            UIManager.ClearPersistentEvents(button.onClick);
            button.onClick.AddListener(OnSelect);
            if (next == null)
            {
                InsertNavigation();
                UpdatePositions();
            }
        }

        private void OnSelect()
        {
            OnClick.Invoke();
        }

        private void InsertNavigation()
        {
            next = previous.navigation.selectOnDown;

            Navigation navigation = new()
            {
                mode = Navigation.Mode.Explicit,
                selectOnUp = previous,
                selectOnDown = next,
                selectOnLeft = null,
                selectOnRight = null
            };

            button.navigation = navigation;

            Navigation previousNav = previous.navigation;
            previousNav.selectOnDown = button;
            previous.navigation = previousNav;

            Navigation nextNav = next.navigation;
            nextNav.selectOnUp = button;
            next.navigation = nextNav;
        }

        private void UpdatePositions()
        {
            Selectable nextEntry = button;

            while (nextEntry != null && !nextEntry.Equals(button))
            {
                MoonlighterAPMod.APInfo($"Moving {nextEntry.transform.name} down..");
                nextEntry.transform.position += new Vector3(0, -60, 0);
                nextEntry = nextEntry.navigation.selectOnDown;
            }
        }

        internal void SetPrevious(Selectable previous)
        {
            this.previous = previous;
        }

        internal void SetName(string name)
        {
            gameObject.name = name;
            Text text = gameObject.GetComponent<Text>();
            text?.text = name;
        } 
    }
}

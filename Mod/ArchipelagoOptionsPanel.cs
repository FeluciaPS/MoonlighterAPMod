using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MoonlighterAPMod.Mod
{
    /**
     * TODO: WIP unused.
     * I'll hopefully pick this up later when I actually understand unity UI stuff better but for now this can fuck off
     */
    internal class ArchipelagoOptionsPanel : Panel
    {
        private InputFieldEntry serverInput;
        private InputFieldEntry portInput;
        private InputFieldEntry slotInput;
        private InputFieldEntry passwordInput;

        private Button connectButton;
        private Button disconnectButton;
        private Text resultMessage;

        private void Start()
        {
            Init();
            serverInput = CreateInputField("Server", new Vector3(0, 90, 0));
            portInput = CreateInputField("Port", new Vector3(0, 30, 0));
            slotInput = CreateInputField("Slot", new Vector3(0, -30, 0));
            passwordInput = CreateInputField("Password", new Vector3(0, -90, 0));

            SetupNavigation([
                serverInput.gameObject,
                portInput.gameObject,
                slotInput.gameObject,
                passwordInput.gameObject
            ]);

            serverInput.gameObject.GetComponent<Selectable>().Select();

        }

        private InputFieldEntry CreateInputField(string name, Vector3 position)
        {
            GameObject entry = new(name);

            Transform canvas = Content.transform.Find("Canvas");
            entry.transform.SetParent(canvas, false);
            entry.transform.localPosition = position;

            entry.AddComponent<RectTransform>();

            Button button = entry.AddComponent<Button>();
            InputFieldEntry inputEntry = entry.AddComponent<InputFieldEntry>();

            GameObject inputField = TMP_DefaultControls.CreateInputField(new TMP_DefaultControls.Resources());

            inputField.name = "InputField";
            inputField.transform.SetParent(entry.transform, false);

            inputField.transform.localPosition = Vector3.zero;
            inputField.transform.localScale = Vector3.one;

            inputEntry.button = button;
            inputEntry.inputField = inputField.GetComponent<TMP_InputField>();

            return inputEntry;
        }


        internal void SetupNavigation(List<GameObject> entries)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                Button current = entries[i].GetComponent<Button>();

                Navigation navigation = current.navigation;
                navigation.mode = Navigation.Mode.Explicit;

                navigation.selectOnRight = null;
                navigation.selectOnLeft = null;
                navigation.selectOnUp = null;
                navigation.selectOnDown = null;

                if (i > 0)
                {
                    Button previous = entries[i - 1].GetComponent<Button>();
                    navigation.selectOnUp = previous;
                }
                
                if (i < entries.Count - 1)
                {
                    Button next = entries[i + 1].GetComponent<Button>();
                    navigation.selectOnDown = next;
                }
                
                current.navigation = navigation;
            }
        }
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoonlighterAPMod.Mod
{
    internal class InputFieldEntry : MonoBehaviour
    {
        internal Button button;
        internal TMP_InputField inputField;

        private void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();

            if (inputField == null)
                inputField = GetComponentInChildren<TMP_InputField>(true);
        }

        internal void EnterInput()
        {
            if (inputField == null)
                return;

            EventSystem.current.SetSelectedGameObject(inputField.gameObject);
            inputField.ActivateInputField();
            inputField.caretPosition = inputField.text.Length;
        }

        internal void ExitInput()
        {
            if (inputField == null)
                return;

            inputField.DeactivateInputField();
            EventSystem.current.SetSelectedGameObject(button.gameObject);
        }
    }
}

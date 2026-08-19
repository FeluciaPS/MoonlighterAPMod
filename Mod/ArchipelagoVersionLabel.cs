using UnityEngine;
using UnityEngine.UI;

namespace MoonlighterAPMod.Mod
{
    internal class ArchipelagoVersionLabel : MonoBehaviour
    {
        internal Text text;
        private Font font;
        internal Font Font 
        {
            get { return font; }
            set { font = value; UpdateText(); }
        }

        private void Awake()
        {
            transform.localPosition = new Vector3(-265, -115, 0);
            text = gameObject.AddComponent<Text>();

            UpdateText();
        }

        internal void UpdateText()
        {
            text.text = $"AP Mod v{ModInfo.PLUGIN_VERSION}";
            text.fontSize = 12;
            text.font = font;
            text.alignment = UnityEngine.TextAnchor.LowerLeft;
        }
    }
}

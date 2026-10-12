using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MoonlighterAPMod.Mod
{
    internal class UIManager : MonoBehaviour
    {
        internal static UIManager Instance;
        internal ArchipelagoOptionsPanel archipelagoOptionsPanel;
        internal ArchipelagoVersionLabel archipelagoVersionLabel;
        internal MainMenuButton archipelagoMenuButton;
        internal Font font;

        private MainMenuPanel mainMenuPanel;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                UnityEngine.Object.Destroy(base.gameObject);
            }
        }

        internal void CreateOptionsPanel()
        {
            MoonlighterAPMod.APInfo("Creating AP options panel...");

            GameObject popupParent = GameObject.Find("PopUps");
            GameObject optionsPanel = popupParent.transform.Find("PopUp - Options(Clone)").gameObject;
            GameObject apOptionsPanel = GameObject.Instantiate(optionsPanel, popupParent.transform);
            // Destroy unwanted children
            Destroy(apOptionsPanel.GetComponent<OptionsPanel>());
            Transform content = optionsPanel.transform.Find("Content");
            Destroy(content.Find("Image - Base")?.gameObject);
            Destroy(content.Find("Image - Line")?.gameObject);
            Destroy(content.Find("Tabs")?.gameObject);
            Destroy(content.Find("Contents")?.gameObject);

            archipelagoOptionsPanel = apOptionsPanel.AddComponent<ArchipelagoOptionsPanel>();
        }

        internal void CreateOrUpdateVersionLabel(Transform parent)
        {
            MoonlighterAPMod.APInfo("Creating AP version label...");
            GameObject apVersionObject = new("APVersionLabel");
            apVersionObject.transform.SetParent(parent, false);
            archipelagoVersionLabel = apVersionObject.AddComponent<ArchipelagoVersionLabel>();

            MoonlighterAPMod.APInfo("Finding and saving font...");
            Transform versionLabel = parent.Find("VersionInfoLabel");
            Text versionLabelText = versionLabel.gameObject.GetComponent<Text>();
            archipelagoVersionLabel.Font = versionLabelText.font;
        }

        internal void CreateOrUpdateArchipelagoMenuButton(MainMenuPanel mainMenuPanel)
        {
            this.mainMenuPanel = mainMenuPanel;
            // Grab the options button [2] by index, it's extremely unlikely someone would put a different
            // button here.
            GameObject previous = mainMenuPanel.mainMenuButtons[2];

            Transform menuButtonsTransform = mainMenuPanel.transform.Find("MainMenuButtons");

            GameObject archipelagoButtonObject = GameObject.Instantiate(previous, menuButtonsTransform);
            
            archipelagoMenuButton = archipelagoButtonObject.AddComponent<MainMenuButton>();
            archipelagoMenuButton.SetName("Archipelago");
            archipelagoMenuButton.SetPrevious(previous.GetComponent<Button>());
            archipelagoMenuButton.OnClick += OpenArchipelagoPanel;
            mainMenuPanel.mainMenuButtons.Add(archipelagoButtonObject);
        }

        internal void OpenArchipelagoPanel()
        {
            GUIManager.Instance.currentPopUp = archipelagoOptionsPanel;
            archipelagoOptionsPanel.Enable();
            mainMenuPanel.Disable();
        }

        internal void CloseArchipelagoPanel()
        {
            GUIManager.Instance.currentPopUp = null;
            archipelagoOptionsPanel.Disable();
        }

        internal static void ClearPersistentEvents(UnityEventBase eventData)
        {
            // I spent 2 hours wrapping my head around reflection just for this one function and I still don't fully know what I'm doing but it WORKS
            // -Felucia
            FieldInfo field = typeof(UnityEventBase).GetField(
                "m_PersistentCalls",
                BindingFlags.Instance | BindingFlags.NonPublic
            );

            var persistentCallGroup = field?.GetValue(eventData);
            MethodInfo removeListenerMethod = persistentCallGroup?.GetType().GetMethod(
                "RemoveListener",
                BindingFlags.Instance | BindingFlags.Public
            );

            int count = eventData.GetPersistentEventCount();
            for (int i = count - 1; i >= 0; i--)
            {
                removeListenerMethod?.Invoke(persistentCallGroup, [i]);
            }
        }

        // Overworld death stuff

        internal void RunOverworldDeath()
        {
            StartCoroutine(OverworldDeathCoroutine());
        }

        private IEnumerator OverworldDeathCoroutine()
        {
            yield return new WaitForSecondsRealtime(2.5f);

            GUIManager.Instance.FadeBoth(delegate
            {
                StartCoroutine(TownLoadCoroutine());
            }, null, 0.5f, activeHUDOnFadeIn: false);
        }

        private IEnumerator TownLoadCoroutine()
        {
            GameManager gameManager = GameManager.Instance;
            HeroMerchant merchant = HeroMerchant.Instance;
            GUIManager.Instance.GoToDungeonLoadingPanel();
            yield return new WaitForSecondsRealtime(1f);
            gameManager.DeleteAllDroppedItems();
            gameManager.DestroyAllDestroyableItems();

            merchant.heroMerchantInventory.RemoveAllNotSafeItems();
            merchant.heroMerchantInventory.ResetAffectedSlots();

            if (gameManager.IsTownSceneLoaded())
            {
                int id = SceneUtility.GetBuildIndexByScenePath("Scenes/Town");
                yield return SceneManager.UnloadSceneAsync(id);
            }

            HeroMerchant.Instance.heroMerchantStats.RecoverAllHealth();

            gameManager.LoadTownScene();
        }
    }
}

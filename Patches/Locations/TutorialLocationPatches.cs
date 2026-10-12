using DG.Tweening;
using HarmonyLib;
using MoonlighterAPMod.Archipelago;
using System.Reflection;
using UnityEngine;

namespace MoonlighterAPMod.Patches.Locations
{
    internal class TutorialLocationPatches
    {
        // TODO: This can be a wrapper instead
        private static readonly FieldInfo countField = typeof(TutorialScene2).GetField(
            "_count",
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        private static readonly FieldInfo oldManMindField = typeof(TutorialScene2).GetField(
            "_greenOldManMind",
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        private static readonly FieldInfo oldManAgentField = typeof(TutorialScene2).GetField(
            "_greenOldManAgent",
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        private static readonly FieldInfo animationInstanceField = typeof(TutorialScene2).GetField(
            "_willStartAnimationInstance",
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        private static readonly MethodInfo nextSceneMethod = typeof(TutorialScene2).GetMethod(
            "Scene8_10",
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        private static void SendTutorialLocation(int id)
        {
            // Send locations directly instead of dropping an item, because tutorial location items can be permanently lost otherwise.
            LocationHelper.SendLocation(MoonlighterAPConstants.TUTORIAL_LOCATIONS[id]);
        }

        [HarmonyPatch(typeof(TutorialScene2))]
        [HarmonyPatch("Scene8_9")]
        internal static class FirstTutorialItemPatch
        {
            internal static bool Prefix(TutorialScene2 __instance)
            {
                if (!ArchipelagoClient.IsClientConnected)
                    return true;

                int count = (int)countField.GetValue(__instance);

                if (count != 7)
                    return true;

                DummyMind _greenOldManMind = (DummyMind)oldManMindField.GetValue(__instance);
                DummyAgent _greenOldManAgent = (DummyAgent)oldManAgentField.GetValue(__instance);
                GameObject _willStartAnimationInstance = (GameObject)animationInstanceField.GetValue(__instance);

                ItemStack apDummyItem = ItemStack.Create(ItemDatabase.GetItemByName("Archipelago Dummy Item"));

                // Replace the usual item with an archipelago sprite 
                HeroMerchantInventory inventory = HeroMerchant.Instance.heroMerchantInventory;

                // Below code is amended from Moonlighter source code
                // TODO: Could maybe transpile this instead
                _greenOldManMind.Owner.SendAgentMessage(new HideTextDialogMessage());
                apDummyItem.transform.parent = GameObject.Find("@WorldCanvas").transform;
                apDummyItem.itemImage.sortingLayerName = "Effects";
                apDummyItem.pickOnWalk.CanPick = false;
                apDummyItem.ShowArt();
                apDummyItem.transform.position = _greenOldManAgent.transform.position;

                apDummyItem.transform.DOMove(_willStartAnimationInstance.transform.position + Vector3.one * 8f, 4f).SetUpdate(isIndependentUpdate: false).SetDelay(1f)
                    .OnComplete(delegate
                    {
                        inventory.SetEquippedItemByType(inventory.GetEquippedItemByType(HeroMerchantInventory.EquipmentSlot.Weapon1), HeroMerchantInventory.EquipmentSlot.Weapon2);
                        SendTutorialLocation(0);
                        GameObject.Destroy(apDummyItem.gameObject);
                        countField.SetValue(__instance, count + 1);
                        nextSceneMethod.Invoke(__instance, null);
                    });


                return false;
            }
        }

        [HarmonyPatch(typeof(TutorialScene3))]
        [HarmonyPatch("Scene11_1")]
        internal static class SecondTutorialItemPatch
        {
            internal static void Prefix() => SendTutorialLocation(0);
        }
    }
}

using HarmonyLib;
using System.Collections.Generic;
using System.Text;
using TMPro;
using TOCS.Templates;
using UnityEngine;

namespace TOCS;
public static class Credentials
{
    public static SpriteRenderer TOCSLogo { get; private set; }

    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    internal class PingTrackerUpdatePatch
    {
        private static void Postfix(PingTracker __instance)
        {
            var offset_x = 1.2f; //右端からのオフセット
            if (HudManager.InstanceExists && HudManager._instance.Chat.chatButton.active) offset_x += 0.8f; //チャットボタンがある場合の追加オフセット
            if (FriendsListManager.InstanceExists && FriendsListManager._instance.FriendsListButton.Button.active) offset_x += 0.8f; //フレンドリストボタンがある場合の追加オフセット
            __instance.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(offset_x, 0f, 0f);
        }
    }
    [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
    internal class VersionShowerStartPatch
    {
        static TextMeshPro SpecialEventText;
        private static void Postfix(VersionShower __instance)
        {
            TMPTemplate.SetBase(__instance.text);
            Main.CredentialsText = $"\r\n<color={Main.ModColor}>{Main.ModName}</color> - {Main.PluginVersion}";
    #if RELEASE
            Main.CredentialsText = $"\r\n<color={Main.ModColor}>{Main.ModName}</color> - {Main.PluginVersion}";
    #endif
            Logger.Info($"v{Main.PluginVersion}", "TOHE version");
            
            var credentials = Object.Instantiate(__instance.text);
            credentials.text = Main.CredentialsText;
            credentials.alignment = TextAlignmentOptions.Right;
            credentials.transform.position = new Vector3(1f, 2.79f, -2f);
            credentials.fontSize = credentials.fontSizeMax = credentials.fontSizeMin = 2f;

            ErrorText.Create(__instance.text);
            if (Main.hasArgumentException && ErrorText.Instance != null)
            {
                ErrorText.Instance.AddError(ErrorCode.Main_DictionaryError);
            }

            if (SpecialEventText == null && TOCSLogo != null)
            {
                SpecialEventText = Object.Instantiate(__instance.text, TOCSLogo.transform);
                SpecialEventText.name = "SpecialEventText";
                SpecialEventText.text = "";
                SpecialEventText.color = Color.white;
                SpecialEventText.fontSizeMin = 3f;
                SpecialEventText.alignment = TextAlignmentOptions.Center;
                SpecialEventText.transform.localPosition = new Vector3(0f, 0.8f, 0f);
            }
            if (SpecialEventText != null)
            {
                SpecialEventText.enabled = TitleLogoPatch.amongUsLogo != null;
            }
        }
    }

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start)), HarmonyPriority(Priority.First)]
    internal class TitleLogoPatch
    {
        public static GameObject amongUsLogo;
        public static GameObject Ambience;
        public static GameObject LoadingHint;

        private static void Postfix(MainMenuManager __instance)
        {
            amongUsLogo = GameObject.Find("LOGO-AU");

            var rightpanel = __instance.gameModeButtons.transform.parent;
            var logoObject = new GameObject("titleLogo_TOCS");
            var logoTransform = logoObject.transform;
            TOCSLogo = logoObject.AddComponent<SpriteRenderer>();
            logoTransform.parent = rightpanel;
            logoTransform.localPosition = new(-0.16f, 0f, 1f); //new(0f, 0.3f, 1f); new(0f, 0.15f, 1f);
            logoTransform.localScale *= 1.2f;

            if (!Options.IsLoaded)
            {
                LoadingHint = new GameObject("LoadingHint");
                LoadingHint.transform.position = Vector3.down;
                var LoadingHintText = LoadingHint.AddComponent<TextMeshPro>();
                LoadingHintText.text = "加载中";
                LoadingHintText.alignment = TextAlignmentOptions.Center;
                LoadingHintText.fontSize = 2f;
                LoadingHintText.transform.position = amongUsLogo.transform.position;
                LoadingHintText.transform.position += new Vector3 (-0.25f, -0.9f, 0f);
                LoadingHintText.color = new Color32(17, 255, 1, byte.MaxValue);
                __instance.playButton.transform.gameObject.SetActive(false);
            }
            if ((Ambience = GameObject.Find("Ambience")) != null)
            {
                if (Options.IsLoaded && LoadingHint != null) __instance.playButton.transform.gameObject.SetActive(true);

                Ambience.SetActive(false);
            }
        }
    }
    [HarmonyPatch(typeof(ModManager), nameof(ModManager.LateUpdate))]
    class ModManagerLateUpdatePatch
    {
        public static void Prefix(ModManager __instance)
        {
            __instance.ShowModStamp();

            LateTask.Update(Time.deltaTime);
            CheckMurderPatch.Update();
        }
        public static void Postfix(ModManager __instance)
        {
            var offset_y = HudManager.InstanceExists ? 1.6f : 0.9f;
            __instance.ModStamp.transform.position = AspectPosition.ComputeWorldPosition(
                __instance.localCamera, AspectPosition.EdgeAlignments.RightTop,
                new Vector3(0.4f, offset_y, __instance.localCamera.nearClipPlane + 0.1f));
        }
    }
}
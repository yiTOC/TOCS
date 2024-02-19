using HarmonyLib;
using System.Collections.Generic;

namespace TOCS;

[HarmonyPatch(typeof(NotificationPopper), nameof(NotificationPopper.AddItem))]
public class NotificationPopperPatch
{
    private static List<string> WaitToSend = new();
    private static bool Prefix(NotificationPopper __instance, string item)
    {
        if (!WaitToSend.Contains(item)) return false;
        WaitToSend.Remove(item);
        return true;
    }
    public static void AddItem(string text)
    {
        WaitToSend.Add(text);
        if (DestroyableSingleton<HudManager>._instance) DestroyableSingleton<HudManager>.Instance.Notifier.AddItem(text);
        else WaitToSend.Remove(text);
    }
}
using AmongUs.Data;
using AmongUs.GameOptions;
using HarmonyLib;
using InnerNet;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TOCS.Roles.Core;

namespace TOCS
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerLeft))]
    class OnPlayerLeftPatch
    {
        static void Prefix([HarmonyArgument(0)] ClientData data)
        {
            if (!GameStates.IsInGame || !AmongUsClient.Instance.AmHost) return;
        }
        public static List<int> ClientsProcessed = new();
        public static void Add(int id)
        {
            ClientsProcessed.Remove(id);
            ClientsProcessed.Add(id);
        }
        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData data, [HarmonyArgument(1)] DisconnectReasons reason)
        {
            Main.playerVersion.Remove(data.Character.PlayerId);
            Logger.Info($"{data?.PlayerName}(ClientID:{data?.Id}/FriendCode:{data?.FriendCode})断开连接(理由:{reason}，Ping:{AmongUsClient.Instance.Ping})", "Session");

            if (AmongUsClient.Instance.AmHost)
            {
                Main.SayStartTimes.Remove(__instance.ClientId);
                Main.SayBanwordsTimes.Remove(__instance.ClientId);

                // 附加描述掉线原因
                switch (reason)
                {
                    case DisconnectReasons.Hacking:
                        RPC.NotificationPop(string.Format("PlayerLeftByAU-Anticheat", data?.PlayerName));
                        break;
                    case DisconnectReasons.Error:
                        RPC.NotificationPop(string.Format("PlayerLeftCuzError", data?.PlayerName));
                        break;
                    case DisconnectReasons.Kicked:
                    case DisconnectReasons.Banned:
                        break;
                    default:
                        if (!ClientsProcessed.Contains(data?.Id ?? 0))
                            RPC.NotificationPop(string.Format("PlayerLeft", data?.PlayerName));
                        break;
                }
                ClientsProcessed.Remove(data?.Id ?? 0);
            }
        }
    }
}
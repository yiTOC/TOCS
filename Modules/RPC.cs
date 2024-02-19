using AmongUs.GameOptions;
using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TOCS;
using TOCS.Modules;

namespace TOCS

{
    //RPC用于处理
    public enum CustomRPC
    {
        //版本检查的RPC
        VersionCheck = 80,
        //请求重试版本检查的RPC
        RequestRetryVersionCheck = 81,
        //通知弹出窗口的RPC
        NotificationPop,
        //设置踢出理由RPC
        SetKickReason,
        //设置死亡理由RPC
        SetDeathReason,
    }
}
[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
internal class RPCHandlerPatch
{
    public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
    {
        //除 CustomRPC 外无其他处理
        if (callId < (byte)CustomRPC.VersionCheck) return;

        var rpcType = (CustomRPC)callId;
        switch (rpcType)
        {
            case CustomRPC.VersionCheck:
                try
                {
                    Version version = Version.Parse(reader.ReadString());
                    string tag = reader.ReadString();
                    string forkId = reader.ReadString();
                    Main.playerVersion[__instance.PlayerId] = new PlayerVersion(version, tag, forkId);

                    // Kick Unmached Player Start
                    if (AmongUsClient.Instance.AmHost && tag != $"{ThisAssembly.Git.Commit}({ThisAssembly.Git.Branch})")
                    {
                        if (forkId != Main.ForkId)
                            _ = new LateTask(() =>
                            {
                                if (__instance?.Data?.Disconnected is not null and not true)
                                {
                                    var msg = string.Format("【{0}】因安装了不同版本的模组被请离房间", __instance?.Data?.PlayerName);
                                    TOCS.Logger.Warn(msg, "Version Kick");
                                    RPC.NotificationPop(msg);
                                    Utils.KickPlayer(__instance.GetClientId(), false, "ModVersionIncorrect");
                                }
                            }, 5f, "Kick");
                    }
                    // Kick Unmached Player End
                }
                catch
                {
                    TOCS.Logger.Warn($"{__instance?.Data?.PlayerName}({__instance.PlayerId}): 版本信息无效。", "RpcVersionCheck");
                    _ = new LateTask(() =>
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.RequestRetryVersionCheck, SendOption.Reliable, __instance.GetClientId());
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }, 1f, "Retry Version Check Task");
                }
                break;
            case CustomRPC.NotificationPop:
                NotificationPopperPatch.AddItem(reader.ReadString());
                break;
            case CustomRPC.SetKickReason:
                ShowDisconnectPopupPatch.ReasonByHost = reader.ReadString();
                break;
            case CustomRPC.SetDeathReason:
                RPC.GetDeathReason(reader);
                break;
        }
    }
}
internal static class RPC
{
    public static void NotificationPop(string text)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.NotificationPop, Hazel.SendOption.Reliable, -1);
        writer.Write(text);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
        NotificationPopperPatch.AddItem(text);
    }
    public static void SendDeathReason(byte playerId, CustomDeathReason deathReason)
    {
        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetDeathReason, SendOption.Reliable, -1);
        writer.Write(playerId);
        writer.Write((int)deathReason);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }
    public static void GetDeathReason(MessageReader reader)
    {
        var playerId = reader.ReadByte();
        var deathReason = (CustomDeathReason)reader.ReadInt32();
        var state = PlayerState.GetByPlayerId(playerId);
        state.DeathReason = deathReason;
        state.IsDead = true;
    }
}
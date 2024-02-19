using HarmonyLib;

namespace TOCS
{
    [HarmonyPatch(typeof(DisconnectPopup), nameof(DisconnectPopup.DoShow))]
    internal class ShowDisconnectPopupPatch
    {
        public static DisconnectReasons Reason;
        public static string ReasonByHost = string.Empty;
        public static void Postfix(DisconnectPopup __instance)
        {
            _ = new LateTask(() =>
            {
                if (__instance == null) return;
                try
                {

                    void SetText(string text)
                    {
                        if (__instance?._textArea?.text != null)
                            __instance._textArea.text = text;
                    }

                    if (!string.IsNullOrEmpty(ReasonByHost))
                        SetText(string.Format("描述通知", ReasonByHost));
                    else switch (Reason)
                        {
                            case DisconnectReasons.Banned:
                                SetText("您被该房间封禁");
                                break;
                            case DisconnectReasons.Kicked:
                                SetText("您被该房间踢出");
                                break;
                            case DisconnectReasons.GameNotFound:
                                SetText("您与服务器的连接已中断\n这可能是因为您的网络不稳定\n也可能是因为服务器不稳定或拒绝了您的访");
                                break;
                            case DisconnectReasons.GameStarted:
                                SetText("该房间正在游戏中，请等待游戏结束后加入");
                                break;
                            case DisconnectReasons.GameFull:
                                SetText("该房间已满人，请稍后重试");
                                break;
                            case DisconnectReasons.IncorrectVersion:
                                SetText("您的AmongUs版本与该房间不同");
                                break;
                        }
                }
                catch { }
            }, 0.01f, "Override Disconnect Text");
        }
    }
}
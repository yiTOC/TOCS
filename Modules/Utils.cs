using AmongUs.Data;
using AmongUs.GameOptions;
using Hazel;
using Il2CppInterop.Runtime.InteropTypes;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using TOCS.Roles.Core;
using UnityEngine;
using static TOCS.Roles.Core.RoleBase;

namespace TOCS
{
    public static class Utils
    {
        public static void KickPlayer(int playerId, bool ban, string reason)
        {
            if (!AmongUsClient.Instance.AmHost) return;
            OnPlayerLeftPatch.Add(playerId);
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetKickReason, SendOption.Reliable, -1);
            writer.Write($"通知.{reason}");
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            _ = new LateTask(() =>
            {
                AmongUsClient.Instance.KickPlayer(playerId, ban);
            }, Math.Max(AmongUsClient.Instance.Ping / 500f, 1f), "Kick Player");
        }
        public static bool HasTasks(GameData.PlayerInfo p, bool ForRecompute = true)
        {
            if (GameStates.IsLobby) return false;
            //Tasksがnullの場合があるのでその場合タスク無しとする
            if (p.Tasks == null) return false;
            if (p.Role == null) return false;
            if (p.Disconnected) return false;

            var hasTasks = true;
            var States = PlayerState.GetByPlayerId(p.PlayerId);
            if (p.Role.IsImpostor)
                hasTasks = false; //タスクはCustomRoleを元に判定する
            // 死んでいて，死人のタスク免除が有効なら確定でfalse
            var role = States.MainRole;
            var roleClass = CustomRoleManager.GetByPlayerId(p.PlayerId);
            if (roleClass != null)
            {
                switch (roleClass.HasTasks)
                {
                    case HasTask.True:
                        hasTasks = true;
                        break;
                    case HasTask.False:
                        hasTasks = false;
                        break;
                    case HasTask.ForRecompute:
                        hasTasks = !ForRecompute;
                        break;
                }
            }
            return hasTasks;
        }
        public static Sprite LoadSprite(string path, float pixelsPerUnit = 1f)
        {
            try
            {
                if (CachedSprites.TryGetValue(path + pixelsPerUnit, out var sprite)) return sprite;
                Texture2D texture = LoadTextureFromResources(path);
                sprite = Sprite.Create(texture, new(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
                sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
                return CachedSprites[path + pixelsPerUnit] = sprite;
            }
            catch
            {
                Logger.Error($"读入Texture失败：{path}", "LoadImage");
            }
            return null;
        }
        public static Texture2D LoadTextureFromResources(string path)
        {
            try
            {
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
                var texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                using MemoryStream ms = new();
                stream.CopyTo(ms);
                ImageConversion.LoadImage(texture, ms.ToArray(), false);
                return texture;
            }
            catch
            {
                Logger.Error($"读入Texture失败：{path}", "LoadImage");
            }
            return null;
        }
        public static Dictionary<string, Sprite> CachedSprites = new();
    }
}
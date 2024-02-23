using AmongUs.GameOptions;
using HarmonyLib;
using Hazel;
using Il2CppSystem.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using TOCS.Attributes;

namespace TOCS.Roles.Core
{
    public static class CustomRoleManager
    {
        //游戏玩法
        public static Dictionary<byte, RoleBase> AllActiveRoles = new(15);
        public static Dictionary<CustomRoles, SimpleRoleInfo> AllRolesInfo = new(CustomRolesHelper.AllRoles.Length);
            public static SimpleRoleInfo GetRoleInfo(this CustomRoles role) => AllRolesInfo.ContainsKey(role) ? AllRolesInfo[role] : null;
        public static RoleBase GetByPlayerId(byte playerId) => AllActiveRoles.TryGetValue(playerId, out var roleBase) ? roleBase : null;
        public static bool OnSabotage(PlayerControl player, SystemTypes systemType)
        {
            bool cancel = false;
            foreach (var roleClass in AllActiveRoles.Values)
            {
                if (!roleClass.OnSabotage(player, systemType))
                {
                    cancel = true;
                }
            }
            return !cancel;
        }
        // ==初始化处理 ==
        [GameModuleInitializer]
        public static void Initialize()
        {
            AllActiveRoles.Clear();
        }

        /// <summary>
        /// 全部对象的销毁事件
        /// </summary>
        public static void Dispose()
        {
            Logger.Info($"Dispose ActiveRoles", "CustomRoleManager");
            AllActiveRoles.Values.ToArray().Do(roleClass => roleClass.Dispose());
        }
    }
    public enum CustomRoles
    {
        //注册的职业
    }
    //游戏中的阵营
    public enum CustomRoleTypes
    {
        Crewmate,
        Impostor,
        Neutral,
        Addon
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AmongUs.GameOptions;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using TOCS.Roles.Core;
using UnityEngine;

[assembly: AssemblyFileVersionAttribute(TOCS.Main.PluginVersion)]
[assembly: AssemblyInformationalVersionAttribute(TOCS.Main.PluginVersion)]
[assembly: AssemblyVersion(TOCS.Main.PluginVersion)]

namespace TOCS
{
    [BepInPlugin(PluginGuid, "Town Of Crew Sea Of Shark", PluginVersion)]
    [BepInIncompatibility("jp.ykundesu.supernewroles")]
    [BepInProcess("Among Us.exe")]
    public class Main : BasePlugin
    {
        //Mod名
        public static readonly string ModName = "Town Of Crew Shark Of Sea";
        //分支ID
        public static readonly string ForkId = "TOSC";
        //Mod颜色
        public static readonly string ModColor = "Yellowgreen";
        //Mod版本
        public const string PluginVersion = "1.0.0";
        public static Dictionary<byte, PlayerVersion> playerVersion = new();
        //mod著名
        public const string PluginGuid = "com.emptybottle.townofcrewsharkofsea";
        //Mod支持的最低版本
        public static readonly string LowestSupportedVersion = "2023.10.24";//也支持11.28和2024.02.07
        //显示文本
        public static string CredentialsText;
        //颜色
        public static Dictionary<CustomRoles, string> roleColors;
        //mod加载
        public static BepInEx.Logging.ManualLogSource Logger;
        //引用示例
        public static Main Instance;
        public static bool hasArgumentException = false;
        public static string ExceptionMessage;
        public static bool ExceptionMessageIsShown = false;
        //TOSC
        public static Dictionary<int, int> SayStartTimes = new();
        public static Dictionary<int, int> SayBanwordsTimes = new();
        public override void Load()
        {
            hasArgumentException = false;
            Instance = this;
            Logger = BepInEx.Logging.Logger.CreateLogSource("TOCS");
            TOCS.Logger.Enable();
            TOCS.Logger.Disable("NotifyRoles");
            TOCS.Logger.Disable("SwitchSystem");
            TOCS.Logger.Disable("CustomRpcSender");
            try
            {
                roleColors = new Dictionary<CustomRoles, string>()
                {
                    //输入职业的颜色
                    //示例：{CustomRoles.职业英文名, "颜色代码"},
                };
            }
            catch (ArgumentException ex)
            {
                TOCS.Logger.Error("错误：字典出现重复项", "LoadDictionary");
                TOCS.Logger.Exception(ex, "LoadDictionary");
                hasArgumentException = true;
                ExceptionMessage = ex.Message;
                ExceptionMessageIsShown = false;
            }
            TOCS.Logger.Msg("========= TOCS 成功加载! =========", "Plugin Load");
        }
    }
}
//死亡原因
public enum CustomDeathReason
{
    //这里写死亡原因的英文
    Suicide,
}
public enum CustomWinner
{
    //抢夺胜利的职业写这
}
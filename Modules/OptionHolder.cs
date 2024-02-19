using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TOCS.Modules;
using UnityEngine;

namespace TOCS;

[HarmonyPatch]
public static class Options
{
    static Task taskOptionsLoad;
    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.Initialize)), HarmonyPostfix]
    public static void OptionsLoadStart_Postfix()
    {
        Logger.Msg("Mod设置开始加载", "Load Options");
        taskOptionsLoad = Task.Run(Load);
        taskOptionsLoad.ContinueWith(t => { Logger.Msg("Mod设置加载结束", "Load Options"); });
    }
    public static bool IsLoaded = false;
    public static void Load()
    {
        //开始加载设置
        if (IsLoaded) return;

        //结束加载设置
        IsLoaded = true;
    }
}
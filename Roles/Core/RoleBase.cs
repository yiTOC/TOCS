using AmongUs.GameOptions;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using TOCS.Modules;
using UnityEngine;

namespace TOCS.Roles.Core;

public abstract class RoleBase : IDisposable
{
    public PlayerControl Player { get; private set; }
    /// <summary>
    /// 玩家状态
    /// </summary>
    public readonly PlayerState MyState;
    /// <summary>
    /// 玩家任务状态
    /// </summary>
    public readonly TaskState MyTaskState;
    /// <summary>
    /// 是否拥有任务
    /// 默认只有在您是船员的时候有任务
    /// </summary>
    protected Func<HasTask> hasTasks;
    /// <summary>
    /// 是否拥有技能按钮
    /// </summary>
    public bool HasAbility { get; private set; }
    /// <summary>
    /// 是否拥有任务
    /// </summary>
    public HasTask HasTasks => hasTasks.Invoke();
    public RoleBase(
        SimpleRoleInfo roleInfo,
        PlayerControl player,
        Func<HasTask> hasTasks = null,
        bool? hasAbility = null,
        bool? canBeMadmate = null
    )
    {
        Player = player;
        this.hasTasks = hasTasks ?? (roleInfo.CustomRoleType == CustomRoleTypes.Crewmate ? () => HasTask.True : () => HasTask.False);
        HasAbility = hasAbility ?? roleInfo.BaseRoleType.Invoke() is
            RoleTypes.Shapeshifter or
            RoleTypes.Engineer or
            RoleTypes.Scientist or
            RoleTypes.GuardianAngel or
            RoleTypes.CrewmateGhost or
            RoleTypes.ImpostorGhost;

        MyState = PlayerState.GetByPlayerId(player.PlayerId);
        MyTaskState = MyState.GetTaskState();

        CustomRoleManager.AllActiveRoles.Add(Player.PlayerId, this);
    }

#pragma warning disable CA1816
    public void Dispose()
    {
        OnDestroy();
        CustomRoleManager.AllActiveRoles.Remove(Player.PlayerId);
        Player = null;
    }
#pragma warning restore CA1816
    /// <summary>
    /// 实例被销毁时调用的函数
    /// </summary>
    public virtual void OnDestroy()
    { }
    public virtual bool OnSabotage(PlayerControl player, SystemTypes systemType) => true;

    // NameSystem
    // 显示的名字结构如下
    // [Role][Progress]
    // [Name][Mark]
    // [Lower][suffix]
    // Progress：任务进度、剩余子弹等信息
    // Mark：通过位置能力等进行目标标记
    // Lower：附加文本信息，模组端则会显示在屏幕下方
    // Suffix：其他信息，例如箭头

    /// <summary>
    /// 作为 seen 重写显示上的 RoleName
    /// </summary>
    /// <param name="seer">将要看到您的 RoleName 的玩家</param>
    /// <param name="enabled">是否显示 RoleName</param>
    /// <param name="roleColor">RoleName 的颜色</param>
    /// <param name="roleText">RoleName 的文本</param>
    public enum HasTask
    {
        True,
        False,
        ForRecompute
    }
}

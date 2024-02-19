using AmongUs.GameOptions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using TOCS.Attributes;
using TOCS.Roles.Core;

namespace TOCS
{
    public class PlayerState
    {
        byte PlayerId;
        public CustomRoles MainRole;
        public List<CustomRoles> SubRoles;
        public CountTypes CountType { get; private set; }
        public bool IsDead { get; set; }
        public CustomDeathReason DeathReason { get; set; }
        public TaskState taskState;
        public bool IsBlackOut { get; set; }
        public (DateTime, byte) RealKiller;
        public PlainShipRoom LastRoom;
        public Dictionary<byte, string> TargetColorData;
        public PlayerState(byte playerId)
        {
            SubRoles = new();
            CountType = CountTypes.OutOfGame;
            PlayerId = playerId;
            IsDead = false;
            taskState = new();
            IsBlackOut = false;
            RealKiller = (DateTime.MinValue, byte.MaxValue);
            LastRoom = null;
            TargetColorData = new();
        }
        public TaskState GetTaskState() { return taskState; }
        public void UpdateTask(PlayerControl player)
        {
            taskState.Update(player);
        }
        public void SetCountType(CountTypes countType) => CountType = countType;
        public byte GetRealKiller()
            => IsDead && RealKiller.Item1 != DateTime.MinValue ? RealKiller.Item2 : byte.MaxValue;
        private static Dictionary<byte, PlayerState> allPlayerStates = new(15);
        public static IReadOnlyDictionary<byte, PlayerState> AllPlayerStates => allPlayerStates;

        public static PlayerState GetByPlayerId(byte playerId) => AllPlayerStates.TryGetValue(playerId, out var state) ? state : null;

        [GameModuleInitializer]
        public static void Clear() => allPlayerStates.Clear();
        public static void Create(byte playerId)
        {
            if (allPlayerStates.ContainsKey(playerId))
            {
                Logger.Warn($"重複したIDのPlayerStateが作成されました: {playerId}", nameof(PlayerState));
                return;
            }
            allPlayerStates[playerId] = new(playerId);
        }
    }
    public class PlayerVersion
    {
        public readonly Version version;
        public readonly string tag;
        public readonly string forkId;
        public PlayerVersion(Version ver, string tag_str, string forkId)
        {
            version = ver;
            tag = tag_str;
            this.forkId = forkId;
        }
        public bool IsEqual(PlayerVersion pv)
        {
            return pv.version == version && pv.tag == tag;
        }
    }
    public class TaskState
    {
        public static int InitialTotalTasks;
        public int AllTasksCount;
        public int CompletedTasksCount;
        public bool hasTasks;
        public int RemainingTasksCount => AllTasksCount - CompletedTasksCount;
        public bool IsTaskFinished => RemainingTasksCount <= 0 && hasTasks;
        public TaskState()
        {
            this.AllTasksCount = -1;
            this.CompletedTasksCount = 0;
            this.hasTasks = false;
        }

        public void Init(PlayerControl player)
        {
            if (player == null || player.Data == null || player.Data.Tasks == null) return;
            if (!Utils.HasTasks(player.Data, false))
            {
                AllTasksCount = 0;
                return;
            }
            hasTasks = true;
            AllTasksCount = player.Data.Tasks.Count;
        }
        public void Update(PlayerControl player)
        {
            //初期化出来ていなかったら初期化
            if (AllTasksCount == -1) Init(player);

            if (!hasTasks) return;

            //クリアしてたらカウントしない
            if (CompletedTasksCount >= AllTasksCount) return;

            CompletedTasksCount++;

            //調整後のタスク量までしか表示しない
            CompletedTasksCount = Math.Min(AllTasksCount, CompletedTasksCount);
        }
        public bool HasCompletedEnoughCountOfTasks(int count) =>
                IsTaskFinished || CompletedTasksCount >= count;
    }
}
public static class GameStates
{
    public static bool InGame = false;
    public static bool IsInGame => InGame;
    public static bool IsLobby => AmongUsClient.Instance.GameState == AmongUsClient.GameStates.Joined;
}
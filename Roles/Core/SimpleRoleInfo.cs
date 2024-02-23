using AmongUs.GameOptions;
using System;
using System.Linq;
using UnityEngine;

namespace TOCS.Roles.Core;

public class SimpleRoleInfo
{
    public CustomRoleTypes CustomRoleType;
    public Func<RoleTypes> BaseRoleType;
    public OptionCreatorDelegate OptionCreator;
    public IntegerValueRule AssignCountRule;
    public Type ClassType;
    public Func<PlayerControl, RoleBase> CreateInstance;
    public CustomRoles RoleName;
    public CountTypes CountType;
    public Color RoleColor;
    public string RoleColorCode;
    public int ConfigId;
    public bool IsDesyncImpostor;
    private Func<AudioClip> introSound;
    public AudioClip IntroSound => introSound?.Invoke();
    public string ChatCommand;
    public bool Experimental;
    public bool Broken;
    public CustomRoles[] AssignUnitRoles;
    private SimpleRoleInfo(
        Type classType,
        Func<PlayerControl, RoleBase> createInstance,
        CustomRoles roleName,
        Func<RoleTypes> baseRoleType,
        CustomRoleTypes customRoleType,
        CountTypes countType,
        int configId,
        OptionCreatorDelegate optionCreator,
        string chatCommand,
        string colorCode,
        bool isDesyncImpostor,
        TabGroup tab,
        Func<AudioClip> introSound,
        bool experimental,
        bool broken,
        IntegerValueRule assignCountRule,
        CustomRoles[] assignUnitRoles
    )
    {
        ClassType = classType;
        CreateInstance = createInstance;
        RoleName = roleName;
        BaseRoleType = baseRoleType;
        CustomRoleType = customRoleType;
        CountType = countType;
        ConfigId = configId;
        OptionCreator = optionCreator;
        IsDesyncImpostor = isDesyncImpostor;
        this.introSound = introSound;
        ChatCommand = chatCommand;
        Experimental = experimental;
        Broken = broken;
        AssignCountRule = assignCountRule;
        AssignUnitRoles = assignUnitRoles;

        if (colorCode == "")
            colorCode = customRoleType switch
            {
                CustomRoleTypes.Impostor => "#ff1919",
                CustomRoleTypes.Crewmate => "#8cffff",
                _ => "#ffffff"
            };
        RoleColorCode = colorCode;

        _ = ColorUtility.TryParseHtmlString(colorCode, out RoleColor);
        
        CustomRoleManager.AllRolesInfo.Add(roleName, this);
    }
    public delegate void OptionCreatorDelegate();
}
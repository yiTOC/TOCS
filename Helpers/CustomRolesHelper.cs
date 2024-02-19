using AmongUs.GameOptions;
using System.Linq;

using TOCS.Roles.Core;

namespace TOCS
{
    static class CustomRolesHelper
    {
        public static readonly CustomRoles[] AllStandardRoles = AllRoles.ToArray();
        public static readonly CustomRoles[] AllRoles = EnumHelper.GetAllValues<CustomRoles>();
    }
    public enum CountTypes
    {
        OutOfGame,
    }
}
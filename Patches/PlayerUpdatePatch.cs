using System.Collections.Generic;

using EquipmentInWater.Core;
using EquipmentInWater.Swimming;

using HarmonyLib;

namespace EquipmentInWater.Patches;

/// <summary>The hide/show weapons key: only the "show" branch checks for swimming.</summary>
[HarmonyPatch(typeof(Player), "Update")]
[Feature(Feature)]
internal static class Player_Update_Patch
{
    /// <summary>Without it, the show key refuses in water as in vanilla; equipping still works.</summary>
    public const string Feature = "Show weapons key while swimming";

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return SwimmingCheck.RemoveCheck(instructions, "Player.Update");
    }
}

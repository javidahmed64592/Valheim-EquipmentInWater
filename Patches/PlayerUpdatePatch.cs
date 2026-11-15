using System.Collections.Generic;

using EquipmentInWater.Swimming;

using HarmonyLib;

namespace EquipmentInWater.Patches;

/// <summary>The hide/show weapons key: only the "show" branch checks for swimming.</summary>
[HarmonyPatch(typeof(Player), "Update")]
internal static class Player_Update_Patch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return SwimmingCheck.RemoveCheck(instructions, "Player.Update");
    }
}

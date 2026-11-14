using System.Collections.Generic;

using EquipmentInWater.Swimming;

using HarmonyLib;

namespace EquipmentInWater.Patches;

[HarmonyPatch(typeof(Humanoid), nameof(Humanoid.EquipItem))]
internal static class Humanoid_EquipItem_Patch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return SwimmingCheck.RemoveCheck(instructions, "Humanoid.EquipItem");
    }
}

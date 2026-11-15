using System.Collections.Generic;

using EquipmentInWater.Swimming;

using HarmonyLib;

namespace EquipmentInWater.Patches;

[HarmonyPatch(typeof(Humanoid), "UpdateEquipment")]
internal static class Humanoid_UpdateEquipment_Patch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return SwimmingCheck.RemoveCheck(instructions, "Humanoid.UpdateEquipment");
    }
}

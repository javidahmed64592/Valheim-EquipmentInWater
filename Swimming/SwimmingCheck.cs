using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

namespace EquipmentInWater.Swimming;

/// <summary>
/// Vanilla keeps the player's hands empty while swimming in three places: <c>UpdateEquipment</c> calls
/// <c>HideHandItems</c> every frame, <c>EquipItem</c> refuses to equip anything, and the hide/show weapons
/// key in <c>Player.Update</c> won't take sheathed items back out. Each is gated on <c>IsSwimming()</c>, so
/// each patch makes that call report "not swimming" inside the one method. Movement and stamina elsewhere
/// still see the real value.
/// </summary>
internal static class SwimmingCheck
{
    private static readonly MethodInfo IsSwimming = AccessTools.Method(typeof(Character), nameof(Character.IsSwimming));

    public static IEnumerable<CodeInstruction> RemoveCheck(IEnumerable<CodeInstruction> instructions, string method)
    {
        int replaced = 0;
        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.Calls(IsSwimming))
            {
                replaced++;
                // Replace the call (consuming `this`) with false, keeping any labels on it.
                yield return new CodeInstruction(OpCodes.Pop) { labels = instruction.labels };
                yield return new CodeInstruction(OpCodes.Ldc_I4_0);
            }
            else
            {
                yield return instruction;
            }
        }

        // A game update can move the check; say so rather than silently doing nothing.
        if (replaced == 0)
        {
            EquipmentInWaterPlugin.Log.LogWarning(method + " has no swimming check to remove: the game may have " +
                "changed, so this part of the mod does nothing.");
        }
    }
}

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using BepInEx;

using HarmonyLib;

using Jotunn.Utils;

namespace EquipmentInWater;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
[BepInDependency(Jotunn.Main.ModGuid)]
[NetworkCompatibility(CompatibilityLevel.NotEnforced, VersionStrictness.None)]
public class EquipmentInWaterPlugin : BaseUnityPlugin
{
    public const string PluginGUID = "javidahmed64592.equipmentinwater";
    public const string PluginName = "Equipment In Water";
    public const string PluginVersion = "0.1.0";

    private Harmony _harmony;

    private void Awake()
    {
        _harmony = new Harmony(PluginGUID);
        _harmony.PatchAll();
    }

    private void OnDestroy()
    {
        _harmony?.UnpatchSelf();
    }
}

/// <summary>
/// Vanilla forces the player to sheathe while swimming in two places: <c>UpdateEquipment</c> calls
/// <c>HideHandItems</c> every frame, and <c>EquipItem</c> refuses to equip anything. Both are gated on
/// <c>IsSwimming()</c>, so each patch makes that call report "not swimming" inside the one method.
/// Movement and stamina elsewhere still see the real value.
/// </summary>
internal static class SwimmingCheck
{
    private static readonly MethodInfo IsSwimming = AccessTools.Method(typeof(Character), nameof(Character.IsSwimming));

    public static IEnumerable<CodeInstruction> RemoveCheck(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.Calls(IsSwimming))
            {
                // Replace the call (consuming `this`) with false, keeping any labels on it.
                yield return new CodeInstruction(OpCodes.Pop) { labels = instruction.labels };
                yield return new CodeInstruction(OpCodes.Ldc_I4_0);
            }
            else
            {
                yield return instruction;
            }
        }
    }
}

[HarmonyPatch(typeof(Humanoid), "UpdateEquipment")]
internal static class UpdateEquipmentPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return SwimmingCheck.RemoveCheck(instructions);
    }
}

[HarmonyPatch(typeof(Humanoid), nameof(Humanoid.EquipItem))]
internal static class EquipItemPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return SwimmingCheck.RemoveCheck(instructions);
    }
}

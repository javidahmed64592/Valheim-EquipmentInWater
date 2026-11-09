using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using BepInEx;
using BepInEx.Logging;

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
    public const string PluginVersion = "0.2.0";

    internal static ManualLogSource Log { get; private set; }

    private Harmony _harmony;

    private void Awake()
    {
        Log = Logger;
        _harmony = new Harmony(PluginGUID);
        _harmony.PatchAll();
    }

    private void OnDestroy()
    {
        _harmony?.UnpatchSelf();
    }
}

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

[HarmonyPatch(typeof(Humanoid), "UpdateEquipment")]
internal static class UpdateEquipmentPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return SwimmingCheck.RemoveCheck(instructions, "Humanoid.UpdateEquipment");
    }
}

[HarmonyPatch(typeof(Humanoid), nameof(Humanoid.EquipItem))]
internal static class EquipItemPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return SwimmingCheck.RemoveCheck(instructions, "Humanoid.EquipItem");
    }
}

/// <summary>The hide/show weapons key: only the "show" branch checks for swimming.</summary>
[HarmonyPatch(typeof(Player), "Update")]
internal static class PlayerUpdatePatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return SwimmingCheck.RemoveCheck(instructions, "Player.Update");
    }
}

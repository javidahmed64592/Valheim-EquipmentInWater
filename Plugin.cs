using BepInEx;

using EquipmentInWater.Core;

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
    public const string PluginVersion = "1.1.0";

    private Harmony _harmony;

    private void Awake()
    {
        _harmony = new Harmony(PluginGUID);
        _ = SafePatcher.PatchAll(_harmony, Config, Logger);
    }

    private void OnDestroy()
    {
        _harmony?.UnpatchSelf();
    }
}

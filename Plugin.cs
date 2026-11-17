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
    public const string PluginVersion = "1.0.0";

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

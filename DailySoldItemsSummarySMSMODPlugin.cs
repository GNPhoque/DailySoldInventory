using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace DailySoldInventory
{
	[BepInPlugin(MyGUID, PluginName, VersionString)]
	public class DailySoldItemsSummarySMSMODPlugin : BaseUnityPlugin
	{
		private const string MyGUID = "com.Phoque.DailySoldInventory";
		private const string PluginName = "DailySoldInventory";
		private const string VersionString = "1.0.0";

		private static readonly Harmony Harmony = new Harmony(MyGUID);
		public static ManualLogSource Log = new ManualLogSource(PluginName);

		/// <summary>
		/// Initialise the configuration settings and patch methods
		/// </summary>
		private void Awake()
		{
			// Apply all of our patches
			Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loading...");
			Harmony.PatchAll();
			Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");

			Log = Logger;
		}
	}
}

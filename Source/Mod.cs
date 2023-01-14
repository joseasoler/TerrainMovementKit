using System;
using System.Collections.Generic;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Linq;
using TerrainMovement.Cell;

namespace TerrainMovement
{
	public sealed class HarmonyStarter : Mod
	{
		private const string HarmonyId = "net.mseal.rimworld.mod.terrain.movement";

		public HarmonyStarter(ModContentPack content) : base(content)
		{
			Assembly terrainAssembly = Assembly.GetExecutingAssembly();
			string DLLName = terrainAssembly.GetName().Name;
			Version loadedVersion = terrainAssembly.GetName().Version;
			Version laterVersion = loadedVersion;

			List<ModContentPack> runningModsListForReading = LoadedModManager.RunningModsListForReading;
			foreach (ModContentPack mod in runningModsListForReading)
			{
				foreach (var item in from f in ModContentPack.GetAllFilesForMod(mod, "Assemblies/",
					         e => e.ToLower() == ".dll")
				         select f.Value)
				{
					var newAssemblyName = AssemblyName.GetAssemblyName(item.FullName);
					if (newAssemblyName.Name != DLLName || newAssemblyName.Version <= laterVersion) continue;
					laterVersion = newAssemblyName.Version;
					Log.Error(
						$"TerrainMovementKit load order error detected. {content.Name} is loading an older version {loadedVersion} before {mod.Name} loads version {laterVersion}. Please put the TerrainMovementKit, or BiomesCore modes above this one if they are active.");
				}
			}

			var harmony = new Harmony(HarmonyId);
			harmony.PatchAll(terrainAssembly);
			// Manual patches.
			RCellFinderPatches.Patch(harmony);
		}
	}
}
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TerrainMovement.Cell
{
	/// <summary>
	/// Allow animal spawns to take place over terrain types that do not allow wandering.
	/// </summary>
	public static class RCellFinderPatches
	{
		/// <summary>
		/// Delegate used internally by RCellFinder.RandomAnimalSpawnCell_MapGen to find desirable tiles.
		/// </summary>
		/// <returns>Method information of the delegate to be patched.</returns>
		private static MethodInfo GetDelegate()
		{
			const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
			return typeof(RCellFinder).GetNestedTypes(flags)
				.SelectMany(nestedType => nestedType.GetMethods(flags))
				.FirstOrDefault(method => method.Name.Contains(nameof(RCellFinder.RandomAnimalSpawnCell_MapGen)));
		}

		/// <summary>
		/// Find the delegate and apply the patch to it.
		/// </summary>
		/// <param name="harmony">Harmony library instance.</param>
		public static void Patch(Harmony harmony)
		{
			var targetDelegate = GetDelegate();
			if (targetDelegate == null)
			{
				Log.Error("" +
				          "[TerrainMovementKit] RCellFinder.RandomAnimalSpawnCell_MapGen patch failed. Could not find delegate!");
				return;
			}

			var delegateTranspiler = new HarmonyMethod(AccessTools.Method(
				typeof(RCellFinderPatches), "TranspileDelegate"));
			harmony.Patch(targetDelegate, transpiler: delegateTranspiler);
		}

		/// <summary>
		/// Removes the if block that checks c.GetTerrain(map).avoidWander.
		/// </summary>
		/// <param name="instructions">Original code instructions.</param>
		/// <returns>Patched code instructions.</returns>
		private static IEnumerable<CodeInstruction> TranspileDelegate(IEnumerable<CodeInstruction> instructions)
		{
			var instructionList = instructions.ToList();

			// Index in which avoidWander appears.
			var avoidWander = AccessTools.Field(typeof(TerrainDef), nameof(TerrainDef.avoidWander));
			var avoidWanderIndex = instructionList.FindIndex(
				instruction => instruction.opcode == OpCodes.Ldfld
				               && (FieldInfo) instruction.operand == avoidWander);

			// Offsets to the beginning and end of the if block.
			var ignoreStart = avoidWanderIndex - 4;
			var ignoreEnd = avoidWanderIndex + 3;

			if (avoidWanderIndex == -1)
			{
				Log.Error(
					"[TerrainMovementKit] RCellFinder.RandomAnimalSpawnCell_MapGen patch failed. Could not find avoidWander call in delegate!");
				ignoreStart = int.MaxValue;
				ignoreEnd = int.MinValue;
			}

			// The label for jumping from the avoidWander block to the next one is now unused.
			instructionList[ignoreEnd + 1].ExtractLabels();
			// Replace it with the label that was pointing to the avoidWander block.
			instructionList[ignoreStart].MoveLabelsTo(instructionList[ignoreEnd + 1]);
			for (var index = 0; index < instructionList.Count; ++index)
			{
				if (index < ignoreStart || index > ignoreEnd)
				{
					yield return instructionList[index];
				}
			}
		}
	}
}
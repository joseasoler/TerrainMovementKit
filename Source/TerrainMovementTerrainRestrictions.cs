using RimWorld;
using Verse;

namespace TerrainMovement
{
	public class TerrainMovementTerrainRestrictions : DefModExtension
	{
		/// <summary>
		/// SwimmingKit treats this stat as the vanilla one, in opposition to its own swim stat as the modded one.
		/// </summary>
		public StatDef disallowedPathCostStat = StatDefOf.MoveSpeed;
	}
}
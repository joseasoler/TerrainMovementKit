using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace TerrainMovement
{
	public class TerrainMovementStatDef : DefModExtension
	{
		public StatDef terrainPathCostStat;
		public StatDef pawnSpeedStat;

		/// <summary>
		/// If you introduce a new stat that has a dramatically different speed than default movement, you may want to be
		/// able to disable it when using a certain locomotion. For example, maybe Stuart Little should use default movement
		/// and texture when Ambling or Walking, but use SandDrillingSpeed when Jogging or Sprinting. You can combine this
		/// with the texture specific to a cost path to enable a texture transition when a pawn goes from wandering to
		/// running (or burrowing) to an objective.
		/// </summary>
		public List<LocomotionUrgency> disallowedLocomotionUrgencies = new List<LocomotionUrgency>();
	}
}
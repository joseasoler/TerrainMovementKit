using System.Collections.Generic;
using RimWorld;
using Verse;

namespace TerrainMovement
{
	/// <summary>
	/// Use alternate pawn graphics when a specific speed stat is in use.
	/// </summary>
	public class TerrainMovementPawnKindGraphics : DefModExtension
	{
		public StatDef pawnSpeedStat;
		public GraphicData bodyGraphicData;
		public GraphicData femaleGraphicData = null;
		public List<GraphicData> alternateGraphicsData = new List<GraphicData>();
	}
}
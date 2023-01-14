using System.Collections.Generic;
using Verse;

namespace TerrainMovement
{
	/// <summary>
	/// Restrict pawn movement depending on terrain type tags.
	/// </summary>
	public class TerrainMovementPawnRestrictions : DefModExtension
	{
		public string stayOffTerrainTag = null;
		public string stayOnTerrainTag = null;
		public bool defaultMovementAllowed = true;

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (var error in base.ConfigErrors())
			{
				yield return error;
			}

			if (stayOffTerrainTag == null && stayOnTerrainTag == null)
			{
				yield return "TerrainMovementPawnRestrictions must specify stayOffTerrainTag or stayOnTerrainTag.";
			}
		}
	}
}
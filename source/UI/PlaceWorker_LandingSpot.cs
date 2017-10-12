using Verse;
using System.Linq;

namespace WM.SelfLaunchingPods
{
	public class PlaceWorker_LandingSpot : PlaceWorker
	{
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Thing thingToIgnore = null)
		{
			var interactionCell = loc + rot.FacingCell;
			var buildings = Map.thingGrid.ThingsAt(interactionCell)
			                   .Where((Thing arg) => arg.def.passability == Traversability.Impassable || arg is Building);

			if (buildings.Any() && buildings.Any((Thing arg) => arg.def != DefOf.WM_TransportPod))
			{
				return (AcceptanceReport.WasRejected);
			}

			var allSpots = Map.listerBuildings.AllBuildingsColonistOfDef(DefOf.WM_LandingSpot);

			if (allSpots.Any((Building arg) => arg.InteractionCell == interactionCell || arg.InteractionCell == loc))
			{
				return (AcceptanceReport.WasRejected);
			}

			return (AcceptanceReport.WasAccepted);
		}
	}
}

using Verse;
using System.Linq;

namespace WM.SelfLaunchingPods
{
	public class PlaceWorker_LandingSpot : PlaceWorker
	{
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Thing thingToIgnore = null)
		{
			var interactionCell = loc + rot.FacingCell;

			if (!interactionCell.Standable(Map) || interactionCell.GetFirstBuilding(Map) != null)
				return (AcceptanceReport.WasRejected);

			var allSpots = Map.listerBuildings.AllBuildingsColonistOfDef(DefOf.WM_LandingSpot);

			if (allSpots.Any((Building arg) => arg.InteractionCell == interactionCell || arg.InteractionCell == loc))
				return (AcceptanceReport.WasRejected);

			return (AcceptanceReport.WasAccepted);
		}
	}
}

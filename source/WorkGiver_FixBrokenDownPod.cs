using System;
using Harmony;
using RimWorld;
using Verse;
using Verse.AI;

namespace WM.SelfLaunchingPods
{
	public class WorkGiver_FixBrokenDownPod : RimWorld.WorkGiver_FixBrokenDownBuilding
	{
		// RimWorld.WorkGiver_FixBrokenDownBuilding
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			var building = t as Building;

			if (building == null)
			{
				return false;
			}
			if (!building.def.building.repairable)
			{
				return false;
			}
			if (t.Faction != pawn.Faction)
			{
				return false;
			}
			if (t.IsForbidden(pawn))
			{
				return false;
			}
			var comp = building.TryGetComp<CompPlannedBreakdownable>();
			if (comp == null || !comp.BrokenDown)
			{
				return false;
			}
			if (pawn.Faction == Faction.OfPlayer && !pawn.Map.areaManager.Home[t.Position])
			{
				JobFailReason.Is(WorkGiver_FixBrokenDownBuilding.NotInHomeAreaTrans);
				return false;
			}
			if (!pawn.CanReserve(building, 1, -1, null, forced))
			{
				return false;
			}
			if (pawn.Map.designationManager.DesignationOn(building, DesignationDefOf.Deconstruct) != null)
			{
				return false;
			}
			if (building.IsBurning())
			{
				return false;
			}
			if (this.FindClosestComponent(pawn) == null)
			{
				JobFailReason.Is(NoComponentsToRepairTrans);
				return false;
			}
			return true;
		}

		//Thing FindClosestComponent(Pawn pawn)
		//{
		//	return ((Thing)Traverse.Create(this).Method("FindClosestComponent").GetValue(new object[] { pawn }));
		//}
		// RimWorld.WorkGiver_FixBrokenDownBuilding
		private Thing FindClosestComponent(Pawn pawn)
		{
			Predicate<Thing> validator = (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x, 1, -1, null, false);
			return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(ThingDefOf.Component), PathEndMode.InteractionCell, TraverseParms.For(pawn, pawn.NormalMaxDanger(), TraverseMode.ByPawn, false), 9999f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
		}

		static string NoComponentsToRepairTrans
		{
			get
			{
				return ((string)Traverse.Create<RimWorld.WorkGiver_FixBrokenDownBuilding>().Field("NoComponentsToRepairTrans").GetValue());
			}
		}
	}
}

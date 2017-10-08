using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace WM.SelfLaunchingPods
{
	public static class VanillaLikeUtils
	{
		// RimWorld.DropCellFinder
		public static bool TryFindDropSpotNear(IntVec3 center, Map map, out IntVec3 result, bool allowFogged, bool canRoofPunch)
		{
			if (DebugViewSettings.drawDestSearch)
			{
				map.debugDrawer.FlashCell(center, 1f, "center");
			}
			Predicate<IntVec3> validator = (IntVec3 c) => VanillaLikeUtils.IsGoodDropSpot(c, map, allowFogged, canRoofPunch) && map.reachability.CanReach(center, c, PathEndMode.OnCell, TraverseMode.PassDoors, Danger.Deadly);
			int num = 5;
			while (!CellFinder.TryFindRandomCellNear(center, map, num, validator, out result))
			{
				num += 3;
				if (num > 16)
				{
					result = center;
					return false;
				}
			}
			return true;
		}

		// RimWorld.DropCellFinder
		public static bool IsGoodDropSpot(IntVec3 c, Map map, bool allowFogged, bool canRoofPunch)
		{
			if (!c.InBounds(map) || !c.Standable(map) || c.GetFirstBuilding(map) != null)
			{
				return false;
			}
			if (!VanillaLikeUtils.CanPhysicallyDropInto(c, map, canRoofPunch))
			{
				if (DebugViewSettings.drawDestSearch)
				{
					map.debugDrawer.FlashCell(c, 0f, "phys");
				}
				return false;
			}
			if (Current.ProgramState == ProgramState.Playing && !allowFogged && c.Fogged(map))
			{
				return false;
			}
			List<Thing> thingList = c.GetThingList(map);
			for (int i = 0; i < thingList.Count; i++)
			{
				Thing thing = thingList[i];
				if (thing is IActiveDropPod || thing.def.category == ThingCategory.Skyfaller)
				{
					return false;
				}
				if (thing.def.category != ThingCategory.Plant && GenSpawn.SpawningWipes(ThingDefOf.ActiveDropPod, thing.def))
				{
					return false;
				}
			}
			return true;
		}

		// RimWorld.DropCellFinder
		private static bool CanPhysicallyDropInto(IntVec3 c, Map map, bool canRoofPunch)
		{
			if (!c.Walkable(map))
			{
				return false;
			}
			RoofDef roof = c.GetRoof(map);
			if (roof != null)
			{
				if (!canRoofPunch)
				{
					return false;
				}
				if (roof.isThickRoof)
				{
					return false;
				}
			}
			return true;
		}
	}
}

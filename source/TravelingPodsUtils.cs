using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace WM.ReusePods
{
	public static class TravelingPodsUtils
	{
		//public static void CreateTravelingPodsWorldObject(int startTile, int targetTile, IntVec3 targetCell, PawnsArriveMode arriveMode, bool attackOnArrival, PodsFleet fleet)
		//{
		//	// ============== mod ============== 

		//	var travelingTransportPods = (TravelingTransportPods_MK)WorldObjectMaker.MakeWorldObject(DefOf.WM_TravelingTransportPods);

		//	// ============== /mod ============== 

		//	travelingTransportPods.Tile = startTile;
		//	travelingTransportPods.SetFaction(Faction.OfPlayer);
		//	travelingTransportPods.destinationTile = targetTile;
		//	travelingTransportPods.destinationCell = targetCell;
		//	travelingTransportPods.arriveMode = arriveMode;
		//	travelingTransportPods.attackOnArrival = attackOnArrival;

		//	travelingTransportPods.Fleet = fleet;

		//	Find.WorldObjects.Add(travelingTransportPods);
		//}

		// RimWorld.CompLaunchable
		public static float FuelNeededToLaunchAtDistance(float dist, int podsAmount)
		{
			return Config.PodFuelUsePerTile * dist + Config.PodFuelUsePerLaunch * podsAmount;
		}

		public static void RemoveAllPawnsFromWorldPawns(IEnumerable<Pawn> pawns)
		{
			foreach (var item in pawns)
			{
				if (item.IsWorldPawn())
				{
					Find.WorldPawns.RemovePawn(item);
				}
			}
		}


		//internal static void SpawnLandedFleetAtTile(PodsFleet fleet, int tile)
		//{
		//	int startingTile;
		//	if (!GenWorldClosest.TryFindClosestPassableTile(tile, out startingTile))
		//	{
		//		startingTile = tile;
		//	}


		//	var wo = (TravelingTransportPods_MK_Landed)WorldObjectMaker.MakeWorldObject(DefOf.WM_TravelingTransportPods_Landed);

		//	wo.Tile = tile;
		//	wo.Fleet = fleet;

		//	Find.WorldObjects.Add(wo);
		//}

		internal static WorldHopper CreateWorldHopper(int tile, IEnumerable<ActiveDropPodInfo> podsInfo, IEnumerable<Thing> podsLandedThings)
		{
			var hopper = (WorldHopper)WorldObjectMaker.MakeWorldObject(DefOf.WM_TravelingTransportPods);

			hopper.Tile = tile;
			hopper.SetFaction(Faction.OfPlayer);

			var max = podsInfo.Count();

			for (int i = 0; i < max; i++)
			{
				hopper.AddPod(podsLandedThings.ElementAt(i), podsInfo.ElementAt(i));
			}

			Find.WorldObjects.Add(hopper);

			return hopper;
		}
	}
}
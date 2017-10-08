using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace WM.SelfLaunchingPods
{
	public static class TravelingPodsUtils
	{
		// RimWorld.CompLaunchable
		public static float FuelNeededToLaunchAtDistance(int dist, int podsCount)
		{
			return (Config.PodFuelUsePerLaunch + Config.PodFuelUsePerTile * dist) * podsCount;
		}

		public static int MaxLaunchDistance(float fuelAmount, int podsCount, bool oneway)
		{
			return Mathf.FloorToInt(((fuelAmount - podsCount * Config.PodFuelUsePerLaunch * (oneway ? 1 : 2)) / podsCount) / (Config.PodFuelUsePerTile * (oneway ? 1 : 2)));
		}

		internal static IEnumerable<WorldTraveler> GetRemoteTradable()
		{
			var list = Find.WorldObjects.AllWorldObjects.Where((WorldObject arg) => arg is WorldTraveler).Cast<WorldTraveler>();

			return	(list
					.Where((WorldTraveler arg) => arg.remoteTrader.CanRemoteTradeNow));
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

		internal static WorldTraveler CreateWorldTraveler(int tile, IEnumerable<ActiveDropPodInfo> podsInfo, IEnumerable<Thing> podsLandedThings)
		{
			var hopper = (WorldTraveler)WorldObjectMaker.MakeWorldObject(DefOf.WM_TravelingTransportPods);

			hopper.Tile = tile;
			hopper.SetFaction(Faction.OfPlayer);

			var max = podsInfo.Count();

			for (int i = 0; i < max; i++)
			{
				hopper.AddPod(podsLandedThings.ElementAt(i), podsInfo.ElementAt(i));
			}

			Find.WorldObjects.Add(hopper);

			return (hopper);
		}

		internal static void ToCaravan(Caravan caravan, IEnumerable<Thing> thingsToTransfer)
		{
			var list = thingsToTransfer.ToList();

			foreach (var item in list)
			{
				item.holdingOwner.Remove(item);
				var pawn = item as Pawn;
				if (pawn != null)
				{
					caravan.AddPawn(pawn, true);
					if (!pawn.IsWorldPawn())
						Find.WorldPawns.PassToWorld(pawn);
				}
				else
					CaravanInventoryUtility.GiveThing(caravan, item);
			}
		}

		internal static bool FromCaravan(WorldTraveler traveler, Caravan caravan, IEnumerable<Thing> thingsToTransfer)
		{
			if (MissingMassCapacity(traveler, thingsToTransfer.ToList()) > 0)
			{
				return (false);
			}

			var list = thingsToTransfer.ToList();
			int i = 0;
			float usedMass = 0;

			list.SortByDescending((Thing arg) => arg.GetStatValue(StatDefOf.Mass) * arg.stackCount);
			foreach (var item in list)
			{
				var info = traveler.PodsInfo.ElementAt(i);
				var podThing = traveler.PodsAsThing.ElementAt(i);

				usedMass += item.GetStatValue(StatDefOf.Mass) * item.stackCount;
				if (item is Pawn)
				{
					if (Find.WorldPawns.Contains((Pawn)item))
						Find.WorldPawns.RemovePawn((Pawn)item);
					usedMass += MassUtility.GearAndInventoryMass(item as Pawn);
				}

				if (item.holdingOwner != null)
					item.holdingOwner.TryTransferToContainer(item, info.innerContainer);
				else
					info.innerContainer.TryAdd(item, item.stackCount);

				if (usedMass > podThing.TryGetComp<CompTransporter>().Props.massCapacity && traveler.PodsCount < i)
				{
					usedMass = 0;
					i++;
				}
			}

			if (!caravan.pawns.Any)
			{
				Find.WorldObjects.Remove(caravan);
			}

			return (true);
		}

		internal static float MissingMassCapacity(WorldTraveler traveler, List<Thing> thingsToLoad)
		{
			return (CollectionsMassCalculator.MassUsage<Thing>(thingsToLoad, IgnorePawnsInventoryMode.Ignore, true) - (traveler.MaxCapacity - traveler.MassUsage));
		}
	}
}
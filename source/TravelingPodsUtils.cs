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

			return hopper;
		}

		internal static void ToCaravan(WorldTraveler traveler, Caravan caravan, IEnumerable<Thing> thingsToTransfer)
		{
			var list = thingsToTransfer.ToList();

			foreach (var item in list)
			{
				item.holdingOwner.Remove(item);
				CaravanInventoryUtility.GiveThing(caravan, item);
				if (item is Pawn)
				{
					//Find.WorldPawns.PassToWorld((Verse.Pawn)item);
					item.holdingOwner.Remove(item);
				}
			}
		}

		internal static bool FromCaravan(WorldTraveler traveler, Caravan caravan, IEnumerable<Thing> thingsToTransfer)
		{
			if (MissingMassCapacity(traveler, caravan) > 0)
			{
				return false;
			}

			var		list = thingsToTransfer.ToList();
			int		i = 0;
			float	usedMass = 0;

			list.SortByDescending((Thing arg) => arg.GetStatValue(StatDefOf.Mass) * arg.stackCount);
			foreach (var item in list)
			{
				var	info = traveler.PodsInfo.ElementAt(i);
				var	podThing = traveler.PodsAsThing.ElementAt(i);

				usedMass += item.GetStatValue(StatDefOf.Mass) * item.stackCount;
				if (item is Pawn)
					usedMass += MassUtility.GearAndInventoryMass(item as Pawn);
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

			return true;
		}

		internal static float MissingMassCapacity(WorldTraveler traveler, Caravan caravan)
		{
			return (Utils.CaravanWeight(caravan)) - (traveler.MaxCapacity - traveler.MassUsage);
		}
	}
}
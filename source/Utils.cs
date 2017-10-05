using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace WM.SelfLaunchingPods
{
	public static class Utils
	{
		public static bool IsMyClass(object instance)
		{
			var t = instance as Thing;
			return instance is CompSelfLaunchable || t != null && (t.TryGetComp<CompSelfLaunchable>() != null || t.def == DefOf.WM_TransportPod);
		}

		internal static IEnumerable<IntVec3> FindLandingSpotsNear(Map map, IntVec3 intVec)
		{
			List<Building>	launchers = FindBuildingsWithinRadius(map, intVec, 7f, DefOf.WM_LandingSpot);

			return launchers.Select((Building arg) => arg.Position);
		}

		internal static Caravan FindCaravanAt(int tile)
		{
			return ((Caravan)Find.WorldObjects.ObjectsAt(tile).FirstOrDefault((WorldObject arg) => arg is Caravan));
		}

		internal static List<Building> FindBuildingsWithinRadius(Map map, IntVec3 center, float radius, ThingDef def)
		{
			List<Building> list = new List<Building>();

			List<Building> thingsList = map.listerBuildings.AllBuildingsColonistOfDef(def).ToList();

			foreach (Building current in thingsList)
			{
				if (current.Position.DistanceToSquared(center) <= Math.Pow(radius, 2))
					list.Add(current);
			}

			return list;
		}

		internal static bool CanLandAt(Map map, IntVec3 pos)
		{
			return !pos.Roofed(map) && IsAtPad(map, pos);
		}

		internal static bool IsAtPad(Map map, IntVec3 pos)
		{
			return (RimWorld.FuelingPortUtility.FuelingPortGiverAtFuelingPortCell(pos, map) != null);
			//return true;
		}

		internal static float CaravanWeight(Caravan caravan)
		{
			return (CollectionsMassCalculator.MassUsage<Pawn>(caravan.pawns.InnerListForReading, IgnorePawnsInventoryMode.DontIgnore, true, false));
		}
	}
}
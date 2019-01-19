using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace WM.SelfLaunchingPods
{
	public static class Utils
	{
		public static bool IsMyClass(object instance)
		{
			var t = instance as Thing;
			return instance is CompSelfLaunchable || t != null && (t.TryGetComp<CompSelfLaunchable>() != null || t.def == ThingDefOf.WM_TransportPod);
		}

		internal static List<IntVec3> FindLandingSpotsNear(Map map, IntVec3 intVec)
		{
			var launchers = FindBuildingsWithinRadius(map, intVec, ModController.LandingSpotMaxRange, ThingDefOf.WM_LandingSpot);

			return
				(launchers
				.Select((Building arg) => new
				{
					map = arg.Map,
					cell = arg.InteractionCell
				})
				 .Where((arg) =>
						arg.cell.Standable(arg.map) &&
						!arg.cell.GetThingList(map).Any((Thing arg2) => arg2.def == ThingDefOf.WM_DropPodIncoming))
				.Select((arg) => arg.cell)
				.Distinct().ToList());
		}

		public static int ThingsOfDefCount(IEnumerable<Thing> allCarriedThings, ThingDef def)
		{
			return (allCarriedThings.Where((Thing arg) => arg.def == def).Sum((Thing arg) => arg.stackCount));
		}

		public static Caravan FindCaravanAt(int tile)
		{
			return ((Caravan)Find.WorldObjects.ObjectsAt(tile).FirstOrDefault((WorldObject arg) => arg is Caravan));
		}

		internal static List<Building> FindBuildingsWithinRadius(Map map, IntVec3 center, float radius, ThingDef def)
		{
			var list = new List<Building>();

			var thingsList = map.listerBuildings.AllBuildingsColonistOfDef(def).ToList();

			foreach (Building current in thingsList)
			{
				if (current.Position.DistanceToSquared(center) <= Math.Pow(radius, 2))
					list.Add(current);
			}

			return (list);
		}

		internal static bool CanLandAt(Map map, IntVec3 pos)
		{
			return !pos.Roofed(map) && IsAtPad(map, pos);
		}

		internal static bool IsAtPad(Map map, IntVec3 pos)
		{
			return (RimWorld.FuelingPortUtility.FuelingPortGiverAtFuelingPortCell(pos, map) != null);
		}

		internal static float CaravanWeight(Caravan caravan)
		{
			return (CollectionsMassCalculator.MassUsage<Pawn>(caravan.pawns.InnerListForReading, IgnorePawnsInventoryMode.DontIgnore, true, false));
		}

		public static IEnumerable<T2> WhereCast<T1, T2>(this IEnumerable<T1> obj)
		{
			return (obj.Where((arg) => arg is T2).Cast<T2>());
		}

		public static bool SafeToLaunch(ThingWithComps t)
		{
			var compPlannedBreakdownable = t.TryGetComp<CompPlannedBreakdownable>();

			return (compPlannedBreakdownable != null &&
					//t.HitPoints - (compPlannedBreakdownable.Props.damageRatePerUse * t.HitPoints) > 0 &&
					!compPlannedBreakdownable.BrokenDown);
		}

		public static float HitpointsRate(Thing parent)
		{
#pragma warning disable IDE0004 // Remove Unnecessary Cast
			return ((float)parent.HitPoints / (float)parent.MaxHitPoints);
#pragma warning restore IDE0004 // Remove Unnecessary Cast
		}

		public static bool PodIsBrokenDown(Thing arg, bool checkHitpoints = false)
		{
			var comp = arg.TryGetComp<CompPlannedBreakdownable>();

			return ((comp.BrokenDown || (checkHitpoints && !comp.DamageLevelAllowsUse)));
		}

		private static readonly Vector2 FuelBarSize = new Vector2(0.62f, 0.2f);
		private static readonly Vector3 FuelBarOffset = new Vector3(0, 0.1f, 0.2f);

		internal static void DrawFuelOverlay(float fuelPercent, Vector3 drawPos)
		{
			GenDraw.FillableBarRequest r = default(GenDraw.FillableBarRequest);

			r.center = drawPos + FuelBarOffset;
			r.size = FuelBarSize;
			r.fillPercent = fuelPercent;
			r.filledMat = Resources.FuelBarFilledMat;
			r.unfilledMat = Resources.FuelBarUnfilledMat;
			r.margin = 0.15f;
			r.rotation = Rot4.West;
			GenDraw.DrawFillableBar(r);
		}

		public static IEnumerable<WorldTraveler> GetSelectedTravelers()
		{
			return (Find.WorldSelector.SelectedObjects.WhereCast<WorldObject, WorldTraveler>());
		}
	}
}
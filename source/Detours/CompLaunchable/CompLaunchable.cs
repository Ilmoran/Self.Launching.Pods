using System;
using Harmony;
using Verse;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;

// Can't find out why I did this at first.

namespace WM.SelfLaunchingPods.Detours.CompLaunchable
{
	//[HarmonyPatch(typeof(RimWorld.CompLaunchable))]
	//[HarmonyPatch("FuelingPortSource", PropertyMethod.Getter)]
	//public static class FuelingPortSource
	//{
	//	static bool Prefix(RimWorld.CompLaunchable __instance, ref Building __result)
	//	{
	//		if (Utils.IsMyClass(__instance))
	//		{
	//			//__result = null;
	//			__result = (Building)__instance.parent;
	//			return (false)
	//		}

	//		return (true)
	//	}

	//	//static void Postfix(RimWorld.CompLaunchable __instance, ref Building __result)
	//	//{
	//	//	if (!Utils.IsMyClass(__instance))
	//	//	{
	//	//		return;
	//	//	}

	//	//	// =========================================================

	//	//	__result = (Building)__instance.parent;

	//	//	// =========================================================
	//	//}
	//}

	//[HarmonyPatch(typeof(RimWorld.CompLaunchable))]
	//[HarmonyPatch("FuelingPortSourceFuel", PropertyMethod.Getter)]
	//public static class FuelingPortSourceFuel
	//{
	//	static bool Prefix(RimWorld.CompLaunchable __instance, ref float __result)
	//	{
	//		if (Utils.IsMyClass(__instance))
	//		{
	//			//__result = null;
	//			var compRefuelable = ((Building)__instance.parent).TryGetComp<CompRefuelable>();
	//			if (compRefuelable != null)
	//				__result = compRefuelable.Fuel;
	//			else
	//				__result = 0f;
	//			return (false)
	//		}

	//		return (true)
	//	}
	//}

	//[HarmonyPatch(typeof(RimWorld.CompLaunchable))]
	//[HarmonyPatch("FuelingPortSourceHasAnyFuel", PropertyMethod.Getter)]
	//public static class FuelingPortSourceHasAnyFuel
	//{
	//	static bool Prefix(RimWorld.CompLaunchable __instance, ref bool __result)
	//	{
	//		if (Utils.IsMyClass(__instance))
	//		{
	//			//__result = null;

	//			var compRefuelable = ((Building)__instance.parent).TryGetComp<CompRefuelable>();
	//			if (compRefuelable != null)
	//				__result = compRefuelable.Fuel > 0f;
	//			else
	//				__result = false;

	//			return (false)
	//		}

	//		return (true)
	//	}
	//}

	//[HarmonyPatch(typeof(RimWorld.CompLaunchable))]
	//[HarmonyPatch("AllInGroupConnectedToFuelingPort", PropertyMethod.Getter)]
	//[HarmonyPatch(typeof(RimWorld.CompLaunchable))]
	//[HarmonyPatch("ConnectedToFuelingPort", PropertyMethod.Getter)]
	//public static class ConnectedToFuelingPort
	//{
	//	static bool Prefix(RimWorld.CompLaunchable __instance, ref bool __result)
	//	{
	//		if (Utils.IsMyClass(__instance))
	//		{
	//			__result = true;
	//			return (false)
	//		}

	//		return (true)
	//	}
	//}

	//[HarmonyPatch(typeof(RimWorld.CompLaunchable), "TryLaunch")]
	//public static class TryLaunch
	//{
	//	static bool Prefix(RimWorld.CompLaunchable __instance, GlobalTargetInfo target, PawnsArriveMode arriveMode, bool attackOnArrival)
	//	{
	//		if (Utils.IsMyClass(__instance))
	//		{
	//			try
	//			{
	//				(__instance as CompSelfLaunchable).TryLaunch(target, arriveMode, attackOnArrival);
	//			}
	//			catch (Exception ex)
	//			{
	//				Log.Error("Exception when lauching " + __instance + ". " + ex + " " + ex.StackTrace);
	//			}
	//			return (false)
	//		}
	//		return (true)
	//	}	//}

	//[HarmonyPatch(typeof(RimWorld.CompLaunchable))]
	//[HarmonyPatch("AllInGroupConnectedToFuelingPort", PropertyMethod.Getter)]
	//public static class AllInGroupConnectedToFuelingPort
	//{
	//	static bool Prefix(RimWorld.CompLaunchable __instance)
	//	{
	//		return !Utils.IsMyClass(__instance);
	//	}

	//	static void Postfix(RimWorld.CompLaunchable __instance, ref bool __result)
	//	{
	//		if (!Utils.IsMyClass(__instance))
	//		{
	//			return;
	//		}

	//		// =========================================================

	//		__result = true;

	//		// =========================================================
	//	}
	//}

}

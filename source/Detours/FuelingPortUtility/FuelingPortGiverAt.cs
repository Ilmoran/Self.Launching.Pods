using System;
using System.Linq;
using Harmony;
using Verse;

namespace WM.SelfLaunchingPods.Detours.FuelingPortUtility
{
	[HarmonyPatch(typeof(RimWorld.FuelingPortUtility), "FuelingPortGiverAtFuelingPortCell")]
	[HarmonyPatch(typeof(RimWorld.FuelingPortUtility), "FuelingPortGiverAt")]
	public static class FuelingPortGiverAt
	{
		public static bool Prefix(ref Building __result, ref IntVec3 c, ref Map map)
		{
			var thingList = c.GetThingList(map);

			var t = thingList.FirstOrDefault((obj) => Utils.IsMyClass(obj));
			if (t != null)
			{
				__result = (Verse.Building)t;
				//__result = null;
				return false;
			}

			return true;
		}	}

	//[HarmonyPatch(typeof(RimWorld.FuelingPortUtility), "FuelingPortGiverAtFuelingPortCell")]
	//public static class FuelingPortGiverAtFuelingPortCell
	//{
	//	public static bool Prefix(ref Building __result, ref IntVec3 c, ref Map map)
	//	{
	//		var thingList = c.GetThingList(map);

	//		var t = thingList.FirstOrDefault((obj) => Utils.IsMyClass(obj));
	//		if (t != null)
	//		{
	//			__result = (Verse.Building)t;
	//			return false;
	//		}

	//		return true;
	//	}
	//}
}

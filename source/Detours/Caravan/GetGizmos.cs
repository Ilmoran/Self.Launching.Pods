using System.Collections.Generic;
using Harmony;
using Verse;

namespace WM.SelfLaunchingPods.Detours.Caravan
{
	[HarmonyPatch(typeof(RimWorld.Planet.Caravan), "GetGizmos")]
	static class GetGizmos
	{
		static void Postfix(ref IEnumerable<Gizmo> __result, RimWorld.Planet.Caravan __instance)
		{
			var newresult = new List<Gizmo>();
			newresult.AddRange(__result);
			newresult.Add(new Command_LoadToCaravan_PawnsAndItems(__instance));
			//newresult.Add(new Command_LoadToCaravan_Pawns(__instance));
			newresult.Add(new Command_LoadToCaravan_Items(__instance));
			__result = newresult;
		}
	}
}

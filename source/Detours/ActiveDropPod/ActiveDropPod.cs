using System;
using System.Reflection;
using RimWorld;
using Verse;
using Verse.Sound;
using Harmony;

namespace WM.SelfLaunchingPods.Detours.ActiveDropPod
{
	[HarmonyPatch(typeof(RimWorld.ActiveDropPod), "PodOpen")]
	static class PodOpen
	{
		class state
		{
			internal Map map;
			internal IntVec3 position;
		}
		static void Prefix(ref state __state, RimWorld.ActiveDropPod __instance)
		{
			__state = new state();

			__state.map = __instance.Map;
			__state.position = __instance.Position;
		}
		static void Postfix(ref state __state, RimWorld.ActiveDropPod __instance)
		{
			if (Utils.IsAtPad(__state.map, __state.position))
			{
				Thing landedPod = ThingMaker.MakeThing(ThingDef.Named("TransportPod"), null);
				landedPod.SetFaction(Faction.OfPlayer);
				GenSpawn.Spawn(landedPod, __state.position, __state.map);
			}
		}
	}
}
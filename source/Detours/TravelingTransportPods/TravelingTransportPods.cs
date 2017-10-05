using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using HugsLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace WM.SelfLaunchingPods.Detours.TravelingTransportPods
{
	[HarmonyPatch(typeof(RimWorld.Planet.TravelingTransportPods), "SpawnDropPodsInMap")]
	public static class SpawnDropPodsInMap
	{
		static bool Prefix()
		{
			return (true);
		}
	}
}

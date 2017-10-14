using System.Collections.Generic;
using Harmony;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace WM.SelfLaunchingPods.Detours.Dialog_LoadTransporters
{
	[HarmonyPatch(typeof(RimWorld.Dialog_LoadTransporters), "AddPawnsToTransferables")]
	static class AddPawnsToTransferables
	{
		static bool Prefix(RimWorld.Dialog_LoadTransporters __instance)
		{
			AddPawnsToTransferables_Internal(__instance);
			return false;
		}

		// RimWorld.Dialog_LoadTransporters
		internal static void AddPawnsToTransferables_Internal(RimWorld.Dialog_LoadTransporters instance)
		{
			List<Pawn> list = RimWorld.Planet.CaravanFormingUtility.AllSendablePawns(getMap(instance), true, false);
			for (int i = 0; i < list.Count; i++)
			{
				AddToTransferables(instance, list[i]);
			}
		}

		private static Map getMap(RimWorld.Dialog_LoadTransporters instance)
		{
			return ((Map)Traverse.Create(instance).Field("map").GetValue());
		}

		private static void AddToTransferables(RimWorld.Dialog_LoadTransporters instance, Thing t)
		{
			//Traverse.Create(instance).Method("AddToTransferables", new object[] { t });
			typeof(RimWorld.Dialog_LoadTransporters).GetMethod("AddToTransferables", AccessTools.all).Invoke(instance, new object[] { t });
		}	}

	[HarmonyPatch(typeof(RimWorld.Dialog_FormCaravan), "AllSendablePawns")]
	static class AllSendablePawns
	{
		static bool Prefix()
		{
			return false;
		}

		static void Postfix(ref List<Pawn> __result, Map map, bool reform)
		{
			__result = CaravanFormingUtility.AllSendablePawns(map, true, reform, reform);
		}	}

	class CaravanFormingUtility
	{
		// RimWorld.Planet.CaravanFormingUtility
		public static List<Pawn> AllSendablePawns(Map map, bool allowDowned, bool allowInMentalState, bool allowEvenIfPrisonerNotSecure = false)
		{
			tmpSendablePawns.Clear();
			List<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
			for (int i = 0; i < allPawnsSpawned.Count; i++)
			{
				Pawn pawn = allPawnsSpawned[i];
				if ((allowDowned || !pawn.Downed) && (allowInMentalState || !pawn.InMentalState) && (!pawn.HostileTo(Faction.OfPlayer) && (pawn.Faction == Faction.OfPlayer || pawn.IsPrisonerOfColony)) && (allowEvenIfPrisonerNotSecure || !pawn.IsPrisoner || pawn.guest.PrisonerIsSecure) && (pawn.GetLord() == null || pawn.GetLord().LordJob is LordJob_VoluntarilyJoinable))
				{
					tmpSendablePawns.Add(pawn);
				}
			}
			return (tmpSendablePawns);
		}

		public static List<Pawn> tmpSendablePawns
		{
			get
			{
				return ((List<Pawn>)Traverse.Create(typeof(RimWorld.Planet.CaravanFormingUtility)).Field("tmpSendablePawns").GetValue());
			}
		}
	}
}
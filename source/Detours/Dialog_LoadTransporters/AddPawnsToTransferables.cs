using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using RimWorld;
using Verse;

namespace WM.SelfLaunchingPods.Detours.Dialog_LoadTransporters
{
	[HarmonyPatch(typeof(RimWorld.Dialog_LoadTransporters), "AddPawnsToTransferables")]
	static class AddPawnsToTransferables
	{
		static void Postfix(RimWorld.Dialog_LoadTransporters __instance)
		{
			try
			{
				var map = getMap(__instance);
				var list = ExtraSendablePawns(map, false, false);

				foreach (var item in list)
				{
					AddToTransferables(__instance, item);
				}
			}
			catch (Exception ex)
			{
				Verse.Log.ErrorOnce("Error when adding extra pawns to sendable pawns list:\n" + ex.ToString(), "wm.error.01".GetHashCode());
			}
		}

		private static Map getMap(RimWorld.Dialog_LoadTransporters instance)
		{
			return ((Map)Traverse.Create(instance).Field("map").GetValue());
		}

		private static void AddToTransferables(RimWorld.Dialog_LoadTransporters instance, Thing t)
		{
			typeof(RimWorld.Dialog_LoadTransporters).GetMethod("AddToTransferables", AccessTools.all).Invoke(instance, new object[] { t });
		}

		internal static IEnumerable<Pawn> ExtraSendablePawns(Map map, bool allowEvenIfInMentalState = false, bool allowEvenIfPrisonerNotSecure = false)
		{
			List<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
			Func<Pawn, bool> validator = delegate (Pawn pawn)
			{
				// Vanilla condition striped from (allowEvenIfDownedOrInMentalState || !pawn.Downed)
				//if ((allowEvenIfDownedOrInMentalState || !pawn.InMentalState) && (pawn.Faction == Faction.OfPlayer || pawn.IsPrisonerOfColony || !pawn.HostileTo(Faction.OfPlayer)) && (pawn.Faction == Faction.OfPlayer || pawn.IsPrisonerOfColony) && (allowEvenIfPrisonerNotSecure || !pawn.IsPrisoner || pawn.guest.PrisonerIsSecure) && (pawn.GetLord() == null || pawn.GetLord().LordJob is LordJob_VoluntarilyJoinable))
				if (pawn.Downed && (allowEvenIfInMentalState || !pawn.InMentalState) && !pawn.HostileTo(Faction.OfPlayer) && (allowEvenIfPrisonerNotSecure || !pawn.IsPrisoner || pawn.guest.PrisonerIsSecure))
				{
					if (pawn.HostFaction == Faction.OfPlayer || pawn.Faction == Faction.OfPlayer)
					{
						return (true);
					}
				}
				return (false);
			};
			return (allPawnsSpawned.Where(validator));
		}
	}
}
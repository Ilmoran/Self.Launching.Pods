//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Harmony;
//using RimWorld;
//using RimWorld.Planet;
//using Verse;

//namespace WM.ReusePods.Detours.CaravanMaker
//{
//	public static class CaravanMaker
//	{
//		// RimWorld.Planet.CaravanMaker
//		public static List<Pawn> tmpPawns
//		{
//			get
//			{
//				return (List<Pawn>)Traverse.Create(typeof(RimWorld.Planet.CaravanMaker)).Field("tmpPawns").GetValue();

//			}
//		}
//	}

//	[HarmonyPatch(typeof(RimWorld.Planet.CaravanMaker), "MakeCaravan")]
//	public static class MakeCaravan
//	{
//		static bool flag { get; set; }
//		static TravelingTransportPods_MK source { get; set; }

//		internal static void DetourOnce(TravelingTransportPods_MK source)
//		{
//			flag = true;
//			MakeCaravan.source = source;
//		}

//		static bool Prefix(ref Caravan __result, IEnumerable<Pawn> pawns, Faction faction, int startingTile, bool addToWorldPawnsIfNotAlready)
//		{
//			//bool flag = Find.WorldObjects.ObjectsAt(startingTile).Any((arg) => arg is TravelingTransportPods_MK && ((TravelingTransportPods_MK)arg).destinationTile == startingTile);

//#if DEBUG
//			Log.Message("MakeCaravan() detour=" + flag);
//#endif
//			if (flag)
//			{
//				flag = false;
//				__result = Internal(pawns, faction, startingTile, addToWorldPawnsIfNotAlready);
//				return false;
//			}
//			return true;
//		}
//		// RimWorld.Planet.CaravanMaker
//		internal static Caravan Internal(IEnumerable<Pawn> pawns, Faction faction, int startingTile, bool addToWorldPawnsIfNotAlready)
//		{
//			if (startingTile < 0 && addToWorldPawnsIfNotAlready)
//			{
//				Log.Warning("Tried to create a caravan but chose not to spawn a caravan but pass pawns to world. This can cause bugs because pawns can be discarded.");
//			}
//			CaravanMaker.tmpPawns.Clear();
//			CaravanMaker.tmpPawns.AddRange(pawns);
//			//MOD
//			var caravan = (TravelingTransportPods_MK_Landed)WorldObjectMaker.MakeWorldObject(DefOf.WM_TravelingTransportPods_Landed);

//			caravan.Fleet = source.Fleet;

//			if (startingTile >= 0)
//			{
//				caravan.Tile = startingTile;
//			}
//			caravan.SetFaction(faction);
//			//caravan.Name = CaravanNameGenerator.GenerateCaravanName(caravan);
//			if (startingTile >= 0)
//			{
//				Find.WorldObjects.Add(caravan);
//			}
//			for (int i = 0; i < CaravanMaker.tmpPawns.Count; i++)
//			{
//				Pawn pawn = CaravanMaker.tmpPawns[i];
//				if (pawn.Spawned)
//				{
//					pawn.DeSpawn();
//				}
//				if (pawn.Dead)
//				{
//					Log.Warning("Tried to form a caravan with a dead pawn " + pawn);
//				}
//				else
//				{
//					caravan.AddPawn(pawn, addToWorldPawnsIfNotAlready);
//					if (addToWorldPawnsIfNotAlready && !pawn.IsWorldPawn())
//					{
//						if (pawn.Spawned)
//						{
//							pawn.DeSpawn();
//						}
//						Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Decide);
//					}
//				}
//			}
//			return caravan;
//		}

//	}
//}

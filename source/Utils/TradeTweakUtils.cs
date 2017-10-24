using RimWorld.Planet;
using Verse;

namespace WM.SelfLaunchingPods
{
	public static class TradeTweakUtils
	{
		static Caravan tmpDummyCaravan;
		static bool respawnNegociatorAfterTrade;
		static ThingOwner tmpNegociatorContainer;
		static ThingOwner tmpInventoryContainer;
		static Pawn negotiator;
		static FactionBase factionBase;
		static WorldTraveler worldTraveler;
		static bool usingTradeDialog;

		public static Caravan PrepareTrade(Pawn arg_playerNegotiator, FactionBase arg_localFactionBase, WorldTraveler arg_worldTraveler, bool arg_usingTradeDialog = true)
		{
			respawnNegociatorAfterTrade = false;
			tmpNegociatorContainer = null;

			negotiator = arg_playerNegotiator;
			factionBase = arg_localFactionBase;
			worldTraveler = arg_worldTraveler;
			usingTradeDialog = arg_usingTradeDialog;

			if (usingTradeDialog)
				Detour.Dialog_Trade.Controler.Setup(worldTraveler.MassCapacity, worldTraveler.MassUsage);

			if (negotiator.Spawned)
			{
				respawnNegociatorAfterTrade = true;
				negotiator.DeSpawn();
			}
			else if (negotiator.holdingOwner != null)
			{
				tmpNegociatorContainer = negotiator.holdingOwner;
				tmpNegociatorContainer.Remove(negotiator);
			}

			tmpInventoryContainer = new ThingOwner<Thing>();
			negotiator.inventory.innerContainer.TryTransferAllToContainer(tmpInventoryContainer);
			tmpDummyCaravan = CaravanMaker.MakeCaravan(new Pawn[] { negotiator }, negotiator.Faction, factionBase.Tile, true);
			TravelingPodsUtils.ToCaravan(tmpDummyCaravan, worldTraveler.AllCarriedThings);
			Find.WorldObjects.Remove(tmpDummyCaravan);

			return (tmpDummyCaravan);
		}

		public static void FinishTrade()
		{
			TravelingPodsUtils.FromCaravan(worldTraveler, tmpDummyCaravan, tmpDummyCaravan.Goods);
			tmpDummyCaravan.RemovePawn(negotiator);
			TravelingPodsUtils.FromCaravan(worldTraveler, tmpDummyCaravan, tmpDummyCaravan.pawns);
			tmpInventoryContainer.TryTransferAllToContainer(negotiator.inventory.innerContainer);
			tmpInventoryContainer.ClearAndDestroyContents();
			if (respawnNegociatorAfterTrade)
			{
				negotiator.SpawnSetup(Find.VisibleMap, false);
			}
			else if (tmpNegociatorContainer != null)
			{
				tmpNegociatorContainer.TryAdd(negotiator);
			}
			if (negotiator.IsWorldPawn())
			{
				Find.WorldPawns.RemovePawn(negotiator);
			}

			if (usingTradeDialog)
				Detour.Dialog_Trade.Controler.Free();
		}
	}
}

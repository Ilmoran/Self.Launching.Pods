using System;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace WM.SelfLaunchingPods
{
	internal class RemoteTrader : ICommunicable
	{
		readonly WorldTraveler worldTraveler;

		public RemoteTrader(WorldTraveler worldTraveler)
		{
			this.worldTraveler = worldTraveler;
		}

		public string GetCallLabel()
		{
			return (String.Format("WM.RemoteTradeWith".Translate(), LocalFactionBase.Name));
		}

		public string GetInfoText()
		{
			return (String.Format("WM.RemoteTradeInfo".Translate()));
		}

		public void TryOpenComms(Pawn negotiator)
		{
			TradeUtils.TradeFromTraveler(this.worldTraveler, negotiator);
		}

		public bool CanRemoteTradeNow
		{
			get
			{
				//return (LocalFactionBase != null && LocalFactionBase.Faction.def.techLevel >= TechLevel.Industrial);
				return (LocalFactionBase != null && !LocalFactionBase.Faction.HostileTo(worldTraveler.Faction));
			}
		}

		FactionBase LocalFactionBase
		{
			get
			{
				return (worldTraveler.LocalFactionBase);
			}
		}
	}

	internal class Dialog_Trade_Remote : Dialog_Trade
	{
		static Caravan dummyCaravan;
		static Pawn negotiator;
		static WorldTraveler worldTraveler;
		static FactionBase factionBase;
		static bool respawnNegociatorAfterTrade;
		static ThingOwner tmpNegociatorContainer;

		static ThingOwner tmpInventoryContainer;
		//static List<Thing> negocitorInventory;
		internal Dialog_Trade_Remote() : base(negotiator, factionBase)
		{
		}

		public override void PostClose()
		{
			base.PostClose();
			FinishTrade();
		}

		internal static void InitTrade(Pawn arg_playerNegotiator, FactionBase arg_localFactionBase, WorldTraveler arg_worldTraveler)
		{
			respawnNegociatorAfterTrade = false;
			tmpNegociatorContainer = null;

			negotiator = arg_playerNegotiator;
			factionBase = arg_localFactionBase;
			worldTraveler = arg_worldTraveler;

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

			dummyCaravan = CaravanMaker.MakeCaravan(new Pawn[] { negotiator }, negotiator.Faction, factionBase.Tile, true);
			//Find.WorldObjects.Add(dummycaravan);
			TravelingPodsUtils.ToCaravan(dummyCaravan, worldTraveler.AllCarriedThings);
		}

		private void FinishTrade()
		{
			TravelingPodsUtils.FromCaravan(worldTraveler, dummyCaravan, dummyCaravan.Goods);
			dummyCaravan.RemovePawn(negotiator);
			TravelingPodsUtils.FromCaravan(worldTraveler, dummyCaravan, dummyCaravan.pawns);
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
				Find.WorldPawns.RemovePawn(negotiator);
		}
	}
}
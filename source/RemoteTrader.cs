using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace WM.SelfLaunchingPods
{
	internal class RemoteTrader : ICommunicable
	{
		WorldTraveler worldTraveler;

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
			Dialog_Trade_Remote.InitTrade(negotiator, LocalFactionBase, worldTraveler);
			var dialog = new Dialog_Trade_Remote();
			Find.WindowStack.Add(dialog);
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
				return (Find.WorldObjects.FactionBaseAt(worldTraveler.Tile));
			}
		}
	}

	internal class Dialog_Trade_Remote : Dialog_Trade
	{
		static Caravan dummyCaravan;
		static Pawn negotiator;
		static WorldTraveler worldTraveler;
		static FactionBase factionBase;

		static ThingOwner tmpContainer;
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

			negotiator = arg_playerNegotiator;
			factionBase = arg_localFactionBase;
			worldTraveler = arg_worldTraveler;

			negotiator.DeSpawn();
			tmpContainer = new ThingOwner<Thing>();
			negotiator.inventory.innerContainer.TryTransferAllToContainer(tmpContainer);

			dummyCaravan = CaravanMaker.MakeCaravan(new Pawn[] { negotiator }, negotiator.Faction, factionBase.Tile, true);
			//Find.WorldObjects.Add(dummycaravan);
			TravelingPodsUtils.ToCaravan(dummyCaravan, worldTraveler.AllCarriedThings);
		}

		private void FinishTrade()
		{
			TravelingPodsUtils.FromCaravan(worldTraveler, dummyCaravan, dummyCaravan.Goods);
			dummyCaravan.RemovePawn(negotiator);
			TravelingPodsUtils.FromCaravan(worldTraveler, dummyCaravan, dummyCaravan.pawns);
			tmpContainer.TryTransferAllToContainer(negotiator.inventory.innerContainer);
			negotiator.SpawnSetup(Find.VisibleMap, false);
		}

	}
}
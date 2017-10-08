using System;
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
			Find.WindowStack.Add(new Dialog_Trade(negotiator, LocalFactionBase));
		}

		public bool CanRemoteTradeNow
		{
			get
			{
				//return (LocalFactionBase != null && LocalFactionBase.Faction.def.techLevel >= TechLevel.Industrial);
				return (LocalFactionBase != null);
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
}
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
}
using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace WM.SelfLaunchingPods
{
	public static class TradeUtils
	{
		public static void TradeFromTraveler(WorldTraveler worldTraveler, Pawn negotiator = null)
		{
			if (negotiator == null)
				negotiator = TradeUtils.BestNegociator(worldTraveler.AllCarriedColonists);
			TradeTweakUtils.PrepareTrade(negotiator, worldTraveler.LocalFactionBase, worldTraveler);
			var dialog = new Dialog_Trade_Remote(negotiator, worldTraveler.LocalFactionBase);
			Find.WindowStack.Add(dialog);
		}

		public static Pawn BestNegociator(IEnumerable<Pawn> pawns)
		{
			var pawns2 = pawns.Where((Pawn arg) => !arg.Downed && !arg.InMentalState);

			if (!pawns2.Any())
				return (null);

			var pawn = pawns2.MaxBy((Pawn arg) => arg.GetStatValue(StatDefOf.TradePriceImprovement));

			return (pawn);
		}


		internal static IEnumerable<WorldTraveler> GetRemoteTradeable()
		{
			var list = Find.WorldObjects.AllWorldObjects.Where((WorldObject arg) => arg is WorldTraveler).Cast<WorldTraveler>();

			return (list.Where((WorldTraveler arg) => arg.remoteTrader.CanRemoteTradeNow));
		}

		public static int FuelAvailableAt(FactionBase factionBase)
		{
			if (factionBase.trader == null || !factionBase.trader.CanTradeNow)
				return 0;

			return (factionBase.trader.StockListForReading
					.Where((Thing arg) => arg.def == DefOf.Chemfuel)
					.Sum((Thing arg) => arg.stackCount));
		}
	}
}

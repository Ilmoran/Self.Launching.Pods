using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace WM.SelfLaunchingPods
{
	public static class TradeUtils
	{
		public static void TradeFromTraveler(WorldTraveler worldTraveler, Pawn negotiator = null)
		{
			if (negotiator == null)
				negotiator = TradeUtils.BestNegociator(worldTraveler.AllCarriedColonists);
			Dialog_Trade_Remote.InitTrade(negotiator, worldTraveler.LocalFactionBase, worldTraveler);
			var dialog = new Dialog_Trade_Remote();
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
	}
}

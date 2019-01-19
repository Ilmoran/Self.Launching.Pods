using RimWorld;
using RimWorld.Planet;
using Verse;

namespace WM.SelfLaunchingPods
{
	public class FactionBaseAvailableFuelCompProperties : WorldObjectCompProperties
	{
		public FactionBaseAvailableFuelCompProperties()
		{
			this.compClass = typeof(FactionBaseAvailableFuelComp);
		}
	}

	public class FactionBaseAvailableFuelComp : WorldObjectComp
	{
		public override string CompInspectStringExtra()
		{
			var SettlementBase = this.parent as SettlementBase;

			if (SettlementBase == null || SettlementBase.trader == null || !SettlementBase.trader.CanTradeNow)
				return "";

			var fuelQt = TradeUtils.FuelAvailableAt((SettlementBase)parent);

			if (fuelQt <= 0)
				return "";

			return (string.Format("WM.FactionBaseInspectStringFuelForSale".Translate(), fuelQt, ThingDefOf.Chemfuel.label));
		}
	}
}

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
		}	}

	public class FactionBaseAvailableFuelComp : WorldObjectComp
	{
		public override string CompInspectStringExtra()
		{
			var factionBase = this.parent as FactionBase;

			if (factionBase == null || factionBase.trader == null || !factionBase.trader.CanTradeNow)
				return "";

			var fuelQt = TradeUtils.FuelAvailableAt((FactionBase)parent);

			if (fuelQt <= 0)
				return "";

			return (string.Format("WM.FactionBaseInspectStringFuelForSale".Translate(), fuelQt, DefOf.Chemfuel.label));
		}
	}
}

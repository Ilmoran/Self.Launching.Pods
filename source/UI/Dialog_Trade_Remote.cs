using RimWorld;
using RimWorld.Planet;
using Verse;

namespace WM.SelfLaunchingPods
{
	internal class Dialog_Trade_Remote : Dialog_Trade
	{
		internal Dialog_Trade_Remote(Pawn negotiator, SettlementBase settlementBase) : base(negotiator, settlementBase)
		{
		}

		public override void PostClose()
		{
			base.PostClose();
			TradeTweakUtils.FinishTrade();
		}
	}
}
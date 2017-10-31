using RimWorld;
using Verse;

namespace WM.SelfLaunchingPods
{
	public class Command_Trade : Command_Traveler
	{
		public Command_Trade(WorldTraveler worldTraveler) : base(worldTraveler)
		{
			this.icon = Resources.GizmoTrade;
			this.action = delegate
			{
				if (TradeUtils.BestNegociator(worldTraveler.AllCarriedColonists) == null)
				{
					Messages.Message("WM.MessageNoCapableNegociatorUseComms".Translate(), MessageTypeDefOf.RejectInput);
					return;
				}
				TradeUtils.TradeFromTraveler(worldTraveler);
			};
		}

		public override bool Visible
		{
			get
			{
				return	(base.Visible &&
						Parent.LocalFactionBase != null &&
						Parent.LocalFactionBase.CanTradeNow);
			}
		}

		public override string Label
		{
			get
			{
				return ("CommandTrade".Translate());
			}
		}
	}
}
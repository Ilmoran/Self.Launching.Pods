using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace WM.SelfLaunchingPods
{
	public class CommsRemoteTradeCompProperties : CompProperties
	{
		public CommsRemoteTradeCompProperties()
		{
			this.compClass = typeof(CommsRemoteTradeComp);
		}
	}

	public class CommsRemoteTradeComp : ThingComp
	{
		public Building_CommsConsole Parent
		{
			get
			{
				return (this.parent as Building_CommsConsole);
			}
		}

		//TODO: can't figure out why this is not working. Using detour.
		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			if (!Parent.CanUseCommsNow)
				return (null);

			var travelers = TradeUtils.GetRemoteTradeable();

			if (!travelers.Any())
				return (null);

			var extraoptions = GetRemoteTradingMenuOptions(selPawn, travelers);

			return (extraoptions);
		}

		IEnumerable<FloatMenuOption> GetRemoteTradingMenuOptions(Pawn myPawn, IEnumerable<WorldTraveler> travelers)
		{
			foreach (var item in travelers)
			{
				var option = new FloatMenuOption(MenuOptionPriority.High);

				option.Label = item.remoteTrader.GetCallLabel();
				option.action = delegate
				{
					var job = new Job(JobDefOf.UseCommsConsole, Parent);

					job.commTarget = item.remoteTrader;
					myPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
				};
				yield return (option);
			}
		}
	}
}
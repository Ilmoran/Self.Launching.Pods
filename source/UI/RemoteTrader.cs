using System;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;

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
			return (String.Format("WM.RemoteTradeWith".Translate(), LocalFactionBase.Label));
		}

		public string GetInfoText()
		{
			return (String.Format("WM.RemoteTradeInfo".Translate()));
		}

		public void TryOpenComms(Pawn negotiator)
		{
			TradeUtils.TradeFromTraveler(this.worldTraveler, negotiator);
		}

        public Faction GetFaction()
        {
            throw new NotImplementedException();
        }

        public FloatMenuOption CommFloatMenuOption(Building_CommsConsole console, Pawn negotiator)
        {
            string label = GetCallLabel();
            Action action = delegate
            {
                var job = new Job(JobDefOf.UseCommsConsole, console);

                job.commTarget = this;
                negotiator.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            };
            var option = new FloatMenuOption(label, action, MenuOptionPriority.High);
            return (option);
        }

        public bool CanRemoteTradeNow
		{
			get
			{
				//return (LocalFactionBase != null && LocalFactionBase.Faction.def.techLevel >= TechLevel.Industrial);
				return (LocalFactionBase != null && !LocalFactionBase.Faction.HostileTo(worldTraveler.Faction));
			}
		}

		SettlementBase LocalFactionBase
		{
			get
			{
				return (worldTraveler.LocalFactionBase);
			}
		}
	}
}
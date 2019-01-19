using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace WM.SelfLaunchingPods
{
	public enum RepairState
	{
		Success,
		NoMaterials,
		NoCapablePawns
	}

	public partial class WorldTraveler : WorldObject
	{
		public int RemainingLaunches
		{
			get
			{
				if (this.PodsAsThing.Any((Thing arg) => !Utils.SafeToLaunch(arg as ThingWithComps)))
					return (0);
				int remainingLaunches = this.PodsAsThing.Min((Thing arg) => arg.TryGetComp<CompPlannedBreakdownable>().RemainingLaunchesUntilBreakdown);
				return (remainingLaunches);

			}
		}

		public int ComponentsCountNeededToRepair
		{
			get
			{
				return (this.PodsAsThing.Count((Thing arg) => Utils.PodIsBrokenDown(arg)));
			}
		}

		public int ComponentsCountAvailable
		{
			get
			{
				return (Utils.ThingsOfDefCount(this.AllCarriedThings, ThingDefOf.ComponentIndustrial));
			}
		}

		public RepairState TryRepair()
		{
			bool flag;

			flag = InventoryUtils.AnyCapablePawn(this.AllCarriedColonists);
			if (!flag)
			{
				return (RepairState.NoCapablePawns);
			}
			if (ComponentsCountNeededToRepair > ComponentsCountAvailable)
			{
				return (RepairState.NoMaterials);
			}

			if (ComponentsCountNeededToRepair > 0)
			{
				this.DiscardThingsOfDefCountOf(ThingDefOf.ComponentIndustrial, ComponentsCountNeededToRepair);
				foreach (ThingWithComps item in this.PodsAsThing)
				{
					item.BroadcastCompSignal(CompPlannedBreakdownable.RepairSignal);
				}
			}

			return (RepairState.Success);
		}

		public bool AnythingToRepair()
		{
			return (this.PodsAsThing.Any((Thing arg) => Utils.PodIsBrokenDown(arg)));
			//return (this.PodsAsThing.Any((Thing arg) => Utils.PodIsBrokenDown(arg) || arg.HitPoints < arg.MaxHitPoints));
		}

		void DamagePods()
		{
			foreach (ThingWithComps item in this.PodsAsThing)
			{
				item.BroadcastCompSignal(CompPlannedBreakdownable.UseSignal);
			}
		}
	}
}

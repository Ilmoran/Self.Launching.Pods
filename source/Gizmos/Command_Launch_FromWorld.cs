using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Sound;

namespace WM.SelfLaunchingPods
{
	public class Command_Launch_FromWorld : Command_Launch
	{
		protected readonly WorldTraveler parent;

		public Command_Launch_FromWorld(WorldTraveler parent)
		{
			this.parent = parent;
		}

		public override float ParentLeastFueledPodFuelLevel
		{
			get
			{
				return (parent.FuelLevelPerPod);
			}
		}

		public override int ParentPodsCount
		{
			get
			{
				return (parent.PodsCount);
			}
		}

		public override int ParentTile
		{
			get
			{
				return (parent.Tile);
			}
		}

		public override IEnumerable<ThingWithComps> PodsList
		{
			get
			{
				return (this.parent.PodsAsThing.Cast<ThingWithComps>());
			}
		}

		public override bool HideWorldAfterLaunch
		{
			get
			{
				return (false);
			}
		}

		public override bool Visible
		{
			get
			{
				return (base.Visible && !this.parent.Traveling && Utils.GetSelectedTravelers().Count() == 1);
			}
		}

		internal override void Launch(int tile, IntVec3 cell, PawnsArrivalModeDef arriveMode, bool attackOnArrival = false)
		{
			parent.Launch(tile, cell, arriveMode, attackOnArrival);
			SoundDefOf.DropPod_Leaving.PlayOneShot(new TargetInfo());
			Find.WorldSelector.Select(this.parent);
		}
	}
}

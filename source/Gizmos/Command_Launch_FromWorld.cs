using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Sound;

namespace WM.SelfLaunchingPods
{
	public class Command_Launch_FromWorld : Command_Launch
	{
		readonly WorldTraveler parent;

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

		public override bool Visible
		{
			get
			{
				return (!parent.Traveling);
			}
		}

		internal override void Launch(int tile, IntVec3 cell, PawnsArriveMode arriveMode = 0, bool attackOnArrival = false)
		{
			parent.Launch(tile, cell, arriveMode, attackOnArrival);
			SoundDefOf.DropPodLeaving.PlayOneShot(new TargetInfo());
		}
	}
}

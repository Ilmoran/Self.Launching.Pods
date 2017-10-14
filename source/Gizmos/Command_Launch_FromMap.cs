using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.Sound;

namespace WM.SelfLaunchingPods
{
	public class Command_Launch_FromMap : Command_Launch
	{
		readonly ThingWithComps parent;

		public Command_Launch_FromMap(ThingWithComps parent)
		{
			this.parent = parent;
		}

		public override float ParentLeastFueledPodFuelLevel
		{
			get
			{
				return parent.TryGetComp<CompSelfLaunchable>()._FuelInLeastFueledFuelingPortSource;
			}
		}

		public override int ParentPodsCount
		{
			get
			{
				return (this.parent.TryGetComp<CompTransporter>().TransportersInGroup(this.parent.Map).Count);
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
				return (this.parent.TryGetComp<CompTransporter>()
						.TransportersInGroup(this.parent.Map)
						.Select((CompTransporter arg) => arg.parent));
			}
		}

		internal override void Launch(int tile, IntVec3 cell, PawnsArriveMode arriveMode = PawnsArriveMode.Undecided, bool attackOnArrival = false)
		{
			GlobalTargetInfo globalTargetInfo;
			var globalTargetInfo2 = new GlobalTargetInfo(tile);
			var mapParent = Find.WorldObjects.MapParentAt(tile);
			Map targetMap = null;

			if (mapParent != null)
			{
				targetMap = mapParent.Map;
			}

			if (targetMap != null)
			{
				globalTargetInfo = new GlobalTargetInfo(cell, targetMap);
			}
			else
			{
				globalTargetInfo = globalTargetInfo2;
			}

			parent.TryGetComp<CompSelfLaunchable>().TryLaunch(globalTargetInfo, arriveMode, attackOnArrival);
			SoundDefOf.DropPodLeaving.PlayOneShot(new TargetInfo());
		}
	}
}

using System;
using RimWorld;
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
				return this.parent.TryGetComp<CompTransporter>().TransportersInGroup(this.parent.Map).Count;
			}
		}

		public override int ParentTile
		{
			get
			{
				return parent.Tile;
			}
		}

		internal override void Launch(int tile, IntVec3 cell)
		{
			parent.TryGetComp<CompSelfLaunchable>().TryLaunch(new RimWorld.Planet.GlobalTargetInfo(tile), PawnsArriveMode.Undecided, false); 
			SoundDefOf.DropPodLeaving.PlayOneShot(new TargetInfo());
		}
	}
}

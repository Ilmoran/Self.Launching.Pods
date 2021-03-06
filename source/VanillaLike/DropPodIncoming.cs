using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace WM.SelfLaunchingPods
{
	public class DropPodIncoming : Skyfaller, IActiveDropPod, IThingHolder
	{
		public Thing landedThing;
		public ActiveDropPodInfo podInfo;

		ActiveDropPodInfo IActiveDropPod.Contents
		{
			get
			{
				return (podInfo);
			}
		}

		IThingHolder IThingHolder.ParentHolder
		{
			get
			{
				return (podInfo.ParentHolder);
			}
		}

		void IThingHolder.GetChildHolders(List<IThingHolder> outChildren)
		{
			podInfo.GetChildHolders(outChildren);
		}

		ThingOwner IThingHolder.GetDirectlyHeldThings()
		{
			return (podInfo.GetDirectlyHeldThings());
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look<Thing>(ref landedThing, "landedThing");
			Scribe_Deep.Look<ActiveDropPodInfo>(ref podInfo, "podInfo");
		}

		// RimWorld.DropPodIncoming
		protected override void Impact()
		{
			for (int i = 0; i < 6; i++)
			{
				Vector3 loc = base.Position.ToVector3Shifted() + Gen.RandomHorizontalVector(1f);
				MoteMaker.ThrowDustPuff(loc, base.Map, 1.2f);
			}
			MoteMaker.ThrowLightningGlow(base.Position.ToVector3Shifted(), base.Map, 2f);

			this.landedThing.TryGetComp<CompSelfLaunchable>().podInfo = this.podInfo; // MOD
			GenSpawn.Spawn(this.landedThing, base.Position, base.Map, base.Rotation); // MOD
			((ThingWithComps)this.landedThing).BroadcastCompSignal(CompPlannedBreakdownable.UseSignal); // MOD

			base.Impact();
		}
	}
}

using Harmony;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WM.SelfLaunchingPods
{
	public class DropPodIncoming : RimWorld.DropPodIncoming
	{
		public Thing landedThing;

		void _HitRoof()
		{
			Traverse.Create(this).Method("HitRoof").GetValue();
		}

		public bool soundPlayed
		{

			get
			{
				return ((bool)Traverse.Create(this as RimWorld.DropPodIncoming).Field("soundPlayed").GetValue());
			}
			set
			{
				Traverse.Create(this as RimWorld.DropPodIncoming).Field("soundPlayed").SetValue(value);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();

			Scribe_References.Look<Thing>(ref landedThing, "landedThing");
		}

		// RimWorld.DropPodIncoming
		public override void Tick()
		{
			this.ticksToImpact--;
			if (this.ticksToImpact == 15)
			{
				this._HitRoof();
			}
			if (this.ticksToImpact <= 0)
			{
				this.Impact();
			}
			if (!this.soundPlayed && this.ticksToImpact < 100)
			{
				this.soundPlayed = true;
				SoundDefOf.DropPodFall.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
			}
		}

		// RimWorld.DropPodIncoming
		private void Impact()
		{
			for (int i = 0; i < 6; i++)
			{
				Vector3 loc = base.Position.ToVector3Shifted() + Gen.RandomHorizontalVector(1f);
				MoteMaker.ThrowDustPuff(loc, base.Map, 1.2f);
			}
			MoteMaker.ThrowLightningGlow(base.Position.ToVector3Shifted(), base.Map, 2f);

			// ============= MOD ============= 

			//RimWorld.ActiveDropPod activeDropPod = (RimWorld.ActiveDropPod)ThingMaker.MakeThing(ThingDefOf.ActiveDropPod, null);
			//activeDropPod.Contents = this.Contents;
			//GenSpawn.Spawn(activeDropPod, base.Position, base.Map, base.Rotation, false);
			this.landedThing.TryGetComp<CompSelfLaunchable>().podInfo = this.Contents;
			GenSpawn.Spawn(this.landedThing, base.Position, base.Map, base.Rotation, false);

			// ============= /MOD ============= 

			RoofDef roof = base.Position.GetRoof(base.Map);
			if (roof != null)
			{
				if (!roof.soundPunchThrough.NullOrUndefined())
				{
					roof.soundPunchThrough.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
				}
				if (roof.filthLeaving != null)
				{
					for (int j = 0; j < 3; j++)
					{
						FilthMaker.MakeFilth(base.Position, base.Map, roof.filthLeaving, 1);
					}
				}
			}
			this.Destroy(DestroyMode.Vanish);
		}
	}
}

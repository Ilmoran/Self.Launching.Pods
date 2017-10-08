using System.Collections.Generic;
using System.Linq;
using Harmony;
using RimWorld;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace WM.SelfLaunchingPods
{
	public class DropPodLeaving : RimWorld.DropPodLeaving
	{
		//=========== mod ============

		public Thing landedThing;

		// ============ /mod ============

		public override void DeSpawn()
		{
			base.DeSpawn();
		}


		private bool _alreadyLeft
		{
			get
			{
				return ((bool)Traverse.Create(this).Field("alreadyLeft").GetValue());
			}
			set
			{
				Traverse.Create(this).Field("alreadyLeft").SetValue(value);
			}
		}

		private ActiveDropPodInfo _contents
		{
			get
			{
				return ((ActiveDropPodInfo)Traverse.Create(this).Field("contents").GetValue());
			}
			set
			{
				Traverse.Create(this).Field("contents").SetValue(value);
			}
		}

		static List<Thing> tmpActiveDropPods
		{
			get
			{
				return ((List<Thing>)Traverse.Create(typeof(RimWorld.DropPodLeaving)).Field("tmpActiveDropPods").GetValue());
			}
		}

		bool _soundPlayed
		{
			get
			{
				return ((bool)Traverse.Create(this).Field("soundPlayed").GetValue());
			}
			set
			{
				Traverse.Create(this).Field("soundPlayed").SetValue(value);
			}
		}
		int _ticksSinceStart
		{
			get
			{
				return ((int)Traverse.Create(this).Field("ticksSinceStart").GetValue());
			}
			set
			{
				Traverse.Create(this).Field("ticksSinceStart").SetValue(value);
			}
		}

		// RimWorld.DropPodLeaving
		public override void Tick()
		{
			if (!this._soundPlayed && this._ticksSinceStart >= -10)
			{
				SoundDefOf.DropPodLeaving.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
				this._soundPlayed = true;
			}
			this._ticksSinceStart++;
			if (!this._alreadyLeft && this._ticksSinceStart >= 220)
			{
				// MOD
				this.GroupLeftMap();
			}
		}

		// RimWorld.DropPodLeaving
		private void GroupLeftMap()
		{
			if (this.groupID < 0)
			{
				Log.Error("Drop pod left the map, but its group ID is " + this.groupID);
				this.Destroy(DestroyMode.Vanish);
				return;
			}
			if (this.destinationTile < 0)
			{
				Log.Error("Drop pod left the map, but its destination tile is " + this.destinationTile);
				this.Destroy(DestroyMode.Vanish);
				return;
			}
			Lord lord = TransporterUtility.FindLord(this.groupID, base.Map);
			if (lord != null)
			{
				base.Map.lordManager.RemoveLord(lord);
			}

			DropPodLeaving.tmpActiveDropPods.Clear();
			DropPodLeaving.tmpActiveDropPods
						  .AddRange(base.Map.listerThings.ThingsInGroup(ThingRequestGroup.ActiveDropPod)
					 	 .Where(arg => ((DropPodLeaving)arg).groupID == this.groupID));

			var podsInfo = (DropPodLeaving.tmpActiveDropPods).Select(arg => (arg as DropPodLeaving).Contents);
			var podsLandedThings = (DropPodLeaving.tmpActiveDropPods).Select(arg => (arg as DropPodLeaving).landedThing);

			var worldobject = TravelingPodsUtils.CreateWorldTraveler(this.Map.Tile, podsInfo, podsLandedThings);

			worldobject.Launch(this.destinationTile, this.destinationCell);

			foreach (DropPodLeaving item in DropPodLeaving.tmpActiveDropPods)
			{
				item._contents = null;
				item._alreadyLeft = true;
				item.Destroy();
			}
		}
	}
}

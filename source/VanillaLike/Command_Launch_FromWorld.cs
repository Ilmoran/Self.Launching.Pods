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

		internal override void Launch(int tile, IntVec3 cell)
		{
			parent.Launch(tile, cell);
			SoundDefOf.DropPodLeaving.PlayOneShot(new TargetInfo());
		}
	}
}

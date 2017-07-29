using System;
using Verse;

namespace WM.ReusePods
{
	public class Command_Launch_FromWorld : Command_Launch
	{
		public Command_Launch_FromWorld()
		{
		}

		private WorldHopper Hopper
		{
			get
			{
				return (WorldHopper)Find.WorldSelector.SingleSelectedObject;
			}
		}
		internal override void Launch(int tile, IntVec3 cell)
		{
			throw new NotImplementedException();
		}

		public override ThingWithComps Parent
		{
			get
			{
				return base.Parent;
			}
		}

		public override int Tile
		{
			get
			{
				return Hopper.Tile;
			}
		}

		public override int MaxLaunchDistance
		{
			get
			{
				return Hopper.MaxLaunchDistance;
			}
		}
	}
}

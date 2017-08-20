using System;
using Verse;

namespace WM.SelfLaunchingPods
{
	public class Command_Launch_FromWorld : Command_Launch
	{
		public Command_Launch_FromWorld(WorldTraveler traveler) : base()
		{
			Traveler = traveler;
		}

		private WorldTraveler Traveler
		{
			get;
			set;
		}
		internal override void Launch(int tile, IntVec3 cell)
		{
			Traveler.Launch(tile, cell);
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
				//this.
				return Traveler.Tile;
			}
		}

		public override int MaxLaunchDistance
		{
			get
			{
				return Traveler.MaxLaunchDistance;
			}
		}
	}
}

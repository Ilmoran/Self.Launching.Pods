using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using Verse;

namespace WM.SelfLaunchingPods
{
	public class Command_LoadToCaravan_Pawns : Command_LoadToCaravan
	{
		public Command_LoadToCaravan_Pawns(Caravan parent) : base(parent)
		{
		}

		public override IEnumerable<Thing> ThingsToLoad
		{
			get
			{
				return (Parent.pawns);
			}
		}

		public override string Label
		{
			get
			{
				return "WM.LoadCaravanPawnsGizmo".Translate();
			}
		}
	}
}
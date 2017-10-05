using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WM.SelfLaunchingPods
{
	public class Command_UnloadCaravan_Pawns : Command_UnloadCaravan
	{
		public Command_UnloadCaravan_Pawns(WorldTraveler parent) : base(parent)
		{
		}

		public override string Label
		{
			get
			{
				return ("WM.UnloadCaravanPawnsGizmo".Translate());
			}
		}

		public override IEnumerable<Thing> ThingsToUnload
		{
			get
			{
				return Parent.AllCarriedPawns.Cast<Thing>();
			}
		}

		public override bool CanDoNow
		{
			get
			{
				return Parent.AllCarriedPawns.Any();
			}
		}

		public override string FailMessage
		{
			get
			{
				if (!CanDoNow)
					return "WM.MessageNoPawnsToUnload".Translate();
				else
					return base.FailMessage;
			}
		}
	}
}

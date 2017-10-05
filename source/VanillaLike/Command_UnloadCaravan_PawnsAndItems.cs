using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WM.SelfLaunchingPods
{
	public class Command_UnloadCaravan_PawnsAndItems : Command_UnloadCaravan
	{
		public Command_UnloadCaravan_PawnsAndItems(WorldTraveler parent) : base(parent)
		{
		}

		public override string Label
		{
			get
			{
				return ("WM.UnloadCaravanGizmo".Translate());
			}
		}

		public override IEnumerable<Thing> ThingsToUnload
		{
			get
			{
				return (Parent.AllCarriedThings);
			}
		}

		public override bool CanDoNow
		{
			get
			{
				return Parent.AllCarriedPawns.Any() && Parent.AllCarriedThings.Any();
			}
		}

		public override string FailMessage
		{
			get
			{
				if (!CanDoNow)
					return ("WM.MessageNothingToUnload".Translate());
				else
					return (base.FailMessage);
			}
		}
	}
}

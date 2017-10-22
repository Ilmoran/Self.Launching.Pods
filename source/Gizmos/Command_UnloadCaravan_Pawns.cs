using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WM.SelfLaunchingPods
{
	public class Command_UnloadCaravan_Pawns : Command_UnloadCaravan
	{
		public Command_UnloadCaravan_Pawns(WorldTraveler parent) : base(parent)
		{
			this.icon = Resources.GizmoUnloadPawns;
		}

		public override string Label
		{
			get
			{
				return ("WM.UnloadCaravanPawnsGizmo".Translate());
			}
		}

		protected override IEnumerable<Thing> ThingsToUnload
		{
			get
			{
				return (Parent.AllCarriedPawns.Cast<Thing>());
			}
		}
	}
}

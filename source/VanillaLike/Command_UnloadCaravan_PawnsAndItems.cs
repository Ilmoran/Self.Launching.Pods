using System.Collections.Generic;
using Verse;

namespace WM.SelfLaunchingPods
{
	public class Command_UnloadCaravan_PawnsAndItems : Command_UnloadCaravan
	{
		public Command_UnloadCaravan_PawnsAndItems(WorldTraveler parent) : base(parent)
		{
            this.icon = Resources.GizmoUnloadEverything;
		}

		public override string Label
		{
			get
			{
				return ("WM.UnloadCaravanGizmo".Translate());
			}
		}

		protected override IEnumerable<Thing> ThingsToUnload
		{
			get
			{
				return (Parent.AllCarriedThings);
			}
		}
	}
}

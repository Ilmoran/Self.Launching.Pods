using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using Verse;

namespace WM.SelfLaunchingPods
{
	public class Command_LoadToCaravan_PawnsAndItems : Command_LoadToCaravan
	{
		public Command_LoadToCaravan_PawnsAndItems(Caravan parent) : base(parent)
		{
		}

		public override IEnumerable<Thing> ThingsToLoad
		{
			get
			{
				var list = Parent.pawns.Cast<Thing>().ToList();

				list.AddRange(Parent.Goods);
				return (list);
			}
		}

		public override string Label
		{
			get
			{
				return "WM.LoadCaravanPawnsAndItemsGizmo".Translate();
			}
		}
	}
}
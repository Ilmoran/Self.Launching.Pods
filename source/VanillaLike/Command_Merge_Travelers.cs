using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.Sound;

namespace WM.SelfLaunchingPods
{
	class Command_Merge_Travelers : Command_Traveler
	{
		public WorldTraveler SingleSelectedTraveler
		{
			get
			{
				return (SelectedTravelers.FirstOrDefault());
			}
		}

		public IEnumerable<WorldTraveler> SelectedTravelers
		{
			get
			{
				return (Find.WorldSelector.SelectedObjects.WhereCast<WorldObject, WorldTraveler>());
			}
		}

		public Command_Merge_Travelers(WorldTraveler worldTraveler) : base(worldTraveler)
		{
			this.icon = Resources.GizmoMergeFleets;
			this.action = delegate
			{
				var list = SelectedTravelers.ToList();
				var n = list.Count;

				for (int i = 1; i < n; i++)
				{
					TravelingPodsUtils.MergeTravelers(SingleSelectedTraveler, list.ElementAt(i));
				}
				SoundDefOf.TickHigh.PlayOneShotOnCamera();
			};
		}

		public override bool Visible
		{
			get
			{
				return (SelectedTravelers.Count() >= 2 &&
						SingleSelectedTraveler != null &&
						 SelectedTravelers.All((WorldTraveler arg) => arg.Tile == SingleSelectedTraveler.Tile));
			}
		}

		public override string Label
		{
			get
			{
				return string.Format("WM.MergeGizmo".Translate(), SelectedTravelers.Count());
			}
		}
	}
}
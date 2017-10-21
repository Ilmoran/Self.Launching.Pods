using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.Sound;

namespace WM.SelfLaunchingPods
{
	class Command_Merge_Travelers_AllOnTile : Command_Traveler
	{
		public WorldTraveler SingleTraveler
		{
			get
			{
				return (TravelersOnTile.FirstOrDefault());
			}
		}

		public IEnumerable<WorldTraveler> TravelersOnTile
		{
			get
			{
				return (Find.WorldObjects.ObjectsAt(this.Parent.Tile).WhereCast<WorldObject,WorldTraveler>());
			}
		}

		public Command_Merge_Travelers_AllOnTile(WorldTraveler worldTraveler) : base(worldTraveler)
		{
			this.icon = Resources.GizmoMergeFleets;
			this.action = delegate
			{
				var list = TravelersOnTile.ToList();

				foreach (var item in list.Skip(1))
				{
					TravelingPodsUtils.MergeTravelers(SingleTraveler, item);
				}

				Find.WorldSelector.Select(SingleTraveler);
				SoundDefOf.TickHigh.PlayOneShotOnCamera();
			};
		}

		public override bool GroupsWith(Gizmo other)
		{
			return (false);
		}

		public override bool Visible
		{
			get
			{
				return (TravelersOnTile.Count() >= 2 &&
				        SingleTraveler != null);
			}
		}

		public override string Label
		{
			get
			{
				return string.Format("WM.MergeAllOnTileGizmo".Translate(), TravelersOnTile.Count());
			}
		}
	}
}
using System.Linq;
using RimWorld;
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
				return (Utils.GetSelectedTravelers().FirstOrDefault());
			}
		}

		public Command_Merge_Travelers(WorldTraveler worldTraveler) : base(worldTraveler)
		{
			this.icon = Resources.GizmoMergeFleets;
			this.action = delegate
			{
				var list = Utils.GetSelectedTravelers().ToList();
				var n = list.Count;

				foreach (var item in list.Skip(1))
				{
					TravelingPodsUtils.MergeTravelers(SingleSelectedTraveler, item);
				}

				RimWorld.SoundDefOf.Tick_High.PlayOneShotOnCamera();
			};
		}

		public override bool Visible
		{
			get
			{
				return (Utils.GetSelectedTravelers().Count() >= 2 &&
						SingleSelectedTraveler != null &&
						Utils.GetSelectedTravelers().All((WorldTraveler arg) => arg.Tile == SingleSelectedTraveler.Tile));
			}
		}

		public override string Label
		{
			get
			{
				return string.Format("WM.MergeGizmo".Translate(), Utils.GetSelectedTravelers().Count());
			}
		}
	}
}
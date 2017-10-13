using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace WM.SelfLaunchingPods
{
	public class CaravanTranferCompProperties : WorldObjectCompProperties
	{
		public CaravanTranferCompProperties()
		{
			this.compClass = typeof(CaravanTranferComp);
		}	}

	public class CaravanTranferComp : WorldObjectComp
	{
		List<Gizmo> gizmos;

		public override void Initialize(RimWorld.WorldObjectCompProperties props)
		{
			base.Initialize(props);

			gizmos = new List<Gizmo>();
			var caravan = this.parent as Caravan;

			gizmos.Add(new Command_LoadToCaravan_PawnsAndItems(caravan));
			gizmos.Add(new Command_LoadToCaravan_Items(caravan));
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			return (gizmos);
		}
	}
}

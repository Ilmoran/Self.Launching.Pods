using System.Linq;

namespace WM.SelfLaunchingPods
{
	public class ModControler : HugsLib.ModBase
	{
		private static readonly string modName = "WM_Self_Launching_Pods";
		internal static readonly float LandingSpotMaxRange = 7f;
		internal static ModControler SingleInstance { get; private set; }

		public override string ModIdentifier
		{
			get
			{
				return (modName);
			}
		}

		public static float PodFuelUsePerTile
		{
			get
			{
				var t = DefOf.WM_TransportPod.comps.First((obj) => obj.GetType() == typeof(CompProperties_SelfLaunchable)) as CompProperties_SelfLaunchable;

				return (t.fuelUsePerTile);
			}
		}

		public static float PodFuelUsePerLaunch
		{
			get
			{
				var t = DefOf.WM_TransportPod.comps.First((obj) => obj.GetType() == typeof(CompProperties_SelfLaunchable)) as CompProperties_SelfLaunchable;

				return (t.fuelUsePerLaunch);
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			SingleInstance = this;
		}

		public override void DefsLoaded()
		{
			base.DefsLoaded();
			DefOf.Caravan.comps.Add(new CaravanTranferCompProperties());
			DefOf.CommsConsole.comps.Add(new CommsRemoteTradeCompProperties());
		}

		internal static class Log
		{
			internal static void Message(string text)
			{
				SingleInstance.Logger.Message(text);
			}
			internal static void Warning(string text)
			{
				SingleInstance.Logger.Warning(text);
			}
			internal static void Error(string text)
			{
				SingleInstance.Logger.Error(text);
			}
		}
	}

	internal static class Log
	{
		internal static void Message(string text)
		{
			ModControler.Log.Message(text);
		}
		internal static void Warning(string text)
		{
			ModControler.Log.Warning(text);
		}
		internal static void Error(string text)
		{
			ModControler.Log.Error(text);
		}
	}
}

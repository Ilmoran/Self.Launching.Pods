using System.Linq;

namespace WM.SelfLaunchingPods
{
	public class Config : HugsLib.ModBase
	{
		private static readonly string modName = "WM_Self_Launching_Pods";
		internal static readonly float LandingSpotMaxRange = 7f;
		internal static Config SingleInstance { get; private set; }
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
			Config.Log.Message(text);
		}
		internal static void Warning(string text)
		{
			Config.Log.Warning(text);
		}
		internal static void Error(string text)
		{
			Config.Log.Error(text);
		}
	}
}

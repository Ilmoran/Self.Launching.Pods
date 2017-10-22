using System.Linq;
using HugsLib;

namespace WM.SelfLaunchingPods
{
	public class ModController : HugsLib.ModBase
	{
		private static readonly string modName = "WM_Self_Launching_Pods";
		internal static readonly float LandingSpotMaxRange = 7f;
		internal static ModController SingleInstance { get; private set; }

		public override string ModIdentifier
		{
			get
			{
				return (modName);
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
			ModController.Log.Message(text);
		}
		internal static void Warning(string text)
		{
			ModController.Log.Warning(text);
		}
		internal static void Error(string text)
		{
			ModController.Log.Error(text);
		}
	}
}

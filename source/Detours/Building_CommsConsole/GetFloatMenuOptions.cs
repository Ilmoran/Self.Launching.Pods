using System.Collections.Generic;
using System.Linq;
using Harmony;
using RimWorld;
using Verse;
using Verse.AI;

namespace WM.SelfLaunchingPods.Detour.Building_CommsConsole
{
	[HarmonyPatch(typeof(RimWorld.Building_CommsConsole), "GetFloatMenuOptions")]
	static class GetFloatMenuOptions
	{
		static void Postfix(RimWorld.Building_CommsConsole __instance, ref IEnumerable<FloatMenuOption> __result, Pawn myPawn)
		{
			if (!__instance.CanUseCommsNow)
				return;

			var travelers = TravelingPodsUtils.GetRemoteTradable();
			if (!travelers.Any())
				return;

			var extraoptions = GetRemoteTradingMenuOptions(__instance, myPawn, travelers);

			__result = __result.Concat(extraoptions);
		}

		static List<FloatMenuOption> GetRemoteTradingMenuOptions(RimWorld.Building_CommsConsole __instance, Pawn myPawn, IEnumerable<WorldTraveler> travelers)
		{
			var extraoptions = new List<FloatMenuOption>();
			foreach (var item in travelers)
			{
				var option = new FloatMenuOption(MenuOptionPriority.InitiateSocial);
				option.Label = item.remoteTrader.GetCallLabel();
				option.action = delegate
				{
					var job = new Job(JobDefOf.UseCommsConsole, __instance);
					job.commTarget = item.remoteTrader;
					myPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
				};
				extraoptions.Add(option);
			}

			return (extraoptions);
		}
	}
}
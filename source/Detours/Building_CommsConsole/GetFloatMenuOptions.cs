using System.Collections.Generic;
using System.Linq;
using Harmony;
using RimWorld;
using Verse;
using Verse.AI;

namespace WM.SelfLaunchingPods.Detours.Building_CommsConsole
{
	//TODO: discard and use comp instead.
	[HarmonyPatch(typeof(RimWorld.Building_CommsConsole), "GetFloatMenuOptions")]
	static class GetFloatMenuOptions
	{
		static void Postfix(RimWorld.Building_CommsConsole __instance, ref IEnumerable<FloatMenuOption> __result, Pawn myPawn)
		{
			if (!__instance.CanUseCommsNow || myPawn.skills.GetSkill(SkillDefOf.Social).TotallyDisabled)
				return;

			var travelers = TradeUtils.GetRemoteTradeable();

			if (!travelers.Any())
				return;

			var extraoptions = GetRemoteTradingMenuOptions(__instance, myPawn, travelers);

			__result = __result.Concat(extraoptions);
		}

		static IEnumerable<FloatMenuOption> GetRemoteTradingMenuOptions(RimWorld.Building_CommsConsole __instance, Pawn myPawn, IEnumerable<WorldTraveler> travelers)
		{
			foreach (var item in travelers)
			{
                yield return item.remoteTrader.CommFloatMenuOption(__instance, myPawn);
			}
		}
	}
}
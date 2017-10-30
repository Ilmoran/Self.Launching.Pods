using Harmony;
using RimWorld;
using Verse;

namespace WM.SelfLaunchingPods.Detour.TradeDeal
{
	[HarmonyPatch(typeof(RimWorld.TradeDeal))]
	[HarmonyPatch("TryExecute")]
	static class TryExecute
	{
		static bool Prefix(ref bool __result)
		{
			if (Dialog_Trade.Controler.Detouring &&
				Dialog_Trade.Controler.CachedMassOffset + Dialog_Trade.Controler.CurrentMassUsage > Dialog_Trade.Controler.MassCapacity)
			{
				//TODO: flash mass
				Messages.Message("WM.MessagePodsFleetCapacityInsufficient".Translate(), MessageTypeDefOf.NeutralEvent);
				__result = false;
				return (false);
			}
			return (true);
		}
	}
}

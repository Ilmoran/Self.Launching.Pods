using System.Collections.Generic;
using System.Linq;
using Harmony;
using RimWorld;
using Verse;

namespace WM.SelfLaunchingPods.Detour.Dialog_Trade
{
	internal static class Controler
	{
		internal static bool Detouring { get; private set; }
		internal static float MassCapacity { get; private set; }
		internal static float CurrentMassUsage { get; private set; }
		internal static float CachedMassOffset { get; private set; }

		internal static float MassUsageAfterTrade(RimWorld.Dialog_Trade instance)
		{
			var cachedTradeables = Traverse.Create(instance).Field("cachedTradeables").GetValue<List<Tradeable>>();

			CachedMassOffset = cachedTradeables.Sum(
							delegate (Tradeable arg)
							{
								float mass;

								mass = arg.AnyThing.GetStatValue(StatDefOf.Mass) * arg.CountToTransfer;
								mass -= arg.CurTotalSilverCost * DefOf.Silver.BaseMass;
								if (arg.AnyThing is Pawn)
									mass += MassUtility.GearAndInventoryMass((Pawn)arg.AnyThing);

								return (mass);
							});

			return (CachedMassOffset);
		}

		internal static void Setup(float massCapacity, float currentMassUsage)
		{
#if DEBUG
			Log.Message("Setup() currentMassUsage=" + currentMassUsage);
#endif
			Detouring = true;
			MassCapacity = massCapacity;
			CurrentMassUsage = currentMassUsage;
		}

		internal static void Free()
		{
			Detouring = false;
		}
	}

	[HarmonyPatch(typeof(RimWorld.Dialog_Trade))]
	[HarmonyPatch("MassCapacity", PropertyMethod.Getter)]
	internal static class MassCapacity
	{
		static bool Prefix(ref float __result)
		{
			if (Controler.Detouring)
			{
				__result = Controler.MassCapacity;
				return (false);
			}
			return (true);
		}	}

	[HarmonyPatch(typeof(RimWorld.Dialog_Trade))]
	[HarmonyPatch("MassUsage", PropertyMethod.Getter)]
	static class MassUsage
	{
		static bool Prefix()
		{
			return (!Controler.Detouring);
		}

		static void Postfix(ref float __result, RimWorld.Dialog_Trade __instance)
		{
			if (Controler.Detouring)
			{
				float massUsageOffset = Controler.MassUsageAfterTrade(__instance);
				__result = Controler.CurrentMassUsage + massUsageOffset;
				//__result = massUsageOffset;
			}
		}
	}
}

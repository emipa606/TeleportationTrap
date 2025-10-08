using RimWorld;
using Verse;

namespace TeleportationTrap;

[DefOf]
public static class EffecterDefOf
{
    public static EffecterDef CustomBlastEMPEffect;

    static EffecterDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(EffecterDefOf));
    }
}
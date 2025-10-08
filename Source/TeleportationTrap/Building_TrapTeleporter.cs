using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;
using Verse.Sound;

namespace TeleportationTrap;

public class Building_TrapTeleporter : Building_Trap
{
    private bool autoRearm; // Tracks the autoRearm state (accessed via reflection)

    private bool isTrapEnabled = true; // Tracks whether the trap is enabled

    // Ensure this Building has a Comp_Linkable component
    private Comp_Linkable LinkableComp => this.TryGetComp<Comp_Linkable>();

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);

        // Access the private 'autoRearm' field using reflection
        var autoRearmField =
            typeof(Building_Trap).GetField("autoRearm", BindingFlags.NonPublic | BindingFlags.Instance);
        if (autoRearmField != null)
        {
            autoRearmField.SetValue(this, false);
        }

        if (!respawningAfterLoad)
        {
            SoundDefOf.TrapArm.PlayOneShot(new TargetInfo(Position, map));
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        // Save and load the isTrapEnabled field
        Scribe_Values.Look(ref isTrapEnabled, "isTrapEnabled", true);

        // Save and load the autoRearm field
        Scribe_Values.Look(ref autoRearm, "autoRearm");
    }

    /// <summary>
    ///     Override the spring chance to return 0 only if the trap is disabled and has linked connections.
    /// </summary>
    protected override float SpringChance(Pawn p)
    {
        if (!isTrapEnabled && LinkableComp?.LinkedThings.Any() == true)
        {
            return 0f; // Disable springing if the trap is disabled and linked
        }

        // Call the base logic for normal behavior
        return base.SpringChance(p);
    }


    public override void DrawExtraSelectionOverlays()
    {
        base.DrawExtraSelectionOverlays();

        // Access the Comp_Linkable and invoke DrawLinks
        var linkableComp = this.TryGetComp<Comp_Linkable>();
        linkableComp?.DrawLinks();
    }

    protected override void SpringSub(Pawn p)
    {
        // Default behavior if trap is disabled but no linked connections
        if (!isTrapEnabled && (LinkableComp == null || !LinkableComp.LinkedThings.Any()))
        {
            // Default behavior: Damage or trap effect
            if (p == null)
            {
                return;
            }

            const float damage = 10f; // Example damage value
            var damageInfo = new DamageInfo(DamageDefOf.Stab, damage, 0, -1f, this);
            p.TakeDamage(damageInfo);

            // Play a generic spring sound
            SoundDefOf.TrapSpring.PlayOneShot(new TargetInfo(Position, Map));

            return;
        }

        // Skip the trap logic entirely if disabled and linked
        if (!isTrapEnabled)
        {
            return;
        }

        // Validate linked objects before proceeding
        LinkableComp?.ValidateLinkedObjects();

        // Play the trap spring sound
        SoundDefOf.TrapSpring.PlayOneShot(new TargetInfo(Position, Map));

        if (p?.Map == null)
        {
            return;
        }

        // Trigger the CustomBlastEMPEffect at the trap's position
        triggerEffect(Position);

        // Ensure there are linked objects
        if (LinkableComp != null && LinkableComp.LinkedThings.Any())
        {
            // Find the first valid linked object that is not this trap
            var target =
                LinkableComp.LinkedThings.FirstOrDefault(t =>
                    t != this && t is { Destroyed: false, Position.IsValid: true });
            if (target != null && target.Position.Walkable(p.Map))
            {
                // Trigger the CustomBlastEMPEffect at the linked building's position
                triggerEffect(target.Position);

                // Safely teleport the pawn
                p.Position = target.Position;
                p.Notify_Teleported();
                Messages.Message("TeTr.BeenTeleported".Translate(p.LabelShort, target.LabelShort), new LookTargets(p),
                    MessageTypeDefOf.PositiveEvent, false);
                return; // Exit the method after successful teleportation
            }
        }

        // Notify that teleportation failed due to no linked object
        Messages.Message("TeTr.TeleportFailed".Translate(p.LabelShort), MessageTypeDefOf.RejectInput, false);
    }


    /// <summary>
    ///     Triggers the CustomBlastEMPEffect at the given position.
    /// </summary>
    /// <param name="position">The position where the effect should play.</param>
    private void triggerEffect(IntVec3 position)
    {
        if (Map == null || position == IntVec3.Invalid)
        {
            return;
        }

        // Create an effecter using the defined EffecterDef
        var effecter = EffecterDefOf.CustomBlastEMPEffect.Spawn();
        effecter.Trigger(new TargetInfo(position, Map), null);
        effecter.Cleanup();
    }


    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }

        // Add the toggle command for enabling/disabling the trap
        yield return new Command_Toggle
        {
            defaultLabel = "TeTr.TeleportationMode".Translate(),
            defaultDesc = "TeTr.TeleportationModeTT".Translate(),
            icon = TexCommand.ForbidOff, // Use an appropriate icon
            isActive = () => isTrapEnabled,
            toggleAction = () => isTrapEnabled = !isTrapEnabled
        };
    }

    protected override void Tick()
    {
        base.Tick();

        // Validate linked objects to ensure integrity
        LinkableComp?.ValidateLinkedObjects();
    }
}
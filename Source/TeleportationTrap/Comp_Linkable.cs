using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace TeleportationTrap;

public class Comp_Linkable : ThingComp
{
    private List<Thing> linkedThings = [];
    public IReadOnlyList<Thing> LinkedThings => linkedThings.AsReadOnly();

    private CompProperties_Linkable Props => (CompProperties_Linkable)props;

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Collections.Look(ref linkedThings, "linkedThings", LookMode.Reference);
    }

    public void ValidateLinkedObjects()
    {
        for (var i = linkedThings.Count - 1; i >= 0; i--)
        {
            var linkedThing = linkedThings[i];
            if (linkedThing == null || linkedThing.Destroyed || linkedThing.Map != parent.Map)
            {
                // Unlink if the linked object is invalid
                unlinkFrom(linkedThing);
            }
        }
    }


    public void DrawLinks()
    {
        foreach (var linkedThing in linkedThings)
        {
            if (linkedThing.Position.IsValid && parent.Map == linkedThing.Map)
            {
                // Draw a green line between linked objects
                GenDraw.DrawLineBetween(parent.DrawPos, linkedThing.DrawPos, SimpleColor.Green);
            }
        }
    }


    private void linkTo(Thing target)
    {
        if (target == null || linkedThings.Contains(target) || !isLinkValid(target))
        {
            return;
        }

        // Unlink from all currently linked objects
        foreach (var linkedThing in linkedThings.ToList())
        {
            unlinkFrom(linkedThing);
        }

        // Establish the new link
        linkedThings.Add(target);
        if (!Props.biDirectional)
        {
            return;
        }

        var targetComp = target.TryGetComp<Comp_Linkable>();
        targetComp?.linkTo(parent);
    }


    private void unlinkFrom(Thing target)
    {
        if (!linkedThings.Contains(target))
        {
            return;
        }

        linkedThings.Remove(target);
        if (!Props.biDirectional)
        {
            return;
        }

        var targetComp = target.TryGetComp<Comp_Linkable>();
        targetComp?.unlinkFrom(parent);
    }

    private bool isLinkValid(Thing target)
    {
        // Check distance
        if (parent.Position.DistanceTo(target.Position) > Props.maxLinkDistance)
        {
            return false;
        }

        // Check if the target's ThingDef is allowed
        return Props.allowedLinkableDefs is not { Count: > 0 } ||
               Props.allowedLinkableDefs.Contains(target.def.defName);
    }

    public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
    {
        base.PostDeSpawn(map, mode);

        if (parent is MinifiedThing)
        {
            // Clear links as the object is now a minified state
            linkedThings.Clear();
        }
    }

    private void drawUnlinkedOverlays()
    {
        // Ensure the parent map is valid
        if (parent?.Map == null)
        {
            return;
        }

        // Check if the selected object is a TeleporterTrapSpike
        if (Find.Selector.SelectedObjects.OfType<Building>()
            .All(selected => selected.def != ThingDefOf.TeleporterTrapSpike))
        {
            return;
        }

        // Get all TeleporterTrapSpike buildings on the map
        var allLinkableThings = parent.Map.listerThings.AllThings
            .Where(t => t.def == ThingDefOf.TeleporterTrapSpike && t.TryGetComp<Comp_Linkable>() != null);

        // Iterate through all linkable things and draw overlay for unlinked ones
        foreach (var thing in allLinkableThings)
        {
            var comp = thing.TryGetComp<Comp_Linkable>();
            if (comp != null && !comp.LinkedThings.Any()) // Only unlinked buildings
            {
                // Draw the static question mark overlay
                parent.Map.overlayDrawer.DrawOverlay(thing, OverlayTypes.QuestionMark);
            }
        }
    }


    public override void PostDraw()
    {
        base.PostDraw();
        drawUnlinkedOverlays();
    }


    public override string CompInspectStringExtra()
    {
        if (!Props.showLinkedObjects)
        {
            return null;
        }

        // Prevent inspection of a MinifiedThing

        if (parent is MinifiedThing)
        {
            return null;
        }

        // Basic linked object info
        var linkedInfo = linkedThings.Count > 0
            ? "TeTr.LinkedTo".Translate(string.Join(", ", linkedThings.Select(t => t.LabelShortCap)))
            : "TeTr.LinkedToNone".Translate();

        if (linkedThings.Count == 0 && Props.drawOutOfFuelOverlay)
        {
            // Show overlay and warning
            parent.Map.overlayDrawer.DrawOverlay(parent, OverlayTypes.QuestionMark);
        }

        // Distance info for the first linked object
        var distanceInfo = "";
        Thing firstLinkedThing;
        if (Props.showLinkDistance && linkedThings.Count > 0)
        {
            firstLinkedThing = linkedThings[0];
            if (firstLinkedThing.Position.IsValid)
            {
                distanceInfo =
                    "TeTr.Distance".Translate(firstLinkedThing.Position.DistanceTo(parent.Position).ToString("F1"));
            }
        }

        // Room items info for the first linked object
        var roomInfo = "";
        if (!Props.showRoomItemsInfo || linkedThings.Count <= 0)
        {
            return linkedInfo + distanceInfo + roomInfo;
        }

        firstLinkedThing = linkedThings[0];
        var room = firstLinkedThing.Position.GetRoom(parent.Map);
        if (room == null)
        {
            return linkedInfo + distanceInfo + roomInfo;
        }

        var roomThings = room.ContainedAndAdjacentThings;
        if (roomThings is { Count: > 0 })
        {
            roomInfo = "TeTr.ItemsInRoom".Translate(string.Join(", ", roomThings.Take(5).Select(t => t.LabelShortCap)) +
                                                    (roomThings.Count > 5
                                                        ? ", ..."
                                                        : "")); // Limit to 5 items for readability
        }


        return linkedInfo + distanceInfo + roomInfo;
    }


    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        foreach (var gizmo in base.CompGetGizmosExtra())
        {
            yield return gizmo;
        }

        if (parent.Faction != Faction.OfPlayer) // Ensure only player-owned objects show the Gizmo
        {
            yield break;
        }

        // Link Gizmo
        yield return new Command_Target
        {
            defaultLabel = "TeTr.LinkTrap".Translate(),
            defaultDesc = "TeTr.LinkTrapTT".Translate(),
            icon = ContentFinder<Texture2D>.Get("UI/Commands/LinkLinkables"),
            action = target =>
            {
                if (target.Thing?.TryGetComp<Comp_Linkable>() == null)
                {
                    return;
                }

                var comp = target.Thing.TryGetComp<Comp_Linkable>();
                if (comp != null && comp.isLinkValid(parent))
                {
                    linkTo(target.Thing);
                    Messages.Message("TeTr.LinkedOK".Translate(), MessageTypeDefOf.PositiveEvent, false);
                }
                else
                {
                    Messages.Message("TeTr.LinkedNotOK".Translate(), MessageTypeDefOf.RejectInput, false);
                }
            },
            targetingParams = new TargetingParameters
            {
                canTargetBuildings = true,
                canTargetItems = false,
                canTargetPawns = false,
                validator = target =>
                {
                    if (target.Thing == null)
                    {
                        return false;
                    }

                    var comp = target.Thing.TryGetComp<Comp_Linkable>();
                    return comp != null && comp.isLinkValid(parent);
                }
            }
        };

        // Unlink Gizmo
        yield return new Command_Action
        {
            defaultLabel = "TeTr.UnLinkTrap".Translate(),
            defaultDesc = "TeTr.UnLinkTrapTT".Translate(),
            icon = ContentFinder<Texture2D>.Get("UI/Commands/HoldFire"),
            action = () =>
            {
                if (linkedThings is { Count: > 0 })
                {
                    foreach (var linkedThing in linkedThings.ToList())
                    {
                        unlinkFrom(linkedThing);
                    }

                    Messages.Message("TeTr.UnLinkedOK".Translate(), MessageTypeDefOf.PositiveEvent, false);
                }
                else
                {
                    Messages.Message("TeTr.UnLinkedNotOK".Translate(), MessageTypeDefOf.RejectInput, false);
                }
            }
        };


        // Teleport Gizmo
        if (Props.enableTeleport)
        {
            yield return new Command_Target
            {
                defaultLabel = "TeTr.Teleport".Translate(),
                defaultDesc = "TeTr.TeleportTT".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/MergeCaravans"),
                action = target =>
                {
                    if (target.Thing is not Pawn targetPawn)
                    {
                        Messages.Message("TeTr.SelectPawn".Translate(), MessageTypeDefOf.RejectInput, false);
                        return;
                    }

                    // Ensure the target is a player-owned colonist or animal
                    if (Props.onlyColonists &&
                        (targetPawn.Faction != Faction.OfPlayer ||
                         !targetPawn.RaceProps.Humanlike && !targetPawn.RaceProps.Animal))
                    {
                        Messages.Message("TeTr.TeleportOnlyPlayer".Translate(),
                            MessageTypeDefOf.RejectInput, false);
                        return;
                    }

                    if (linkedThings.Count <= 0)
                    {
                        Messages.Message("TeTr.NotLinked".Translate(), MessageTypeDefOf.RejectInput, false);
                        return;
                    }

                    // Get the current linked object
                    var currentLinked = linkedThings.FirstOrDefault();
                    if (currentLinked != null)
                    {
                        var job = JobMaker.MakeJob(JobDefOf.TeleportPawn, currentLinked);
                        targetPawn.jobs.TryTakeOrderedJob(job);
                        Messages.Message(
                            "TeTr.IsTeleporting".Translate(targetPawn.LabelShort, currentLinked.LabelShort),
                            MessageTypeDefOf.PositiveEvent, false);
                        return;
                    }

                    Messages.Message("TeTr.NoLinkFound".Translate(), MessageTypeDefOf.RejectInput,
                        false);
                },
                targetingParams = new TargetingParameters
                {
                    canTargetPawns = true,
                    validator = target =>
                    {
                        if (target.Thing is Pawn targetPawn)
                        {
                            return Props.onlyColonists &&
                                   targetPawn.Faction == Faction.OfPlayer &&
                                   (targetPawn.RaceProps.Humanlike || targetPawn.RaceProps.Animal);
                        }

                        return false;
                    }
                }
            };
        }
    }
}
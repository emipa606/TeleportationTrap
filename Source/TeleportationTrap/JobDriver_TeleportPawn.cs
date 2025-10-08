using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace TeleportationTrap;

public class JobDriver_TeleportPawn : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        var initialTarget = job.GetTarget(TargetIndex.A).Thing;
        if (initialTarget == null)
        {
            Messages.Message("TeTr.TeleportFailedInfo".Translate(), MessageTypeDefOf.RejectInput, false);
            yield break;
        }

        // Go to the target location
        yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.Touch);

        // Perform teleportation to the other linked object
        var teleport = new Toil
        {
            initAction = () =>
            {
                if (initialTarget.TryGetComp<Comp_Linkable>() is not { } linkableComp ||
                    linkableComp.LinkedThings.Count <= 0)
                {
                    Messages.Message("TeTr.TeleportFailedComp".Translate(),
                        MessageTypeDefOf.RejectInput, false);
                    return;
                }

                var linkedThing = linkableComp.LinkedThings.FirstOrDefault(t => t != initialTarget);
                if (linkedThing != null)
                {
                    pawn.Position = linkedThing.Position;
                    pawn.Notify_Teleported();
                    Messages.Message("TeTr.BeenTeleported".Translate(pawn.LabelShort, linkedThing.LabelShort),
                        new LookTargets(pawn),
                        MessageTypeDefOf.PositiveEvent, false);
                }
                else
                {
                    Messages.Message("TeTr.TeleportFailedInfo".Translate(),
                        MessageTypeDefOf.RejectInput, false);
                }
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };

        yield return teleport;
    }
}
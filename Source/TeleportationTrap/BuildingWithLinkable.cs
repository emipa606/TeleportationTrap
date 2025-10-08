using Verse;

namespace TeleportationTrap;

public class BuildingWithLinkable : Building
{
    public override void DrawExtraSelectionOverlays()
    {
        base.DrawExtraSelectionOverlays();

        // Access the Comp_Linkable and invoke DrawLinks
        var linkableComp = this.TryGetComp<Comp_Linkable>();
        linkableComp?.DrawLinks();
    }
}
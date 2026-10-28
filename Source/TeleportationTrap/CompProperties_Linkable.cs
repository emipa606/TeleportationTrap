using System.Collections.Generic;
using Verse;

namespace TeleportationTrap;

public class CompProperties_Linkable : CompProperties
{
    public readonly bool biDirectional = true;
    public readonly bool drawOutOfFuelOverlay = true;

    // Enable teleport toggle
    public readonly bool enableTeleport = false;
    public readonly float maxLinkDistance = 25.0f;
    public readonly bool onlyColonists = false;

    // Visibility toggles
    public readonly bool showLinkDistance = true;
    public readonly bool showLinkedObjects = true;
    public readonly bool showRoomItemsInfo = false;
    public bool allowMultipleLinks = true;
    public List<string> allowedLinkableDefs;

    public CompProperties_Linkable()
    {
        compClass = typeof(Comp_Linkable);
    }
}
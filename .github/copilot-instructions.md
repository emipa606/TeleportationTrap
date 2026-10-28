# GitHub Copilot Instructions for RimWorld Mod: Teleportation Trap (Continued)

## Overview and Purpose
The **Teleportation Trap (Continued)** mod is an update to the SilkCircuits mod, adding a new security device to the RimWorld game. This device allows players to teleport enemies to or from a linked teleportation trap, making it an effective tool for both combat and raid management. The trap requires chemfuel for power and psychite tea to refine and amplify psychic energy.

## Key Features and Systems
- **Teleportation Functionality**: Players can link two teleportation traps to teleport enemies between them. When triggered, the trap locks onto an intruder’s signature and violently ejects them to the designated location.
- **Customization Options**: Players can toggle the teleportation mode on and off and customize the glow color of the traps.
- **Compatibility**: The mod is compatible with RimWorld 1.5+ and does not conflict with any other known mods.

## Coding Patterns and Conventions
- **Building_TrapTeleporter**: This class handles the core functionality of the teleportation trap, including its visual representation, data exposure, gizmos, linking, setup, and trigger effects.
- **BuildingWithLinkable**: This abstract class is used for any building that can be linked to another building, providing common functionality for linkable components.
- **Comp_Linkable**: This component handles the linking and unlinking of buildings, including drawing links, validating linked objects, and exposing data.
- **ThingDefOf**: This class defines custom ThingDefs used by the mod, such as the teleportation trap itself.
- **CompProperties_Linkable**: This class defines properties for the linkable component, allowing for customization in the mod's XML files.

## XML Integration
The mod integrates with RimWorld's XML system to define various aspects of the teleportation trap:

- **EffecterDef**: Defines the visual and audio effects of the teleportation trap.
- **fleckDef**: Defines the visual effects, such as particles, associated with the teleportation process.
- **JobDef**: Defines the jobs that the mod adds, such as the action of teleporting.
- **minifiedDef**: Defines the minified versions of the teleportation trap.
- **soundDef**: Defines the sound effects associated with the teleportation trap.
- **ThingDef**: Defines the teleportation trap itself, including its appearance, cost, and other properties.

## Harmony Patching
The mod uses Harmony patching to integrate its functionality with the existing RimWorld codebase:

- **TriggerEffect**: The trigger effect of the teleportation trap is patched to implement the teleportation logic.
- **DrawExtraSelectionOverlays**: This method is patched to draw additional selection overlays, such as links, for linkable buildings.
- **ExposeData**: This method is patched to expose data for serialization and deserialization.
- **GetGizmos**: This method is patched to add custom gizmos, such as buttons for linking and unlinking traps.

## Suggestions for Copilot
To make the most of GitHub Copilot when working on the **Teleportation Trap (Continued)** mod:

- **Code Completion**: Use Copilot to quickly generate code for new features, such as new gizmos or additional properties.
- **Debugging**: Use Copilot to help identify and fix potential issues in existing code, particularly in complex methods like the trigger effect.
- **XML Snippets**: Use Copilot to generate XML snippets for new Defs, ensuring consistency with existing definitions.
- **Documentation**: Use Copilot to help generate documentation for new classes and methods, ensuring that the codebase remains easy to understand and maintain.

By following these guidelines, you can leverage GitHub Copilot to enhance your development workflow and ensure that the **Teleportation Trap (Continued)** mod remains a valuable tool for RimWorld players.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.


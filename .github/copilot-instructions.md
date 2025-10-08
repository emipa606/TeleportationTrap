# Copilot Instructions for RimWorld Modding: Teleportation Trap (Continued)

## Mod Overview and Purpose
**Teleportation Trap (Continued)** is an update of the original mod by SilkCircuits, aiming to add a strategic layer to base defense in RimWorld. This mod introduces a novel security device capable of teleporting enemies to or from a linked trap. The main goal of this mod is to enhance gameplay by providing players with a powerful tool to manage raids and enemy movement strategically.

## Key Features and Systems
- **Teleportation Mechanism**: Devices teleport enemies to a designated linked trap, consuming both traps in the process.
- **Resource Utilization**: Traps require steel, chemfuel, and psychite tea to be built and operated, integrating into the game's resource management mechanics.
- **Linking Interface**: Easily manage trap links and settings through a custom interface menu, with visual indicators for linked (green line) and unlinked (yellow question mark) traps.
- **Operational Modes**: 
  - Enabled + Linked: Teleports and consumes both traps.
  - Disabled + Linked: Acts as a receiver only and consumes both traps.
  - Unlinked: Functions as a single-use, non-teleporting spike trap.
- **Advanced Customization**: Supports glow color customization (requires Advanced Lights research) for aesthetic personalization.
- **Safety Parameters**: Only targets enemies; colonists and animals are unaffected for balanced gameplay.

## Coding Patterns and Conventions
- **Class Organization**: Use descriptive class names like `Building_TrapTeleporter` and `Comp_Linkable` to clarify functionality. Each class should encapsulate related methods and properties.
- **Method Naming**: Names like `ValidateLinkedObjects`, `DrawLinks`, and `LinkTo` clearly express their purpose, ensuring maintainability and readability.
- **Encapsulation**: Keep methods such as `TriggerEffect` private if they are meant for internal use within a class, promoting encapsulation and preventing unintended access.

## XML Integration
The mod likely uses XML for defining various game elements such as trap costs, research requirements, and graphics. Ensure XML files define:
- **Building Configurations**: Include status indicators, research prerequisites, and cost properties.
- **Defining Trap Interactions**: Use XML to manage trap interactions and operational behaviors.

## Harmony Patching
- **Purpose**: Use Harmony to alter or extend base game methods where necessary. This approach ensures compatibility with updates and other mods.
- **Best Practices**: 
  - Target specific methods that require modification or extension without redundantly patching unchanged or unrelated methods.
  - Ensure Harmony patches are isolated by using unique IDs and descriptive naming for patches to clarify patch intentions and maintain traceability.

## Suggestions for Copilot
- **Keep Code Contextualized**: Given the defined classes and methods, assist in generating consistent method implementations that follow established naming conventions and coding styles.
- **Suggesting Enhancements**: Propose improvements or optimizations in linking logic, such as caching linked states to reduce computation costs or enhance visual feedback.
- **Error Handling**: Recommend incorporating robust error handling, especially in linking mechanics, to prevent null reference issues or invalid operations.
- **Documenting**: Assist developers by proposing comments and summaries for complex methods and classes, enhancing codebase understandability for future contributors.

By adhering to these guidelines and utilizing Copilot for intelligent code suggestions, developers can efficiently extend and maintain the functionality of the Teleportation Trap mod while ensuring a seamless user experience in RimWorld.

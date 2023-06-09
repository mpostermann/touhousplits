# For the user manual, please see the [wiki](https://github.com/mpostermann/touhousplits/wiki)

# Touhou Splits

Touhou Splits is a score-tracking program for Touhou and other PC shmups. The main benefit of this is to enable streamers to track and display
their current score as compared to their PB (personal best). This helps the player and viewers know whether the player is on-pace to get a new PB.

# Building

The code is built to target .NET Framework 4

# Code Components

There are two main code components:

1. `TouhouSplits.Service`: Contains the majority of the code for reading scores from a game's running process, and saving/loading splits to file.
2. `TouhouSplits.UI`: A WPF GUI for letting users view their scores and update their PBs.

Each component also has associated test projects: `TouhouSplits.Service.UnitTests`, `TouhouSplits.UI.UnitTests`, and `TouhouSplits.IntegrationTests`.

## Adding support for games

Game support is configured in `Games.xml`. Each supported `Game` should have the following properties:

* `@id`: A unique id for the game.
* `@favoriteslist`: The filename to store data about what splits were most recently loaded.
* `Name`: The display name for the game.
* `Hook/@process`: The name of the Windows process to hook into. Multiple names can be specified by separating each name with a | character.
* `Hook/@strategy`: What strategy to use for reading the game's score from the running process.
Depending on the Hook strategy, additional attributes may be specified as well.

## Hooks

The following general-use hooks are provided out of the box.

### Kernel32StaticHookStrategy

Reads a value directly from a given memory address.

Attributes:
* `@address`: The memory address to read from.
* `@encoding`: How to read the bytes stored at the memory address. Supported values are `int32`, `int64`, and `arrayofnumbers`. See "Encoding Types" below.

### Kernel32PtrHookStrategy

Follows a pointer to find the memory address to read.

Attributes:
* `useThreadStack0`: (Optional) true or false. Defaults to false. Determines whether to resolve the memory address off of THREADSTACK0. If false, the memory address will be resolved based off the running executable.
* `@address`: The memory address of the pointer. When useThreadStack0 is true, this will be a negative address.
* `@offsets`: The pointer offsets. Multiple offsets can be specified by separating each with a | character.
* `@encoding`: How to read the bytes stored at the memory address. Supported values are `int32`, `int64`, and `arrayofnumbers`. See "Encoding Types" below.

### TouhouStaticHookStrategy and TouhouPtrHookStrategy

Same as Kernel32StaticHookStrategy and Kernel32PtrHookStrategy, but multiplies the read value by 10 (because later Touhou games record the last digit of the score separately).

### Encoding Types

Hook strategies each support different encoding types for reading the actual score variable. The following describes the available encoding types:
* `int32`: A signed 4-byte integer
* `int64`: A signed 8-byte integer
* `arrayofnumbers`: Each digit of the score is stored as a 4-byte integer in an array. The `@length` attribute must be specified to indicate how long the array is (ie the max number of digits that can be in the score).
For example, in an array of length 8, the score 110,434 would be stored as `[0, 0, 1, 1, 0, 4, 3, 4]`.


## Adding a new Hook Strategy

Some games games might require a custom hook to account for logic specific to that game. A new hook can be added by implementing `IHookStrategy`,
then updating `HookStrategyFactory.Create()` to return a new instance of that hook when specified.

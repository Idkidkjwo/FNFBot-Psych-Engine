# FNFBot Rewrite

A bot for automatically playing Friday Night Funkin' and its modifications. Significantly improved and reworked version of the original FNFBot.

## Features

- Automatic key pressing with precise timing
- Support for songs from Psych Engine and other modifications
- Support for hold notes
- Note visualization on screen for preview
- Adjustable offset for perfect synchronization
- Cleaning of special note properties incompatible with the engine
- Smooth offset adjustment without interrupting playback

## Controls

- **F1** - Start/stop playback
- **F2** - Increase offset (+1)
- **F3** - Decrease offset (-1)

## Installation

1. Download the latest release from the [Releases](https://github.com/yourusername/FNFBot-Rewrite/releases) section
2. Extract the archive to any folder
3. Run FNFBot20.exe
4. Specify the path to your Friday Night Funkin' game directory
5. Select a song from the list and enjoy!

## Version 2.2 Improvements

- Fixed issue with processing special notes in Psych Engine
- Added support for various song naming formats
- Fixed note overlap issue when displaying
- Optimized thread management for stable operation
- Added smooth offset adjustment without reloading songs
- Improved displayed note cleaning for better performance

## Development

The project is developed in C# using Windows Forms. For proper compilation, you need:

- Visual Studio 2019 or newer
- .NET Framework 4.8
- Newtonsoft.Json
- InputSimulator

## License

MIT License

### What is FNFBot?

FNFBot is a bot program that lets users automatically play Friday Night Funkin' charts.

### How do I use FNFBot?

Video Tutorial:

https://www.youtube.com/watch?v=i7pte79xABg&feature=youtu.be&ab_channel=KadeDev

FNFBot has 3 main sections, as shown here:


![3Sections](https://i.imgur.com/fwlUZPg.png)


The **red** section is where you enter all the data like the game's directory on your computer.


The **green** area is the console, this outputs useful information.

Examples:

- What happened when you pressed a keybind
- What notes the bot's planning on hitting
- When the bot completes a song

The **blue** section is where the bot renders the notes that are *probably* there, including the length of held notes.

### Keybinds
FNFBot currently has 3 keybinds, and in the future it will have modifyable keybinds.

Currently the keybinds are as follows:

| Keybind      | Description |
| ----------- | ----------- |
| F1      | Start/Stop Playing the selected map       |
| F2   | Increase the offset        |
| F3   | Decrease the offset        |

Offset = the amount of time in miliseconds to hit before/after the note time.
Default: 25

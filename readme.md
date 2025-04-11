# FNFBot Rewrite

A bot for automatically playing Friday Night Funkin' and its modifications. Significantly improved and reworked version of the original FNFBot.

## Features

- Automatic key pressing with precise timing (FNFBot, Kade)
- Support for songs from Psych Engine and other modifications (Warning: FNFBot can convert Hurt Notes in normal notes.)
- Support for hold notes (FNFBot, Kade)
- Note visualization on screen for preview (bug, nedded fix.)
- Adjustable offset for perfect synchronization 
- Cleaning of special note properties incompatible with the engine

## Controls

- **F1** - Start/stop playback
- **F2** - Increase offset (+1)
- **F3** - Decrease offset (-1)

## Development

The project is developed in C# using Windows Forms. For proper compilation, you need:

- Visual Studio 2019 or newer
- .NET Framework 4.8
- Newtonsoft.Json
- InputSimulator...

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
Default: 25-30.




**ORIGINAL**: KadeDev.
**VERSION OF THIS BOT**: FNFBot 2.2.

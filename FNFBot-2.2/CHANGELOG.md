# Changelog

## Version 2.2

### Bug Fixes
- Fixed processing of JSON files from Psych Engine with special notes
- Fixed incompatibility issues with note duration data types
- Fixed memory leaks in thread creation
- Fixed note display issues when changing offset
- Fixed thread abort issues when exiting the program

### New Features
- Added smooth offset adjustment without reloading songs
- Added support for saving songs cleaned of special properties
- Added smarter cleaning of displayed notes for better performance
- Added support for various song file naming formats and difficulties

### Improvements
- Optimized thread management for stable operation
- Improved error handling when loading songs
- Improved key release when the bot is shutting down
- Added more informative error messages
- Clearer note display on screen

## Version 2.0

### New Features
- Rewritten from scratch in C# using Windows Forms
- Added support for hold notes
- Added note visualization on screen
- Added offset adjustment using F2/F3 keys
- Added saving offset settings between program launches 
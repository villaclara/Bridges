# Bridges

Bridges is a simple childhood game we used to play. The idea is straightforward: someone writes the numbers from 1 to 20 on a sheet of paper, and two players take turns connecting the numbers with lines, such as 1 to 2, 2 to 3, and so on. The challenge is to avoid crossing lines, because once a line is crossed, a bridge is formed. The game continues until all numbers are connected, and the winner is the player who ends up with the fewest bridges.

Rules:
- Crossing another line → a bridge is created
- Crossing your own bridge → no penalty
- Crossing your opponent’s bridge → +5 bridges
- Passing through a number → no penalty
- The player with the fewest bridges at the end wins

## Development
This game was developed together with my brother [@villaclaraaa](https://github.com/villaclaraaa) using [Unity](https://unity.com/) and released as a WebGL project, making it playable directly in the browser on any device. It features both local and online multiplayer, with networking powered by [Unity Netcode for GameObjects](https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@2.4/manual/index.html) with features such as RPCs and network synchronization. The Multiplayer Lobby system was built with [Unity Relay](https://unity.com/products/relay) enabling smooth matchmaking and connectivity across platforms.

## Features
- A simple, nostalgic paper-like style
- Local two-player mode on a single screen
- Online multiplayer available across any device
- Option to instantly rematch after a multiplayer game ends
- Fair gameplay ensured by dynamically syncing the screen size with the host player in multiplayer sessions

The game is currently under development. More info will be added soon...

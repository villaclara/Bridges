# Bridges

Bridges is a simple childhood game we used to play. The idea is straightforward: someone writes the numbers from 1 to 15 on a sheet of paper, and two players take turns connecting the numbers with lines, such as 1 to 2, 2 to 3, and so on. The challenge is to avoid crossing lines, because once a line is crossed, a bridge is formed. The game continues until all numbers are connected, and the winner is the player who ends up with the fewest bridges.

Rules:
- Crossing another line → a bridge is created
- Crossing your own bridge → no penalty
- Crossing your opponent’s bridge → +5 bridges
- Passing through a number → no penalty
- The player with the fewest bridges at the end wins

## Development
This game was developed together with my brother [@villaclaraaa](https://github.com/villaclaraaa) using [Unity](https://unity.com/) and released as a WebGL project, making it playable directly in the browser on any device and also comes as Windows standalone build. It features both local and online multiplayer, with networking powered by [Unity Netcode for GameObjects](https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@2.4/manual/index.html) with features such as RPCs and network synchronization. The Multiplayer Lobby system was built with [Unity Relay](https://unity.com/products/relay) enabling smooth matchmaking and connectivity across platforms. 

## Features
- Nostalgic, paper-like art style
- Local two-player mode on a single screen
- Online multiplayer available across any device
- Instant rematch option after a multiplayer game ends
- Fair gameplay ensured by dynamically syncing screen size with the host in multiplayer sessions

The game is available:
- directly play at [Github Pages](https://villaclara.github.io/bridges). 
- directly plat at [itch.io](https://villaclara.itch.io/bridges),
- download windows zip archive at [itch.io](https://villaclara.itch.io/bridges).

See you in the game and check some game screenshots:

<img width="300" height="600" alt="photo_2025-09-10_01-40-23-portrait" src="https://github.com/user-attachments/assets/0f1dbfc7-aa76-4964-8a2b-831c94c1543e" />
<img width="300" height="600" alt="photo_2025-09-10_01-29-01-portrait" src="https://github.com/user-attachments/assets/39113ac1-0630-472d-8657-a8c049192691" />
<img width="300" height="600" alt="photo_2025-09-10_01-29-00-portrait" src="https://github.com/user-attachments/assets/b8a76732-504e-4886-9825-92a7b5724a25" />
<img width="1598" height="928" alt="Screenshot 2025-09-10 013248" src="https://github.com/user-attachments/assets/9bbad053-4d5d-4d05-9f35-7faeb262b480" />

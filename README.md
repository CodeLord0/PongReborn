# PongReborn

A modern, visually stunning remaster of the classic Pong game, built with **MonoGame** in C#. Experience the timeless arcade action with electric music and contemporary graphics that bring this legendary game back to life.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Core Features](#core-features)
- [Screenshots](#screenshots)
- [Project Structure](#project-structure)
- [Technologies](#technologies)
- [Installation](#installation)
- [Usage](#usage)
- [Gameplay Guide](#gameplay-guide)
- [Credits](#credits)
- [Contributing](#contributing)
- [License](#license)

---

## 🎮 Overview

**PongReborn** is a faithful yet modern reimagining of the classic Pong arcade game. Originally created in the 1970s, Pong revolutionized gaming and remains a beloved classic. This project combines the simplicity and addictiveness of the original with enhanced visuals, dynamic audio, and smooth gameplay powered by the MonoGame framework.

Whether you're a nostalgic gamer looking to relive the arcade experience or a developer interested in game development fundamentals, PongReborn offers an accessible and engaging experience.

### Key Characteristics
- **C# Development**: Built entirely in C# using the MonoGame framework for cross-platform compatibility
- **Retro-Modern Aesthetic**: Combines classic Pong mechanics with updated visuals and audio
- **Lightweight**: Minimal dependencies with a focused scope on core gameplay
- **Educational**: Excellent reference for game development beginners

---

## ✨ Core Features

- **Classic Pong Gameplay**: Two-paddle, one-ball arcade action with familiar mechanics
- **Dynamic Visuals**: Modern sprite-based graphics with smooth animations
- **Immersive Audio**: Electric background music (Da Phonk by Daft Punk) for an enhanced gaming atmosphere
- **Responsive Controls**: Smooth keyboard input for intuitive paddle control
- **Score Tracking**: Real-time scoring system to track performance across matches
- **Professional Typography**: Custom Woodzy font for a polished user interface
- **High-Quality Asset Integration**: Carefully curated sprite assets optimized for gameplay

---

## 📸 Screenshots

### Gameplay

<img width="807" height="546" alt="PongReborn Gameplay" src="https://github.com/user-attachments/assets/16837fcc-e189-4bdf-889e-d27cd18f0126" />

---

## 📁 Project Structure

```
PongReborn/
├── PongReborn/                 # Main game project (MonoGame executable)
│   ├── Content/               # Game assets (sprites, audio, fonts)
│   │   ├── Textures/         # Image files for game objects
│   │   ├── Audio/            # Music and sound effects
│   │   └── Fonts/            # Custom game fonts
│   ├── Game1.cs              # Main game class and game loop
│   └── [Other game classes]  # Game logic, entity management, collision detection
│
├── PongReborn.Server/         # Server-side networking (optional multiplayer support)
│   └── [Server implementation]
│
├── PongReborn.Shared/         # Shared code between client and server
│   └── [Common data models and utilities]
│
├── .vscode/                   # VS Code settings and debugging configuration
├── .gitignore                 # Git ignore rules for C# projects
└── README.md                  # This file

```

### Directory Details

- **PongReborn**: The main executable game application where all core gameplay logic resides
- **PongReborn.Server**: Contains server-side code for potential networked multiplayer features
- **PongReborn.Shared**: Shared utilities and data structures used across client and server
- **Content**: All game assets including textures, audio, and fonts

---

## 🛠️ Technologies

| Technology | Purpose |
|------------|---------|
| **MonoGame** | Cross-platform game development framework |
| **C#** | Primary programming language |
| **.NET Framework** | Runtime environment |
| **Visual Studio / VS Code** | Development environment |

---

## 💾 Installation

### Prerequisites

Before you begin, ensure you have the following installed:

- **[.NET SDK](https://dotnet.microsoft.com/download)** (version 6.0 or higher recommended)
- **[MonoGame](https://www.monogame.net/)** - Download and install from the official website
- **Visual Studio 2022** or **Visual Studio Code** with C# extension
- **Git** for cloning the repository

### Setup Steps

1. **Clone the Repository**
   ```bash
   git clone https://github.com/CodeLord0/PongReborn.git
   cd PongReborn
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

3. **Open in IDE**
   - **Visual Studio**: Open `PongReborn.sln`
   - **VS Code**: Open the folder with `code .`

4. **Build the Project**
   ```bash
   dotnet build
   ```

5. **Run the Game**
   ```bash
   cd PongReborn
   dotnet run
   ```

---

## 🎯 Usage

### Running the Game

After installation, simply execute:

```bash
dotnet run
```

The game window will launch with the Pong game ready to play.

### Build Configurations

- **Debug**: For development with detailed logging
  ```bash
  dotnet build -c Debug
  dotnet run
  ```

- **Release**: Optimized build for performance
  ```bash
  dotnet build -c Release
  dotnet run --configuration Release
  ```

---

## 🕹️ Gameplay Guide

### Controls

| Input | Action |
|-------|--------|
| **Up Arrow** or **W** | Move left paddle up |
| **Down Arrow** or **S** | Move left paddle down |
| **I** or **↑** | Move right paddle up |
| **K** or **↓** | Move right paddle down |
| **ESC** | Pause/Exit |

### Objective

- **Win the Match**: Get the ball past your opponent's paddle to score points
- **First to Score**: The first player/AI to reach the target score wins
- **Ball Physics**: The ball bounces off paddles and walls with realistic physics
- **Difficulty**: The AI opponent will challenge you with intelligent paddle movement

### Game Flow

1. Game starts with the ball at center court
2. Players control paddles to hit the ball back and forth
3. If the ball passes a paddle, the opponent scores a point
4. First player to reach the winning score wins the match
5. Games can be replayed from the main menu

---

## 🎨 Credits

This project stands on the shoulders of talented creators and classic game design:

- **Sprites**: [Simple Ping Pong Assets](https://myebstudios.itch.io/simple-ping-pong-assets) - High-quality, retro-inspired sprite pack
- **Music**: *Da Phonk* by Daft Punk - A track that captures the electric energy of arcade gaming
- **Font**: **Woodzy** - A custom typography choice that complements the modern-retro aesthetic
- **Framework**: [MonoGame](https://www.monogame.net/) - Enabling cross-platform game development in C#

---

## 🤝 Contributing

Contributions are welcome! Whether you're fixing bugs, adding features, or improving documentation, your help is valued.

### How to Contribute

1. **Fork** the repository
2. **Create a feature branch**
   ```bash
   git checkout -b feature/YourFeatureName
   ```
3. **Commit your changes**
   ```bash
   git commit -m "Add description of your changes"
   ```
4. **Push to your branch**
   ```bash
   git push origin feature/YourFeatureName
   ```
5. **Open a Pull Request** with a clear description of the changes

### Suggestions for Contributions

- Enhanced AI difficulty levels
- Sound effects and audio improvements
- Additional game modes (survival, endless, etc.)
- Network multiplayer support
- Mobile platform support
- Performance optimizations
- Documentation improvements

---

## 📝 License

This project is open source. Please see the LICENSE file in the repository for details on usage rights and restrictions.

---

## 🚀 Future Enhancements

Potential features for future releases:

- [ ] Local multiplayer support (two players on keyboard)
- [ ] Online multiplayer over network
- [ ] Multiple difficulty levels with adjustable AI
- [ ] Power-ups and special effects
- [ ] Leaderboard system
- [ ] Mobile platform ports (iOS/Android via MonoGame)
- [ ] Customizable themes and skins
- [ ] Advanced paddle effects and ball trails

---

## 📧 Support & Feedback

For bugs, feature requests, or general feedback, please open an issue on the [GitHub Issues](https://github.com/CodeLord0/PongReborn/issues) page.

---

**Enjoy the game and thanks for playing PongReborn!** 🎮✨

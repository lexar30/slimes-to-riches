# Slimes to Riches

A small 2D incremental game made with Unity and C#.

Kill slimes, earn gold, build automated defenses, upgrade them, and eventually become rich enough to buy a kingdom.

Target playtime: ~15 minutes.

## Tested Configuration

The project was run and tested with:

- Windows 11 Pro 25H2 (64-bit)
- Unity 6.4, Editor 6000.4.8f1
- Universal Render Pipeline (URP) 17.4.0

Other configurations have not been tested.

## Gameplay

The game takes place in a square arena populated by wandering slimes.

- Clicking/tapping the arena deals area damage around the cursor.
- Slimes drop gold when killed.
- Gold is spent on automated defenses and upgrades.
- The slime population increases dynamically with the player's kill rate, keeping the arena slightly more populated than the current damage output can comfortably handle.
- The final goal is to afford the **Buy Kingdom** purchase, visible from the beginning of the game.

## Slimes

There are three slime rarities:

- **Red — Common**
- **Green — Uncommon**
- **Blue — Rare**

Higher-rarity slimes have more health, give more gold, and spawn less frequently.

Slimes can also spawn in different sizes. Size modifies their health and reward without changing their rarity.

Slimes periodically stop, choose a random point inside the arena, move there, and repeat.

## Defenses

### Archers

Two independently purchasable groups:

- **Vertical Archers** — up to 10 levels
- **Horizontal Archers** — up to 10 levels

Each level adds one archer to both opposite sides of the arena.

Archers fire physical projectiles across the arena. A projectile damages the first slime it collides with.

Shared archer upgrades:

- Damage — 5 levels
- Attack Speed — 5 levels

Archer upgrades become available after purchasing the first archer level.

### Cannons

Up to 5 cannons can be purchased.

Each cannon is represented by a targeting marker that randomly moves around the arena and periodically deals area damage at its current position.

Upgrades:

- Damage — 5 levels
- Attack Speed — 5 levels

Cannon upgrades become available after purchasing the first cannon.

## Manual Attack

Available from the beginning.

Upgrades:

- Damage — 5 levels
- Attack Radius — 5 levels

## Progression

All main systems are visible from the start:

- Vertical Archers
- Horizontal Archers
- Cannons
- Manual Attack upgrades
- Buy Kingdom

Prices and exact balance values will be determined during gameplay tuning.

There is no prestige, offline progression, or save system.

## UI

The gameplay arena always keeps a square aspect ratio.

The surrounding UI adapts to the available screen shape:

- portrait/square — information above the arena, upgrades below;
- landscape — UI panels move to the sides.

## Visual Style

Simple clean 2D graphics with non-pixel-art sprites.

## Development Scope

Planned as a small finished Unity/C# project focused on:

- gameplay systems;
- 2D physics and projectile collisions;
- configurable upgrades;
- dynamic spawning;
- responsive UI;
- short incremental-game progression.

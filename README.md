# Cosmic Slug: Immaculate Constellation (Unity 6.3 LTS)

The truth was never meant to surface.
In 1947 something crashed in the desert. It wasn’t the first — and it won’t be the last.
Cosmic Slug: Immaculate Constellation is a high-octane run-and-gun shooter inspired by classic arcade legends. Blast your way through decades of secret history as you uncover a hidden war between shadow governments and extraterrestrial forces embedded within humanity’s timeline.
From Roswell’s burning wreckage to frozen Antarctic blacksites, from underground bunkers beneath Washington to orbital defense platforms above Earth — the conspiracy is bigger than anyone imagined.
Features:
•	Explosive pixel-art action with tight, responsive arcade gameplay
•	Multiple playable operatives from the clandestine “Constellation Unit”
•	Transforming combat vehicles, experimental reverse-engineered alien weapons
•	Boss fights against biomechanical horrors and classified military monstrosities
•	A campaign spanning 70 years of redacted history
The skies were never empty.
They were occupied.
Lock. Load. Declassify.

Built with **Unity 6000.3.10f1 LTS** (Unity 6.3) using the **Universal Render Pipeline (Universal 2D template)**.

## Requirements
- Unity Hub
- Unity Editor: **6000.3.10f1 LTS**
- Git (recommended via GitHub Desktop)

## Project Setup (First Time)
1. Clone this repository (recommended: **GitHub Desktop**).
2. Open Unity Hub → **Add** → select the project folder.
3. Open the project in Unity **6000.3.10f1**.

## How to Run
1. Open the project in Unity.
2. Open the main scene: `Assets/_Project/Scenes/Main.unity`
3. Press **Play**.

## Repo Structure
- `Assets/_Project/Scenes` – Scenes
- `Assets/_Project/Scripts` – C# scripts
- `Assets/_Project/Prefabs` – Prefabs
- `Assets/_Project/Art` – Sprites and other art
- `Assets/_Project/Audio` – Audio files

## Development Workflow (Codex + GitHub)
We work in small steps:
1. Create a Codex task for a single milestone (movement, camera, shooting, etc.).
2. Codex produces a Pull Request (PR).
3. Review PR on GitHub (check the "Files changed" tab).
4. Merge the PR into `main`.
5. Pull latest changes locally (GitHub Desktop: Fetch → Pull).
6. Open Unity and test (Play Mode; fix any console errors before continuing).

## Milestones (Planned)
- [ ] Player movement + jump (game-feel improvements)
- [ ] Camera follow + bounds
- [ ] Weapons (rifle) + bullets + pooling
- [ ] Damage + invulnerability frames
- [ ] Enemy soldier AI
- [ ] Pickups + score + checkpoints
- [ ] Mini-boss encounter

## Notes
This repo uses a Unity .gitignore to avoid committing generated folders such as `Library/`, `Temp/`, and build outputs.

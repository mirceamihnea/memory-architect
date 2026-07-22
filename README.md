# Memory Architect

A first-person 3D puzzle / escape-room game built in Unity. Players explore a set of memory-themed rooms, examine and collect objects, and solve environmental puzzles — combination safes, locked doors, hidden notes — to progress from room to room.

## Gameplay & Mechanics

- First-person exploration with mouse-look and interaction (examine, pick up, use)
- Inventory system for carrying, holding, and combining items
- Locked doors and key-based progression between rooms
- Safe / combination-lock puzzle
- Readable notes for environmental storytelling
- Multi-room progression: main hall → Room 1 → Room 2 → Room 3
- Account login and progress tracking via a backend API

## Tech Stack

| Layer | Technology |
|---|---|
| Game client | Unity 6000.3.10f1 (Unity 6), C# |
| Backend | Node.js, Express |
| CI/CD | GitLab CI (config included under `frontend/ci/` and `frontend/.gitlab-ci.yml` for reference) |

## Project Structure

```
memory-architect/
├── frontend/   # Unity project — the game itself
├── backend/    # Node/Express API (auth, progress tracking)
└── testing/    # Test suite
```

## Getting Started

### Frontend (Unity)

1. Install Unity Hub and Unity `6000.3.10f1` (or a compatible Unity 6 LTS release).
2. Open `frontend/` as a Unity project.
3. Open `Assets/Scenes/MainMenu.unity` and press Play.

### Backend

```bash
cd backend
npm install
npm start
```

The server listens on `http://localhost:3000`.

## Screenshots

_Coming soon._

## Status

Active development. The `testing/` suite is a work in progress — see [testing/README.md](testing/README.md).

## License

Shared for portfolio purposes. All rights reserved.

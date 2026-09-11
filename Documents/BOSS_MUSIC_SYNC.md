# Boss Music Synchronization

`BossMusicSyncSystem` is the shared clock for boss encounters. It advances at 60 ticks per second and exposes a `BossMusicClockSnapshot` containing:

- Track key, elapsed seconds, and normalized track progress.
- Absolute beat and subdivision indexes plus their fractional progress.
- Loop number, playback state, and synchronization version.

## Authority

In single player, the local game owns the clock. In multiplayer, the server owns it. Clients advance locally between corrections, but accept only snapshots with a newer synchronization version and snap immediately to the received elapsed time. Server snapshots include the track metadata so a joining client can initialize its local clock.

## Track metadata

Register a validated `BossMusicTrack` before starting an encounter. The track key should identify the music asset used by the boss. BPM, duration, beats per bar, and subdivisions per beat are required because tModLoader's exposed music APIs do not currently provide BPM metadata.

`BossMusicMetadataResolver` is the extension point for a supported runtime audio metadata reader. It caches results and returns `Unavailable` or `Invalid` rather than inventing timing values when metadata cannot be read.

## Timeline-driven attacks

Use `BossMusicTimeline<T>` for ordered beat-relative clips. A clip has an ID, start beat, duration in beats, payload, and optional condition. A `BossMusicTimelineController<T>` should be owned by a boss state; call `Sample` for the current active clips and `Update` once per AI update to receive clips entered since the previous snapshot. The controller detects timeline wraparound and does not require an independent attack timer.

`BossMusicTween` provides easing and scalar or custom interpolation. Use `ClipProgress` with a sampled clip to derive movement, spawn, animation, or phase values from the synchronized clock. Gameplay actions that modify the world should remain server-authoritative; clients can sample the same timeline for presentation.

## Lifecycle

Use `TryStart`, `Pause`, `Resume`, `Seek`, and `Stop` on the authoritative side. Register or resolve track metadata before calling `TryStart`. World load and unload reset the clock, registry, resolver cache, and diagnostic state. `Diagnostics` exposes the current timing and metadata state for optional debug UI or logging.

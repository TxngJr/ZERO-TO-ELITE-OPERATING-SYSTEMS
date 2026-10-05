#!/usr/bin/env python3
"""Teaching page-replacement simulator: FIFO, OPT, LRU and CLOCK."""

from __future__ import annotations

from collections import deque


def validate_frames(frames_count: int) -> None:
    if frames_count <= 0:
        raise ValueError("frames_count must be > 0")


def fifo(refs: list[int], frames_count: int) -> int:
    validate_frames(frames_count)
    frames: set[int] = set()
    order: deque[int] = deque()
    faults = 0

    for page in refs:
        if page in frames:
            continue

        faults += 1
        if len(frames) == frames_count:
            victim = order.popleft()
            frames.remove(victim)

        frames.add(page)
        order.append(page)

    return faults


def optimal(refs: list[int], frames_count: int) -> int:
    validate_frames(frames_count)
    frames: set[int] = set()
    faults = 0

    for i, page in enumerate(refs):
        if page in frames:
            continue

        faults += 1
        if len(frames) < frames_count:
            frames.add(page)
            continue

        future = refs[i + 1 :]

        def next_use(p: int) -> int:
            try:
                return future.index(p)
            except ValueError:
                return 10**9

        victim = max(frames, key=next_use)
        frames.remove(victim)
        frames.add(page)

    return faults


def lru(refs: list[int], frames_count: int) -> int:
    validate_frames(frames_count)
    frames: set[int] = set()
    last_used: dict[int, int] = {}
    faults = 0

    for time, page in enumerate(refs):
        if page not in frames:
            faults += 1

            if len(frames) == frames_count:
                victim = min(frames, key=lambda p: last_used[p])
                frames.remove(victim)
                last_used.pop(victim)

            frames.add(page)

        last_used[page] = time

    return faults


def clock(refs: list[int], frames_count: int) -> int:
    validate_frames(frames_count)
    frames: list[int | None] = [None] * frames_count
    referenced = [0] * frames_count
    hand = 0
    faults = 0

    for page in refs:
        if page in frames:
            index = frames.index(page)
            referenced[index] = 1
            continue

        faults += 1

        while frames[hand] is not None and referenced[hand] == 1:
            referenced[hand] = 0
            hand = (hand + 1) % frames_count

        frames[hand] = page
        referenced[hand] = 1
        hand = (hand + 1) % frames_count

    return faults


def main() -> None:
    refs = [7, 0, 1, 2, 0, 3, 0, 4, 2, 3, 0, 3, 2]
    frames = 3

    print("Reference string:", refs)
    print("Frames:", frames)
    print("FIFO :", fifo(refs, frames))
    print("OPT  :", optimal(refs, frames))
    print("LRU  :", lru(refs, frames))
    print("CLOCK:", clock(refs, frames))

    belady = [1, 2, 3, 4, 1, 2, 5, 1, 2, 3, 4, 5]
    f3 = fifo(belady, 3)
    f4 = fifo(belady, 4)

    print("\nBelady demonstration:")
    print("Reference string:", belady)
    print("FIFO with 3 frames:", f3)
    print("FIFO with 4 frames:", f4)

    if f4 > f3:
        print("Belady anomaly observed: more frames produced more FIFO faults.")


if __name__ == "__main__":
    main()

#!/usr/bin/env python3
"""Teaching CPU scheduling simulator.

Assumptions:
- one CPU
- integer time
- one CPU burst per process
- zero context-switch cost
- lower priority number means higher priority
- deterministic tie breakers
"""

from __future__ import annotations

import argparse
import json
from collections import deque
from dataclasses import dataclass, replace
from pathlib import Path
from typing import Iterable


@dataclass
class Process:
    pid: str
    arrival: int
    burst: int
    priority: int = 0
    remaining: int = 0
    first_start: int | None = None
    completion: int | None = None

    def __post_init__(self) -> None:
        if self.arrival < 0:
            raise ValueError(f"{self.pid}: arrival must be >= 0")
        if self.burst <= 0:
            raise ValueError(f"{self.pid}: burst must be > 0")
        if self.remaining == 0:
            self.remaining = self.burst


DEFAULT = [
    Process("P1", 0, 8, 2),
    Process("P2", 1, 4, 1),
    Process("P3", 2, 2, 3),
    Process("P4", 3, 1, 2),
]


def clone_processes(processes: Iterable[Process]) -> list[Process]:
    return [
        replace(
            p,
            remaining=p.burst,
            first_start=None,
            completion=None,
        )
        for p in processes
    ]


def add_segment(timeline: list[tuple[str, int, int]], pid: str, start: int, end: int) -> None:
    if start == end:
        return
    if timeline and timeline[-1][0] == pid and timeline[-1][2] == start:
        old_pid, old_start, _ = timeline[-1]
        timeline[-1] = (old_pid, old_start, end)
    else:
        timeline.append((pid, start, end))


def idle_to(timeline: list[tuple[str, int, int]], current: int, target: int) -> int:
    if target > current:
        add_segment(timeline, "IDLE", current, target)
    return target


def finish_metrics(processes: list[Process]) -> dict[str, dict[str, int]]:
    result: dict[str, dict[str, int]] = {}
    for p in sorted(processes, key=lambda x: x.pid):
        assert p.completion is not None
        assert p.first_start is not None
        tat = p.completion - p.arrival
        wait = tat - p.burst
        response = p.first_start - p.arrival
        result[p.pid] = {
            "AT": p.arrival,
            "BT": p.burst,
            "PR": p.priority,
            "CT": p.completion,
            "TAT": tat,
            "WT": wait,
            "RT": response,
        }
    return result


def nonpreemptive(
    processes: list[Process],
    key_name: str,
) -> tuple[list[Process], list[tuple[str, int, int]]]:
    ps = clone_processes(processes)
    timeline: list[tuple[str, int, int]] = []
    time = 0
    done: set[str] = set()

    while len(done) < len(ps):
        ready = [p for p in ps if p.pid not in done and p.arrival <= time]

        if not ready:
            next_arrival = min(p.arrival for p in ps if p.pid not in done)
            time = idle_to(timeline, time, next_arrival)
            continue

        if key_name == "fcfs":
            p = min(ready, key=lambda x: (x.arrival, x.pid))
        elif key_name == "sjf":
            p = min(ready, key=lambda x: (x.burst, x.arrival, x.pid))
        elif key_name == "priority":
            p = min(ready, key=lambda x: (x.priority, x.arrival, x.pid))
        else:
            raise ValueError(key_name)

        if p.first_start is None:
            p.first_start = time

        start = time
        time += p.remaining
        p.remaining = 0
        p.completion = time
        done.add(p.pid)
        add_segment(timeline, p.pid, start, time)

    return ps, timeline


def preemptive(
    processes: list[Process],
    key_name: str,
) -> tuple[list[Process], list[tuple[str, int, int]]]:
    ps = clone_processes(processes)
    timeline: list[tuple[str, int, int]] = []
    time = 0
    completed = 0

    while completed < len(ps):
        ready = [p for p in ps if p.remaining > 0 and p.arrival <= time]

        if not ready:
            next_arrival = min(p.arrival for p in ps if p.remaining > 0)
            time = idle_to(timeline, time, next_arrival)
            continue

        if key_name == "srtf":
            p = min(ready, key=lambda x: (x.remaining, x.arrival, x.pid))
        elif key_name == "priority-preemptive":
            p = min(ready, key=lambda x: (x.priority, x.arrival, x.pid))
        else:
            raise ValueError(key_name)

        if p.first_start is None:
            p.first_start = time

        add_segment(timeline, p.pid, time, time + 1)
        p.remaining -= 1
        time += 1

        if p.remaining == 0:
            p.completion = time
            completed += 1

    return ps, timeline


def round_robin(
    processes: list[Process],
    quantum: int,
) -> tuple[list[Process], list[tuple[str, int, int]]]:
    if quantum <= 0:
        raise ValueError("quantum must be > 0")

    ps = clone_processes(processes)
    by_arrival = sorted(ps, key=lambda x: (x.arrival, x.pid))
    queue: deque[Process] = deque()
    timeline: list[tuple[str, int, int]] = []
    time = 0
    next_index = 0
    completed = 0

    def enqueue_arrivals(up_to: int) -> None:
        nonlocal next_index
        while next_index < len(by_arrival) and by_arrival[next_index].arrival <= up_to:
            queue.append(by_arrival[next_index])
            next_index += 1

    while completed < len(ps):
        enqueue_arrivals(time)

        if not queue:
            assert next_index < len(by_arrival)
            new_time = by_arrival[next_index].arrival
            time = idle_to(timeline, time, new_time)
            enqueue_arrivals(time)

        p = queue.popleft()

        if p.first_start is None:
            p.first_start = time

        run_for = min(quantum, p.remaining)
        start = time
        time += run_for
        p.remaining -= run_for
        add_segment(timeline, p.pid, start, time)

        # Processes that arrived during this quantum enter before the
        # current process is requeued.
        enqueue_arrivals(time)

        if p.remaining == 0:
            p.completion = time
            completed += 1
        else:
            queue.append(p)

    return ps, timeline


def mlfq(
    processes: list[Process],
    quantums: tuple[int, ...] = (2, 4, 8),
    boost_interval: int = 20,
) -> tuple[list[Process], list[tuple[str, int, int]]]:
    """Simplified teaching MLFQ.

    Rules:
    - new jobs enter Q0
    - always run highest non-empty queue
    - quantum exhaustion demotes one level
    - higher-queue arrivals preempt lower-queue work
    - periodic boost moves all runnable jobs to Q0
    """

    if not quantums or any(q <= 0 for q in quantums):
        raise ValueError("MLFQ quantums must be positive")
    if boost_interval <= 0:
        raise ValueError("boost_interval must be > 0")

    ps = clone_processes(processes)
    by_arrival = sorted(ps, key=lambda x: (x.arrival, x.pid))
    queues = [deque() for _ in quantums]
    level = {p.pid: 0 for p in ps}
    slice_used = {p.pid: 0 for p in ps}

    timeline: list[tuple[str, int, int]] = []
    time = 0
    next_index = 0
    completed = 0
    current: Process | None = None
    current_level = 0
    last_boost = -1

    def enqueue_arrivals() -> None:
        nonlocal next_index
        while next_index < len(by_arrival) and by_arrival[next_index].arrival <= time:
            p = by_arrival[next_index]
            level[p.pid] = 0
            slice_used[p.pid] = 0
            queues[0].append(p)
            next_index += 1

    def highest_ready_level() -> int | None:
        for i, q in enumerate(queues):
            if q:
                return i
        return None

    while completed < len(ps):
        enqueue_arrivals()

        if (
            time > 0
            and time % boost_interval == 0
            and time != last_boost
        ):
            last_boost = time
            boosted: list[Process] = []

            if current is not None:
                boosted.append(current)
                current = None

            for q in queues:
                while q:
                    boosted.append(q.popleft())

            for p in boosted:
                if p.remaining > 0:
                    level[p.pid] = 0
                    slice_used[p.pid] = 0
                    queues[0].append(p)

        if current is None:
            ready_level = highest_ready_level()
            if ready_level is None:
                if next_index >= len(by_arrival):
                    break
                new_time = by_arrival[next_index].arrival
                time = idle_to(timeline, time, new_time)
                continue

            current_level = ready_level
            current = queues[current_level].popleft()

        # A higher-priority queue can preempt current work.
        ready_level = highest_ready_level()
        if ready_level is not None and ready_level < current_level:
            queues[current_level].append(current)
            current = None
            continue

        assert current is not None

        if current.first_start is None:
            current.first_start = time

        add_segment(timeline, current.pid, time, time + 1)
        current.remaining -= 1
        slice_used[current.pid] += 1
        time += 1

        if current.remaining == 0:
            current.completion = time
            completed += 1
            current = None
            continue

        if slice_used[current.pid] >= quantums[current_level]:
            new_level = min(current_level + 1, len(quantums) - 1)
            level[current.pid] = new_level
            slice_used[current.pid] = 0
            queues[new_level].append(current)
            current = None

    return ps, timeline


def load_processes(path: str | None) -> list[Process]:
    if path is None:
        return clone_processes(DEFAULT)

    raw = json.loads(Path(path).read_text(encoding="utf-8"))
    if not isinstance(raw, list):
        raise ValueError("input JSON must be a list")

    result = []
    for item in raw:
        result.append(
            Process(
                pid=str(item["pid"]),
                arrival=int(item["arrival"]),
                burst=int(item["burst"]),
                priority=int(item.get("priority", 0)),
            )
        )
    return result


def print_timeline(timeline: list[tuple[str, int, int]]) -> None:
    print("\nTimeline:")
    print(" | ".join(f"{start}-{end}:{pid}" for pid, start, end in timeline))


def print_metrics(processes: list[Process]) -> None:
    metrics = finish_metrics(processes)
    headers = ("PID", "AT", "BT", "PR", "CT", "TAT", "WT", "RT")
    print("\n" + " ".join(f"{h:>6}" for h in headers))

    for pid, m in metrics.items():
        print(
            f"{pid:>6}"
            f"{m['AT']:>6}"
            f"{m['BT']:>6}"
            f"{m['PR']:>6}"
            f"{m['CT']:>6}"
            f"{m['TAT']:>6}"
            f"{m['WT']:>6}"
            f"{m['RT']:>6}"
        )

    count = len(metrics)
    avg_tat = sum(m["TAT"] for m in metrics.values()) / count
    avg_wt = sum(m["WT"] for m in metrics.values()) / count
    avg_rt = sum(m["RT"] for m in metrics.values()) / count

    print(f"\nAverage TAT={avg_tat:.2f} WT={avg_wt:.2f} RT={avg_rt:.2f}")


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "--algo",
        required=True,
        choices=[
            "fcfs",
            "sjf",
            "srtf",
            "priority",
            "priority-preemptive",
            "rr",
            "mlfq",
        ],
    )
    parser.add_argument("--input")
    parser.add_argument("--quantum", type=int, default=2)
    parser.add_argument("--boost", type=int, default=20)
    args = parser.parse_args()

    processes = load_processes(args.input)

    if args.algo in {"fcfs", "sjf", "priority"}:
        result, timeline = nonpreemptive(processes, args.algo)
    elif args.algo in {"srtf", "priority-preemptive"}:
        result, timeline = preemptive(processes, args.algo)
    elif args.algo == "rr":
        result, timeline = round_robin(processes, args.quantum)
    else:
        result, timeline = mlfq(processes, boost_interval=args.boost)

    print(f"Algorithm: {args.algo}")
    print_timeline(timeline)
    print_metrics(result)


if __name__ == "__main__":
    main()

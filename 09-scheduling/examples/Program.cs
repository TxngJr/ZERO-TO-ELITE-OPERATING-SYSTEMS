record Job(string Id, int Arrival, int Burst, int Priority);

class State
{
    public Job Job { get; }
    public int Remaining { get; set; }
    public int FirstStart { get; set; } = -1;
    public int Completion { get; set; } = -1;
    public int QueueLevel { get; set; }
    public int UsedInQuantum { get; set; }

    public State(Job job)
    {
        Job = job;
        Remaining = job.Burst;
    }
}

record Segment(string Id, int Start, int End);

class Program
{
    static readonly Job[] Workload =
    {
        new("P1", 0, 8, 2),
        new("P2", 1, 4, 1),
        new("P3", 2, 2, 3),
        new("P4", 3, 1, 2)
    };

    static int Main(string[] args)
    {
        string algo = args.Length == 0 ? "fcfs" : args[0].ToLowerInvariant();
        int quantum = args.Length > 1 && int.TryParse(args[1], out int q) ? q : 2;

        if (algo == "self-test")
        {
            SelfTest();
            return 0;
        }

        (List<State> states, List<Segment> timeline) result = algo switch
        {
            "fcfs" => NonPreemptive("fcfs"),
            "sjf" => NonPreemptive("sjf"),
            "priority" => NonPreemptive("priority"),
            "srtf" => Preemptive("srtf"),
            "priority-preemptive" => Preemptive("priority"),
            "rr" => RoundRobin(quantum),
            "mlfq" => Mlfq(),
            _ => throw new ArgumentException($"Unknown algorithm: {algo}")
        };

        Print(algo, result.states, result.timeline);
        return 0;
    }

    static List<State> Fresh() => Workload.Select(j => new State(j)).ToList();

    static void AddSegment(List<Segment> timeline, string id, int start, int end)
    {
        if (start == end) return;

        if (timeline.Count > 0)
        {
            Segment last = timeline[^1];
            if (last.Id == id && last.End == start)
            {
                timeline[^1] = last with { End = end };
                return;
            }
        }

        timeline.Add(new Segment(id, start, end));
    }

    static (List<State>, List<Segment>) NonPreemptive(string mode)
    {
        List<State> states = Fresh();
        List<Segment> timeline = new();
        HashSet<string> done = new();
        int time = 0;

        while (done.Count < states.Count)
        {
            List<State> ready = states
                .Where(s => !done.Contains(s.Job.Id) && s.Job.Arrival <= time)
                .ToList();

            if (ready.Count == 0)
            {
                int next = states.Where(s => !done.Contains(s.Job.Id)).Min(s => s.Job.Arrival);
                AddSegment(timeline, "IDLE", time, next);
                time = next;
                continue;
            }

            State selected = mode switch
            {
                "sjf" => ready.OrderBy(s => s.Job.Burst)
                              .ThenBy(s => s.Job.Arrival)
                              .ThenBy(s => s.Job.Id)
                              .First(),
                "priority" => ready.OrderBy(s => s.Job.Priority)
                                   .ThenBy(s => s.Job.Arrival)
                                   .ThenBy(s => s.Job.Id)
                                   .First(),
                _ => ready.OrderBy(s => s.Job.Arrival)
                          .ThenBy(s => s.Job.Id)
                          .First()
            };

            selected.FirstStart = time;
            int start = time;
            time += selected.Remaining;
            selected.Remaining = 0;
            selected.Completion = time;
            done.Add(selected.Job.Id);
            AddSegment(timeline, selected.Job.Id, start, time);
        }

        return (states, timeline);
    }

    static (List<State>, List<Segment>) Preemptive(string mode)
    {
        List<State> states = Fresh();
        List<Segment> timeline = new();
        int time = 0;
        int completed = 0;

        while (completed < states.Count)
        {
            List<State> ready = states
                .Where(s => s.Remaining > 0 && s.Job.Arrival <= time)
                .ToList();

            if (ready.Count == 0)
            {
                int next = states.Where(s => s.Remaining > 0).Min(s => s.Job.Arrival);
                AddSegment(timeline, "IDLE", time, next);
                time = next;
                continue;
            }

            State selected = mode == "srtf"
                ? ready.OrderBy(s => s.Remaining)
                       .ThenBy(s => s.Job.Arrival)
                       .ThenBy(s => s.Job.Id)
                       .First()
                : ready.OrderBy(s => s.Job.Priority)
                       .ThenBy(s => s.Job.Arrival)
                       .ThenBy(s => s.Job.Id)
                       .First();

            if (selected.FirstStart < 0) selected.FirstStart = time;

            AddSegment(timeline, selected.Job.Id, time, time + 1);
            selected.Remaining--;
            time++;

            if (selected.Remaining == 0)
            {
                selected.Completion = time;
                completed++;
            }
        }

        return (states, timeline);
    }

    static (List<State>, List<Segment>) RoundRobin(int quantum)
    {
        if (quantum <= 0) throw new ArgumentOutOfRangeException(nameof(quantum));

        List<State> states = Fresh();
        List<State> arrivals = states.OrderBy(s => s.Job.Arrival).ThenBy(s => s.Job.Id).ToList();
        Queue<State> queue = new();
        List<Segment> timeline = new();

        int time = 0;
        int next = 0;
        int completed = 0;

        void EnqueueArrivals()
        {
            while (next < arrivals.Count && arrivals[next].Job.Arrival <= time)
                queue.Enqueue(arrivals[next++]);
        }

        while (completed < states.Count)
        {
            EnqueueArrivals();

            if (queue.Count == 0)
            {
                if (next >= arrivals.Count)
                    throw new InvalidOperationException("No runnable or future process.");

                int target = arrivals[next].Job.Arrival;
                AddSegment(timeline, "IDLE", time, target);
                time = target;
                EnqueueArrivals();
            }

            State current = queue.Dequeue();
            if (current.FirstStart < 0) current.FirstStart = time;

            int run = Math.Min(quantum, current.Remaining);
            int start = time;
            time += run;
            current.Remaining -= run;
            AddSegment(timeline, current.Job.Id, start, time);

            // Processes arriving during the quantum enter before the current
            // process is put back at the tail.
            EnqueueArrivals();

            if (current.Remaining == 0)
            {
                current.Completion = time;
                completed++;
            }
            else
            {
                queue.Enqueue(current);
            }
        }

        return (states, timeline);
    }

    static (List<State>, List<Segment>) Mlfq()
    {
        // Explicit teaching policy:
        // Q0 quantum=2, Q1 quantum=4, Q2 quantum=8.
        // New jobs enter Q0.
        // A task keeps the CPU until it finishes, exhausts its queue quantum,
        // or a task appears in a strictly higher-priority queue.
        // Quantum exhaustion demotes one level.
        // Every 20 time units, runnable tasks are boosted to Q0.
        int[] quantums = { 2, 4, 8 };
        const int boostInterval = 20;

        List<State> states = Fresh();
        List<State> arrivals = states.OrderBy(s => s.Job.Arrival).ThenBy(s => s.Job.Id).ToList();
        Queue<State>[] queues = { new(), new(), new() };
        List<Segment> timeline = new();

        int time = 0;
        int next = 0;
        int completed = 0;
        int lastBoost = -1;
        State? current = null;
        int currentLevel = -1;

        void EnqueueArrivals()
        {
            while (next < arrivals.Count && arrivals[next].Job.Arrival <= time)
            {
                State arriving = arrivals[next++];
                arriving.QueueLevel = 0;
                arriving.UsedInQuantum = 0;
                queues[0].Enqueue(arriving);
            }
        }

        int HighestReadyLevel()
        {
            for (int i = 0; i < queues.Length; i++)
                if (queues[i].Count > 0)
                    return i;

            return -1;
        }

        void BoostAllRunnable()
        {
            List<State> runnable = new();

            if (current is not null)
            {
                runnable.Add(current);
                current = null;
                currentLevel = -1;
            }

            foreach (Queue<State> queue in queues)
                while (queue.Count > 0)
                    runnable.Add(queue.Dequeue());

            foreach (State state in runnable)
            {
                if (state.Remaining <= 0) continue;
                state.QueueLevel = 0;
                state.UsedInQuantum = 0;
                queues[0].Enqueue(state);
            }
        }

        while (completed < states.Count)
        {
            EnqueueArrivals();

            if (time > 0 && time % boostInterval == 0 && lastBoost != time)
            {
                lastBoost = time;
                BoostAllRunnable();
            }

            int highest = HighestReadyLevel();

            // Only a strictly higher-priority queue preempts the current task.
            if (current is not null && highest >= 0 && highest < currentLevel)
            {
                queues[currentLevel].Enqueue(current);
                current = null;
                currentLevel = -1;
            }

            if (current is null)
            {
                highest = HighestReadyLevel();

                if (highest < 0)
                {
                    if (next >= arrivals.Count)
                        throw new InvalidOperationException("No runnable or future process.");

                    int target = arrivals[next].Job.Arrival;
                    AddSegment(timeline, "IDLE", time, target);
                    time = target;
                    continue;
                }

                currentLevel = highest;
                current = queues[currentLevel].Dequeue();
            }

            if (current.FirstStart < 0)
                current.FirstStart = time;

            AddSegment(timeline, current.Job.Id, time, time + 1);
            current.Remaining--;
            current.UsedInQuantum++;
            time++;

            if (current.Remaining == 0)
            {
                current.Completion = time;
                current.UsedInQuantum = 0;
                completed++;
                current = null;
                currentLevel = -1;
                continue;
            }

            if (current.UsedInQuantum >= quantums[currentLevel])
            {
                int newLevel = Math.Min(currentLevel + 1, queues.Length - 1);
                current.QueueLevel = newLevel;
                current.UsedInQuantum = 0;
                queues[newLevel].Enqueue(current);
                current = null;
                currentLevel = -1;
            }
        }

        return (states, timeline);
    }

    static void SelfTest()
    {
        var fcfs = NonPreemptive("fcfs");
        Dictionary<string, int> expectedCt = new()
        {
            ["P1"] = 8,
            ["P2"] = 12,
            ["P3"] = 14,
            ["P4"] = 15
        };

        foreach (State state in fcfs.Item1)
            if (state.Completion != expectedCt[state.Job.Id])
                throw new Exception($"FCFS completion mismatch for {state.Job.Id}");

        var rr = RoundRobin(2);
        State p2 = rr.Item1.Single(s => s.Job.Id == "P2");
        if (p2.FirstStart != 2)
            throw new Exception("RR queue ordering is incorrect.");

        var mlfq = Mlfq();
        Segment first = mlfq.Item2.First(s => s.Id != "IDLE");

        if (first.Id != "P1" || first.Start != 0 || first.End != 2)
            throw new Exception(
                "MLFQ quantum is incorrect: P1 should keep Q0 CPU from t=0 to t=2."
            );

        foreach (State state in mlfq.Item1)
        {
            int tat = state.Completion - state.Job.Arrival;
            int wt = tat - state.Job.Burst;
            int rt = state.FirstStart - state.Job.Arrival;

            if (tat < 0 || wt < 0 || rt < 0)
                throw new Exception("Scheduling metric invariant failed.");
        }

        Console.WriteLine("Chapter09 self-test PASS");
    }

    static void Print(string algo, List<State> states, List<Segment> timeline)
    {
        Console.WriteLine($"Algorithm: {algo}");
        Console.WriteLine("Timeline:");
        Console.WriteLine(string.Join(" | ", timeline.Select(s => $"{s.Start}-{s.End}:{s.Id}")));

        Console.WriteLine();
        Console.WriteLine("PID  AT  BT  PR  CT  TAT  WT  RT");

        foreach (State s in states.OrderBy(s => s.Job.Id))
        {
            int tat = s.Completion - s.Job.Arrival;
            int wt = tat - s.Job.Burst;
            int rt = s.FirstStart - s.Job.Arrival;

            Console.WriteLine(
                $"{s.Job.Id,-4} {s.Job.Arrival,2}  {s.Job.Burst,2}  {s.Job.Priority,2}  " +
                $"{s.Completion,2}  {tat,3}  {wt,2}  {rt,2}"
            );
        }

        double avgWt = states.Average(s => s.Completion - s.Job.Arrival - s.Job.Burst);
        double avgTat = states.Average(s => s.Completion - s.Job.Arrival);
        double avgRt = states.Average(s => s.FirstStart - s.Job.Arrival);

        Console.WriteLine($"Average WT={avgWt:F2} TAT={avgTat:F2} RT={avgRt:F2}");
    }
}

record Job(string Id, int Arrival, int Burst, int Priority);

class State
{
    public Job Job { get; }
    public int Remaining { get; set; }
    public int FirstStart { get; set; } = -1;
    public int Completion { get; set; } = -1;

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

    static void Main(string[] args)
    {
        string algo = args.Length == 0 ? "fcfs" : args[0].ToLowerInvariant();
        int quantum = args.Length > 1 && int.TryParse(args[1], out int q) ? q : 2;

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
                int next = states
                    .Where(s => !done.Contains(s.Job.Id))
                    .Min(s => s.Job.Arrival);
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
                ? ready.OrderBy(s => s.Remaining).ThenBy(s => s.Job.Arrival).ThenBy(s => s.Job.Id).First()
                : ready.OrderBy(s => s.Job.Priority).ThenBy(s => s.Job.Arrival).ThenBy(s => s.Job.Id).First();

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
        int[] quantum = { 2, 4, 8 };
        List<State> states = Fresh();
        List<State> arrivals = states.OrderBy(s => s.Job.Arrival).ThenBy(s => s.Job.Id).ToList();
        Queue<State>[] queues = { new(), new(), new() };
        Dictionary<string, int> level = states.ToDictionary(s => s.Job.Id, _ => 0);
        Dictionary<string, int> used = states.ToDictionary(s => s.Job.Id, _ => 0);
        List<Segment> timeline = new();

        int time = 0;
        int next = 0;
        int completed = 0;

        while (completed < states.Count)
        {
            while (next < arrivals.Count && arrivals[next].Job.Arrival <= time)
            {
                State arriving = arrivals[next++];
                level[arriving.Job.Id] = 0;
                queues[0].Enqueue(arriving);
            }

            int qLevel = Array.FindIndex(queues, q => q.Count > 0);

            if (qLevel < 0)
            {
                int target = arrivals[next].Job.Arrival;
                AddSegment(timeline, "IDLE", time, target);
                time = target;
                continue;
            }

            State current = queues[qLevel].Dequeue();
            if (current.FirstStart < 0) current.FirstStart = time;

            AddSegment(timeline, current.Job.Id, time, time + 1);
            current.Remaining--;
            used[current.Job.Id]++;
            time++;

            while (next < arrivals.Count && arrivals[next].Job.Arrival <= time)
            {
                State arriving = arrivals[next++];
                level[arriving.Job.Id] = 0;
                queues[0].Enqueue(arriving);
            }

            if (current.Remaining == 0)
            {
                current.Completion = time;
                completed++;
                continue;
            }

            if (used[current.Job.Id] >= quantum[qLevel])
            {
                int newLevel = Math.Min(qLevel + 1, queues.Length - 1);
                level[current.Job.Id] = newLevel;
                used[current.Job.Id] = 0;
                queues[newLevel].Enqueue(current);
            }
            else
            {
                queues[qLevel].Enqueue(current);
            }
        }

        return (states, timeline);
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
            Console.WriteLine($"{s.Job.Id,-4} {s.Job.Arrival,2}  {s.Job.Burst,2}  {s.Job.Priority,2}  {s.Completion,2}  {tat,3}  {wt,2}  {rt,2}");
        }

        double avgWt = states.Average(s => s.Completion - s.Job.Arrival - s.Job.Burst);
        double avgTat = states.Average(s => s.Completion - s.Job.Arrival);
        double avgRt = states.Average(s => s.FirstStart - s.Job.Arrival);

        Console.WriteLine($"Average WT={avgWt:F2} TAT={avgTat:F2} RT={avgRt:F2}");
    }
}

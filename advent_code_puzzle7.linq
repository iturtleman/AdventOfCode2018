<Query Kind="Program">
  <NuGetReference>Elders.PowerCollections</NuGetReference>
  <NuGetReference>Microsoft.Tpl.Dataflow</NuGetReference>
  <Namespace>System.Threading.Tasks.Dataflow</Namespace>
  <Namespace>Wintellect.PowerCollections</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
	var testInput = @"Step C must be finished before step A can begin.
Step C must be finished before step F can begin.
Step A must be finished before step B can begin.
Step A must be finished before step D can begin.
Step B must be finished before step E can begin.
Step D must be finished before step E can begin.
Step F must be finished before step E can begin.";

	var test = ChainEvents(testInput.Split('\n').Select(GetLetterPairFromInputLine));
	var test_reversed = ChainEvents(testInput.Split('\n').Select(GetLetterPairFromInputLineReversed));
	//CalculateOrderString(test).Dump();
	CalculateLengthOfTimeToBuild(test, test_reversed, workers:2, secondsToDelay:0).Dump();

	var input = ChainEvents(File.ReadAllLines(@"C:\Users\Ivan Lloyd\Dropbox\Ivan Lloyd\Programming\AdventOfCode\advent_code_puzzle7.txt").Select(GetLetterPairFromInputLine));
	//CalculateOrderString(input).Dump();

	var reversedinputs = ChainEvents(File.ReadAllLines(@"C:\Users\Ivan Lloyd\Dropbox\Ivan Lloyd\Programming\AdventOfCode\advent_code_puzzle7.txt").Select(GetLetterPairFromInputLineReversed));

	CalculateLengthOfTimeToBuild(input, reversedinputs, workers:5).Dump();
}

int CalculateLengthOfTimeToBuild(Dictionary<string, HashSet<string>> actionable, Dictionary<string, HashSet<string>> waiters, int workers, int secondsToDelay = 60)
{
	actionable.Dump();
	waiters.Dump();
	HashSet<String> waitingSteps = waiters.Keys.ToHashSet();

	var processQueue = new Queue<String>();
	//enqueue all steps that do not need to wait
	foreach (var p in actionable
		.Where(kvp => !waiters.ContainsKey(kvp.Key))
		.Select(kvp => kvp.Key)
	)
	{
		processQueue.Enqueue(p);
	}
		
	var runningQueue = new List<Tuple<string, Task<int>>>(2);

	var sw = Stopwatch.StartNew();
	
	while (processQueue.Any())
	{
		while (runningQueue.Count < workers && processQueue.Any())
		{
			var curr = processQueue.Dequeue();
			//re-queue this for later
			if(waitingSteps.Contains(curr)){
				processQueue.Enqueue(curr);
			}
			runningQueue.Add(SpawnTaskForPart(curr,secondsToDelay));
		}
		while (runningQueue.Count >0 && !runningQueue.Any(t=>t.Item2.IsCompleted))
		{
			int completedIndex = Task.WaitAny(runningQueue.Select(q => q.Item2).ToArray());
			var item = runningQueue[completedIndex];
			if (actionable.ContainsKey(item.Item1))
			{
				//clean up waiting list
				foreach (var edge in actionable[item.Item1])
				{
					waiters[edge].Remove(item.Item1);
					if (waiters[edge].Count == 0)
					{
						waiters.Remove(edge);
						waitingSteps.Remove(edge);
					}
				}
				//enqueue those that are free
				foreach (var p in actionable[item.Item1].Where(kvp => !waiters.ContainsKey(kvp)))
				{
					processQueue.Enqueue(p);
				}
			}
			waitingSteps.Remove(item.Item1);
			waiters.Remove(item.Item1);
			runningQueue.RemoveAt(completedIndex);
		}
	}
	sw.Stop();
	return (int)sw.Elapsed.TotalSeconds;
}

Tuple<String, Task<int>> SpawnTaskForPart(string key)
{
	return SpawnTaskForPart(key, 60);	
}
Tuple<String, Task<int>> SpawnTaskForPart(string key, int secondsToDelay)
{
	return Tuple.Create(key, Task.Factory.StartNew(() =>
	{
		int delayTime = secondsToDelay + (key[0]) - 64;
		Thread.Sleep(TimeSpan.FromSeconds(delayTime));
		return delayTime;
	}));
}

String CalculateOrderString(Dictionary<string, HashSet<string>> input)
{
	var orderedString = GetStepOrderFromMap(input).Dump();

	var found = new HashSet<char>();
	var sb = new StringBuilder();
	foreach (var r in orderedString.Reverse())
	{
		if (!found.Contains(r))
			sb.Append(r);
		found.Add(r);
	}
	return String.Join(string.Empty, sb.ToString().Reverse());
}

String GetStepOrderFromMap(Dictionary<string, HashSet<string>> orderedOperations)
{
	var processQueue = new OrderedBag<String>();
	HashSet<String> waitingSteps = orderedOperations.Values.SelectMany(val => val).ToHashSet();
	new { waitingSteps = waitingSteps }.Dump();

	//enqueue all steps that do not need to wait
	processQueue.AddMany(orderedOperations.Where(kvp => !waitingSteps.Contains(kvp.Key)).Select(kvp => kvp.Key));


	var sb = new StringBuilder();

	while (processQueue.Any())
	{
		new { processQueue = processQueue, sb = sb.ToString(), }.Dump();

		var curr = processQueue.RemoveFirst();
		//remove duplicates
		while (processQueue.Any() && processQueue.GetFirst() == curr)
			processQueue.RemoveFirst();
		sb.Append(curr);
		//queue up the rest
		if (orderedOperations.ContainsKey(curr))
			processQueue.AddMany(orderedOperations[curr]);
	}

	return sb.ToString();
}

Dictionary<string, HashSet<string>> ChainEvents(IEnumerable<Tuple<string, string>> orderDef)
{
	var eventMap = new Dictionary<string, HashSet<string>>();
	foreach (var pair in orderDef)
	{
		if (!eventMap.ContainsKey(pair.Item1))
		{
			eventMap[pair.Item1] = new HashSet<string>() { pair.Item2 };
		}
		else
		{
			eventMap[pair.Item1].Add(pair.Item2);
		}
	}
	new { eventMap = eventMap }.Dump();
	return eventMap;
}

Regex r = new Regex(@"Step (\w+) must be finished before step (\w+) can begin.", RegexOptions.Compiled);
Tuple<string, string> GetLetterPairFromInputLine(String line)
{
	var match = r.Match(line);
	return Tuple.Create(match.Groups[1].Value, match.Groups[2].Value);
}
Tuple<string, string> GetLetterPairFromInputLineReversed(String line)
{
	var match = r.Match(line);
	return Tuple.Create(match.Groups[2].Value, match.Groups[1].Value);
}
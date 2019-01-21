<Query Kind="Program" />

void Main()
{
	var testInput = @"Step C must be finished before step A can begin.
Step C must be finished before step F can begin.
Step A must be finished before step B can begin.
Step A must be finished before step D can begin.
Step B must be finished before step E can begin.
Step D must be finished before step E can begin.
Step F must be finished before step E can begin.";

	var test_reversed = ChainEvents(testInput.Split('\n').Select(GetLetterPairFromInputLineReversed));
	CalculateOrderString(test_reversed).Dump();

	var input = ChainEvents(File.ReadAllLines(@"C:\Users\Ivan Lloyd\Dropbox\Ivan Lloyd\Programming\AdventOfCode\advent_code_puzzle7.txt").Select(GetLetterPairFromInputLineReversed));
	CalculateOrderString(input).Dump();
	
}

String CalculateOrderString(Dictionary<string, HashSet<string>> input)
{
	var reversedOut = GetStepOrderFromMap(input).Reverse();

	var found = new HashSet<char>();
	var sb = new StringBuilder();
	foreach (var r in reversedOut)
	{
		if (!found.Contains(r))
			sb.Append(r);
		found.Add(r);
	}
	return sb.ToString();
}

String GetStepOrderFromMap(Dictionary<string, HashSet<string>> orderedOperations)
{
	var processQueue = new Queue<String>();
	HashSet<String> waitingSteps = orderedOperations.Values.SelectMany(val => val).ToHashSet();
	new { waitingSteps = waitingSteps }.Dump();

	//enqueue all steps that do not need to wait
	foreach (var step in orderedOperations.Where(kvp => !waitingSteps.Contains(kvp.Key)).OrderByDescending(kvp => kvp.Key))
	{
		processQueue.Enqueue(step.Key);
	}
	new { processQueue = processQueue }.Dump();

	var sb = new StringBuilder();

	while (processQueue.Any())
	{
		var curr = processQueue.Dequeue();
		sb.Append(curr);
		if (orderedOperations.ContainsKey(curr))

			foreach (var s in orderedOperations[curr].OrderByDescending(val => val))
			{
				processQueue.Enqueue(s);
			}
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
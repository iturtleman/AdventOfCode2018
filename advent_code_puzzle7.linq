<Query Kind="Program">
  <NuGetReference>Elders.PowerCollections</NuGetReference>
  <NuGetReference>Microsoft.Tpl.Dataflow</NuGetReference>
  <Namespace>System.Threading.Tasks.Dataflow</Namespace>
  <Namespace>Wintellect.PowerCollections</Namespace>
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

	var test_reversed = ChainEvents(testInput.Split('\n').Select(GetLetterPairFromInputLine));
	CalculateOrderString(test_reversed).Dump();

	var input = ChainEvents(File.ReadAllLines(@"C:\Users\Ivan Lloyd\Dropbox\Ivan Lloyd\Programming\AdventOfCode\advent_code_puzzle7.txt").Select(GetLetterPairFromInputLine));
	CalculateOrderString(input).Dump();
	
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
	return String.Join(string.Empty,sb.ToString().Reverse());
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
		new { processQueue = processQueue,sb=sb.ToString(), }.Dump();
	
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
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

	var test_reversed = ChainEvents(testInput.Split('\n').Select(CreateEdgeFromLine));
	CalculateOrderString(test_reversed).Dump();

	var input = ChainEvents(File.ReadAllLines(@"C:\Users\Ivan Lloyd\Dropbox\Ivan Lloyd\Programming\AdventOfCode\advent_code_puzzle7.txt").Select(CreateEdgeFromLine));
	CalculateOrderString(input).Dump();
	
}

int CalculateLengthOfTimeToBuild(Dictionary<string, HashSet<string>> actionable, Dictionary<string, HashSet<string>> waiters, int workers, int secondsToDelay = 60){
	return secondsToDelay/workers;
}

String CalculateOrderString(Dictionary<string, Node> input)
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

String GetStepOrderFromMap(Dictionary<string, Node> orderedOperations)
{
	var processQueue = new OrderedSet<String>();
	HashSet<String> waitingSteps = orderedOperations.Values.SelectMany(val => val.OutboundEdges.Select(e=>e.Child.Name)).ToHashSet();
	new { waitingSteps = waitingSteps }.Dump();

	//enqueue all steps that do not need to wait
	processQueue.AddMany(orderedOperations.Where(kvp => !waitingSteps.Contains(kvp.Key)).Select(kvp => kvp.Key));


	var sb = new StringBuilder();

	while (processQueue.Any())
	{
		//new { processQueue = processQueue,sb=sb.ToString(), }.Dump();
	
		var curr = processQueue.RemoveFirst();
		//remove duplicates
		while (processQueue.Any() && processQueue.GetFirst() == curr)
			processQueue.RemoveFirst();
		sb.Append(curr);
		//queue up the rest
		if (orderedOperations.ContainsKey(curr))
			processQueue.AddMany(orderedOperations[curr].OutboundEdges.Select(e=>e.Child.Name));
	}

	return sb.ToString();
}

Dictionary<string, Node> ChainEvents(IEnumerable<Edge> orderDef)
{
	var eventMap = new Dictionary<string, Node>();
	foreach (var edge in orderDef)
	{
		if (!eventMap.ContainsKey(edge.Parent.Name))
		{
			eventMap[edge.Parent.Name] = edge.Parent;
		}

		if (!eventMap.ContainsKey(edge.Child.Name))
		{
			eventMap[edge.Child.Name] = edge.Child;
		}


		eventMap[edge.Parent.Name].OutboundEdges.Add(edge);
		eventMap[edge.Child.Name].InboundEdges.Add(edge);
	}
	new { eventMap = eventMap }.Dump();
	return eventMap;
}

Regex r = new Regex(@"Step (\w+) must be finished before step (\w+) can begin.", RegexOptions.Compiled);
Edge CreateEdgeFromLine(String line)
{
	var match = r.Match(line);
	var edge = new Edge() { Parent = new Node() { Name = match.Groups[1].Value }, Child = new Node() { Name = match.Groups[2].Value}};
	edge.Parent.OutboundEdges.Add(edge);
	edge.Child.InboundEdges.Add(edge);
	return edge;
}

public class Node
{
	public string Name;
	public HashSet<Edge> OutboundEdges = new HashSet<Edge>();
	public HashSet<Edge> InboundEdges = new HashSet<Edge>();
	public override bool Equals(object obj)
	{
		if (obj == null || !(obj is Edge)) return false;
		var e = obj as Node;
		return e.Name.Equals(Name);
	}
	public override int GetHashCode()
	{
		return Name.GetHashCode();
	}
}

public class Edge 
{
	public Node Parent;
	public Node Child;

	public override bool Equals(object obj)
	{
		if(obj==null||!(obj is Edge)) return false;
		var e = obj as Edge;
		return e.Parent == Parent && e.Child == Child;
	}

	public override int GetHashCode()
	{
		return Parent.GetHashCode()^Child.GetHashCode();
	}
}

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

	var testData = ChainEvents(testInput.Split('\n').Select(CreateEdgeFromLine));
	CalculateOrderString(testData).Dump();

	var input = ChainEvents(File.ReadAllLines(@"C:\Users\Ivan Lloyd\Dropbox\Ivan Lloyd\Programming\AdventOfCode\advent_code_puzzle7.txt").Select(CreateEdgeFromLine));
	CalculateOrderString(input).Dump();


	new { CalculateLengthOfTimeToBuild = CalculateLengthOfTimeToBuild(testData, 2, 0) }.Dump();
	new { CalculateLengthOfTimeToBuild = CalculateLengthOfTimeToBuild(input, 5, 60) }.Dump();
}

int CalculateLengthOfTimeToBuild(Dictionary<char, Node> graphNodes, int workers, int secondsToDelay)
{
	var processQueue = new OrderedSet<Node>();
	var completedTasks = new HashSet<char>();
	var tasks = Enumerable.Range(0,workers).Select(a=> new Worker()).ToArray();
	int currentTime = 0;
	//enqueue all steps that do not need to wait
	processQueue.AddMany(graphNodes.Where(kvp => kvp.Value.InboundEdges.Count == 0).Select(kvp => kvp.Value));
	enqueueTasks(processQueue, tasks, currentTime, secondsToDelay);

	var sb = new StringBuilder();

	//do the tasks (either enqueue or run any not complete)
	while (processQueue.Any() || GetCurrentRunningCount(tasks, currentTime-1) > 0)
	{
		//enqueue new tasks
		var availableWorkers = tasks.Where(t => IsTaskComplete(t, currentTime)).ToList();
		foreach (var t in availableWorkers)
		{
			if (t.ProcessNode != null)
			{
				completedTasks.Add(t.ProcessNode.Name);

				var children = t.ProcessNode.OutboundEdges.Select(e => e.Child).ToList();
				var toAdd = children.Where(n =>
					!completedTasks.Contains(n.Name)
					&& n.InboundEdges.All(inEdges => completedTasks.Contains(inEdges.Parent.Name))
					&& !availableWorkers.Any(to=>to.ProcessNode != null && to.ProcessNode.Name == n.Name)).ToList();
				processQueue.AddMany(toAdd);
				t.ProcessNode = null;
				t.CompletionTime = int.MinValue;

			}
		}

		// each ready worker can take a task
		enqueueTasks(processQueue, tasks, currentTime, secondsToDelay);
		//new { processQueue = processQueue, tasks = tasks, completedTasks = completedTasks, currentTime = currentTime }.Dump();
		currentTime++;
	}

	return currentTime-1;
}

void enqueueTasks(OrderedSet<Node> processQueue, Worker[] tasks, int currentTime, int secondsToDelay)
{
	foreach (var t in tasks.Where(t => IsTaskComplete(t, currentTime)))
	{
		if (processQueue.Any())
		{
			var q = processQueue.RemoveFirst();
			t.ProcessNode = q;
			t.CompletionTime = (currentTime + secondsToDelay + q.Delay).Dump();
		}
	}
}

int GetCurrentRunningCount(IEnumerable<Worker> tasks, int currentTime){
	return tasks.Where(t=>t.CompletionTime >= currentTime).Count();
}
bool IsTaskComplete(Worker t, int currentTime){
	return t.CompletionTime <= currentTime;
}

String CalculateOrderString(Dictionary<char, Node> input)
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

String GetStepOrderFromMap(Dictionary<char, Node> graphNodes)
{
	var processQueue = new OrderedSet<Node>();
	HashSet<char> waitingSteps = graphNodes.Values.SelectMany(val => val.OutboundEdges.Select(e=>e.Child.Name)).ToHashSet();
	new { waitingSteps = waitingSteps }.Dump();

	//enqueue all steps that do not need to wait
	processQueue.AddMany(graphNodes.Values.Where(n=>!n.InboundEdges.Any()));


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
		if (graphNodes.ContainsKey(curr.Name))
			processQueue.AddMany(graphNodes[curr.Name].OutboundEdges.Select(e=>e.Child));
	}

	return sb.ToString();
}

Dictionary<char, Node> ChainEvents(IEnumerable<Tuple<char,char>> orderDef)
{
	var eventMap = new Dictionary<char, Node>();
	foreach (var edge in orderDef)
	{
		var e = new Edge() { Parent = getOrCreateNode(eventMap, edge.Item1), Child = getOrCreateNode(eventMap, edge.Item2)};
		
		eventMap[edge.Item1].OutboundEdges.Add(e);
		eventMap[edge.Item2].InboundEdges.Add(e);
	}
	new { eventMap = eventMap }.Dump();
	return eventMap;
}

Node getOrCreateNode(Dictionary<char, Node> eventMap, char nodeName)
{
	Node retval;
	if (!eventMap.TryGetValue(nodeName, out retval))
	{
		retval = eventMap[nodeName] = new Node() { Name = nodeName };
	}
	return retval;
}

Regex r = new Regex(@"Step (\w+) must be finished before step (\w+) can begin.", RegexOptions.Compiled);
Tuple<char,char> CreateEdgeFromLine(String line)
{
	var match = r.Match(line);
	return Tuple.Create(match.Groups[1].Value[0],match.Groups[2].Value[0]);
}

public class Node : IComparable, IEquatable<Node>
{
	public char Name;
	public HashSet<Edge> OutboundEdges = new HashSet<Edge>();
	public HashSet<Edge> InboundEdges = new HashSet<Edge>();
	public override bool Equals(object obj)
	{
		var e = obj as Node;
		if (e == null)
			return false;
		return Name.Equals(e.Name);
	}

	public override int GetHashCode()
	{
		return Name.GetHashCode();
	}

	public int CompareTo(object obj)
	{
		var e=obj as Node;
		if (Name == e.Name)
			return 0;
		return Name.CompareTo(e.Name);
	}

	public int Delay { get { return Name - 'A' + 1; } }

	public override string ToString()
	{
		return Name.ToString();
	}

	public bool Equals(Node other)
	{
		if (other == null)
			return false;
		return Name.Equals(other?.Name);
	}
}

public class Edge : IComparable
{
	public Node Parent;
	public Node Child;

	public override bool Equals(object obj)
	{
		var e = obj as Edge;
		if(e==null)
		return false;
		return Parent.Equals(e.Parent) && Child.Equals(e.Child);
	}

	public override int GetHashCode()
	{
		return Parent.GetHashCode()^Child.GetHashCode();
	}
	
	public int CompareTo(object obj)
	{
		if (obj == null || !(obj is Edge)) return -1;
		var e = obj as Edge;
		if (Parent == e.Parent && Child == e.Child)
			return 0;
		return Parent.GetHashCode() < e.Parent.GetHashCode() && Child.GetHashCode() < e.Child.GetHashCode()? -1: 1;
	}
}

public class Worker
{
	public Node ProcessNode;
	public int CompletionTime = int.MinValue;
}
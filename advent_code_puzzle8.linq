<Query Kind="Program">
  <NuGetReference>Elders.PowerCollections</NuGetReference>
  <NuGetReference>Microsoft.Tpl.Dataflow</NuGetReference>
  <Namespace>System.Threading.Tasks.Dataflow</Namespace>
  <Namespace>Wintellect.PowerCollections</Namespace>
</Query>

void Main()
{
	var input = ConvertToNumbers("2 3 0 3 10 11 12 1 1 0 1 99 2 1 1 2");
	parseNode(input).Dump();
	
	parseNode(ConvertToNumbers(File.ReadAllText(@"C:\Users\Ivan Lloyd\Dropbox\Ivan Lloyd\Programming\AdventOfCode\advent_code_puzzle8.txt"))).Dump();
}

List<int> ConvertToNumbers(string v)
{
	return v.Split(
		new char[] { ' ' },
		StringSplitOptions.RemoveEmptyEntries
	)
	.Select(Int32.Parse).ToList();
}

// Define other methods and classes here
public Node parseNode (IEnumerable<int> input){
	int numNodes = input.First();
	int numMetadata = input.Skip(1).First();
	var retval=new Node(){
		Name=(char)((int)'A'+nodesSoFar++),
	};
	// skip the first two characters which give metadata info
	int lengthAlongString=2;
	while(numNodes > 0)
	{
		var toAdd = parseNode(input.Skip(lengthAlongString));
		retval.Children.Add(toAdd);
		
		lengthAlongString+=toAdd.TotalItemsConsumed;
		numNodes--;
	}
	retval.Metadata.AddRange(input.Skip(lengthAlongString).Take(numMetadata));
	return retval;
}
public static int nodesSoFar=0;
public class Node
{
	public char Name;
	public int TotalItemsConsumed
	{
		get
		{
			// count the 2 numbers for metadata
			return Metadata.Count + Children.Sum(c => c.TotalItemsConsumed)+2;
		}
	}
	public int SumMetadata
	{
		get
		{
			return Metadata.Sum() + Children.Sum(c => c.SumMetadata);
		}
	}

	public int Value
	{
		get
		{
			return !Children.Any()
				? Metadata.Sum()
				: Metadata.Select(m =>
				{
					var index = m - 1;
					if (index < 0 || index >= Children.Count)
						return 0;
					return Children[index].Value;
				}).Sum();
		}
	}

	public List<int> Metadata = new List<int>();
	public List<Node> Children = new List<Node>();
}
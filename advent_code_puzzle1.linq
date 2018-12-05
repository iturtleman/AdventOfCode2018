<Query Kind="Program" />

void Main()
{
	var frequencyAdjustments = File.ReadAllLines(@"C:\Users\Ivan Lloyd\Dropbox\Ivan Lloyd\Programming\AdventOfCode\advent_code_puzzle1.txt")
	.Select(l => int.Parse(l))
	.ToList();

	//part 1
	frequencyAdjustments.Sum().Dump();

	int currentFrequency = 0;
	Dictionary<int, int> foundOnes = new Dictionary<int, int>();

	for (int i = 0; true; i = ((i + 1) % frequencyAdjustments.Count))
	{
		var val = (currentFrequency += frequencyAdjustments[i]);
		if (foundOnes.ContainsKey(val))
		{
			//part 2
			val.Dump();
			break;
		}
		foundOnes[val] = 1;
	};

}

// Define other methods and classes here
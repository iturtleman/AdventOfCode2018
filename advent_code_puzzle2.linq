<Query Kind="Program" />

void Main()
{
	var boxes = File.ReadAllLines(@"C:\Users\Ivan Lloyd\Dropbox\Ivan Lloyd\Programming\AdventOfCode\advent_code_puzzle2.txt");

	// part 1
	var letterCounts = boxes
		.Select(l => l.GroupBy(c => c)
		.ToDictionary(grp => grp.Key, grp => grp.Count())).ToList()
		.Select(dic =>
		{
			var charsOnBox = dic.Where(kvp => kvp.Value == 2 || kvp.Value == 3);
			return new { Twos = charsOnBox.Any(kvp => kvp.Value == 2), Threes = charsOnBox.Any(kvp => kvp.Value == 3) };
		})
		.Where(answers => answers.Twos || answers.Threes)
		.ToList();

	(letterCounts.Where(a => a.Twos).Count() * letterCounts.Where(a => a.Threes).Count()).Dump();


	// part 2
	boxes.SelectMany(boxSerialTok1cuts).GroupBy(kvp => kvp.Key).Where(grp => grp.Count() > 1).Dump();

}

// Define other methods and classes here
Dictionary<string, string> boxSerialTok1cuts(string serial)
{
	var retval = new Dictionary<string, string>();
	for (int i = 0; i < serial.Length; ++i)
	{
		retval[serial.Remove(i, 1)] = serial;
	}
	return retval;
}
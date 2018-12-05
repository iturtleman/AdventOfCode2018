<Query Kind="Program" />

void Main()
{
	var lines = File.ReadAllLines(@"C:\Users\Ivan Lloyd\Dropbox\Ivan Lloyd\Programming\AdventOfCode\advent_code_puzzle4.txt")
	.OrderBy(l => l)
	.ToList();

	var dict = new Dictionary<int, List<Sleep>>();
	int guardID = 0;
	DateTime shiftStart = DateTime.Now;

	foreach (var l in lines)
	{
		var match = guardRegex.Match(l);
		if (match.Success)
		{
			guardID = int.Parse(match.Groups[1].Value);
			shiftStart = DateTime.Parse(dateTime.Match(l).Groups[1].Value);
		}
		else if (l.Contains("asleep"))
		{
			if (!dict.ContainsKey(guardID))
				dict[guardID] = new List<Sleep>();
			dict[guardID].Add(new Sleep()
			{
				ShiftStart = shiftStart,
				Start = DateTime.Parse(dateTime.Match(l).Groups[1].Value),
			}
			);
		}
		else
		{
			var last = dict[guardID].Last();
			last.End = DateTime.Parse(dateTime.Match(l).Groups[1].Value);
			last.TimeAsleep = last.End - last.Start;
			last.MinutesAsleep = Enumerable.Range(last.Start.Minute, (int)last.TimeAsleep.TotalMinutes).ToList();
		}
	}

	// part 1
	var orderedSleeps = dict.SelectMany(kvp => kvp.Value.Select(v => new { Guard = kvp.Key, Sleep = v }))
	.GroupBy(a => a.Guard)
	.Select(grp => new { guard = grp.Key, TotalTimeSlept = grp.Sum(s => s.Sleep.TimeAsleep.TotalMinutes) })
	.OrderByDescending(a => a.TotalTimeSlept)
	;//.Dump();
	var sleepiestGuard = orderedSleeps.First().guard;
	var sleepiestMinute = dict[sleepiestGuard].SelectMany(sleep => sleep.MinutesAsleep)
	.GroupBy(minute => minute)
	.OrderByDescending(min => min.Count())//.Dump()
	.First().Key;

	new{Guard=sleepiestGuard, sleepiestMinute=sleepiestMinute,Hash=(sleepiestGuard * sleepiestMinute)}.Dump();

	// part 2
	var guardsSleepiestMinute=dict.Select(kvp =>
		new
		{
			Guard = kvp.Key,
			MostSleptMinute = kvp.Value.SelectMany(sleep => sleep.MinutesAsleep)
		.GroupBy(minute => minute)
		.OrderByDescending(min => min.Count())
		}
		).OrderByDescending(grp => grp.MostSleptMinute.First().Count());//.Dump();
		var sleepiestGuardMinute=guardsSleepiestMinute.First().Dump();
	(sleepiestGuardMinute.Guard*sleepiestGuardMinute.MostSleptMinute.First().Key).Dump();
}

// Define other methods and classes here
Regex guardRegex = new Regex(@"Guard #(\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
Regex dateTime = new Regex(@"\[(.*?)\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
class Sleep
{
	public DateTime ShiftStart;
	public DateTime Start;
	public DateTime End;
	public TimeSpan TimeAsleep;
	public List<int> MinutesAsleep;
}
<Query Kind="Program" />

void Main()
{
	var polymer = File.ReadAllText(@"C:\Users\Ivan Lloyd\Dropbox\Ivan Lloyd\Programming\AdventOfCode\advent_code_puzzle5.txt").Trim();


	new { Polymer = polymer, Length = polymer.Length }.Dump();

	//part 1
	var sw = Stopwatch.StartNew();
	depolymerize("dabAcCaCBAcCcaDAxz");
	depolymerize(polymer);
	sw.Stop();
	sw.Dump();

	sw = Stopwatch.StartNew();
	depolymerize2("dabAcCaCBAcCcaDAxz");
	depolymerize2(polymer);
	sw.Stop();
	sw.Dump();

	sw = Stopwatch.StartNew();
	depolymerize3("dabAcCaCBAcCcaDAxz");
	depolymerize3(polymer);
	sw.Stop();
	sw.Dump();

	//part 2
	CHARS.Select(c =>new {
		Polymer = depolymerize2(Regex.Replace(polymer, c.ToString(), string.Empty, RegexOptions.IgnoreCase)),
		Character = c,
		}
	).OrderBy(a=>a.Polymer.Length).Dump()
	.First().Polymer.Length.Dump();
}

// Define other methods and classes here
int intifyChar(char c)
{
	return (int)c;
}
char charifyInt(int c)
{
	return (char)c;
}
bool shouldDelete(int a, int b)
{
	//chars are 32 different A vs a
	return Math.Abs(a - b) == 32;
}
bool shouldDelete(char a, char b)
{
	return a != b && char.ToLower(a) == char.ToLower(b);
}

String depolymerize(String polymer)
{
	int size = 0;
	while (polymer.Length != size)
	{
		size = polymer.Length;
		StringBuilder sb = new StringBuilder();
		int i = 0, last = polymer.Length - 1;
		while (i < last)
		{
			if (!shouldDelete(polymer[i], polymer[i + 1]))
			{
				sb.Append(polymer[i++]);
			}
			else
			{
				i++;
				i++;
			}
		}
		sb.Append(polymer[last]);
		polymer = sb.ToString();
	}
	new { Polymer = polymer, Length = polymer.Length }.Dump();
	return polymer;
}

String depolymerize2(String polymer)
{
	StringBuilder sb = new StringBuilder();
	var polymerShift = polymer.Substring(1);

	int lastIndex = 0;
	for (int i = 0; i < polymerShift.Length; ++i)
	{
		if (Math.Abs(intifyChar(polymer[i]) - intifyChar(polymerShift[i])) == 32)
		{
			sb.Append(polymer.Substring(lastIndex, i - lastIndex));
			lastIndex = i + 2;
			//the next one cannot compress this pass
			++i;
		}
	}
	sb.Append(polymer.Substring(lastIndex));

	if (polymer.Length == sb.Length)
	{
		//new { Polymer = polymer, Length = polymer.Length }.Dump();
		return polymer;
	}

	return depolymerize2(sb.ToString());
}

String CHARS = "abcdefghijklmnopqrstuvwxyz";
String depolymerize3(String polymer)
{
	int lengh = 0;
	//(int)'A'=65
	//(int)'a'=97
	//((int)'A'-(int)'Z')=32
	var oldData = polymer;
	while (true)
	{
		foreach (var c in CHARS)
		{
			polymer = polymer
				.Replace(c + c.ToString().ToUpper(), String.Empty)
				.Replace(c.ToString().ToUpper() + c, String.Empty);
		}

		if (oldData == polymer)
			break;
		oldData = polymer;
	}


	new { Polymer = polymer, Length = polymer.Length }.Dump();
	return polymer;
}
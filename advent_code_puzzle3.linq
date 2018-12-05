<Query Kind="Program" />

void Main()
{
	var claims = File.ReadAllLines(@"C:\Users\Ivan Lloyd\Dropbox\Ivan Lloyd\Programming\AdventOfCode\advent_code_puzzle3.txt")
	.Select(l =>
	{
		var vals = l.Split(new char[] { '#', '@', ' ', ',', 'x', ':', }, StringSplitOptions.RemoveEmptyEntries);
		return new Claim()
		{
			Number = int.Parse(vals[0]),
			Left = int.Parse(vals[1]),
			Top = int.Parse(vals[2]),
			Width = int.Parse(vals[3]),
			Height = int.Parse(vals[4]),
		};
	}).ToList();


	// part 1
	foreach (var claim in claims)
	{
		for (int i = claim.Left, width = claim.Left + claim.Width; i < width; ++i)
		{
			for (int j = claim.Top, height = claim.Top + claim.Height; j < height; ++j)
			{
				cloth[i, j]++;
			}
		}
	}

	int inchesContested = 0;
	foreach (var inch in cloth)
	{
		if (inch > 1)
			inchesContested++;
	}
	inchesContested.Dump();

	// part 2
	foreach (var claim in claims)
	{
		if (isClaimUncontested(claim))
			claim.Dump();
	}

}

// Define other methods and classes here
bool isClaimUncontested(Claim claim)
{
	for (int i = claim.Left, width = claim.Left + claim.Width; i < width; ++i)
	{
		for (int j = claim.Top, height = claim.Top + claim.Height; j < height; ++j)
		{
			if (cloth[i, j] > 1)
			{
				return false;
			}
		}
	}
	return true;
}

int[,] cloth = new int[15000, 15000];

class Claim
{
	public int Number;
	public int Left;
	public int Top;
	public int Width;
	public int Height;
}
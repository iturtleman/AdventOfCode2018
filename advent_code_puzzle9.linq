<Query Kind="Program">
  <NuGetReference>Elders.PowerCollections</NuGetReference>
  <NuGetReference>Microsoft.Tpl.Dataflow</NuGetReference>
  <Namespace>System.Threading.Tasks.Dataflow</Namespace>
  <Namespace>Wintellect.PowerCollections</Namespace>
</Query>

void Main()
{
	GetHighScore(ConvertToNumbers("9 players; last marble is worth 25 points: high score is 32")).Dump();
	
	GetHighScore(ConvertToNumbers("9 players; last marble is worth 46 points: high score is 32")).Dump();
	
	GetHighScore(ConvertToNumbers("10 players; last marble is worth 1618 points: high score is 8317")).Dump();

	GetHighScore(ConvertToNumbers("13 players; last marble is worth 7999 points: high score is 146373")).Dump();

	GetHighScore(ConvertToNumbers("17 players; last marble is worth 1104 points: high score is 2764")).Dump();

	GetHighScore(ConvertToNumbers("21 players; last marble is worth 6111 points: high score is 54718")).Dump();

	GetHighScore(ConvertToNumbers("30 players; last marble is worth 5807 points: high score is 37305")).Dump();

	GetHighScore(ConvertToNumbers(File.ReadAllText(@"C:\Users\Ivan Lloyd\Dropbox\Ivan Lloyd\Programming\AdventOfCode\advent_code_puzzle9.txt"))).Dump();
}

int GetHighScore(List<int> list)
{
	currMarbleValue=0;
	int[] scores = new int[list.First()];
	int lastMarbleWorth = list.Skip(1).First();
	Marble current = makeMarble();
	current.Clockwise = current;
	current.CounterClockWise = current;
	int currentPlayerId=0;

	while (currMarbleValue <= lastMarbleWorth)
	{
		var newMarble = makeMarble();
		if (newMarble.Name % 23 == 0)
		{
			scores[currentPlayerId] += newMarble.Name;
			var sevenToCounterClockwise = current.CounterClockWise.CounterClockWise.CounterClockWise.CounterClockWise.CounterClockWise.CounterClockWise.CounterClockWise;
			scores[currentPlayerId] += sevenToCounterClockwise.Name;
			sevenToCounterClockwise.CounterClockWise.Clockwise=sevenToCounterClockwise.Clockwise;
			sevenToCounterClockwise.Clockwise.CounterClockWise=sevenToCounterClockwise.CounterClockWise;
			current =sevenToCounterClockwise.Clockwise;
		}
		else
		{
			newMarble.Clockwise = current.Clockwise.Clockwise;
			newMarble.CounterClockWise = current.Clockwise;
			newMarble.Clockwise.CounterClockWise = newMarble;
			newMarble.CounterClockWise.Clockwise = newMarble;
			current=newMarble;
		}
		currentPlayerId = (currentPlayerId + 1) % scores.Length;
	}
	return scores.Max();
}

Marble makeMarble()
{
	var m = new Marble()
	{
		Name = currMarbleValue++
	};
	return m;
}
Regex r = new Regex(@"(\d+).*?(\d+)", RegexOptions.Compiled);
List<int> ConvertToNumbers(string v)
{
	var retval = new List<int>(2);

	var m = r.Match(v);
	retval.Add(int.Parse(m.Groups[1].Value));
	retval.Add(int.Parse(m.Groups[2].Value));

	return retval;
}

// Define other methods and classes here

public static int currMarbleValue=0;
public class Marble
{
	public int Name;
	
	public Marble Clockwise;
	public Marble CounterClockWise;
}
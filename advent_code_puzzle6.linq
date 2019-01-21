<Query Kind="Program" />

void Main()
{
	var test_points = GetPointsFromString(@"1, 1
1, 6
8, 3
3, 4
5, 5
8, 9".Split('\n'));
	CalculateLargestArea(DoGraphOperationWithManhatanDistances(test_points, GetNearestPoint).Dump());

	CalculateNumberpPointsUnderThreshold(DoGraphOperationWithManhatanDistances(test_points, GetSumManhatanDistances), 32);

	var danger_points = GetPointsFromString(File.ReadAllLines(@"C:\Users\Ivan Lloyd\Dropbox\Ivan Lloyd\Programming\AdventOfCode\advent_code_puzzle6.txt"));
	CalculateLargestArea(DoGraphOperationWithManhatanDistances(danger_points, GetNearestPoint).Dump());
	
	
	CalculateNumberpPointsUnderThreshold(DoGraphOperationWithManhatanDistances(danger_points, GetSumManhatanDistances),10000);
}

void GetSumManhatanDistances(GraphPoint[,] finalGraph,Point2D currPt,List<GraphPoint> currWeights)
{
	finalGraph[currPt.x, currPt.y].weight = currWeights.Sum(weight=>weight.weight);
}

int CalculateNumberpPointsUnderThreshold(GraphPoint[,] finalGraph, int maxDistanceFromAllPoints)
{
	int totalX = finalGraph.GetLength(0);
	int totalY = finalGraph.GetLength(1);
	
	int pointsUnderThreshold=0;

	for (int i = 0; i < totalX; ++i)
	{
		for (int j = 0; j < totalY; ++j)
		{
			if (finalGraph[i, j].weight < maxDistanceFromAllPoints)
			{
				pointsUnderThreshold++;
			}
		}
	}
	new {pointsUnderThreshold=pointsUnderThreshold}.Dump();
	return pointsUnderThreshold;
}

int CalculateLargestArea(GraphPoint[,] finalGraph)
{
	int totalX = finalGraph.GetLength(0);
	int totalY = finalGraph.GetLength(1);
	//want to ignore any points on the outside edge
	var infiniteFields = new HashSet<Point2D>();
	for (int i = 0; i < totalX; ++i)
	{
		infiniteFields.Add(finalGraph[i, 0].pt);
		infiniteFields.Add(finalGraph[i, totalY - 1].pt);
	}
	for (int j = 0; j < totalY; ++j)
	{
		infiniteFields.Add(finalGraph[0, j].pt);
		infiniteFields.Add(finalGraph[totalX - 1, j].pt);
	}

	new { infiniteFields = infiniteFields }.Dump();
	var area = new Dictionary<Point2D, int>();
	for (int i = 0; i < totalX; ++i)
	{
		for (int j = 0; j < totalY; ++j)
		{
			GraphPoint curr = finalGraph[i, j];
			if (!infiniteFields.Contains(curr.pt) && curr.weight >= 0)
			{
				if (!area.ContainsKey(curr.pt))
				{
					area[curr.pt] = 1;
				}
				else
				{
					area[curr.pt]++;
				}
			}
		}
	}

	var maxArea = area.Max(kvp=>kvp.Value);
	new {maxArea=maxArea}.Dump();
	return maxArea;
}

public HashSet<Point2D> GetPointsFromString(IEnumerable<String> v)
{
	int i=0;
	return v.Select(line => new Point2D
	{
		name = ((char)(65+i++)).ToString(),
		x = int.Parse(Regex.Split(line, ",").First().Trim()),
		y = int.Parse(Regex.Split(line, ",").Last().Trim())
	}
	).ToHashSet();
}

void GetNearestPoint(GraphPoint[,] finalGraph,Point2D currPt,List<GraphPoint> currWeights)
{
	int i=currPt.x;
	int j=currPt.y;
	int minWeight = currWeights.First().weight;
	//something else is at the same min distance
	if (currWeights[1].weight == minWeight)
	{
		finalGraph[i, j].weight = -1;
		finalGraph[i, j].pt = null;
	}
	else
	{
		finalGraph[i, j].pt = currWeights[0].pt;
		finalGraph[i, j].weight = currWeights[0].weight;
	}
}

GraphPoint[,] DoGraphOperationWithManhatanDistances(HashSet<Point2D> danger_points, System.Action<GraphPoint[,],Point2D,List<GraphPoint>> actionToGraph)
{
	var minx = danger_points.Min(point => point.x);
	var miny = danger_points.Min(point => point.y);
	var maxx = danger_points.Max(point => point.x);
	var maxy = danger_points.Max(point => point.y);
	var totalX = maxx - minx + 1;
	var totalY = maxy - miny + 1;

	new { minx = minx, miny = miny, maxx = maxx, maxy = maxy, totalX = totalX, totalY = totalY }.Dump();

	//shift to 0
	foreach (var pt in danger_points)
	{
		pt.x -= minx;
		pt.y -= miny;
	}

	new { danger_points=danger_points }.Dump();


	var finalGraph = new GraphPoint[totalX, totalY];
	for (int i = 0; i < totalX; ++i)
	{
		for (int j = 0; j < totalY; ++j)
		{
			finalGraph[i, j] = new GraphPoint();
		}
	}

	//fill the graph
	for (int i = 0; i < totalX; ++i)
	{
		for (int j = 0; j < totalY; ++j)
		{
			var currPt = new Point2D() { x = i, y = j };

			var currWeights = danger_points.Select(pt => new GraphPoint{pt=pt,weight=CalculateManhatanDistance(pt, currPt)}).OrderBy(pt=>pt.weight).ToList();
			actionToGraph(finalGraph,currPt,currWeights);
		}
	}

	new { finalGraph=finalGraph}.Dump();

	//shift back
	foreach (var pt in danger_points)
	{
		pt.x += minx;
		pt.y += miny;
	}
	return finalGraph;
}

// Define other methods and classes here
public class Point2D
{
	public String name;
	public int x;
	public int y;
}
public class GraphPoint
{
	public Point2D pt;
	// -1 means contested
	public int weight=int.MaxValue;
}

int CalculateManhatanDistance(Point2D start, Point2D end){
	return Math.Abs(end.x-start.x) +Math.Abs(end.y-start.y);
}


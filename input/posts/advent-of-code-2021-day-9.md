Title: Advent of Code 2021 - Smoke Basin
Published: 10/12/2021
Tags:
    - Code
    - CSharp
    - AdventOfCode
    - AdventOfCode2021
Lead: Searching through smoky lows and highs
---

A nice palate clenser of a day, well if you enjoy search puzzles.
If you feel like joining me you can find todays challenge [here](https://adventofcode.com/2021/day/9) and follow along or if you'd rather just read the code then [you can find it here.](https://github.com/Romanx/AdventOfCode/blob/main/src/years/2021/day-nine/Challenge.cs)

# Parsing Input
We're going to be reusing our point class from [Day 5](advent-of-code-2021-day-5) so if you haven't moved that to a shared place then I'd recommend it now since we're going to be adding some more useful methods to it.

Let's turn our input into something useful.
We could use a multidimensional array here to model our information but I prefer using a `Dictionary<Point, char>` so i'm going to do that here.
```csharp
var lines = inputLines.AsArray();
Dictionary<Point, char> dict = new Dictionary<Point, char>();

for (var y = 0; y < lines.Length; y++)
{
    var line = lines[y].AsSpan();
    for (var x = 0; x < line.Length; x++)
    {
        var point = new Point(x, y);
        dict[point] = line[x];
    }
}
```

This is a useful method when ever we're provided with a grid of characters.
We're going to actually map this dictionary into another dictionary since for this challenge we want `Dictionary<Point, int>` for our heights but I wanted to leave the original here since its useful.
```csharp
var map = dict.ToDictionary(k => k.Key, v => v.Value - '0');
```

Another useful trick here in that if you have a `char` in the range of `0 - 9` and wan't to convert it to its int representation.
Rather than doing parsing tricks you can actually subtract the character `'0'`.
If this doesn't seem obvious why it works don't worry it isn't obvious.
This subtracts the ascii number for `'0'` from the other number i.e. `'5'` which becomes `53 - 48 = 5`.

# Part 1
Now we've got a structure to be working with, we need to find the lowest points by looking at all the cardinal neighbours.
That is those that are Up, Down, Left and Right.
This is quite a common problem that we will face so lets come up with a nice way to do it.
A `Direction` class that we can use in combination with the `Point` class should simplify how we do this.

```csharp
public enum DirectionType
{
    North,
    East,
    South,
    West,
}

public readonly record struct Direction
{
    private Direction(DirectionType directionType)
    {
        DirectionType = directionType;
    }

    public DirectionType DirectionType { get; }

    public static Direction North { get; } = new Direction(DirectionType.North);
    public static Direction East { get; } = new Direction(DirectionType.East);
    public static Direction South { get; } = new Direction(DirectionType.South);
    public static Direction West { get; } = new Direction(DirectionType.West);

    public static Direction[] CardinalDirections { get; } = new[]
    {
        North, East, South, West
    }
}
```
Two notes here worth mentioning I'm using compass directions rather than grid directions since I find it easier to reason about but you can alias them easily enough.
The second is that .NET does not have rich enumerations like Java so I'm using a class with static members and a private constructor to simulate this since it makes it easier to use.

Now we have this useful class we can add the following operator to point and use it like this:
```csharp
public readonly record struct Point
{
    public static Point operator +(Point point, Direction direction) => direction.DirectionType switch
    {
        DirectionType.North => point + new Point(0, -1),
        DirectionType.East => point + new Point(1, 0),
        DirectionType.South => point + new Point(0, 1),
        DirectionType.West => point + new Point(-1, 0),
        _ => point
    };
}

var newPoint = point + Direction.North;
```

Okay so now we have a method of getting the point in a specific direction.
We can write a helper to get the all the points in the cardinal directions from a provided point like this:
```csharp
public static IEnumerable<Point> GetDirectNeighbours(Point point)
    => Direction.CardinalDirections.Select(dir => point + dir);
```

Okay so we're back to the challenge.
So our algorithm works like this:
  - Go through each position in the height map.
    - Look at each cardinal neighbour.
    - If the current point is lower than all of the surrounding points.
      - Add to the set of lowest points.

_Note:_ Since our `GetDirectionNeighbours` simply returns all the cardinal points we have to filter out those not in our height map and ignore them.

```csharp
var lowPoints = new HashSet<Point>();

foreach (var (point, height) in map)
{
    var neighbours = GetDirectNeighbours(point);

    var lowest = neighbours // Go through all possible neighbours
        .Select(n => map.TryGetValue(n, out var neighbourHeight) ? neighbourHeight : (int?)null) // Get their height or null for missing.
        .Where(n => n is not null) // Remove the null items
        .All(neighbourHeight => height < neighbourHeight)// Are all of the items higher than the current point's height

    if (lowest)
    {
        lowPoints.Add(point);
    }
}
```
We're using a HashSet here so we don't double count our low points since only distinct items are allowed in the set.
To get our shiny ⭐ we take all our low points and sum up their heights + 1.

```csharp
var score = lowPoints
    .Sum(point => 1 + map[point]);

Console.WriteLine($"Risk level {score}");
```

# Part 2
So for part 2 we need to find the basins in the height map. We can do this by ignoring any positions with the max height of 9.
An interesting thing about the input data is that our basins are surrounded by mountains of height 9 so we can use a fill algorithm to find our basin and stop as we can't go past any neighbours of height 9.
If that didn't make sense hopefully this picture helps some. Note that the the Xs stop where the neighbours are 9 so we can't go further.
```
21999XXXXX
398789X9XX
985678989X
8767896789
9899965678
```

So starting with our heightmap from Part 1.
We're going to get a set of candidate points, anything that isn't height 9, and then find the basin that contains that point.
The basin we found can then be added to our list of basins and all the points in the basin can be removed from our candidate points since we already know the whole basin.
We keep doing this until there are no points left since they're all contained in one basin or another.
```csharp
var candidatePoints = map
    .Where(kvp => kvp.Value != 9)
    .Select(kvp => kvp.Key)
    .ToHashSet();

var basins = new List<HashSet<Point2d>>();

while (candidatePoints.Count > 0)
{
    var current = validPoints.First();
    var height = map[current];

    var basin = FindBasinContainingPoint(current, map);

    basins.Add(basin);

    candidatePoints.ExceptWith(basin);
}
```
Since we're using a hashset of candidate points we can use `ExceptWith` to remove the points in the basin efficently.
The real logic for Part 2 contained in `FindBasinContainingPoint`.
I'm using a variation of a [Breadth First Search](https://en.wikipedia.org/wiki/Breadth-first_search) which returns all the points the search visited.
The variation I'm using is from RedBlobGames and optimized to not use a queue like the standard implementation.
You can find more details [about it here.](https://www.redblobgames.com/pathfinding/a-star/implementation.html#optimize-bfs-queue)

```csharp
var currentFrontier = new List<Point2d>();
var nextFrontier = new List<Point2d>();
currentFrontier.Add(source);
var visited = new HashSet<Point2d>
{
    source
};

while (currentFrontier.Count > 0)
{
    foreach (var current in currentFrontier)
    {
        foreach (var next in GetDirectNeighbours(current))
        {
            if (map.TryGetValue(next, out var nextHeight) && nextHeight != 9)
            {
                if (visited.Add(next) is true)
                {
                    nextFrontier.Add(next);
                }
            }
        }
    }
    
    // An effecient clear and swap of two lists.
    (currentFrontier, nextFrontier) = (nextFrontier, currentFrontier);
    nextFrontier.Clear();
}

return visited;
```

We start with our source point and add that to our current search set and add it to our visited set since obviously we visited where we started.

The core of the algorithm is going through each of the current points, finding all their adjacent points that are _valid_ (more on this in a second) and adding them to the next set of points we're going to look at.
The validity of points will vary, in our case they have to be in our height map and they have to be less than height `9` since we can't go past that height.

Working with our example from above we can show how the algorithm spreads to collect the points in the basin. 
We're going to start in the top right corner.
I'm using `C` for current, `N` for next and `V` for visited.
```
21999432NC    2199943NCV    219994NCVV    21999NCVVV    21999CVVVV    21999VVVVV
398789492N    39878949NC    39878949CV    398789N9VV    398789C9VV    398789V9VV
9856789892 -> 985678989N -> 985678989C -> 985678989V -> 985678989V -> 985678989V
8767896789    8767896789    8767896789    8767896789    8767896789    8767896789
9899965678    9899965678    9899965678    9899965678    9899965678    9899965678
```

Hopfully that's a clear diagram of how the search works.
When we run our of points in our current frontier then we can't go anywhere else so we have our final set of points we visited.

So to get our shiny ⭐ for the day.
We have to order the basins we found by their size going from largest to smallest, take the top 3 and then multiply their sizes together.
I'm using aggregate here for the multipliaction but a loop works just as well.

```csharp
var sizes = basins
    .OrderByDescending(basin => basin.Count)
    .Take(3)
    .Aggregate(1, (seed, set) => seed * set.Count);

Console.WriteLine($"Largest Basins Multiplied {sizes}");
```

There we have it, a shiny ⭐ and we learnt something about graph search algorithms.
On to the next day.
Title: Advent of Code 2021 - Passage Pathing
Published: 29/04/2022 13:15
Tags:
    - Code
    - CSharp
    - AdventOfCode
    - AdventOfCode2021
Lead: Big Cave, Little Cave, Cardboard box 📦
---

Another day and another delay.
Hopefully someone out there in the internet is finding these useful if not i'm finding it helpful going back over my solutions with fresh eyes so I guess we'll keep on keeping on.

For day 12 we have a graph problem.
We need to find all the paths from beginning to end of a cave system applying some rules on if a path is still valid or we should stop.
If you feel like joining me you can find this challenge [here](https://adventofcode.com/2021/day/12) and follow along or if you'd rather just read the code then [you can find it here.](https://github.com/Romanx/AdventOfCode/blob/main/src/years/2021/day-twelve/Challenge.cs)

# Parsing
We're going to start here by taking our input and turning it into a map from the cave id to the cave itself.
So it'll look something like this:
```
start -> Cave { Id: start, CaveType: Entrance, Connections: [A, b] }
A -> Cave { Id: A, CaveType: Large, Connections: [start, c, b, end] }
b -> Cave { Id: b, CaveType: Small, Connections: [start, A, d, end] }
c -> Cave { Id: c, CaveType: Small, Connections: [A] }
d -> Cave { Id: d, CaveType: Small, Connections: [b] }
end -> Cave { Id: end, CaveType: Exit, Connections: [A, b] }
```

This way we can make our code look a little cleaner and use immutable data structures during our graph search which hopefully should make it easier to reason about.
It has to be done in a two stage process since the mapping we're provided may not be in the correct order, so the target node may not appear before the source node.

```csharp
var map = new Dictionary<string, List<string>>();
foreach (var line in lines)
{    
    var split = line.Split('-');
    var start = split[0];
    var end = split[1];

    // Try add them to our map above or the default if it's not already there
    map.TryAdd(start, new List<string>());
    map.TryAdd(end, new List<string>());

    // Add the start to the end and the end to the start
    map[start].Add(end);
    map[end].Add(start);
}
```

Now we've got that mapping we can make it something nicer to deal with in code, in this case a readonly record struct and an enumeration for the CaveType.

```csharp
readonly record struct Cave(string Id, CaveType CaveType, string[] Connections);

enum CaveType 
{
    Entrance,
    Exit,
    Small,
    Large
}
```

And finally the mapping from our Dictionary of strings to these types
```csharp
var result = new Dictionary<string, Cave>();
foreach (var (id, connections) in map)
{
    var caveType = id switch
    {
        "start" => CaveType.Entrance,
        "end" => CaveType.Exit,
        // This is a little hack but based on our input data
        // if the first char is lower then the rest is too
        _ when char.IsLower(id[0]) => CaveType.Small,
        _ => CaveType.Large,
    };

    result[id] = new Cave(id, caveType, connections.ToArray());
}
```

# The Actual Work
Now we're going to split this into the algorithm for finding the number of paths through the graph which is the main task for this day.
The function for finding all the paths is very similar to [day 9](/posts/advent-of-code-2021-day-9) and the flood fill to find the basins in that it's an adapted breadth first search.

One of the differences is that we have some state for each route through the graph since we need to avoid visiting small caves twice.
We're going to do this by keeping track of which small caves we've already visited.
Our state is going to look like this

```csharp
record CaveRoute(
    string[] Path,
    Dictionary<Cave, int> SmallCaveVisits);
```

Now we've got our state we can define our search.
Our search is going to take a function which accepts both the target cave and the current state.
We're going to use this function to decide if we can make the transition to the target cave.

```csharp
static int FindPaths(
    Dictionary<string, Cave> graph,
    Func<Cave, CaveRoute, bool> caveFunc)
{
    var pathCount = 0;

    // We're always going to begin at the entrance so find it here.
    var (_, start) = graph.First(g => g.Value.CaveType is CaveType.Entrance);
    var currentFrontier = new List<CaveRoute>();
    var nextFrontier = new List<CaveRoute>();

    // Add our starting state for our search
    currentFrontier.Add(new CaveRoute(new[] { start }, new Dictionary<Cave, int>())));

    while (currentFrontier.Count > 0)
    {
        foreach (var current in currentFrontier)
        {
            // [Insert step logic here]
        }

        // Our nice fancy shortcut for swap and clear
        (currentFrontier, nextFrontier) = (nextFrontier, currentFrontier);
        nextFrontier.Clear();
    }

    return pathCount;
}
```

Most of that should be very similar to the code from Day 9 so hopefully the comments make it clear.
The actual logic for each step is missing and we're going to add it next.
This goes into the loop for the step logic in the method above.
You can make this a method if you'd like or just in-line it.

```csharp
// Lookup the current cave definition: ^1 is a shorthand for the last item in the list
Cave currentCave = current.Path[^1];
foreach (var next in currentCave.Connections)
{
    // Get the target cave definition by its id.
    Cave nextCave = graph[next];

    // If we're at the target then yay
    if (nextCave.CaveType is CaveType.Exit)
    {
        // You should add the current.Path to a set if you wanted
        // to see the paths but we only care about the count so 
        // increment here.
        pathCount++;
    }
    else if (nextCave.CaveType is CaveType.Entrance)
    {
        // If it's the start we can't go back so don't add another step to the frontier.
    }
    else if (nextCave.CaveType is CaveType.Small)
    {
        // Call the provided function to check if we're allowed 
        // to move to the next cave based on our state.
        if (caveFunc(nextCave, current))
        {
            // If we can move to the next then we need to increment the number of times
            // we've visited the cave.
            int count = current.SmallCaveVisits.GetValueOrDefault(nextCave);

            // Add to the frontier of the search a state with the updated path
            nextFrontier.Add(current with
            {
                Path = current.Path.Add(nextCave),
                SmallCaveVisits = current.SmallCaveVisits.SetItem(nextCave, count + 1)
            });
        }
    }
    else
    {
        // It must be a large cave we're in so just add to the
        // frontier we visited the large cave.
        nextFrontier.Add(current with
        {
            Path = current.Path.Add(nextCave)
        });
    }
}
```

# Part 1
Now we have our FindPaths function we can actually solve Part 1!
```csharp
Dictionary<string, Cave> graph = ParseCaves(lines);
var paths = FindPaths(
    graph,
    CaveFunction);

Console.WriteLine($"Number of paths: {paths}");

static bool CaveFunction(Cave cave, CaveRoute route)
{
    // We only allow one entry to the small cave
    // so if it exists then we shouldn't transition.
    return route.SmallCaveVisits.ContainsKey(cave) is false;
}
```

That's a first ⭐ for the day.

# Part 2
Part 2 only has a minor change from Part 1, which is that there is a different rule for transitioning in small caves.
We can now visit **one** small cave *twice* but only one.

```csharp
Dictionary<string, Cave> graph = ParseCaves(lines);
var paths = FindPaths(
    graph,
    CaveFunction);

Console.WriteLine($"Number of paths: {paths}");

static bool CaveFunction(Cave cave, CaveRoute route)
{
    var visitedTwice = route.SmallCaveVisits.Any(kvp => kvp.Value is 2);

    // If i've not visited any twice it doesn't matter just visit it.
    if (visitedTwice is false)
    {
        return true;
    }

    return route.SmallCaveVisits.ContainsKey(cave) is false;
}
```

And with that second ⭐ we're done with day 12 of Advent of code.
Most of the work was shared between the two parts thanks to our use of a function to determine if we can move to a small cave.
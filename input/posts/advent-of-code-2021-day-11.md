Title: Advent of Code 2021 - Dumbo Octopus
Published: 01/04/2022 09:27
Tags:
    - Code
    - CSharp
    - AdventOfCode
    - AdventOfCode2021
Lead: Flash ⚡ Ahhhhhh! Savior of the keys
---

So when I left off with see you tomorrow I didn't plan for it to be a few months!
Life unfortunately got in the way but I did manage to finish all of the 2021 challenges and so still plan to post write-ups of my solutions albeit with a delay.

So for day 11 we have a simulation of octopi flashing and finding out when we reach specific states.
Awesome.
If you feel like joining me you can find this challenge [here](https://adventofcode.com/2021/day/11) and follow along or if you'd rather just read the code then [you can find it here.](https://github.com/Romanx/AdventOfCode/blob/main/src/years/2021/day-eleven/Challenge.cs)

# Parsing
So we have a grid of digits from 0 to 9 which each represent an octopus.
This looks very similar to our parsing for [day 9](/posts/advent-of-code-2021-day-9) so I'm going to abstract that into a helper so we can use it here.
By the time we're done we should have a `Dictionary<Point, int>` representing each octopi and their flash number.

# Simulating Each Step
The puzzle explains what happens each step so but we're going to go through it piece by piece together.

Firstly we're going to take our current map and copy it.
This makes our step method [pure](https://blog.ploeh.dk/2020/02/24/discerning-and-maintaining-purity/) and avoids us mutating the existing state allowing us to make it easier to reason about what happened when.
When we have our copied state we then increment all the the octopi flash values by one.
```csharp
var next = new Dictionary<Point, int>(current);
foreach (var point in octopi.Keys)
{
    next[point]++;
}
```

Now this is where the fun starts. Each octopi that goes over a flash value of 9 needs to flash.
This will then increase the flash value of all octopi around it.
Those flashed octopi may have now gotten a flash value of 9 (or over) and so need to flash and so on.
There's a limitation here in that each octopi can only flash once per step.

So we're going to simulate this in waves much like finding our basins in Day 9.

```csharp
// A set of all flashed positions to avoid duplicate flashes.
var flashed = new HashSet<Point2d>();

// The current set of octopi that need to flash
var current = new Queue<Point2d>(next
    .Where(kvp => kvp.Value > 9)
    .Select(kvp => kvp.Key));

while (current.TryDequeue(out var point))
{
    if (flashed.Contains(point))
    {
        continue;
    }

    flashed.Add(point);
    
    // Flash the octopi
}
```

So we have our set of octopi that need to flash in a queue and remove them off one by one.
If we've already seen the octopi then we've flashed them already and can skip them, otherwise we need to now add them to our flashed list.

Now we're going to borrow some more code from Day 9.
We need to find all the points around the current octopi and we can do this with our Direction class.
I'm going to leave this an exercise to the reader since we need to add 4 new positions `NorthEast`, `NorthWest`, `SouthEast` and `SouthWest`.
I've named the collection of all the directions as the property `Directions.All` which you'll see in the code below.

```csharp
var neighbours = Direction.All.Select(dir => point + dir);
// Go through each neighbouring point
foreach (var neighbour in neighbours)
{
    // If the point is in our set of octopi, otherwise it's out of the map.
    if (next.ContainsKey(neighbour))
    {
        // Increment the flashed value for the octopi
        next[neighbour]++;

        // If they're now greater than 9 then add them to our queue of flashing octopi
        if (next[neighbour] > 9)
        {
            current.Enqueue(neighbour);
        }
    }
}

// Set the flashes octopi back to zero.
foreach (var point in flashed)
{
    builder[point] = 0;
}
```

# Part 1
So looking at Part 1 we have to simulate 100 steps with our field of octopi and find out how many flashes ocurred overall.

```csharp
var total = 0u;
Dictionary<Point, int> octopi = ParseOctopi(input);

for (var i = 0; i < 100; i++)
{
    (octopi, var flashed) = Step(octopi);

    total += flashed;
}

Console.WriteLine($"Number of flashes: {total}");
```

Nice and simple after the hard work in our Step method. One shiny ⭐ for us.

# Part 2
For part 2 we have to keep stepping until we find a turn where every octopi flashes at once.
Since we track how many octopi flashed each turn we just need to compare that number to the total number of octopi and when they match we're find the right turn.

```csharp
var octopi = input.ParseOctopi();

var turn = 0;

while (true)
{
    turn++;
    (octopi, var flashed) = Step(octopi);

    // All of them flashed
    if (flashed == octopi.Count)
    {
        break;
    }
}

Console.WriteLine($""All flashed on turn: {turn}");
```

And with that second ⭐ we're done with day 11 of Advent of code.
Most of the work was shared between the two parts which is always nice, hopefully you enjoyed it.

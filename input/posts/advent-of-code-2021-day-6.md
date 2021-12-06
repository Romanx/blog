Title: Advent of Code 2021 - Lanternfish
Published: 06/12/2021
Tags:
    - Code
    - CSharp
    - AdventOfCode
    - AdventOfCode2021
Lead: I'm gonna count ya little fish-y
---

We have a counting problem today and I don't like Math.
If you feel like joining me in my struggle you can find todays challenge [here](https://adventofcode.com/2021/day/6) and follow along or if you'd rather just read the code then [you can find it here.](https://github.com/Romanx/AdventOfCode/blob/main/src/years/2021/day-six/Challenge.cs)

This challenge lead me down a path by hinting that we should model the lantern fish growth rate which suggested to me some kind of Math-y function which I'm poor at.
After struggling for awhile and after completing Part 1 in a brute force approach and realising that this really was an exponential problem I looked for help elsewhere.

I follow [@BradWilson](https://twitter.com/bradwilson) of [xuint](https://xunit.net/) fame on twitter and know he streams himself doing these challenges so looked to him for some help.
He had a lovely little solution which I'm going to go through with you so we can all learn together. Thanks Brad!

# Part 1
We have to count the number of fish over a period of days as they multiply.
The trick that Brad realised that I didn't is that the fish only live for a short period of time. 
A maximum of 8 days infact.
This means the fish can only be in one of 9 buckets (0-9).

```csharp
var fishes = input.Split(',').Select(int.Parse);

var fishCounts = new long[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

foreach (var fish in fishes)
{
    fishCounts[fish]++;
}
```
So we create some buckets, and increment the bucket when there is a fish at that time of it's life.
We increment here since there can be more than one fish in our starting set at the same time of it's life.

Once we have our starting set we have to simulate the days passing.
```csharp
for (var i = 0; i < days; i++)
{
    // Get the fish that have offspring this day
    var newFish = fishCounts[0];

    // Loop through every age above zero and shuffle them down.
    for (var age = 1; age < fishCounts.Length; age++)
    {
        fishCounts[age - 1] = fishCounts[age];
    }

    // Set the new fish at count of 8;
    fishCounts[8] = newFish;

    // Add the fish that had offspring back to position 6 as the cycle begins again.
    fishCounts[6] += newFish;
}
```

At the end of our number of days our array contains all the fish on a given part of their lifecycle.
To get the total number of fish we add up all the buckets and there we have it.
```csharp
var days = 80;

// Previous Code Here

var count = fishCounts.Sum();
Console.WriteLine($"Fishes after {days} days: {count}");
```

# Part 2
Since we came up with a really efficent method of representing a lot of fish it's quite simple to calulcate the number of fish on day 256.
```csharp
var days = 256;

// Previous Code Here

var count = fishCounts.Sum();
Console.WriteLine($"Fishes after {days} days: {count}");
```

There we have it, two lovely ⭐ without any of that compilated algebra malarkey.
If you did solve this by calculating the growth function I'd love to see how you do it!
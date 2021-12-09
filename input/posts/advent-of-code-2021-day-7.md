Title: Advent of Code 2021 - The Treachery of Whales
Published: 07/12/2021
Tags:
    - Code
    - CSharp
    - AdventOfCode
    - AdventOfCode2021
Lead: Crab-marines Assemble!
---

Another counting puzzle but seems a little easier for me to understand.
If you feel like joining me you can find todays challenge [here](https://adventofcode.com/2021/day/7) and follow along or if you'd rather just read the code then [you can find it here.](https://github.com/Romanx/AdventOfCode/blob/main/src/years/2021/day-seven/Challenge.cs)

# Part 1
By now I'm sure you're an expert at parsing inputs like this but take the content, split it by `,` and turn it into integers (note my actual input fit into this size so hopefully yours will to, otherwise use `long`!)

So my first note is that it can be any number between the smallest crab position and the largest since anything outside of that will simply use more fuel so we can limit our search to `>= lowestCrabPosition && <= highestCrabPosition`

```csharp
var min = crabs.Min();
var max = crabs.Max();

var leastFuelUsed = Enumerable.Range(min, max)
    .Min(position => CalculateTotalCost(position, crabs));
```

I decided to be lazy here and use some linq methods to find the min and max but we could have used a loop.
We can get the numbers between min and max in a sequence by using `Enumerable.Range`.
Since we wan't the lowest fuel position I'm going to use Min to find only return the lowest from our calculations.
Continuing my lazy day I'm going to pass in all the crabs and the position we're checking and call it a day.

```csharp
static int CalculateTotalCost(int target, IEnumerable<int> crabs)
{
    return crabs.Sum(crab => Math.Abs(crab - target));
}
```

For calculating the cost for the position we go through each of the crabs and find out how far they would have to move from their current position to the target.
We use `Math.Abs` here since they could be going right-to-left which would be negative but we only care about the while part of the number so we'll strip the sign.

We Sum all the crabs together and we have our cost for the target.
Combined with our Min in the first method we have our leastFuelUsed and we just have to print it out and get our shiny star ⭐.

# Part 2
Turns out our fuel calculation was wrong, darn.
No worries we can just adjust our calculation to use the new formula.
The sequence for a few numbers looks like this
```
1 -> 1: 0
1 -> 2: 1
1 -> 3: 3
1 -> 4: 6
1 -> 5: 10
```
The formula for this is `(N * (N + 1)) / 2`. You can also think of it as `Enumerable.Range(1, Math.Abs(crab - target)).Sum()` since it equates to the same thing but the formula is definately faster.
With our new formula the `CalculateTotalCost` method becomes this:
```csharp
static int CalculateTotalCost(int target, IEnumerable<int> crabs)
{
    return crabs
        .Sum(crab =>
        {
            var distance = Math.Abs(crab - target);
            return (distance * (distance + 1)) / 2;
        });
}
```

The rest of our original code stays the same and we get our answer for the second ⭐ of the day. Super!
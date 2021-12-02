Title: Advent of Code 2021 - Sonar Sweep
Published: 01/12/2021
Tags: 
    - Code 
    - CSharp
    - AdventOfCode
    - AdventOfCode2021
Lead: Someone tripped and fell so of course the elves need 50⭐ again...
---

It's that time of year, it gets dark earlier and earlier and the elves are up to their old tricks of making mistakes and having me fix them.
This time they've lost Santas sleigh keys by dropping them into the ocean and we have to find 50⭐ to make the experimental antennas signal strength higher to find them.

Sounds like some fun, let's get started on day 1. You can find the challenge [here.](https://adventofcode.com/2021/day/1)
So feel free to go off, have a read and come back when you want to see how I solved the problem.

## Part 1
So we have to take all of the input in pairs and calculate if they have increased or decreased.
I decided to use [Morelinq](https://github.com/morelinq/MoreLINQ) to simplify the solution using the `Window` function which splits the input into subsequences of the provided size.
```csharp
var distances = input.Lines.Ints();

var windowed = distances.Window(2);
// [199, 200], [200, 208], [208, 210]...
```

With these windows we can then compare them to see if there was an increase or decrease from the previous then output the total number of increases.

```csharp
var distances = input.Lines.Ints();

var changes = distances
    .Window(2)
    .Select(x => CalculateChange(x[1], x[0]));

Console.WriteLine($"Number of Increases: {changes.Count(x => x is ChangeType.Increase)}");

static ChangeType CalculateChange(int current, int previous)
{
    if (current == previous)
    {
        return ChangeType.NoChange;
    }
    else if (current > previous)
    {
        return ChangeType.Increase;
    }
    else
    {
        return ChangeType.Decrease;
    }
}
```

# Part 2
A nice simple incremental change here. 
We're no longer using windows of 2, we're using windows of 3, adding all the values in the window together and then comparing the windowed totals to see if those increased or decreased.
I used the window function again but using 3 instead.
```csharp
var distances = input.Lines.Ints();

var windowed = distances.Window(3);
// [199, 200, 208], [200, 208, 210], [208, 210, 200]...
```

Then continuing the linq chain, we sum the windowed values creating our totals.
```csharp
var totals = distances
    .Window(3)
    .Select(window => window.Sum());
// 607, 618, 618...
```

Lastly we repeat the process in part 1 by creating windows of size 2 of the totals and comparing them to find if there's an increase or decrease then output the total number of increases.
```csharp
var distances = input.Lines.Ints();

var changes = distances
    .Window(3)
    .Select(x => x.Sum())
    .Window(2)
    .Select(x => CalculateChange(x[1], x[0])); 
// The calculate change method ironically is unchanged from above.

Console.WriteLine($"Number of Increases: {changes.Count(x => x is ChangeType.Increase)}");
```

That's two stars found for the start of our journey!
Let's see where the adventure takes us in the next 24 days.

My full solution can be found [here](https://github.com/Romanx/AdventOfCode/blob/main/src/years/2021/day-one/Challenge.cs) if you want to see it all together.
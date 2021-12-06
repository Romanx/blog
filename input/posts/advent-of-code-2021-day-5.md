Title: Advent of Code 2021 - Hydrothermal Venture!
Published: 06/12/2021
Tags:
    - Code
    - CSharp
    - AdventOfCode
    - AdventOfCode2021
Lead: Reminder the submarine is a non-smoking area
---
Today we've got to avoid some hydrothermal vents if possible.
We definately don't want to go over those big ones though.

You can find todays challenge [here](https://adventofcode.com/2021/day/5) and follow along with me or if you'd rather just read the code then [you can find it here.](https://github.com/Romanx/AdventOfCode/blob/main/src/years/2021/day-five/Challenge.cs)

# Parsing Inputs
The submarine has nicely provided us a list of nearby lines of vents.
These are in the form of `0,9 -> 5,9` being the start and end of the line.
They can be going horizontal or vertical.
The lines can also overlapping forming a super-vent! That's actually a made up name but they're _bad_ alright so we have to find them.

The first thing here is we're going to need something to represent a Point or `(x, y)`.
I would suggest puting this in a common location since you may need this a few times during advent of code.
It's just a feeling.
Here's a nice simple Point readonly struct with some useful operations.
```csharp
public readonly record struct Point(int X, int Y)
{
    public static Point operator +(Point left, Point right)
            => new(left.X + right.X, left.Y + right.Y);

    public static Point operator -(Point left, Point right)
        => new(left.X - right.X, left.Y - right.Y);

    public static Point Parse(string value)
    {
        var split = value.Split(",");

        if (split.Length != 2)
        {
            throw new InvalidOperationException($"Cannot convert '{value}' to Point");
        }

        return new Point(
            int.Parse(split[0]),
            int.Parse(split[1]));
    }
}
```
This has an X and Y coordinate, the ability to add two points together and subtract two points (not useful here but nice for symmetry) and lastly a parse method which expects a string in the form `x,y`.
I've added some error handling in the parse method since the last thing you want is this to be the thing causing your logic to be incorrect.

Next up we're going to create a type to represent a line of vents.
```csharp
record VentLine(Point start, Point end)
{
    public IEnumerable<Point2d> Points => CalculatePoints();

    private IEnumerable<Point2d> CalculatePoints()
    {
        throw new NotImplementedException();
    }
}
```
I've got a placeholder for creating the points inbetween the start and end since we're going to need it to find out where the lines are overlapping.

Now to our parsing of the input, assuming we're loading the information from the file and going through it line by line.
```csharp
var ventLines = new List<VentLine>();
foreach (var line in lines)
{
    var split = line.Split("->", StringSplitOptions.TrimEntries);
    var start = Point.Parse(split[0]);
    var end = Point.Parse(split[1]);

    ventLines.Add(new VentLine(start, end));
}
```

# Part 1
Now we have something to be working with part 1 gives us a key bit of information we haven't yet accounted for.
It says to _"only consider horizontal and vertical lines"_ which means that the input __must__ include lines that are not horizontal or vertical!

So we have to change our parsing logic a little.
For now we're going to drop those that aren't valid on the floor and we can pick them up later.

```csharp
var ventLines = new List<VentLine>();
foreach (var line in lines)
{
    var split = line.Split("->", StringSplitOptions.TrimEntries);
    var start = Point.Parse(split[0]);
    var end = Point.Parse(split[1]);

    if (start.Y == end.Y)
    {
        return new GridLine(start, end, LineType.Horizontal);
    }
    else if (start.X == end.X)
    {
        return new GridLine(start, end, LineType.Vertical);
    }
}

enum LineType
{
    Unknown, // Always have a default case!
    Horizontal,
    Vertical,
    Diagonal,
}
```
We've added an enumeration for the type of line, this will be useful for filtering out lines we care about in each part.

Next we have to work out what points are actually contined within the line.
Coming back to our `VentLine.Points` method we left broken earlier.
So an interesting difficulty here is that lines can be going right-to-left and bottom-to-top so we have to account for that also depending on the start and end positions of the line.

I decided to go with calculating an adjustment value that we can use to update our position on the line going from the start until we reach the end.
Since we're going point by point our adjustment is only ever going to be `+1` or `-1` on either axis.
We can work out if we're going `+` or `-` by comparing the start and end together like so.

```csharp
var x = (End.X - Start.X) switch
{
    0 => 0,
    > 0 => 1,
    < 0 => -1,
};
```
This is using new C# 10 features allowing us to perform greater than `>` or less than `<` expressions in our switch comparison.

Calculating the adjustment value for the `y` axis is identical but with the axis changed from `X` to `Y` so I'll leave that in your capable hands.
Now we have these two values we can make a `Point` representing our adjustment.

Now we have the adjustment it is a case of stepping along the line. We're going to use C#s yield feature allowing us to lazily return the values rather than building them up in memory. 
It could be a very long line!?
I don't know `¯\_(ツ)_/¯`

```csharp
var adjustment = new Point(x, y);

var start = Start;
while (start != End)
{
    yield return start;
    start += adjustment;
}
yield return End;
```

We first yield the start of the line then we add our adjustment to get the next position along the line and so on and so forth until we reach the end.
Since our condition is when start is not equal to end we have to lastly return the End position.

Now we've got all our points we can work out where our lines overlap.
We're going to do this using linq and grouping and it may look like a load of noise but we'll go through it.

```csharp
var numberOfOverlappingVents = ventLines
    .Where(v => v.Type is LineType.Horizontal or LineType.Vertical)
    .SelectMany(i => i.Points)
    .GroupBy(i => i)
    .Where(i => i.Count() >= 2)
    .Select(i => i.Key)
    .Count();

Console.WriteLine($"Number of Overlapping Vents: {numberOfOverlappingVents}");
```

So step by step here is what this word soup does:
  - `Where` filter the vent lines down to only those we know are Horizontal or Vertical
  - `SelectMany` takes every point from each vents and returns them as one big list.
  - `GroupBy` Take the one big list and put it into buckets based on the point. Because the same point can be shared by more than one vent that will be where they overlap.
  - `Where` Now we have the points grouped up we only want the ones where the buckets have 2 or more in it. So nothing on its lonesome.
  - `Select` Now we only want the point itself and don't care about the buckets we grouped them into since they're all the same.
  - `Count` We don't actually care about which points, just how many so let's just get a count of them.

And then we have the count of the overlapping points between the _horizonal_ and _vertical_ lines! Hurrah a ⭐ for us.

# Part 2
So for part 2 we now need to consider the diagonal lines we dropped on the floor.
The input tells us kindly that all the inputs will be either horizontal, vertical or diagonal at exactly 45 degress.
This means we don't _have_ to deal with any logic to verify the other lines are truly diagonal and just trust the input.
I'm going to but if you don't want to then feel free to look into calculating the slope of a line and using that to drop lines or error as you see fit.

If we go back to our parsing code we're going to adjust it slightly for the Vent Lines that are diagonal.

```csharp
var ventLines = new List<VentLine>();
foreach (var line in lines)
{
    var split = line.Split("->", StringSplitOptions.TrimEntries);
    var start = Point.Parse(split[0]);
    var end = Point.Parse(split[1]);

    if (start.Y == end.Y)
    {
        return new GridLine(start, end, LineType.Horizontal);
    }
    else if (start.X == end.X)
    {
        return new GridLine(start, end, LineType.Vertical);
    }
    else
    {
        return new GridLine(start, end, LineType.Diagonal);
    }
}
```

And with an anticlimatic boom that's part 2 completed besides actually calculating the overlapping vents again but this time including the diagonal lines.
```csharp
var numberOfOverlappingVents = ventLines
    .SelectMany(i => i.Points)
    .GroupBy(i => i)
    .Where(i => i.Count() >= 2)
    .Select(i => i.Key)
    .Count();

Console.WriteLine($"Number of Overlapping Vents: {numberOfOverlappingVents}");
```

The same logic as before without the filtering of any kind of vent lines. Nice and clean and shiny ⭐ for us.
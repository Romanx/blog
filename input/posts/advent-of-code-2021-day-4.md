Title: Advent of Code 2021 - Giant Squid!
Published: 05/12/2021
Tags:
    - Code
    - CSharp
    - AdventOfCode
    - AdventOfCode2021
Lead: Squid! Bingo! Squid-ingo?!
---

Welcome all, this is my new friend the Giant Squid.
Today we decided to play some bingo with them and see who would win first.
You can find todays challenge [here](https://adventofcode.com/2021/day/4) and follow along with me or if you'd rather just read the code then [you can find it here.](https://github.com/Romanx/AdventOfCode/blob/main/src/years/2021/day-four/Challenge.cs)

# Parsing Inputs
We're not going to have much of a parsing section today since most of the problem here is solved by parsing the input into something that can be used for solving the problem.

I will mention that when handling the input I broke it up into separate sections delimited by empty lines, you can think of these as paragraphs.

Now you have a list of paragraphs you can treat them individually so the first item is the sequence of numbers we've been given and everything after is a grid to be parsed.

# Part 1

I actually went a different way with this at first and then revised my solution after completing both parts since it both made more sense and I understood the problem better.

Initially I wen't with a simple solution of parsing out each of the grids, then drawing each number in turn and marking them off on all the grids.
After marking the number on each grid I checked if it had won and if so stopped and basked in my glory.

When looking at this again, I realised that the sequence of numbers we were given reflected each turn that would be played.
So when parsing the inputs I could also look at when each number would be marked off in the turn order and create a map of this from `number -> turn`
```csharp
var numbersAndTurn = paragraphs[0]
    .Split(',')
    .Select(int.Parse)
    .Index(startIndex: 1)
    .ToDictionary(x => x.Value, x => x.Key);

var grids = new List<BingoGrid>();
var scratch = new (int Turn, int Number)[5, 5];

foreach (var grid in paragraphs[1..])
{
    grids.Add(ParseGrid(grid, scratch, numbersAndTurn));
}

readonly record struct BingoGrid(int WinningTurn, int FinalScore);
```
Now we have a map of numbers to the turn that they will be marked.
Using this we can loop through the rest of the paragraphs and parse the grids individually.
I'm trying to be conservitive with memory here by reusing the same 5x5 grid when calculating the turn that a grid wins on and what it's final score is.

The next bit is where the real logic of the challenge comes into focus in the `ParseGrid` method.

```csharp
var span = scratch.AsSpan2D(); // Microsoft.Toolkit.HighPerformance
for (var y = 0; y < grid.Length; y++)
{
    var row = grid[y].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    for (var x = 0; x < row.Length; x++)
    {
        var number = int.Parse(row[x]);
        span[x, y] = (numbersOnTurn[number], number);
    }
}
```

Hopefully the logic here is quite clear, we go through each line in the grid, split it on a space and then go through each number individually.
It's worth nothing that the way the grids are formatted on the input we will still have some leading/trailing whitespace but `int.Parse` doesn't care about this so we'll move on with our lives.

Once we have the number we will set it in the grid as a [tuple](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-tuples) of two `ints` comprising of the Turn and the Number itself.
This allows us the group the two pieces of information together for each piece of the grid.
Another option would be to use a class or struct but I find this clearer for local calculations.

Now we have this information we can go through each row and column finding out when they would be completed. 
The earliest turn that a row or column is completed is the winning turn for the grid.

```csharp
var winningTurns = new List<int>(10);
for (var y = 0; y < span.Height; y++)
{
    var last = -1;
    foreach (var (turn, _) in span.GetRow(y)) // Microsoft.Toolkit.HighPerformance
    {
        last = Math.Max(turn, last);
    }
    winningTurns.Add(last);
}

for (var x = 0; x < span.Width; x++)
{
    var last = -1;
    foreach (var (turn, _) in span.GetColumn(x)) // Microsoft.Toolkit.HighPerformance
    {
        last = Math.Max(turn, last);
    }
    winningTurns.Add(last);
}

var winningTurn = winningTurns.Min();
```

To find the winning turns we loop through each of the rows and columns.
The `Span2d` type I'm using here give nice helpers for iterating over a row or a column although you could write these yourself on a standard multidimensional array. See the section on Span2d below for more information.

Here i'm keeping track of all winning turns but you could easily rewrite this to perform the same logic with `Math.Min` as I am finding the last number marked in the column/row with `Math.Max`.
It's easy to miss the edge case here with the `-1` this is since we know that no cell will ever be less than turn 1 so we can make sure we always get the maximum here since they will all be above it.

Now we have the turn that the grid won on we can calculate the score and the last number that got set.
```csharp
var winningNumber = numbersOnTurn
    .First(x => x.Value == winningTurn)
    .Key;

var score = 0;
for (var y = 0; y < span.Height; y++)
{
    foreach (var (turn, number) in span.GetRow(y))
    {
        if (turn > winningTurn)
        {
            score += number;
        }
    }
}

return new BingoGrid(winningTurn, winningNumber * score);
```

Since we have a map of number to turn going the other direction is a bit of a pain so i've used some simple Linq here.
An alternative would be to create another dictionary going the other direction and if this were a bottleneck that would be a good fix.
For here though? It's good enough.

The same with calculating the score. 
I kept wracking my brain to figure out a way to do it while finding the earliest turn above but without keeping a score for each candidate turn I couldn't think of one so I decided to do the simple.

Loop through our grid, find any that are set after the winning turn (which means they must be unmarked when we won) and add them to our score.
The final score is multiplying the score together with the number that we last marked to win the game.

```csharp
var winner = grids.MinBy(grid => grid.WinningTurn);

Console.WriteLine($"Winning Turn {winner.WinningTurn}");
Console.WriteLine($"Winner Score {winner.FinalScore}");
```
So actually completing part 1 after all that madness? Nice and clean.
We're using the newly added `MinBy` linq operator in .NET 6 which finds us the minimum by a given value in collection while but without changing the type of the collection.
Here's our lovely shiny ⭐ for part 1.

## Part 2
Well for us all our hard work above pays off, we now need to find the last grid to win.
```csharp
var worstGrid = grids.MaxBy(grid => grid.WinningTurn);

Console.WriteLine($"Last Winning Turn {worstGrid.WinningTurn}");
Console.WriteLine($"Last Winner Score {worstGrid.FinalScore}");
```
We get to use the sibling of the `MinBy` operator the `MaxBy` operator! It does the exact opposite of before.

Here's our well earned ⭐ for part 2.

---

### Microsoft.Toolkit.HighPerformance - Span2d
The eagle-eyed among you may have noticed a method called `AsSpan2D()` with a comment next to it.
This is an extension method provided by the `Microsoft.Toolkit.HighPerformance` nuget package that has some lovely helpers for dealing with common tasks that often have performance sensitive characteristics.
One of these tasks is dealing with data in a 2d multidimensional grid so `int[,]` not `int[][]` which is known as a [Jagged array](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/arrays/jagged-arrays)

For more information on this package I'd recommend taking a look at the [introduction](https://docs.microsoft.com/en-us/windows/communitytoolkit/high-performance/introduction) and having a play yourself.

I will try to make it clear when I'm using one of the methods from libraries.
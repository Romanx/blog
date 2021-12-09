Title: Advent of Code 2021 - Seven Segment Search
Published: 09/12/2021
Tags:
    - Code
    - CSharp
    - AdventOfCode
    - AdventOfCode2021
Lead: Deduction, Confusion and a little bit of rage
---

Another day and another fustrating challenge from the Elves.
If you feel like joining me you can find todays challenge [here](https://adventofcode.com/2021/day/8) and follow along or if you'd rather just read the code then [you can find it here.](https://github.com/Romanx/AdventOfCode/blob/main/src/years/2021/day-eight/Challenge.cs)

# Parsing Inputs
Our input is a set of signal patterns and some output values split by a pipe `|`.
If we take each line as we can do the following to get this into a useful shape.

```csharp
var entries = new List<Entry>();

foreach (var line in lines)
{
    var split = line.Split('|', StringSplitOptions.TrimEntries);
    var patterns = split[0]
        .Split(' ', StringSplitOptions.TrimEntries)
        .Select(x => SortStringOrder(x))
        .ToImmutableHashSet();

    var outputValues = split[1]
        .Split(' ', StringSplitOptions.TrimEntries)
        .Select(x => SortStringOrder(x))
        .ToImmutableArray();

    builder.Add(new Entry(patterns, outputValues));
}

static string SortStringOrder(string input)
{
    var characters = input.ToCharArray().AsSpan();
    characters.Sort();
    return new string(characters);
}
```

We're going to sort the signal patterns since the order of the characters doesn't represent anything besides that the signal includes that character.
This way the same values in different orders are equivalent which will become useful later.

# Part 1
We're going to count the number of entries that have output values of a specific length.

```csharp
var count = 0;
foreach (var entry in entries)
{
    count += entry.OutputValues
        .Count(static value => {
            return value.Length switch
            {
                2 => true, // Number 1
                3 => true, // Number 7
                4 => true, // Number 4
                7 => true, // Number 8
                _ => false
            }
        });
}

Console.WriteLine($"Digit Count: {count}");
```
So if the length matches one of our unique values then we Count it and add it to our total. 
That's our first ⭐ and now we get to the pain.

# Part 2
For part 2 we have to each entry by looking at the signal patterns for that entry and working out what signals represent each number.

We already know how to find four of the numbers from part 1 so our decode method starts like this:
```csharp
var map = new string[10];

// Easy Numbers
map[1] = signalPatterns.First(x => x.Length == 2);
map[4] = signalPatterns.First(x => x.Length == 4);
map[7] = signalPatterns.First(x => x.Length == 3);
map[8] = signalPatterns.First(x => x.Length == 7);
```

The bit that took me way to long to work out is that this is an set problem at the end of the day.
We can work out the other missing numbers by finding their overlap.
I have an array of strings here where the index is the number and the string is the signals that make up the number.

Here's an example of how to deduce one of the numbers.
We have to look for the number we know that has overlapping segments with our target number.
So for the target number `9` we can see that it has 4 segments overlapping with the number `4` which we know.

It looks like this when in the seven segment display:
```
  9:        4:    
 aaaa      ....   
b    c    b    c  
b    c    b    c  
 dddd      dddd   
.    f    .    f  
.    f    .    f  
 gggg      ....   
```

Going back to code, to deduce the pattern for the number 9, it has to be a length of 6 (so we have 6 segments lit) and it must intersect with the pattern for number 4 with 4 signals overlapping (all of them).

```csharp
map[9] = signalPatterns.First(x =>
    x.Length == 6 &&
    map[4].Intersect(x).Count() == 4);
```

Rather than going through one by one, here are the rest with comments:
```csharp

// - Must be length 6
// - Must not be the same pattern as the number 9
// - Overlaps with the pattern for number 1 by 2 signals.
map[0] = signalPatterns.First(x =>
    x.Length == 6 &&
    x != map[9] &&
    map[1].Intersect(x).Count() == 2);

// - Must be length 6
// - Must not be the same pattern as the number 9 or number 0.
map[6] = signalPatterns.First(x =>
    x.Length == 6 &&
    x != map[9] &&
    x != map[0]);

// - Must be of length 5 
// - Overlaps entirely with the pattern for number 1.
map[3] = signalPatterns.First(x =>
    x.Length == 5 &&
    map[1].Intersect(x).Count() == 2);

// - Must be of length 5
// - Must not be the same pattern as the number 3 
// - The candidate signal must overlap entirely with the signal for 9.
map[5] = signalPatterns.First(x =>
    x.Length == 5 &&
    x != map[3] &&
    map[9].Intersect(x).Count() == 5);

// - Must be of length 5
// - Must not be the same pattern as the number 3 or number 5.
map[2] = signalPatterns.First(x =>
    x.Length == 5 &&
    x != map[3] &&
    x != map[5]);
```

Now we have our signal map, decoding the the entry is quite easy.
We can use `Array.IndexOf` to get the number from the signal, the actual input is scrambled but since we sorted our characters they will all be the same order so this lookup works neatly.
For each entry we have to multiply the value by 10 to move it into the right column. 

```csharp
var decoded = DecodeDisplay(entry.SignalPatterns);
var result = 0;

foreach (var item in entry.OutputValues)
{
    result = (result * 10) + Array.IndexOf(decoded, item);
}
```

Now we can calculate the our result by adding up the value from all the decoded entries like so:
```csharp
var result = entries
    .Sum(entry => DecodeEntry(entry));

Console.WriteLine($"Result: {result}");
```

And there's our second ⭐ for a fustrating challenge.
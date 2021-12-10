Title: Advent of Code 2021 - Syntax Scoring
Published: 10/12/2021 11:31
Tags:
    - Code
    - CSharp
    - AdventOfCode
    - AdventOfCode2021
Lead: Dueling banjos but for algorithms
---

For todays challenge we have a syntax parsing challenge so if you've ever wondered how compilers do it, we get a small taste today.
If you feel like joining me you can find todays challenge [here](https://adventofcode.com/2021/day/10) and follow along or if you'd rather just read the code then [you can find it here.](https://github.com/Romanx/AdventOfCode/blob/main/src/years/2021/day-ten/Challenge.cs)

No parsing today since the challenges themselves are more about the parsing than anything else!

# Part 1
So we're going to focus on corrupted lines here so those that aren't following the rules 🚨.
These are those that don't have a matching closing bracket to their opening brackets so we need to keep track of all the brackets that were opened.

We're going to track this using a stack, so for all the opening brackets `(` `[` `{` `<` we're going to push the character on to the stack.
If it's not an opening bracket then it must be a closing bracket!
The bracket that is closing _must_ match the last opened bracket otherwise we've got a corrupted line and we can skip out here.
Here's the code for what I just described above.
```csharp
static readonly Dictionary<char, char> bracketMap = new()
{
    ['('] = ')',
    ['['] = ']',
    ['{'] = '}',
    ['<'] = '>',
};

LineResult ScoreLine(string line)
{
    var chunks = new Stack<char>();
    foreach (var c in line)
    {
        // If it's an open chunk
        if (bracketMap.ContainsKey(c))
        {
            chunks.Push(c);
        }
        else
        {
            var current = chunks.Pop();
            var close = bracketMap[current];
            if (c != close)
            {
                return new LineResult(LineType.Corrupted, ScoreCorrupted(c));
            }
        }
    }

    return new LineResult(LineType.Incomplete, 0);
}

readonly record struct LineResult(LineType Type, long Score);

enum LineType
{
    Incomplete,
    Corrupted,
}
```
I'm ignoring the incomplete lines here and using a map of the opening bracket to the closing bracket since it's nice and easy to lookup.
The challenge gives us the way to score corrupted lines based on the kind of bracket that is wrong but I'll add it below since it's a nice example of [Switch Expressions](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/switch-expression).

```csharp
static int ScoreCorrupted(char c) => c switch
{
    ')' => 3,
    ']' => 57,
    '}' => 1197,
    '>' => 25137,
    _ => throw new InvalidOperationException($"Not a valid scoring character: '{c}'")
};
```

Now we have our scores, getting our shiny ⭐ for part 1 becomes quite clean
```csharp
var total = lines
    .Select(line => ScoreLine(line))
    .Where(line => line.Type is LineType.Corrupted)
    .Sum(line => line.Score);

Console.WriteLine($"Error Score: {total}");
```

# Part 2
For part 2 we've got to deal with those pesky incomplete lines.
Luckly we already have all the information we need.
The incomplete lines are those that we reach the end of the line but still have items on our stack because we've opened brackets and not closed them.
To close them off we need to take each of the items on the stack in turn, map them to their closing counterpart and those would be the missing brackets.

At the end of our `ScoreLine` method above, rather than returning a placeholder for incomplete lines, the logic described above becomes this:
```csharp
var autocompleteScore = chunks
    .Select(c => bracketMap[c])
    .Aggregate(0L, (acc, c) => (acc * 5) + ScoreAutocomplete(c));

return new LineResult(LineType.Incomplete, autocompleteScore);
```

We're using aggregate to total up all our results, this is because each previous result is multiplied by `5` before adding on the score from auto-completing the bracket.
The scoring method is provided in the challenge as before but for completeness here it is
```csharp
static int ScoreAutocomplete(char c) => c switch
{
    ')' => 1,
    ']' => 2,
    '}' => 3,
    '>' => 4,
    _ => throw new InvalidOperationException($"Not a valid scoring character: '{c}'")
};
```

So then to get our second ⭐ of the day we need to take all our autocompleted lines, sort them by their score and then take the median one (the one in the middle).
```csharp
var scores = lines
    .Select(line => ScoreLine(line))
    .Where(line => line.Type is LineType.Incomplete)
    .OrderBy(x => x.Score)
    .ToArray();

var median = scores[scores.Length / 2];

output.WriteProperty("Autocomplete Score", median.Score);
```

And we're done with day 10 of Advent of code.
Hopefully you're having fun with the challenges, either following along with me or using these as a comparison to how awesome you're solution is.
If you got this far on your own I'm proud of you, if you followed along with me I'm proud of you, Hell if you just got here randomly then i'm proud of you also.

See you tomorrow for Day 11!
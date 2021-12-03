Title: Advent of Code 2021 - Binary Diagnostic!
Published: 03/12/2021
Tags:
    - Code
    - CSharp
    - AdventOfCode
    - AdventOfCode2021
Lead: I'm about to bit-flip this table
---

Lets lead with the fact I'm not that good at binary and I find these problems harder than I feel I should. I did solve it however and you can find todays challenge [here](https://adventofcode.com/2021/day/2) and follow along with me.

If you'd rather just read the code then [you can find it here.](https://github.com/Romanx/AdventOfCode/blob/main/src/years/2021/day-three/Challenge.cs)

# Part 1
We're going to skip parsing today since we're going to read our data as characters and get some information on the input as we go for the problem.
I'm not sure if there's a nice binary method of finding the most common bit in a position based on a set of binary values so instead I decided to create my own map of the information upfront and reuse it.

_Note:_ It's worth adding that I rewrote these methods multiple times through part 1 and part 2 as my assumptions proved wrong.
Don't worry too much if you have to rewrite parts of what you've already done, just keep an eye you don't start getting different results since it's hard to work backwards to where something broke.

Two approaches of calculating the position information struck me at first and both seemed equally as much work (in terms of execution).
- You loop through each number in turn and then through each position in the number.
- You loop through each position and then you loop through each number in turn.

I ended up on the latter approach after getting to part 2 and seeing it would better for reuse there.
I'm using a readonly record struct again here for ease.

```csharp
static PositionInfo CalculatePositionInfo(int index, IEnumerable<string> numbers)
{
    var zeroes = 0;
    var ones = 0;
    foreach (var number in numbers)
    {
        var span = number.AsSpan();
        var value = span[index];
        if (value is '1')
        {
            ones++;
        }
        else
        {
            zeroes++;
        }
    }

    return new PositionInfo(ones, zeroes);
}

readonly record struct PositionInfo(int NumberOfOnes, int NumberOfZeros);
```

Now we can calculate the position info for a specific index in the set of numbers.
We can use this to calculate a map of position to info.

```csharp
static Dictionary<int, PositionInfo> ParseBitIndexCounts(IEnumerable<string> lines)
{
    var dictionary = new Dictionary<int, PositionInfo>();
    var arr = lines.ToArray();
    var numberLength = arr[0].Length;

    for (var i = 0; i < numberLength; i++)
    {
        var info = CalculatePositionInfo(i, arr);

        dictionary[i] = info;
    }

    return dictionary;
}
```

With this map we can calculate the information we need for part 1.

```csharp
var bitInformation = ParseBitIndexCounts(numbers);

var gammaRateBits = CalculateGammaRate(bitInformation);
var gammaRateValue = Convert.ToInt32(gammaRateBits, 2);

var epsilonRateBits = CalculateEpsilonRate(gammaRateValue, bitInformation.Count);
var epsilonRateValue = Convert.ToInt32(epsilonRateBits, 2);

Console.WriteLine($"Gamma Rate Bits {gammaRateBits}");
Console.WriteLine($"Epsilon Rate Bits {epsilonRateBits}");
Console.WriteLine($"Gamma Rate Value {gammaRateValue}");
Console.WriteLine($"Epsilon Rate Value {epsilonRateValue}");

Console.WriteLine($"Power Consumption {gammaRateValue * epsilonRateValue}");

static string CalculateGammaRate(Dictionary<int, PositionInfo> bitIndexCounts)
{
    Span<char> gammaRate = new char[bitIndexCounts.Count];

    foreach (var (index, info) in bitIndexCounts)
    {
        gammaRate[index] = info.NumberOfOnes > info.NumberOfZeros
            ? '1'
            : '0';
    }

    return new string(gammaRate);
}

static string CalculateEpsilonRate(int gammaRate, int length)
{
    var str = Convert.ToString(~gammaRate, 2);

    return str[^length..];
}
```
Some of this code is used for debugging.
But is useful to see how the problem is solved.
To calculate the gamma rate we loop through the information map we generated, and set the values in an array.
We're using a `Span<char>` here since we're going to be using this bit representation to get a decimal value and there's a useful overload that takes a `ReadOnlySpan<char>`.

To convert a binary string into a decimal representation we can use a useful method `Convert.ToInt32(string, int)` the second parameter hints what base the binary string is in allowing us to parse base 2 strings i.e. binary.

To calculate the Epsilon Rate it is the binary inverted representation of the Gamma Rate. We could loop through and turn all the `'0'`s to `'1'`s or since we have the decimal representation we can use the Bitwise complement operator `~` operator to reverse all the bits for us.

I'm converting the inverted value into a string so we can make the binary value is correct but we could just short circuit that and use the inverted value direct.

Multiply the two together and we have our result. One glorious ⭐ for our radar antenna.

# Part 2
For part 2 we have to narrow our input to find the Oxygen generator and C02 scrubber ratings.
Each of these have a bit critera to determine which bit value we're looking to filter our candidate values by.
```csharp
static int FindTargetValue(
    string[] numbers,
    Func<int, string, PositionInfo, bool> criteriaFunction)
{
    var index = 0;
    var workingSet = new List<string>(numbers);

    while (workingSet.Count > 1)
    {
        var scratch = new List<string>(workingSet);
        var info = CalculatePositionInfo(index, scratch);

        foreach (var number in workingSet)
        {
            if (criteriaFunction(index, number, info) is false)
            {
                scratch.Remove(number);
            }
        }

        workingSet = scratch;
        index++;
    }

    return Convert.ToInt32(workingSet[0], 2);
}
```
To find the target value we loop through each position and reduce the working set of numbers down by the criteria provided.
We have two sets of numbers since if you try to remove from a set that you're iterating over you'll get an exception so we copy the list and remove anything that doesn't match our critera and then make the result the next working set and move to the next index.
Once we have a single result then we conver the binary result to decimal value which is the number we were looking for.

```csharp
static bool OxygenCriteria(int index, string number, PositionInfo info)
{
    if (info.NumberOfOnes >= info.NumberOfZeros)
    {
        return number[index] == '1';
    }
    else
    {
        return number[index] == '0';
    }
}

static bool ScrubberCriteria(int index, string number, PositionInfo info)
{
    if (info.NumberOfZeros <= info.NumberOfOnes)
    {
        return number[index] == '0';
    }
    else
    {
        return number[index] == '1';
    }
}
```
Here are our two critera functions.
We take the index, the number we're looking at and the information for the position.
The comparision here is important since if the number of zeroes and ones match then depending on the critera we wan't to set either `'0'` or `'1'`.

Now we have the criteria functions and the function to search for the value that match the criteria in full. Our solution part 2 becomes quite simple excluding the functions above.
```csharp
var oxygenRating = FindTargetValue(numbers, OxygenCriteria);
var scrubberRating = FindTargetValue(numbers, ScrubberCriteria);

Console.WriteLine($"Oxygen Rating {oxygenRating}");
Console.WriteLine($"CO2 scrubber rating {scrubberRating}");
Console.WriteLine($"Life Support Rating {oxygenRating * scrubberRating}");
```

And there we get the second ⭐ of the day.
I think there is probably a neat bit operation method of doing part 1 and I'd be interested to see it but I'm quite happy with the solution I ended up with.
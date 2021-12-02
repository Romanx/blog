Title: Advent of Code 2021 - Dive!
Published: 02/12/2021
Tags:
    - Code
    - CSharp
    - AdventOfCode
    - AdventOfCode2021
Lead: What does this lever do?
---

You can find todays challenge [here](https://adventofcode.com/2021/day/2) and follow along with me.

If you'd rather just read the code then [you can find it here.](https://github.com/Romanx/AdventOfCode/blob/main/src/years/2021/day-two/Challenge.cs)

## Parsing Inputs
Lots of advent of code problems start with parsing your input into something that allows you to easily perform the operations you need to.
We usually get some nice examples so we can test our parsing however it's worth nothing that _these are not always representative of the full input_.

It's always worth opening your full input, taking a look through and making sure that the types (remember I'm using .NET here) you're planning to use fully cover your input.
I have spent many hours debugging a working algorithm that is returning the wrong results purely because I made a mistake in my parsing code.

For this problem I decided to define a few key types that i'm going to use to solve the problem.
I need a type to represent the current location of the Submarine and another for the commands that i'm going to parse and perform.
Here they are:
```csharp
readonly record struct Submarine(int Position, int Depth);

readonly record struct Command(CommandType Type, int Units);

enum CommandType
{
    Forward,
    Up,
    Down,
}
```

I'm using a new fancy C# 10 feature here of record structs however standard records or even classes will work fine here.
I've created an enumeration for the command type to make it easier in my algorithm to make sure I haven't missed one of the cases.
`record structs` are mutable by default and I personally prefer to keep my structures as immutable as possible while doing _AdventOfCode_ since it makes some of the problems slightly easier, especially if you need to cache results.
Adding readonly to the front makes them immutable and the only way to change them is to use `with expresions` which I'll cover below.

I'm going to exclude the code of reading from the input file and focus on converting the lines into the types we've just defined.
```csharp
foreach (var line in lines)
{
    var span = line.Span;
    var split = span.IndexOf(' ');

    var type = Enum.Parse<CommandType>(span[..split], ignoreCase: true);
    var units = int.Parse(span[split..]);

    commands.Add(new Command(type, units));
}
```

I'm using a lower level `Span` type here in .NET to make sure my parsing is fast and memory efficent.
Both `Enum.Parse` and `int.Parse` have overloads that take a `Span<char>` so we don't have to copy strings to convert them to the right types.
Looking at our input I can use the space between the command type and the number of units to split the information to parse them correctly.

_Note: `Enum.Parse` is type sensitive so keep .NET semantics make sure to add the ignoreCase parameter_

## Part 1
Now on to the juicy bit, moving our submarine in the right direction and depth!

The core of our algorithm is to loop through our commands and depending on the type of command, we have to change either the Position or the Depth of the submarine.

```csharp
Submarine submarine = new(Position: 0, Depth: 0);

foreach (var command in commands)
{
    submarine = command.Type switch
    {
        CommandType.Forward => submarine with { Position = submarine.Position + command.Units },
        CommandType.Up => submarine with { Depth = submarine.Depth - command.Units },
        CommandType.Down => submarine with { Depth = submarine.Depth + command.Units },
        _ => throw new NotImplementedException(),
    };
}
```

We're using two nice features added to more recent versions of .NET here.
`Switch expressions` and record `With Expressions`.

Switch expressions simplify performing a switch on a value and assigning the result of the switch to a variable.
It also will warn you if your expression is not exhaustive (does not cover all the possible cases) and suggest you add the catch all case `_` which is similar to `default` in standard switch statements.

With expressions allow you to perform non-destructive mutations on a record.
This means you can change some values and copying the existing any that you did not change into a new instance.
In the case above it means we can set the new position of the Submarine without changing the depth.

Once we've performed all the commands, the Submarine must be in the right position so we just output the results
```csharp
Console.WriteLine($"Depth: {submarine.Depth}");
Console.WriteLine($"Position: {submarine.Position}");

Console.WriteLine($"Result: {submarine.Depth * submarine.Position}");
```

Lovely another ⭐ for the collection.

# Part 2
As usual [RTFM](https://en.wikipedia.org/wiki/RTFM) would have saved a lot of time.
Looks like we have another variable to keep track of and the commands do slightly different things.
Luckily all the work we put into parsing our input isn't wasted so we just have to write a new algorithm and add the `Aim` property to our submarine.
Since we're using non-destructive mutations, adding the aim property to our submarine doesn't even break our existing code, Result!

```csharp
Submarine submarine = new(Position: 0, Depth: 0, Aim: 0);

foreach (var command in commands)
{
    submarine = command.Type switch
    {
        CommandType.Forward => submarine with
        {
            Position = submarine.Position + command.Units,
            Depth = submarine.Depth + (submarine.Aim * command.Units)
        },
        CommandType.Up => submarine with { Aim = submarine.Aim + command.Units },
        CommandType.Down => submarine with { Aim = submarine.Aim - command.Units },
        _ => throw new NotImplementedException(),
    };
}
```

The changes to the commands aren't too drastic, the big difference is that `CommandType.Forward` updates two properties instead of the previous one.
There's also an interesting small gotcha here which becomes evident when you output the results again.
With the provided test input we get:
```
Depth: -60
Position: 15
Result: -900
```

Our depth is now a negative value, this is because it is calculated based on the existing depth and the aim value which can be negative since it's decremented when the down command is applied.

To get the expected result we can use a handy `Math.Abs` method to get the absolute value of the Depth and Position properties which we can then multiple to get the expected result netting us another shiny ⭐.

## Summary
That was a fun challenge for day 2 and we got to use some new fancy C# 10 features to help us solve it in a clear way.
Let's see where we go next!
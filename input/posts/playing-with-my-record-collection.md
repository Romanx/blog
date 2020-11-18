Title: Playing with my record collection
Published: 18/11/2020
Tags: 
    - Code 
    - CSharp
    - New Features
Lead: Does C# 9 record equality work with collections?
---

It's been a while since my last post however this one is born from curiosity and not being able to find the answer to this particular question so hopefully it helps someone else like me.

C# 9 and .NET 5 has been released to much fanfare at .NET Conf 2020 last week which introduced a lot of lovely language features, with one of the major ones being records.

Records is a class that the compiler imbues with several value-like behaviours and generally speaking they are defined by their contents and not their identity (or reference). 
Although they can be mutable their primary use is for immutable data models.
A standard example of this kind of record is a Person class:
```cs
public record Person(string FirstName, string? LastName);
```

In this case the compiler will generate all the equality method and operators comparing instances by value so that these two instances would be equal.
```cs
Person a = new("Steve", "Rogers");
Person b = new("Steve", "Rogers");

a == b; // True
a.Equals(b); // True
a.GetHashCode() == b.GetHashCode(); // True
```

What happens however if you have a collection as part of your record? What if we change our person and instances to this:
```cs
public record Person(string FirstName, string? LastName, string[] Nicknames);

Person a = new("Steve", "Rogers", new[] { "Captain America", "Captain Rogers" });
Person b = new("Steve", "Rogers", new[] { "Captain America", "Captain Rogers" });
```

We've now added a lovely nickname field to our person, they have the exact same values and since records generally speaking are defined by their contents they should be identical right? No. I thought they would be but unfortunately this does not seem to be the case.

To see what the compiler generates we're going to use an amazing tool by [Andrey Shchekin (@ashmind)] called [sharplab.io](https://sharplab.io).
This allows us to target a specific build of the .NET compiler and see what the generated code actually looks like.
The equality checks for our modified code looks like this:
```cs
public virtual bool Equals(Person other)
{
    return other != null 
	    && EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(FirstName, other.FirstName) 
	    && EqualityComparer<string>.Default.Equals(LastName, other.LastName) 
	    && EqualityComparer<string[]>.Default.Equals(Nicknames, other.Nicknames);
}
```
The full example you can find [here.](https://sharplab.io/#v2:EYLgZgpghgLgrgJwgZwLQGMD2A3KCCWUAdjKkvAkcgDQAmIA1AD4ACATAIwCwAULywGYABEiwJaQgAoQEyTEQAULDgAYhAMXyyYAOSgBbCNSHKVAfiEAZKMl0GjJ1QG0AukJ350AayL3kASgBuXl5fQ2QAByh0CEcAOgAlOBJ8QziAYUx9CPwAGxkAZRlsTxReAG9eIWqTYXYhAElkAFEADxgZX1yGonwYcoBfXgGgA=)

The generated equality check here uses the generic `EqualityComparer<string[]>` which compares the instances by reference which does not do what we want since these are two distinct arrays with the same values.
What about if we use ImmutableArray<string>? this feels more correct since we get an immutable instance of the array which can't be modified but unfortuantely the default equality for immutable arrays is reference equality and so the same effect.

## Is there nothing we can do?
There is something we can do however it isn't as nice as I'd like. On records you can add methods and override others. 
In this case we can thank the compiler for being so helpful but override the equality and hashcode functions ourselves while leaving it to generate the properies for us.
The resulting class would look like this:
```cs
public record Person(string FirstName, string? LastName, string[] Nicknames)
{
    public virtual bool Equals(Person? other)
    {
        return other is not null
            && EqualityComparer<string>.Default.Equals(FirstName, other.FirstName)
            && EqualityComparer<string?>.Default.Equals(LastName, other.LastName)
            && Nicknames.SequenceEqual(other.Nicknames);
    }

    public override int GetHashCode()
    {
        HashCode hashcode = new();
        hashcode.Add(FirstName);
        hashcode.Add(LastName);
        foreach (var item in Nicknames)
        {
            hashcode.Add(item);
        }

        return hashcode.ToHashCode();
    }
}
```

As you can see we have overridden the equals and made sure to use SequenceEqual from System.Linq to make sure we're structurally comparing the two arrays.
We also have to override GetHashCode() since we want to add the members of the array to the hashcode so they also match as part of the equality contract.
I'm doing this using the wonderful HashCode struct added as part of .NET Core 2.1 and dotnet standard 2.1.

After these changes all the equality we expect to work, works as intended.
It's unfortunate that we have to write this code ourselves however it does act the same as how structs currently do so they are matching value type behaviour here.
Perhaps in future microsoft might change the underlying behaviour to act more like I expected it to however the nice thing about record types are that it does this for free and it can be optimised for us without adding any more complexity to our codebase.

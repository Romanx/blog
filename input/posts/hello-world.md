Title: Hello world
Published: 22/7/2018
Tags: Introduction
Lead: Everything starts with a Hello World doesn't it?
---

```csharp
public class Blog
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Is this thing on?...");
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }
}
```

I've never really written a blog and I'm not entirely sure I have anything interesting to say but I think it's good that this is here whenever I think I do.

My writing style is very *"stream of consiousness"* which may drive some of you nuts, I'll try to minimize that when writing deep dive technical posts but I'm sure it'll slip in.

My first post (at least) will be about [Stubble](https://github.com/stubbleorg/stubble) my performant cross platform renderer for the Mustache templating language.
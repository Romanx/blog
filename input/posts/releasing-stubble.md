Title: Releasing Stubble
Published: 29/7/2018
Tags: 
    - Stubble 
    - Code 
    - CSharp
Lead: After 3 years it's finally done
---

The last few posts have been talking about [Nustache](https://github.com/JDiamond/Nustache) and [Stubble](https://github.com/StubbleOrg/Stubble) the projects that I am the maintainer and in the case of Stubble the creator of.

For those of you that have come to this without any context.
Nustache is the main Mustache renderer for .NET and what the majority of people are using for rendering Mustache templates. 
Stubble is the successor to Nustache that I've been working on as it's replacement.

I've gone into why I decided to not just update Nustache and instead try replace it with a more *'modern'* alternative in [part 1](trimming-my-nustache-part-1) and in [part 2](trimming-my-nustache-part-2) I went into the guiding principles for Stubble and what my aims were for the project.

## Version 1.0
I've been putting of releasing the first major version of Stubble since i'm a perfectionist and always think there's more I could be doing.
Even while writing this release post I came up with a performance improvement.
At some point you need to let the child fly the nest and I think Stubble is in a place where it would benefit from being used in anger by others.

### Battle Testing
For those of you slightly concerned about a brand new library without any credentials.
Stubble has been used in production for the last year or so of Alpha's by my previous employer and as far as I'm aware still is.
It's also fully covered by the Mustache spec tests and in many of my tests handles edge cases with more grace than Nustache.

### Performance
Since performance was one of my core tenants of the project I'm happy to show that Stubble is better than Nustache in a scaled test using [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet).

The test was performed using the same data and templates as the Mustache.java project which is an example of rendering a twitter timeline.
Here is the output from BenchmarkDotNet.

![A graph showing stubble is faster than Nustache](/images/stubble1-0-perf.png)

``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
Intel Core i7-5820K CPU 3.30GHz (Broadwell), 1 CPU, 12 logical and 6 physical cores
  [Host]    : .NET Framework 4.6.1 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3131.0
  RyuJitX64 : .NET Framework 4.6.1 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3131.0

Job=RyuJitX64  Jit=RyuJit  Platform=X64  

|                     Method |        Mean |      Error |     StdDev | Scaled | ScaledSD |   Gen 0 |   Gen 1 | Allocated |
|--------------------------- |------------:|-----------:|-----------:|-------:|---------:|--------:|--------:|----------:|
|  TimelineBenchmark_Visitor |   733.15 us | 14.0487 us | 13.1411 us |   1.00 |     0.00 | 22.4609 |  2.9297 | 141.19 KB |
|  NustacheTimelineBenchmark | 1,211.94 us | 20.8596 us | 18.4915 us |   1.65 |     0.04 | 85.9375 | 11.7188 | 532.91 KB |
| TimelineBenchmark_Compiled |    23.36 us |  0.1990 us |  0.1861 us |   0.03 |     0.00 |  9.3079 |  1.3123 |  57.55 KB |
```

#### Compilation
It's worth noting that in the test above we only tested compilation using Stubble since Nustache was unable to compile the template.
This seemed to be due to Nustache being much stricter with properties not existing on objects that are specified in the templates.
Stubble is more lenient about this by default however it can be set to throw on values not existing.

And yes compiled templates really are that much faster than uncompiled templates.
The benchmark does not include the initial compilation just the render but if your use case doesn't require lambdas then I would recommend compiling and caching your templates.

## Whats next?
Next I have a few small projects I'd like to work on outside of Stubble,
this includes [Cake.Coverlet](https://github.com/Romanx/Cake.Coverlet) which I wrote to support Stubble since I was duplicating the same code across the projects.

I'm also planning to contribute to the Mustache community for .NET, finding places that Nustache is currently used and helping them migrate the Stubble since it'll unblock any cross platform migrations they may want to make. The first of these will likely be [docfx](https://github.com/dotnet/docfx).

As for things to add to Stubble there are a few extensions or supplimentary projects I think will be good.
  - A dotnet core global tool for a mustache renderer.
  - A Wyam Mustache template integration (which this blog uses)
  - An example helpers library with a special type of mustache tag to aid in formatting objects with the IFormattable interface.
  - A roslyn based compiler for the mustache templates.

Hopefully you've found this series interesting and if you have any feedback on the project please feel free to open github issues.

The newly released 1.0 can be found on NuGet [here.](https://www.nuget.org/packages/Stubble.Core)
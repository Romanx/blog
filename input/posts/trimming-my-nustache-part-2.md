Title: Trimming my Nustache
Published: 27/7/2018
Tags: 
    - Stubble 
    - Code 
    - CSharp
Lead: "Part 2: Shaping my Stubble"
---

So last time we spoke about why I decided to create a whole new library to replace [Nustache](https://github.com/jdiamond/nustache) rather than just updating it.
If you haven't read that blog feel free to go read it [here](trimming-my-nustache-part-1) but if you only care about [Stubble](https://github.com/stubbleorg/stubble) then read on.

> **Note:** For those of you that have come to this without any context.
Nustache is the main Mustache renderer for .NET and what the majority of people are using for rendering Mustache templates.

The list of features that I mentioned that drove the design of Stubble were partially taken from my personal requirements for a Mustache library and partially to get away from some of Nustache's issues.
I'll list them individually and break down why they were important.

### Trimmed down (ha-ha) 
I wanted only required functionality in the core library which also is where the name came from.
Extensions to this core functionality should be able to be provided via NuGet. This forced separation of concepts and was a direct extension from the *'helpers'* being baked into Nustache which I disliked.

It can be confusing for users what is standard Mustache and what is non-compliant Nustache functionality for users which I wanted to avoid by making it explict and opt-in.

### Tested, Spec Compliant, CI/CD
These all fall into the same category so I'll group them.
Nustache was tested and spec compliant when I took over but had no CI/CD to enforce that this was the case.
Stubble from the start had the spec tests auto generated from the Mustache spec files allowing me to know what still needed to be done.

It's worth noting here that Stubble is _entirely_ spec compliant including the whitespace tests which can be incredibly fiddly. 
I know this since it took me 8 months to get them working!
Many Mustache libraries ignore these tests since they only really affect layout but it was important to me I wasn't ignoring any of the tests.

### Cross platform from the start
.NET Core was just taking off with project.json and NetStandard when I started the project.
The general direction for .NET seemed to be moving this way so keeping on top of this trend seemed sensible and future proofing.

### Performance conscious
I wanted performance to not be a reason for not using Stubble. If there's a more performant way of doing something i.e. Not using a regex parser then I should be doing it.

Originally I rolled my own performance testing suite comparising Nustache and Stubble using that Mustache.java twitter test although I replaced this with the excellent [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) as the ecosystem is aligning on it as a tool and covers cases I hadn't.

This has served me well since Alexandre Mutel aka [Xoofx](http://xoofx.com) did a comparsion with their own templating engine for the Liquid templating language against other languages in the ecosystem. Stubble shows as being 2nd in their tests and 1st against other Mustache implementations.
The test isn't quite an idomatic mustache one since they have some logic to truncate in their template as a lambda however its a good test. 

You can find the blog post [here](http://xoofx.com/blog/2017/11/13/implementing-a-text-templating-language-and-engine-for-dotnet/) if you're interested in reading more.

### Template discovery
Nustache has no real way of 'finding' templates besides what is passed to its render method as the template and partials. 
I liked the [mustache.php](https://github.com/bobthecow/mustache.php/) idea of template loaders so had the same concept although out the box there is only a default implementation keeping with my trimmed down philosophy.

This allowed me to add async template discovery since my own use case at the time included loading templates from a database and blocking a thread while waiting on the DB call directly conflicted with my desire to be performant. 
Adding an async API explictly tells the user that their may be an async action occuring and it needs to be awaited as opposed to just blocking.

## So are you done yet?
It's taken me a long time to get to a stage where I'm happy to fully release Stubble since I've been tweaking it for three years now and have changed the interface multiple times.

I kept comparing it to Nustache and finding functionality I hadn't provided or *'better'* ways to do things such as configuring the renderer. I finally added a compilation renderer in the [Stubble.Compilation](https://www.nuget.org/packages/Stubble.Compilation) package so I'm happy to consider the core functionality there.

This leads to me to the final post. The release of my fast, cross-platform, and simple Mustache renderer for .NET which is **Coming Soon!**

---

Please stay tuned for this and add me to your [RSS](feed.rss) or [Atom](feed.atom) feed.
It'll have performance comparision graphs a list of final features and possible future enhancements that will come later.
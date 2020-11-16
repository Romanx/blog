Title: Stubble extensions and enhancements
Published: 16/11/2020
Tags: 
    - Code
    - CSharp
    - Stubble
    - Mustache
    - Standards
Lead: Thoughts on standards and differences
---

When building Stubble one of the things I set out to avoid was the situation that [Nustache](https://github.com/jdiamond/Nustache), the precursor to Stubble, got itself into by adding non-standard mustache functionality by default.
It could be quite easy for users to fall into using these non-standard components and then you've inadvertently made your templates pinned to the library you're using and if things get changed in future the version.

For Stubble I wanted the core library to stay as simple and "trimmed down" as possible so that your templates could be moved to another implementation without hassle.
This mainly derrived from the fact that I wanted people to migrate to Stubble easily since it was much faster than Nustache so it made sense to build things this way with extension points for opt-in functionalities.

# Standards and Specifications
When you've got specifications like mustache there is a clear contract of what a library is expected to do across different implementations. 
You can find the mustache spec [here.](https://mustache.github.io/mustache.5.html)
It's not a long or complicated spec but the main issue come with whitespace handling which many libraries opt out of doing since it doesn't change how the template looks that often.

The mustache spec also has defined tests for the spec which you can generate tests from, in fact we used this from the start of Stubble to define where we were on our journey to being a working mustache implementation.
So I think you can tell that I'm a fan overall.

There is a problem here in that sometimes people disagree with the spec or want to extend the spec and add new features that all implementations then have to match to be considered compliant to that version.
Provided that your spec and tests are versioned I personally don't have an issue. Users should be able to know which versions their template is valid for and perhaps we could even provide tooling for such situations.

# Divergence
Another part of this is management and ownership of the spec. If this isn't maintained then it can stagnate much like the mustache spec and be left to be effectively fixed at a point in time.
Mustache has been around for the long time and has evolved over time. 
At some point, prior to my involvement in the ecosystem the core mustache spec was left on version 1.1.3 in 2015 and has not changed since.
The repository on github as of posting this has 17 outstanding pull requests and 57 open issues, most inactive for years.

Many of these outstanding pull requests are adding tests for new languages (such as go), new features such as inheritance and improving definitions on edge cases that the spec exists to clarify.
Features such as inheritance you'll find in a few mustache implementations that are based on that pull request.
Since this core spec project became unmaintained the community diverged and so did the feature set meaning users can't easily know if their templates will work across implementations and languages.

# Extensibility
My thoughts on this problem solidified when working on Nustache and my attempt to fix it exists in Stubble.
I wanted an extensibility model where I could add non-base spec features in a declarative additive maner where the user understood that they were binding themselves to an implementation.
This has the downside that Stubble could be faster if it didn't have to provide all of these extensible points however I think the pros outweigh the cons.
Ideally the situation would have never occured and the community could have taken ownership of the mustache spec much like many other open source projects however stubble being from an earlier age of the internet has been left behind.

# Closing thoughts
Projects evolved and peoples lives and circumstances change. 
Things are often are replaced, die altogether, or fall into a read-only maintainence only state much like the Nustache project that I started working on.
This is only practical since most of us do open source in our spare time without corporate sponsorship so eventually something has to give and hopefully it isn't the maintainers sanity or wellbeing.

If it's possible to have a plan for your projects life going forward, even if that is marking it as read-only and effectively "finished" then it can allow people an idea of where they stand.
If you work for a company who's work is built on top of the hard work of these open source maintainers, convince your company that without these people they will directly be impacted and that they should fund or help the project.
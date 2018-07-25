Title: Trimming my Nustache
Published: 25/7/2018
Tags: 
    - Stubble 
    - Code 
    - C#
Lead: "Part 1: Forgive the title, it was too good to pass up."
---
Lets talk about [Stubble](https://github.com/stubbleorg/stubble) and why I decided not to just improve Nustache and instead undertake a rewrite.

Some backstory first.

## In the beginning there was... history?

Nustache is a project started by JDiamond and it's goal was to be a [Mustache](https://mustache.github.io/) parser and renderer for .Net.
Later on JDiamond moved on from the project leaving a notice that if anyone wanted commit access then to contact him.

I took over stewardship of the project back in 2015 after finding a bug in the library and tried to contribute a fix back which lead me to being given commit access as a maintainer.

After fixing my bug and releasing a new version I decided to apply some *'best practices'* to the project such as CI and CD and tried to clear the backlog of outstanding pull requests but didn't actually perform any *new* developments.

## Stewardship vs Progress

After some time of merging pull requests and trying to handle user requested developments I started to resent how the renderer was architected.
I was struggling to fix some problems that I had with the project partially due to my inexperience with the codebase and frankly since I disagreed with some of the decisions made and how I'd need to implement the fixes.

I decided to try progressive enhancement and slowly migrate the existing codebase into something less in my thoughts 'legacy'.
I started by tackling the regex based parser since the code was difficult to grok and I thought that it would be an isolated part of the system to replace as long as the underlying structures and classes remained the same.
The plan was to take the [mustache.js](https://github.com/janl/mustache.js) parser and convert it into C# as their implementation was concice and clear.

While starting this rewrite this I realised I was effectively removing someone elses work for no practical gain but my own.
If I was going to rewrite the parser then I should probably rewrite the renderer and then where do I stop.
I didn't want to wholesale replace the core implementation of a project that I was only stewarding.

JDiamond may decide to sometime come back and take ownership again. He may disagree with what I'd decided to do and sure he hasn't worked with it but what of the users that are currently using the library? If I change it drastically then is it really the same project?

So I wrote a list of what I wanted from a Mustache library and decided to use that as the basis of a new library that I would plan to replace Nustache.

Now we've got the backstory let's actually talk about Stubble in part 2...**(coming soon)**
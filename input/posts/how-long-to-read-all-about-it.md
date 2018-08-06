Title: How long to read all about it?
Published: 06/8/2018
Tags: 
    - Wyam
    - Extensions
    - Code 
    - C#
Lead: Calculating blog reading times for Wyam blogs
---

I decided I wanted to take a short break from the world of facial hair and indulge my exploratory side by [spelunking](https://en.wikipedia.org/wiki/Caving) into the world of [Wyam](https://wyam.io/) the library (and ecosystem) that gives this blog life.

## What is Wyam?
Wyam is a C# based static content toolkit that can be used to generate any kind of static content. 
It's built on the foundation of flexible modules that you combine together to create different kinds of outputs.
I'm currently using the Blog recipe along with the *CleanBlog* template which I've not really altered much.

If you're interested in more info I'd recommend their website [here.](https://wyam.io/)
The docs are exceptional and it's pretty easy to find your way around and configure whatever you need.
It's worth noting that the community around Wyam is great and [@daveaglick](https://twitter.com/daveaglick) the original creator is very active on Twitter so show them some love.

## So how long?
While writing my recent blog posts I found myself wondering how many words they were and how long it would likely take to read since at the time they felt like epics.
I'd seen sites such as **Medium** display an estimate of how long it would take to read so I wondered if I could integrate that into my site and at the same time learn the internals of Wyam.

I looked around the internet and found sure enough that it wasn't a complicated algorithm. 
Take the content, split it on space characters and then count the words.
With this you need to decide on how many *words per minute* your calculation should be based.
I took the total of 200 which many places proclaim to be the average but I couldn't find any source for that.
With that number you can calculate how long it would take to read by dividing the total words by the reading speed and rounding the result. I'm calculating in total seconds to read and then rounding it to the nearest minute which you can see at the top of the page.

## Wyam.ReadingTime
So what does this have to do with Wyam? I wrote a module which integrates into the generation pipeline for my blog and performs this calculation on my posts adding the resulting information to the metadata that I can use when rendering the output.

To have the metadata being generated and added to the pipeline I put the following in my `config.wyam`

```csharp
Settings[Keys.Host] = "romanx.co.uk";
Settings[BlogKeys.Title] = "Romanx";
Pipelines.AddReadingTimeMeta(wordsPerMinute: 200);
```

Then I overwrote my `_Index.cshtml` which is used as the basis for rendering my homepage to add the information to each post listing.
I simplified the output when it was in seconds because who really cares If I think it'll take 23 seconds to read a post?
```csharp
var readingMeta = post.Get<ReadingTimeMeta>(ReadingTimeKeys.ReadingTime);
<p class="post-meta">
    Posted on @(post.Get<DateTime>(BlogKeys.Published).ToLongDateString(Context))
    <br />
    Estimated read time:  @(readingMeta.Minutes == 0 ? $"a couple of seconds" : $"{readingMeta.Minutes} minutes")
</p>
```

You can find the library [here](https://github.com/Romanx/Wyam.ReadingTime) on GitHub and on nuget [here](https://www.nuget.org/packages/Wyam.ReadingTime/).
Please let me know on GitHub or on Twitter if you have any feedback.
Thanks!
Title: Adding some Coverlet to my Cake
Published: 02/12/2018
Tags: 
    - Code 
    - C#
    - Cake
    - Coverlet
Lead: Warning - Cake can be addictive
---

It's been awhile since I've posted, partially because I've not being doing much that is in a place that I want to talk about yet. Keep your eyes open for a post about the Stubble Helpers extension I'm writing in the future though.

Today I'm going to talk about one of my other open source projects that I've mentioned in one of my Stubble posts which is [Cake.Coverlet](https://github.com/Romanx/Cake.Coverlet).

If you've not heard of [Coverlet](https://github.com/tonerdo/coverlet) before, then a quick summary would be that it's an open source, cross-platform coverage tool for dotnet core. Scott Hanselman has written a better write-up here of their new global tool functionality [here](https://www.hanselman.com/blog/NETCoreCodeCoverageAsAGlobalToolWithCoverlet.aspx).

I've been using Coverlet for awhile in Stubble since [OpenCover](https://github.com/OpenCover/opencover) another great open source project had some dependencies slowing its adoption to dotnet core which lead me to using alpha builds to get any kind of coverage. Since I was using [Cake](https://cakebuild.net) as my build tool I wanted to integrate Coverlet into my experience and ideally get some strong typing with it, and so [Cake.Coverlet](https://github.com/Romanx/Cake.Coverlet) was born.

If you're using Cake then adding Cake.Coverlet is as easy as this snippet:
```cs
#addin nuget:?package=Cake.Coverlet

Task("Test")
    .IsDependentOn("Build")
    .Does<MyBuildData>((data) =>
{
    var testSettings = new DotNetCoreTestSettings {
    };

    var coveletSettings = new CoverletSettings {
        CollectCoverage = true,
        CoverletOutputFormat = CoverletOutputFormat.opencover,
        CoverletOutputDirectory = Directory(@".\coverage-results\"),
        CoverletOutputName = $"results-{DateTime.UtcNow:dd-MM-yyyy-HH-mm-ss-FFF}"
    };

    DotNetCoreTest("./test/My.Project.Tests/My.Project.Tests.csproj", testSetting, coveletSettings);
}
```

Then your tests will be run, the coverage will be displayed in the output and written to a file in the opencover format. Simples.

### Global Tools
Global tools in the dotnet world are the new hotness, even [Wyam](https://wyam.io/blog/version-2.0) has just added one in 2.0. Congrats to the team. So we had an issue opened by [@StanleyGoldman](https://github.com/Romanx/Cake.Coverlet/issues/7) asking for us to support calling the coverlet global tool rather than hooking into msbuild directly. They made a good start on this (which is always appreciated) and then we finished it off.

I took some of the enhancements suggested by the inimitable Scott Hanselman in his blog above and tried to incorporate them and what we've ended up with I think is a great way to use coverlet.

```cs
#addin nuget:?package=Cake.Coverlet

Task("Test")
    .IsDependentOn("Build")
    .Does<MyBuildData>((data) =>
{
    var coveletSettings = new CoverletSettings {
        CollectCoverage = true,
        CoverletOutputFormat = CoverletOutputFormat.opencover,
        CoverletOutputDirectory = Directory(@".\coverage-results\"),
        CoverletOutputName = $"results-{DateTime.UtcNow:dd-MM-yyyy-HH-mm-ss-FFF}"
    };

    // I want to specify the specific dll file and the project exactly.
    Coverlet(
        "./test/My.Project.Tests/bin/Debug/net46/My.Project.Tests.dll", 
        "./test/My.Project.Tests/My.Project.Tests.csproj", 
        coveletSettings);

    // I want to specify just the project file and the dll can be
    // inferred from the name of the project file.
    Coverlet(
        "./test/My.Project.Tests/My.Project.Tests.csproj", 
        coveletSettings);

    // I want to specify just the project directory, we will discover
    // any proj file in the directory (take the first) and infer the 
    // name from the found project.
    Coverlet(
        "./test/My.Project.Tests",
        coveletSettings);
}
```

#### Caveats
Now as there always is there are some caveats with how to use this. You either need to have coverlet already installed as a global tool by using `dotnet tool install --global coverlet.console` or you could [Cake.DotNetTool.Module](https://www.gep13.co.uk/blog/introducing-cake.dotnettool.module) which allows you to install dotnet tools with Cake and defers to the dotnet tool to install the cake tool instead of nuget. Gary Ewan Park explains it better in their blog post so please read that for more detail.

> Hey I heard you liked tools so I used tools to install tools to install your tools

Hopefully you enjoy this project. You can find the library here on [GitHub](https://github.com/Romanx/Cake.Coverlet) and on nuget [here](https://www.nuget.org/packages/Cake.Coverlet/). Please let me know on GitHub or on Twitter if you have any feedback. Thanks!
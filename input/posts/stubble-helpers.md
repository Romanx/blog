Title: A helping hand with your Stubble
Published: 16/11/2020
Tags: 
    - Code
    - CSharp
    - Stubble
    - Mustache
Lead: Custom made tools for your templates
---

Another post, another long delay. In my last post I teased that I'd been working on another extension to Stubble which would add [handlebars](https://handlebarsjs.com/) style helpers to your mustache templates.

This extension while simple has some fun complexity in terms of how people may want to use this in a strongly typed manner unlike handlebars. A nice simple example is this:
```csharp
var culture = new CultureInfo("en-GB");
var helpers = new Helpers()
    .Register("FormatCurrency", (HelperContext context, decimal count) => count.ToString("C", culture));

var stubble = new StubbleBuilder()
    .Configure(conf => conf.AddHelpers(helpers))
    .Build();

var result = stubble.Render("{{FormatCurrency Count}}", new { Count = 100.26m });

Assert.Equal("£100.26", result);
```

Hopefully it's clear that this helper allows the user to add their own non-standard tag-style to their templates which has custom functionality which in this case formats a decimal with a specific culture.

There's also a slightly more advanced case where you can access a slimmed version of the render context to access any variable in your object by name. This gives you more control but also more chances for things exploding at runtime.

```csharp
var helpers = new Helpers()
    .Register("PrintListWithComma", (context) => string.Join(", ", context.Lookup<int[]>("List")));

var builder = new StubbleBuilder()
    .Configure(conf => conf.AddHelpers(helpers))
    .Build();

var res = builder.Render("List: {{PrintListWithComma}}", new { List = new[] { 1, 2, 3 } });

Assert.Equal("List: 1, 2, 3", res);
```

You can also have static arguments in your template that will be passed into your helper. There are some caveats to this which I'll note below the example:
```csharp
var culture = new CultureInfo("en-GB");
var helpers = new Helpers()
    .Register("FormatCurrency", (HelperContext context, decimal count) => count.ToString("C", culture));

var stubble = new StubbleBuilder()
    .Configure(conf => conf.AddHelpers(helpers))
    .Build();

var result = stubble.Render("{{FormatCurrency 10}}", new { });

Assert.Equal("£10.00", result);
```
### Cavets
- The type should match or be convertable to the argument type.
- If you're writing a constant string as an argument then it should be escaped with quotes either `"` or `'`. Quoted strings are treated as verbatim and will not be attempted to be looked up in the context however their type will still be converted.
- If you have a quote in your string for example It's then you can escape it with a `/` like so: `It/'s`.


# Stubble's Extension Philosophy
While writing this I was going to expand on why I didn't include this in the main Stubble package along with some thoughts on creating libraries based on standards and how/where to deviate/extend from that standard.
If this doesn't interest you then thanks for reading this post, if it does then please see my post on [Stubble extensions and enhancements here.](stubble-extensions-and-enhancements)
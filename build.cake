#tool nuget:?package=Wyam
#addin nuget:?package=Cake.Wyam
#addin "Cake.Npm"

var target = Argument("target", "Default");

const string SiteId = "d6d97cef-2890-4ed4-87fe-8b0d32c34930";

Task("Build")
   .Does(() =>
   {
       Wyam(new WyamSettings
       {
           Recipe = "Blog",
           Theme = "CleanBlog",
           UpdatePackages = true
       });
   });
   
Task("Preview")
   .Does(() =>
   {
       Wyam(new WyamSettings
       {
           Recipe = "Blog",
           Theme = "CleanBlog",
           UpdatePackages = true,
           Preview = true,
           Watch = true
       });
   });

Task("Deploy")
    .IsDependentOn("Build")
    .Does(() =>
    {
        // Install the Netlify CLI locally and then run the deploy command
        NpmInstall("netlify-cli");
        StartProcess(
            MakeAbsolute(File("./node_modules/.bin/netlify.cmd")), 
            $" deploy -p -d=output");
    });

Task("Default")
   .IsDependentOn("Build");
   
RunTarget(target);
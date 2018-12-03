#tool nuget:?package=Wyam
#addin nuget:?package=Cake.Wyam
#addin "Cake.Npm"

var target = Argument("target", "Default");

const string SiteName = "gifted-rosalind-45186b";

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
            $"deploy --dir=output --site {SiteName}");
    });

Task("Default")
   .IsDependentOn("Build");
   
RunTarget(target);
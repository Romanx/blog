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
    .Does(() =>
    {
        // Add NETLIFY_TOKEN to your enviornment variables
        string token = "6cf98accd198932d8cc88d5d24c77f77c69da244a90d9dc24531940ff9b66830";//EnvironmentVariable("NETLIFY_TOKEN");
        if(string.IsNullOrEmpty(token))
        {
            throw new Exception("Could not get NETLIFY_TOKEN environment variable");
        }

        // Install the Netlify CLI locally and then run the deploy command
        NpmInstall("netlify-cli");
        StartProcess(
            MakeAbsolute(File("./node_modules/.bin/netlify.cmd")), 
            $"deploy -p output -s {SiteName} -t {token}");
    });

Task("Default")
   .IsDependentOn("Build");
   
RunTarget(target);
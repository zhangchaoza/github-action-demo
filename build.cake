#tool dotnet:?package=GitVersion.Tool&version=6.0.0
#addin nuget:?package=Cake.FileHelpers&version=7.0.0
#addin nuget:?package=Newtonsoft.Json&version=13.0.3

var targets = Arguments("target", "Default");

Task("PrintGithub")
    .Does(() =>
    {
        var isGitHubActionsBuild = GitHubActions.IsRunningOnGitHubActions;
        Information("IsGitHubActionsBuild   : {0}", isGitHubActionsBuild);

        foreach(var envVar in EnvironmentVariables())
        {
            Information("Key: {0} \tValue: \"{1}\"",envVar.Key,envVar.Value);
        }
    });

Task("Gitversion")
    .Does(() =>
    {
        if(!BuildSystem.IsLocalBuild)
        {
            // Writing version variables to $GITHUB_ENV file for 'GitHubActions'.
            GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.BuildServer });
            Information("{0}",System.IO.File.ReadAllText(Environment.GetEnvironmentVariable("GITHUB_ENV")));
        }

        GitVersion gitVersion = GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.Json });
        Information("{0}",Newtonsoft.Json.JsonConvert.SerializeObject(gitVersion,Newtonsoft.Json.Formatting.Indented));
    });

Task("Default")
    .IsDependentOn("PrintGithub")
    .IsDependentOn("Gitversion");

RunTargets(targets);
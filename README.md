# solsta-horde
Proof-of-Concept Implementation of a Horde BuildGraph Task that can be executed to upload a build from Horde to Solsta as part of a BuildGraph.
This repository comes without support and does not accept pull requests.

# Assumptions

* You are using Unreal Engine 5.5.

# How to use

Below you will find a couple of code snippets that you must paste into a few different files.

## BuildAndTestProject.xml

Location: `Engine\Build\Graph\Tasks\BuildAndTestProject.xml`
Line: 695 (Paste between <!-- Publish (Packages) (Horde) -->  and <!-- Check Build Size -->)

<!-- COPYME: Publish to Solsta -->
<Node Name="$(PlatformPublishSolstaNodeName)">
    <ForEach Name="PackageConfiguration" Values="$(PackageConfigurations)" Separator="+">
        <SolstaDeploy
            BuildToolsDirectory="C:/Tools/solsta"
            ConsoleDirectory="C:/Tools/solsta/deploy/7.2.49/console"
            ConsoleCredentials="C:/Tools/solsta/solsta_auth.json"
            Source="$(ProjectOutputDirectory)/$(StagedPlatformFolder)"
            SyncDirectory="$(ProjectOutputDirectory)/SolstaSync"
            AutoCreate="true"
            ProductName="$(PreNodeName)"
            EnvName="$(PackageConfiguration)"
            RepositoryName="$(TargetPlatform)"
            Version="$(Change)-$(EscapedBranch)"
            SyncAttributes="true"
            SyncTimestamps="true"
            Exclude="*.pdb"
        />
    </ForEach>
</Node>	

Location: Engine\Build\Graph\Tasks\BuildAndTestProject.xml
Line: 834 (Paste under <!-- Declare labels for CIS -->)

<!-- COPYME: Solsta Task -->
<Label Category="Clients" Name="$(PreLabelName)Publish to Solsta $(TargetPlatform)" Requires="$(PlatformPublishSolstaNodeName)" />

## SolstaTask.cs

Copy to `Engine\Source\Programs\AutomationTool\BuildGraph\Tasks`.

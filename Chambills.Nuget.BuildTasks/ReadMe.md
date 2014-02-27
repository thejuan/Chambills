Chambills.Nuget.BuildTasks 
=========

A collection of tasks for dealing with Nuget.

Example Target for locking all versions in a nuget package.

```XML
  <Target Name="LockDependencyVersion">

    <PropertyGroup>
      <NuSpecFile>$(TempOutputDir)\$(PackageId).nuspec</NuSpecFile>
    </PropertyGroup>

    <UnZip ZipFileName="$(PackagePath)" TargetDirectory="$(TempOutputDir)"  />

    <Delete Files="$(PackagePath)" />
    
    <ItemGroup>
      <PackageFiles Include="$(TempOutputDir)\**" />
    </ItemGroup>
    
    <LockDependencyVersions NuSpecFile="$(NuSpecFile)" />
    
    <Zip Files="@(PackageFiles)"  ZipFileName="$(PackagePath)" WorkingDirectory="$(TempOutputDir)" />

    <RemoveDir Directories="$(TempOutputDir)" />
  </Target>
  ```

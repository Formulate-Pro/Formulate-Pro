# NuGet

To create the nupkg file, run these commands (after building the solution in release mode):

```
cd "C:\r\Formulate-Pro\src\Formulate.Pro"
"C:\r\Formulate-Pro\src\packages\NuGet.CommandLine.5.5.1\tools\NuGet.exe" pack Formulate.Pro.nuspec
```

That assumes the repo has been cloned to C:\r\Formulate-Pro.

Then you can push the package to NuGet with this command:

```
"C:\r\Formulate-Pro\src\packages\NuGet.CommandLine.5.5.1\tools\NuGet.exe" push Formulate.Pro.1.0.2.nupkg some-long-key-here -Source https://api.nuget.org/v3/index.json
```

Making sure to replace the version number and key.

# Build

Before creating the NuGet package, you'll need to build a few things:

* Prepare the frontend with `npm i`.
* Rebuild the Visual Studio solution with "Release" selected. This will automatically cause `npm run build` to run.

# NuGet

Once you have built everything, you can create the nupkg file by running the following commands:

```
cd /d "d:\r\Formulate-Pro\src\Formulate.Pro"
"d:\r\Formulate-Pro\src\packages\NuGet.CommandLine.5.5.1\tools\NuGet.exe" pack Formulate.Pro.nuspec
```

That assumes the repo has been cloned to d:\r\Formulate-Pro.

Then you can push the package to NuGet with this command:

```
"d:\r\Formulate-Pro\src\packages\NuGet.CommandLine.5.5.1\tools\NuGet.exe" push Formulate.Pro.1.0.2.nupkg some-long-key-here -Source https://api.nuget.org/v3/index.json
```

Making sure to replace the version number and key.
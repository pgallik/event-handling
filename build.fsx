#r "paket:
version 7.0.2
framework: net6.0
source https://api.nuget.org/v3/index.json
nuget Be.Vlaanderen.Basisregisters.Build.Pipeline 6.0.3 //"

#load "packages/Be.Vlaanderen.Basisregisters.Build.Pipeline/Content/build-generic.fsx"

open Fake.Core
open Fake.Core.TargetOperators
open ``Build-generic``

let assemblyVersionNumber = (sprintf "%s.0")
let nugetVersionNumber = (sprintf "%s")

let buildSource = build assemblyVersionNumber
let publishSource = publish assemblyVersionNumber
let pack = packSolution nugetVersionNumber

supportedRuntimeIdentifiers <- [ "linux-x64" ]

// Library ------------------------------------------------------------------------
Target.create "Lib_Build" (fun _ ->
    buildSource "Be.Vlaanderen.Basisregisters.EventHandling"
    buildSource "Be.Vlaanderen.Basisregisters.EventHandling.Autofac"
    buildSource "Be.Vlaanderen.Basisregisters.EventHandling.Microsoft"
)

Target.create "Lib_Publish" (fun _ ->
    publishSource "Be.Vlaanderen.Basisregisters.EventHandling"
    publishSource "Be.Vlaanderen.Basisregisters.EventHandling.Autofac"
    publishSource "Be.Vlaanderen.Basisregisters.EventHandling.Microsoft"
)

Target.create "Lib_Pack" (fun _ -> pack "Be.Vlaanderen.Basisregisters.EventHandling")

// --------------------------------------------------------------------------------
Target.create "PublishAll" ignore
Target.create "PackageAll" ignore

// Publish ends up with artifacts in the build folder
"DotNetCli"
==> "Clean"
==> "Restore"
==> "Lib_Build"
==> "Lib_Publish"
==> "PublishAll"

// Package ends up with local NuGet packages
"PublishAll"
==> "Lib_Pack"
==> "PackageAll"

Target.runOrDefault "Lib_Build"

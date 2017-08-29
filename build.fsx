// --------------------------------------------------------------------------------------
// FAKE build script
//
// This is a plain F# script which is executed by the FAKE build runner
// 
// NOTE this will contain compiler errors until after paket has restored the FAKE packages
// --------------------------------------------------------------------------------------

// load the tools needed by the script
#I @"packages/FAKE/tools/"    
#r @"FakeLib.dll"

// open the namespaces
open Fake
open System
open System.IO

// set the current directory to the root directory
Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

// Project info (used in AssemblyInfo files)
let project = "XLCatlin.DataLab.XCBRA"
let summary = "Component of XLCatlin.DataLab.XCBRA"
let company = "XL Catlin"
let copyright = "Copyright \u00A9 XL Catlin 2017"

// Setup global properties for the build
let solutionFile = "Dazzle.sln"
let temporaryBuildPath = // these are the locations used by .NET projects
    !! "src/**/bin"      // "!!" is the FAKE symbol for "all files matching this pattern"
    ++ "src/**/obj"      // "--" is the FAKE symbol for "and also all files matching this pattern"
    ++ "test/**/bin" 
    ++ "test/**/obj"  

let buildDir = "./bin/"
let benchmarkBinDir = "./binBenchmark/"

// Pattern specifying assemblies to be tested 
let expectoAssemblies =
    !! "test/**/bin/Release/*Test*.exe"   // "!!" is the FAKE symbol for "all files matching this pattern"

// Pattern specifying assemblies to be tested 
let fastNunitAssemblies =
    !! "test/**/bin/Release/*Test*.dll"   // "!!" is the FAKE symbol for "all files matching this pattern"
    -- "test/*DbTest*/**"                 // "--" is the FAKE symbol for "excluding all files matching this pattern"
    -- "test/*benchmark*/**"              // "--" is the FAKE symbol for "excluding all files matching this pattern"

// db tests
let dbTestAssemblies =
    !! "test/**/bin/Release/*DbTest*.dll"   // "!!" is the FAKE symbol for "all files matching this pattern"
    -- "test/*benchmark*/**"                // "--" is the FAKE symbol for "excluding all files matching this pattern"

// Pattern specifying assemblies to be benchmarked
let benchmarkAssemblies = 
    !! "src/**/bin/Release/*Benchmark*.exe"


let isCiBuild = hasBuildParam "ciBuild"


// setup local database connection strings. These are needed for the type providers to work at compile time
// when not running under CI, these are "Dazzle" and "DazzleTest"
// when running under CI, these are "Dazzle-ci" and "DazzleTest-ci"
let dbPostFix = if isCiBuild then "-ci" else ""
let localDbConnection = sprintf "Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;Database=Dazzle%s;Pooling=False;Application Name=Dazzle" dbPostFix
let localTestDbConnection = sprintf "Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;Database=DazzleTest%s;Pooling=False;Application Name=Dazzle" dbPostFix


// ------------------------------------------------------------------------
// Create release notes
// ------------------------------------------------------------------------

open Fake.ReleaseNotesHelper

// Read additional information from the release notes document
let release = LoadReleaseNotes "RELEASE_NOTES.md"

// ------------------------------------------------------------------------
// BUILD TARGET: "AssemblyInfo"
//
// Generate assembly info files with the correct version & up-to-date information
// ------------------------------------------------------------------------

open Fake.AssemblyInfoFile

// Helper active pattern for project types
let (|Fsproj|Csproj|Vbproj|) (projFileName:string) = 
    match projFileName with
    | f when f.EndsWith("fsproj") -> Fsproj
    | f when f.EndsWith("csproj") -> Csproj
    | f when f.EndsWith("vbproj") -> Vbproj
    | _                           -> failwith (sprintf "Project file %s not supported. Unknown project type." projFileName)


Target "AssemblyInfo" (fun _ ->
    let getAssemblyInfoAttributes projectName =
        [ Attribute.Title (projectName)
          Attribute.Product project
          Attribute.Description summary
          Attribute.Version release.AssemblyVersion
          Attribute.FileVersion release.AssemblyVersion
          Attribute.Company company
          Attribute.Copyright copyright
          Attribute.ComVisible false ]

    let getProjectDetails projectPath =
        let projectName = Path.GetFileNameWithoutExtension(projectPath)
        ( projectPath, 
          Path.GetDirectoryName(projectPath),
          (getAssemblyInfoAttributes projectName)
        )

    !! "src/**/*.??proj"
    |> Seq.map getProjectDetails
    |> Seq.iter (fun (projFileName, folderName, attributes) ->
        match projFileName with
        | Fsproj -> CreateFSharpAssemblyInfo (folderName @@ "AssemblyInfo.fs") attributes
        | Csproj -> CreateCSharpAssemblyInfo ((folderName @@ "Properties") @@ "AssemblyInfo.cs") attributes
        | Vbproj -> CreateVisualBasicAssemblyInfo ((folderName @@ "My Project") @@ "AssemblyInfo.vb") attributes))

// ------------------------------------------------------------------------
// BUILD TARGET: "Clean"
//
// remove all the files in 
//    * the temporaryBuildPath
//    * the buildDir
//    * the benchmarkBinDir
// ------------------------------------------------------------------------
Target "Clean" (fun _ -> CleanDirs ((temporaryBuildPath |> Seq.toList) @ [ buildDir; benchmarkBinDir ]))


// ------------------------------------------------------------------------
// BUILD TARGET: "BuildSolution"
//
// Builds all projects in the solution
// ------------------------------------------------------------------------

let defaultBuildConfiguration = "CiBuild"

// a flag to set for which projects are included in the build
let buildConfiguration = 
    let config =
        match getBuildParamOrDefault "config" "CiBuild" with
        | "DazzleOnly" -> "DazzleOnly"
        // default
        | _ -> defaultBuildConfiguration 
    trace (sprintf "Setting BuildConfiguration to %s" config)
    config

// build with minimal logging
let minimalReleaseRebuild config = 
    MSBuildHelper.build (fun defaults -> 
        { defaults with Verbosity = Some MSBuildVerbosity.Minimal; Targets = [ "Rebuild" ]; Properties = [ "Configuration", buildConfiguration ] })

// build with verbose logging
let detailedReleaseRebuild config = 
    MSBuildHelper.build (fun defaults -> 
        { defaults with Verbosity = Some MSBuildVerbosity.Detailed ; Targets = [ "Rebuild" ]; Properties = [ "Configuration", buildConfiguration ] })

Target "BuildSolution" (fun _ -> solutionFile |> minimalReleaseRebuild buildConfiguration)

// ------------------------------------------------------------------------
// BUILD TARGET: "CopyBinaries"
//
// Copies binaries from default VS location to expected bin folder
// But keeps a subdirectory structure for each project in the 
// src folder to support multiple project outputs
// ------------------------------------------------------------------------

Target "CopyBinaries" (fun _ ->
    !! "src/**/*.??proj"
    |> Seq.choose (fun f -> 
        let fromDir = Path.GetDirectoryName f @@ @"bin/Release" 
        let toDir = buildDir @@ Path.GetFileNameWithoutExtension f
        if Directory.Exists fromDir then
            // some directories might not have been included in the build config
            Some (fromDir,toDir)
        else
            None
        )
    |> Seq.iter (fun (fromDir, toDir) -> CopyDir toDir fromDir (fun _ -> true)))

// ------------------------------------------------------------------------
// BUILD TARGET: "RunFastTests"
// 
// Run only fast tests (non-database,non performance) using test runner
// ------------------------------------------------------------------------

open Fake.Testing
open Fake.Testing.Expecto

Target "RunFastTests" (fun _ ->
    expectoAssemblies
    |> (fun files -> 
        trace (files |> Seq.toList |> sprintf "Running tests: %A" )
        files)
    |> Expecto (fun p -> 
        { p with
            Parallel = false
            Summary = true
            FailOnFocusedTests = true
            })

    //fastNunitAssemblies
    //|> fun a -> printfn "setting %A" a; a
    //|> NUnit3 (fun p -> 
    //     { p with
    //        ShadowCopy = false
    //        Where = "(cat != database) and (cat != performance)" 
    //        TimeOut = TimeSpan.FromMinutes 20.
    //        ResultSpecs = ["FastTestResults.xml"] })
    )

// ------------------------------------------------------------------------
// BUILD TARGET: "RunDbTests"
//
// Run database tests only
// ------------------------------------------------------------------------

Target "RunDbTests" (fun _ ->
    dbTestAssemblies
    |> NUnit3 (fun p -> 
         { p with
            ShadowCopy = false
            Where = "(cat == database)" 
            TimeOut = TimeSpan.FromMinutes 20.
            ResultSpecs = ["DbTestResults.xml"] })
    )

// ------------------------------------------------------------------------
// BUILD TARGET: "RunPerformanceTests"
//
// Run performance tests only
// ------------------------------------------------------------------------

Target "RunPerformanceTests" (fun _ ->
    benchmarkAssemblies
    |> NUnit3 (fun p -> 
         { p with
            ShadowCopy = false
            Where = "(cat == performance)" 
            TimeOut = TimeSpan.FromMinutes 20.
            ResultSpecs = ["PerformanceTestResults.xml"] })
    )

// ------------------------------------------------------------------------
// BUILD TARGET: "RunBenchmarks"
// ------------------------------------------------------------------------

// return the directory to run the benchmarks in
let benchmarkExecDir assemblyPath = 
    let shortName = Path.GetFileNameWithoutExtension(assemblyPath)
    let timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHHmmss")
    let execDirectory = Path.Combine(benchmarkBinDir,shortName,timestamp) 
    Directory.CreateDirectory(execDirectory) |> ignore // ensure it exists
    execDirectory 

// return the directory to store the benchmarks
let benchmarkReportDir assemblyPath = 
    let shortName = Path.GetFileNameWithoutExtension(assemblyPath)
    let timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHHmmss")
    let reportDirectory = Path.Combine("benchmarks",shortName,timestamp) 
    Directory.CreateDirectory(reportDirectory) |> ignore // ensure it exists
    reportDirectory 

let copyBenchmarkBinaries assemblyPath execDirectory = 
    let srcDir = Path.GetDirectoryName assemblyPath 
    CopyDir srcDir execDirectory (fun _ -> true)

let copyBenchmarkReports execDirectory reportDirectory = 
    trace (sprintf "Copying benchmark reports from %s to: %s" execDirectory reportDirectory)
    let mdFiles = Directory.EnumerateFiles(execDirectory,"*.md")  // there's probably a better way to do this in FAKE
    let txtFiles = Directory.EnumerateFiles(execDirectory,"*.txt")
    CopyFiles reportDirectory mdFiles 
    CopyFiles reportDirectory txtFiles 

Target "RunBenchmarks" (fun _ ->
    benchmarkAssemblies
    |> Seq.iter (fun assemblyPath -> 
        let execDirectory = benchmarkExecDir assemblyPath
        copyBenchmarkBinaries assemblyPath execDirectory 
        if Shell.Exec(assemblyPath,dir=execDirectory) = 0 then
            let reportDirectory = benchmarkReportDir assemblyPath 
            copyBenchmarkReports execDirectory reportDirectory 
        )
    )

// ------------------------------------------------------------------------
// BUILD TARGET: "CleanDb"
// BUILD TARGET: "BuildDb"
//
// Clean the DB project and build the DB project
// --------------------------------------------------------------------------------------

// Database related
let DazzleDbPath = @"data/Dazzle.Database/"
let DazzleDbName = "Dazzle.Database"
let DazzleDbProj = DazzleDbPath + DazzleDbName + ".sqlproj"
let DazzleDbDacPac = DazzleDbPath + @"bin/Release/" + DazzleDbName + ".dacpac"

Target "CleanDb" (fun _ ->
    let binPath = DazzleDbPath + @"bin/Release"
    CleanDirs [ binPath ])

Target "BuildDb" (fun _ -> DazzleDbProj |> minimalReleaseRebuild defaultBuildConfiguration)

// ------------------------------------------------------------------------
// BUILD TARGET: "DeployLocalDb"
// BUILD TARGET: "DeployLocalDbTest"
//
// Deploy the DB project to the local database server
// --------------------------------------------------------------------------------------

open Fake.Sql.DacPac

// Configuration
let transformConnectionStrings path connectionStrings =
    path
    |> Seq.collect(fun config -> connectionStrings |> Seq.map(fun conn -> config, conn))
    |> Seq.iter(fun (config, (connectionKey, connectionString)) ->
        printfn "setting %s <- \"%s\" in %s" connectionKey connectionString config
        try updateConnectionString connectionKey connectionString config
        with _ -> ())

/// Update the connection strings in the app.config, but only for projects under /src/
let transformCompiledConnectionStrings = transformConnectionStrings !!("src/**/app.config")
let transformTestConnectionStrings = transformConnectionStrings !!("test/**/app.config")

let deployDbTo destination =
    deployDb(fun args ->
        { args with
            RecreateDb = true
            Source = currentDirectory </> DazzleDbDacPac
            Destination = destination })

/// the database for local development + compile
Target "DeployLocalDb" (fun _ ->
    transformCompiledConnectionStrings [ "DazzleDb", localDbConnection ]
    deployDbTo localDbConnection)

/// the database for local unit tests
Target "DeployLocalDbTest" (fun _ ->
    transformTestConnectionStrings [ "DazzleDb", localTestDbConnection ]
    deployDbTo localTestDbConnection)

// ------------------------------------------------------------------------
// BUILD TARGET: "CopyDbBinaries"
//
// Copies DB binaries from default VS location to expected bin folder
// But keeps a subdirectory structure for each project in the 
// src folder to support multiple project outputs
// ------------------------------------------------------------------------

Target "CopyDbBinaries" (fun _ ->
    !! (DazzleDbPath + "*.sqlproj")
    |> Seq.map (fun f ->
        let destinationRoot = @"bin/Release"
        Path.GetDirectoryName f @@ destinationRoot, buildDir @@ Path.GetFileNameWithoutExtension f)
    |> Seq.iter (fun (fromDir, toDir) -> CopyDir toDir fromDir (fun _ -> true)))


// ------------------------------------------------------------------------
// BUILD TARGET: "Build"
//
// build + fast tests. The default target for development
// ------------------------------------------------------------------------

// a placeholder for the chain of build tasks
Target "Build" DoNothing  

// Build chain. Start with target "Clean", then target "CleanDb", etc, ending with "Build"
"Clean"
    ==> "CleanDb"
    ==> "AssemblyInfo" 
//    ==> "BuildDb"
//    ==> "DeployLocalDb"    // must be done before BuildSolution so that type providers work
//    ==> "DeployLocalDbTest"
    ==> "BuildSolution"
    ==> "RunFastTests"
    ==> "CopyBinaries"
//    ==> "CopyDbBinaries"
    ==> "Build"


// ------------------------------------------------------------------------
// BUILD TARGET: "DbTests"
//
// build + test + with extra database tests
// ------------------------------------------------------------------------

// a placeholder for the chain of build tasks
Target "DbTests" DoNothing

// DbTests chain. Start with target "RunFastTests", then target "RunDbTests", etc.
"RunFastTests"
    ==> "RunDbTests"
    ==> "DbTests"


// ------------------------------------------------------------------------
// BUILD TARGET: "PerfTests"
//
// build + test + with extra performance tests
// ------------------------------------------------------------------------

// a placeholder for the chain of build tasks
Target "PerfTests" DoNothing

// PerfTests chain
"RunFastTests"
    ==> "RunPerformanceTests"
    ==> "PerfTests"


// ------------------------------------------------------------------------
// BUILD TARGET: "AllTests"
//
// build + all tests
// ------------------------------------------------------------------------

// a placeholder for the chain of build tasks
Target "AllTests" DoNothing

// AllTests chain
"Build"
    ==> "PerfTests"
    ==> "DbTests"
    ==> "AllTests"

// ------------------------------------------------------------------------
// DEFAULT BUILD TARGET: "Build"
//
// Run all targets by default. Invoke 'build <Target>' on command to override
// ------------------------------------------------------------------------

RunTargetOrDefault "Build"
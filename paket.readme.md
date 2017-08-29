# How Paket works

Paket is an alternative to Nuget developed by the F# community  -- see https://fsprojects.github.io/Paket/

The executable `paket.exe` lives in the `.paket` subdirectory. If missing, the latest version is downloaded using the `paket.bootstrapper.exe` executable.

There are two sets of files used by paket:

* global files (in the root directory): 
   * `paket.dependencies` contains the list of all packages used by *every* project in the solution
   * `paket.lock` contains the explicit versions used, so that a `paket restore` always restores the identical versions.
* per-project file (in the project source directory):    
   * `paket.references` contains the list of all packages used by this specific project. This is used by paket to update the `.proj` file for the project.
   
## To restore the packages for a build

* Run `.paket\paket.exe restore`   

## To add a new package not used by any project

* Add the package to the global `paket.dependencies`
* Add the package to `paket.references` of each project that uses it
* Run `.paket\paket.exe install`   
* This will also update the `.proj` files to reference the new package

## To add a new package already used by another project

* It should already be in the global `paket.dependencies`
* Add the package to `paket.references` of each project that uses it
* Run `.paket\paket.exe install`   
* This will also update the `.proj` files to reference the new package

## To remove a package 

* Remove from the global `paket.dependencies` (if no other project uses it)
* Remove the package to `paket.references` of each project 
* Run `.paket\paket.exe install`   
* This will also update the `.proj` files to dereference the removed package

## To update the lock file to use latest version of all packages

* Run `.paket\paket.exe update`   
# Savanna.SqlDb

This project contains:

* Definitions of SQL tables, view, etc. and scripts for deploying them.
* Implementation of IEventStore, IReadModelStore, etc, backed by a SQL Server database

See the README for Savanna.Core for more information on how this fits in with the other projects.

## Deployment to development environment

This project uses the Fsharp.Data.Sql type provider, so the database must
be present and correct before the project is compiled. 

* Set the development database connection in app.config
* Run `fsi devdeploy.fsx` (Windows) or `mono fsharpi devdeploy.fsx` (*nix)
  to create the database tables.
  See the comments in devdeploy.fsx for more information.
* Only then should the project be compiled, because the type provider needs the database
  to exist!

WARNING - all existing data will be destroyed. This is for
development only, not production or migration!

## Deployment to production environment

TODO Migration without losing data

## Platform notes

This project uses the type provider and so only works under .NET and Mono.
It does not yet work under .NET Core. See [this issue](https://github.com/Microsoft/visualfsharp/issues/2406).


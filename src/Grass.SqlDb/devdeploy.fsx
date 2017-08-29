// Create all tables, views, etc in the database.
//
// WARNING - all existing data will be destroyed. This is for
// development only, not production or migration!
//
// First, existing constraints and indexes are dropped
// then tables and views and procs are deployed (dropped then recreated)
// the cross-table constraints and FKs are recreated last.
//
// * Each file must have a suffix of .Table, .View, .Proc, .Constraint so that
// this script can process them in order.
//
// * Uniqueness Constraints and PKs should be included in the `Table` files.
// * Cross table constraints (FKs etc) should be included in seperate `Constraint` files.

#r "System.Data"
#r "System.Xml"
#r "System.Xml.Linq"

System.Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

open System
open System.Data
open System.Data.SqlClient
open System.Xml
open System.Xml.XPath
open System.Xml.Linq

let log msg = 
    printfn "%s" msg

let readConnectionString() =
    let appSettingsDoc = System.Xml.XmlDocument()
    appSettingsDoc.Load("App.config")
    let xpath = "//connectionStrings/add[@name = 'Dazzle']"
    let element = appSettingsDoc.SelectSingleNode(xpath) :?> XmlElement
    element.Attributes.["connectionString"].InnerText 

let usingConnection f =
    let connString = readConnectionString()
    use conn = new SqlConnection(connString)
    conn.Open()
    try
        f conn
    finally
        conn.Close()

let executeScalar cmdText = 
    usingConnection (fun conn -> 
        use cmd = new SqlCommand(cmdText)
        cmd.Connection <- conn
        cmd.ExecuteScalar() 
        )

    (*
    let test() =
        let result = executeScalar "SELECT GETDATE() as Date"
        let result' = result :?> System.DateTime
        result'.TimeOfDay 
    test()
    *)

let executeNonQuery cmdText = 
    usingConnection (fun conn -> 
        use cmd = new SqlCommand(cmdText)
        cmd.Connection <- conn
        cmd.ExecuteNonQuery() 
        )

/// execute a DDL command 
/// Return true for success, false for failure.
let executeCommand cmdText =
    try
        executeNonQuery cmdText |> ignore
        log "...OK" 
        true
    with
    | ex -> 
        log ("..." + ex.Message)
        false

/// all the schemas owned by dazzle
let dazzleSchemas = ["dbo";"dazzle"]

/// Attempt to drop all constraints. 
/// Return the number of errors
let dropConstraints() =
    let cmdTextTemplate = """
DECLARE @sql NVARCHAR(MAX);
SET @sql = N'';

SELECT @sql = @sql + N'
  ALTER TABLE ' + QUOTENAME(s.name) + N'.'
  + QUOTENAME(t.name) + N' DROP CONSTRAINT '
  + QUOTENAME(c.name) + ';'
FROM sys.objects AS c
INNER JOIN sys.tables AS t
	ON c.parent_object_id = t.[object_id]
INNER JOIN sys.schemas AS s 
	ON t.[schema_id] = s.[schema_id]
WHERE c.[type] IN ('D','C','F','PK','UQ')
AND s.[name] IN ({0})
ORDER BY c.[type]; -- drop FKs before PKs and UKs

PRINT @sql;
EXEC sys.sp_executesql @sql;
"""         
    let dazzleSchemasText = 
        dazzleSchemas 
        |> List.map (fun s1 -> "'" + s1 + "'")
        |> List.reduce (fun s1 s2 -> s1 + "," + s2)

    // use .NET format strings to make substitution easier.
    let cmdText = String.Format(cmdTextTemplate,dazzleSchemasText)
    if executeCommand cmdText then 0 else 1

// test
// dropConstraints()


let deployOne scriptFile = 
    let cmdText = IO.File.ReadAllText(scriptFile)
    log (sprintf "Deploying %s" scriptFile)
    executeCommand cmdText 

/// Attempt to deploy all the files. Return the number of errors
let deployFiles files =
    files
    |> Seq.map deployOne
    |> Seq.filter not // invert and filter
    |> Seq.length

/// Attempt to deploy all the tables. Return the number of errors
let deployTables() =
    IO.Directory.EnumerateFiles(".","*.Table.sql")
    |> deployFiles 

let deployViews() =
    IO.Directory.EnumerateFiles(".","*.View.sql")
    |> deployFiles 

let deployProcs() =
    IO.Directory.EnumerateFiles(".","*.Proc.sql")
    |> deployFiles 
        
let deployConstraints() =
    IO.Directory.EnumerateFiles(".","*.Constraint.sql")
    |> deployFiles 


let deployAll() =
    [
    dropConstraints
    deployTables
    deployViews
    deployProcs
    deployConstraints
    ] 
    |> List.sumBy (fun f -> f() )
    |> printfn "Number of errors: %i"

deployAll() 
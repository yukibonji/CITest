open Expecto

[<EntryPoint>]
let main argv = 
    let seriesConfig = 
        { defaultConfig with 
            ``parallel`` = false // makes debugging easier
        }  
    runTestsInAssembly seriesConfig argv 
    

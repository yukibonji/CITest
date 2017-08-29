module XLCatlin.DataLab.XCBRA.ApiSerialization

// Define the serialization to use so that the test clients use the same one
//let serialize o = Newtonsoft.Json.JsonConvert.SerializeObject o
//let inline deserialize<'t> json = Newtonsoft.Json.JsonConvert.DeserializeObject<'t> json

let serialize o = 
    Microsoft.FSharpLu.Json.Compact.serialize o

let inline deserialize< ^t> json = 
    Microsoft.FSharpLu.Json.Compact.deserialize< ^t> json


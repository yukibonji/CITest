namespace XLCatlin.DataLab.XCBRA.GraphQL

// ==========================================
// GraphQL implementations for the Buildings in the domain model
// ==========================================

open GraphQL.Types
open XLCatlin.DataLab.XCBRA.DomainModel

type BuildingSummaryType() as this = 
    inherit ObjectGraphType<BuildingSummary>()
    do
        this.Field("Id",fun x -> x.id.Value).Description("The Id of the Building.") |> ignore 
        this.Field("LocationId",fun x -> x.locationId.Value).Description("The Id of the Location.") |> ignore 
        this.Field("Name",(fun x -> x.name), nullable=true).Description("The description of the Building.") |> ignore
    

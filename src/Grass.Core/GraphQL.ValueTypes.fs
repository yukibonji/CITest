namespace XLCatlin.DataLab.XCBRA.GraphQL

// ==========================================
// GraphQL implementations for the common ValueTypes in the domain model
// ==========================================

open GraphQL.Types
open XLCatlin.DataLab.XCBRA.DomainModel

type AddressType() as this = 
    inherit ObjectGraphType<Address>()
    do
        this.Name <- "AddressType"
        this.Field("PostalCountry",expression=(fun x -> x.postalCountry)).Description("PostalCountry") |> ignore 
        //this.Field<<string>>("AddressLines", resolve = (fun context -> context.Source.AddressLines :> obj)) |> ignore
        //this.Field(fun x -> x.AddressLines).Description("AddressLines") |> ignore 
        this.Field("AdministrativeArea",fun x -> x.administrativeArea).Description("AdministrativeArea") |> ignore 
        this.Field("Locality",fun x -> x.locality).Description("Locality") |> ignore 
        this.Field("DependentLocality",(fun x -> x.dependentLocality |> Option.toObj), nullable=true).Description("DependentLocality") |> ignore
        this.Field("PostalCode",expression=(fun x -> x.postalCode)).Description("PostalCode") |> ignore 
        this.Field("SortingCode",expression=(fun x -> x.sortingCode |> Option.toObj), nullable=true).Description("SortingCode") |> ignore
        this.Field("LanguageCode",expression=(fun x -> x.languageCode |> Option.toObj), nullable=true).Description("LanguageCode") |> ignore


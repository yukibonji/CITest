namespace XLCatlin.DataLab.XCBRA.DomainModel


// -- Domain type --------------------------------------------------------------

/// Locations are groupings buildings that are the insurable asset of a client.
type  Location =
    { id : LocationId
    ; name : string
    ; description : Option<string>
    ; address : Address
    ; geocode : Option<Coordinate>
    ; siteCondition : Option<QualitativeRating>
    ; plantLayout : Option<QualitativeRating>
    ; ownership : Option<Ownership>
    ; lastSurveyDate : Option<System.DateTime>
    }



// -- Read Model ---------------------------------------------------------------

/// Filters that can be used when searching for locations
type LocationFilter =
    { country : List<Country>
    ; searchString : Option<string>
    ; searchArea : Option<Coordinate * decimal>
    ; ownership : List<Ownership>
    ; occupancy : List<Occupancy>
    ; totalArea : Range<decimal>
    ; totalInsuredValue : Currency * Range<decimal>
    ; siteCondition : List<QualitativeRating>
    ; plantLayout : List<QualitativeRating>
    ; lastSurveyDate : Option<System.DateTime>
    ; favouritesOnly : bool
    }


[<RequireQualifiedAccess>]
[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>] 
module LocationFilter = 

    let empty = 
        { country = [] 
        ; searchString = None 
        ; searchArea  = None
        ; ownership = [] 
        ; occupancy  = []
        ; totalArea = Range.empty
        ; totalInsuredValue = (Currency.usd, Range.empty)
        ; siteCondition = []
        ; plantLayout = []
        ; lastSurveyDate = None
        ; favouritesOnly = false
        }

/// The result of a location search
type LocationSummary = 
    { id : LocationId
    ; name : string
    ; description : string 
    ; address : Address
    ; isFavourite : bool
    }

 
// -- Commands -----------------------------------------------------------------

/// TODO: split this out into semantically meaningful events
/// This requires us to understand e.g. the situations in which an
/// engineer would revisit his assessment of site condition.
/// My initial thought is that this should be phrased in terms of the 
/// responses to engineering recommendations 
type LocationCommand =
    | Create of locationId : LocationId * name : string * address : Address * geocode : Option<Coordinate>
    | UpdateName of locationId : LocationId * name : string
    | UpdateDescription of locationId : LocationId * description : string
    | UpdateAddress of locationId : LocationId * address : Address * geocode : Coordinate option
    /// TODO: what the reasons why we would upgrade or downgrade a site's 
    /// reported condition etc. ?
    | UpdateSiteCondition of locationId : LocationId * siteCondition : QualitativeRating
    | UpdatePlantLayout of locationId : LocationId * plantLayout : QualitativeRating
    | UpdateOwnership of locationId : LocationId * ownership : Ownership 
    | UpdateLastSurveyDate of locationId : LocationId * surveryDate : System.DateTime

       
// ==========================
// Events
// ==========================

type LocationEvent 
    = Created of locationId : LocationId * name : string * address : Address * geocode : Option<Coordinate>
    | NameUpdated  of locationId : LocationId * name : string
    | DescriptionUpdated  of locationId : LocationId * description : string
    | AddressUpdated  of locationId : LocationId * address : Address * geocode : Coordinate option
    | SiteConditionUpdated of locationId : LocationId * siteCondition : QualitativeRating
    | PlantLayoutUpdated of locationId : LocationId * plantLayout : QualitativeRating
    | OwnershipUpdated of locationId : LocationId * ownership : Ownership 
    | LastSurveyDateUpdated of locationId : LocationId * surveryDate : System.DateTime

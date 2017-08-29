namespace XLCatlin.DataLab.XCBRA.DomainModel


// -- Building Properties ------------------------------------------------------

type Occupancy = 
    | Warehouse 
    // might be more later

type Construction 
    = FireResistant
    | LightNonCombustible
    | LimitedCombustible
    | Combustible

type Ownership =
    | Owned
    | OwnedOccupant
    | Leased
    | Tenant


// -- Building interface -------------------------------------------------------

type Building<'userInput> = 
    { id : BuildingId
    ; locationId : LocationId
    ; name : string 
    ; occupancy : Occupancy
    ; userInput : 'userInput
    }


// -- ReadModel ----------------------------------------------------------------


/// Filters that can be used when searching for buildings
type BuildingFilter = 
    { locationId : LocationId option
    ; name : string option
    ; occupancy : List<Occupancy>
    }

type BuildingSummary = Building<unit>
    



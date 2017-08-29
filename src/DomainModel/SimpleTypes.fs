
namespace XLCatlin.DataLab.XCBRA.DomainModel
// -- Simple Types -------------------------------------------------------------
// Simple (non-compound) types such as identifiers and enums 
// that are shared across more than one entity.
// Types that are used only by one entity/aggregate are stored in that file.



// ----------------------------------------------------------------------------
// Identifiers

type BuildingId = BuildingId of string   
    with
    member this.Value = (this |> function BuildingId id -> id)

type LocationId = LocationId of string
    with
    member this.Value = (this |> function LocationId id -> id)

type Year = Year of int 
    with
    member this.Value = (this |> function Year id -> id)

type CurrencyCode = CurrencyCode of string
    with
    member this.Value = (this |> function CurrencyCode id -> id)

type DazzleCalculationId = DazzleCalculationId of string   
    with
    member this.Value = (this |> function DazzleCalculationId id -> id)


// ----------------------------------------------------------------------------
// Measures

type [<Measure>] Metre
type [<Measure>] Ms
type [<Measure>] Minute
type [<Measure>] Percent


type Distance = decimal<Metre>
type Area = decimal<Metre^2>
type DurationInMs = int<Ms>
type DurationInMins = decimal<Minute>
type Percentage  = decimal<Percent>

// -------------------------------------
// Enums

type Certainty 
    = NotCertain
    | QuiteCertain 
    | VeryCertain 
   

type QualitativeRating =
    | Poor
    | Fair
    | Good
    | Excellent


type Activity
    = ThirdPartyLogistics
    | Public
    | ProductDistribution
    | SiteService
    | SiteSpares
    | FLEX
    | SelfStorage
    | FulfillmentCenter
    | SupermarketDistribution


type BuildingExternalSiteExposure
    = Light
    | Medium
    | Severe


type Combustibility 
    = FR
    | NC
    | C


type DesignCode 
    = NFPA
    | FM
    | VDS
    | APSAD
    | CEA4001
    | EN12845
    | ASNZ2118
    | LPCorEquivalent
    | OtherLocalStandard
    | NoStandard


module DesignCode = 

    let (|NFPAorFM|Group1|OtherLocalStandard|NoStandard|) = function
        | NFPA
        | FM ->  NFPAorFM
        | VDS
        | APSAD
        | CEA4001
        | EN12845
        | ASNZ2118
        | LPCorEquivalent ->  Group1
        | OtherLocalStandard -> OtherLocalStandard
        | NoStandard -> NoStandard


type WorkerDensity
    = Empty 
    | Low 
    | Medium 
    | High 

type FireBrigade 
    = Professional 
    | OnSite 
    | Volunteer

type Forklift
    = Electrical
    | InternalCombustion
    | Manual

type Lighting
    = HID of isProtected : bool
    | Flourescent
    | LED

type BuiltEnvironment
    = Urban
    | Suburban
    | Rural

type StockGroup
    = Group1 
    | Group2
    | Group3 
    | Group4
    | CartonedUnexpandedGroupAPlastics 
    | CartonedExpandedGroupAPlastics 
    | ExposedUnexpandedGroupAPlastics    
    | ExposedExpandedGroupAPlastics 

type Shifts 
    = NoShifts
    | One  
    | Two  
    | Three 
    
type WorkerTurnover
    = Normal
    | High


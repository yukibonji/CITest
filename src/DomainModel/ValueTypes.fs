namespace XLCatlin.DataLab.XCBRA.DomainModel


// Value types (non-entities) such as Currency and Address that are shared 
// across the domain.
// 
// Types that are used only by one entity/aggregate are stored in that file.


// -------------------------------------
// Generic

type Range<'t> = { 
    upper : Option<'t>
    lower : Option<'t>
    }

[<RequireQualifiedAccess>]
module Range = 

    let empty = 
        { upper = None 
        ; lower = None 
        }

/// Represents all possible states for a particular question we want to ask
/// The question may have been asked or not.
/// When a question is asked, a user may then do one of three things
/// - answer 
/// - say they don't know 
/// - skip for now
type UserResponse<'t> 
    = Answered of 't 
    | Unknown  
    | NotAsked 
    | Skipped


type RangeEstimate<'t> = 
    { lowerBound : 't
    ; upperBound : 't
    ; median : 't
    }

type UncertainObservation<'t> = 
    { observation : 't 
    ; certainty : Certainty
    }

// -------------------------------------
// Money-related

type Currency = 
    { entity : string
    ; currency : string
    ; alphabeticCode : string
    ; numericCode : string
    ; minorUnit : int option
    }

[<RequireQualifiedAccess>]
[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>] 
module Currency = 
    let gbp =
        { entity = "UNITED KINGDOM OF GREAT BRITAIN AND NORTHERN IRELAND (THE)"
        ; currency = "Pound Sterling"
        ; alphabeticCode = "GBP"
        ; numericCode = "826"
        ; minorUnit = Some 2
        }


    let usd =
        { entity = "UNITED STATES OF AMERICA (THE)"
        ; currency = "US Dollar"
        ; alphabeticCode = "USD"
        ; numericCode = "840"
        ; minorUnit = Some 2
        }


    let aud =
        { entity = "AUSTRALIA"
        ; currency = "Australian Dollar"
        ; alphabeticCode = "AUD"
        ; numericCode = "036"
        ; minorUnit = Some 2
        }


    let cad =
        { entity = "CANADA"
        ; currency = "Canadian Dollar"
        ; alphabeticCode = "CAD"
        ; numericCode = "124"
        ; minorUnit = Some 2
        }


    let jpy =
        { entity = "JAPAN"
        ; currency = "Yen"
        ; alphabeticCode = "JPY"
        ; numericCode = "392"
        ; minorUnit = Some 0
        }


    let cny =
        { entity = "CHINA"
        ; currency = "Yuan Renminbi"
        ; alphabeticCode = "CNY"
        ; numericCode = "156"
        ; minorUnit = Some 2
        }


    let chf =
        { entity = "SWITZERLAND"
        ; currency = "Swiss Franc"
        ; alphabeticCode = "CHF"
        ; numericCode = "756"
        ; minorUnit = Some 2
        }



type AssetValue = {
    currencyCode : CurrencyCode 
    value :  decimal
    }

// -------------------------------------
// Location-related

type Country = 
    { name : string
    ; alpha2 : string
    ; alpha3 : string
    ; numeric : int
    }


type Address = 
    { postalCountry : string
    ; addressLines : string list
    ; administrativeArea : string
    ; locality : string
    ; dependentLocality : string option
    ; postalCode : string
    ; sortingCode : string option
    ; languageCode : string option
    }


type Angle = {
  degrees: decimal
  minutes: decimal 
  seconds: decimal 
} with
    member self.ToDecimal()  = 
      self.degrees + (self.minutes / 60M) + (self.seconds / 3600M)

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>] 
module Angle = 
    let (|Angle|) (angle: Angle) =
        ( angle.degrees
        , angle.minutes
        , angle.seconds
        )

    let (|DecimalAngle|) (angle: Angle) =
        angle.ToDecimal()

type Latitude 
  = North of Angle 
  | South of Angle
with 
    member lat.ToDecimal()  = 
      match lat with
      | North angle -> angle.ToDecimal()
      | South angle -> -angle.ToDecimal()

    override  lat.ToString() = 
      match lat with
      | North angle -> 
        sprintf 
          """%M° %M' %M" N""" 
          angle.degrees 
          angle.minutes 
          angle.seconds 
      | South angle ->
        sprintf 
          """%M° %M' %M" S""" 
          angle.degrees 
          angle.minutes 
          angle.seconds 

    static member FromDecimal (lat:decimal) = 
        let latabs = lat * 1000000M |> floor |> abs
        if latabs > 90000000M then None
        else
            let x = latabs / 1000000M
            let degs = floor x
            let y = (x - degs) * 60M
            let mins = floor y
            let secs = 60M * y - 60M * mins
            let angle = 
              { degrees = degs
              ; minutes = mins
              ; seconds = secs
              } 
            if lat >= 0M then 
              Some <| North angle 
            else 
              Some <| South angle


type Longitude 
  = East of Angle 
  | West of Angle
with 
    member long.ToDecimal() = 
      match long with 
      | East angle -> angle.ToDecimal()
      | West angle -> -angle.ToDecimal()

    override  long.ToString() =
      match long with 
      | East angle ->
        sprintf """%M° %M' %M" E""" 
          angle.degrees 
          angle.minutes 
          angle.seconds 
      | West angle ->
        sprintf """%M° %M' %M" W""" 
          angle.degrees 
          angle.minutes 
          angle.seconds 

    static member FromDecimal (long: decimal) =
        let longabs = long * 1000000M |> floor |> abs
        if longabs > 180000000M then None
        else
            let x = longabs / 1000000M
            let degs =  floor x 
            let y = (x - degs) * 60M
            let mins = floor y
            let secs = 60M * y - 60M * mins 
            let angle =
              { degrees = degs
              ; minutes = mins
              ; seconds = secs
              } 
            if long >= 0M then 
              Some <| East angle 
            else 
              Some <| West angle


type Coordinate  = {
    latitude: decimal
    longitude: decimal
} with
    override self.ToString() = 
      sprintf "(%0.3M,%0.3M)" 
              (self.latitude) 
              (self.longitude)


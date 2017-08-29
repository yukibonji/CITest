module Data.Address exposing (Address, summary, encode, decoder)

import Json.Encode as Encode exposing (Value)
import Json.Decode as Decode exposing (Decoder)
import Json.Encode.Extra as EncodeExtra
import Json.Decode.Pipeline as Pipeline exposing (decode, required, optional)
import Helpers exposing ((=>))


{- |
   # Address

    A data structure for international postal addresses, based on the one used
    in Google's `libaddressinput` library.

    Addresses may seem simple, but even within the US there are many quirks
    (hyphenated street addresses, etc.), and internationally addresses vary a
    great deal. The most sane and complete in many ways is the OASIS
    "extensible Address Language", xAL, which is a published and documented
    [XML schema](http://www.oasis-open.org/committees/ciq/download.shtml)

   ## Fields

   ### Postal Country
   CLDR (Common Locale Data Repository) country code

   ### Address Lines
   The most specific part of any address. They may be left empty if more
   detailed fields are used instead, or they may be used in addition to these
   if the more detailed fields do not fulfill requirements, or they may be used
   instead of more detailed fields to represent the street-level part.

   ### Administrative Area
   Top-level administrative subdivision of this country.

   ### Locality
   Generally refers to the city/town portion of an address.

   ### Dependent Locality
   Dependent locality or sublocality. Used for neighborhoods or suburbs.

   ### Postal Code
   Values are frequently alphanumeric.

   ### Sorting code
   This corresponds to the SortingCode sub-element of the xAL
   PostalServiceElements element. Use is very country-specific.

   ### Organization
   The firm or organization. This goes at a finer granularity than address
   lines in the address.

   ### Recipient
   The recipient. This goes at a finer granularity than address lines
   in the address. Not present in xAL.

   ### Language Code
   BCP-47 language code for the address.
-}


type alias Address =
    { postalCountry : String
    , addressLines : List String
    , administrativeArea : String
    , locality : String
    , dependentLocality : Maybe String
    , postalCode : String
    , sortingCode : Maybe String
    , languageCode : Maybe String
    }



-- Helpers ---------------------------------------------------------------------


summary :
    Address
    -> String
summary { locality, postalCode, postalCountry } =
    locality ++ ", " ++ postalCode ++ ", " ++ postalCountry



-- { "postalCountry": "United Kingdom", "addressLines": [ "8-12", "Warwick Lane" ], "administrativeArea": "England", "locality": "Greater London", "postalCode": "EC4P 4BN", "languageCode": "en" }
-- Serialization ---------------------------------------------------------------


encode :
    Address
    -> Value
encode address =
    Encode.object
        [ "postalCountry" => Encode.string address.postalCountry
        , "addressLines" => Encode.list (List.map Encode.string address.addressLines)
        , "administrativeArea" => Encode.string address.administrativeArea
        , "locality" => Encode.string address.locality
        , "dependentLocality" => EncodeExtra.maybe Encode.string address.dependentLocality
        , "postalCode" => Encode.string address.postalCode
        , "sortingCode" => EncodeExtra.maybe Encode.string address.sortingCode
        , "languageCode" => EncodeExtra.maybe Encode.string address.languageCode
        ]


decoder : Decoder Address
decoder =
    decode Address
        |> required "postalCountry" Decode.string
        |> required "addressLines" (Decode.list Decode.string)
        |> required "administrativeArea" Decode.string
        |> required "locality" Decode.string
        |> optional "dependentLocality" (Decode.maybe Decode.string) Nothing
        |> required "postalCode" Decode.string
        |> optional "sortingCode" (Decode.maybe Decode.string) Nothing
        |> optional "languageCode" (Decode.maybe Decode.string) Nothing

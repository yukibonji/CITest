module Data.Building.Activity exposing (Activity, encode, decoder, toString, fromString, toJsonString)

import Json.Encode as Encode exposing (Value)
import Json.Decode as Decode exposing (Decoder)
import Helpers exposing (resultToDecoder)


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


toJsonString : Activity -> String
toJsonString activity =
    case activity of
        ThirdPartyLogistics ->
            "ThirdPartyLogistics"

        Public ->
            "Public"

        ProductDistribution ->
            "ProductDistribution"

        SiteService ->
            "SiteService"

        SiteSpares ->
            "SiteSpares"

        FLEX ->
            "FLEX"

        SelfStorage ->
            "SelfStorage"

        FulfillmentCenter ->
            "FulfillmentCenter"

        SupermarketDistribution ->
            "SupermarketDistribution"


toString : Activity -> String
toString activity =
    case activity of
        ThirdPartyLogistics ->
            "Third Party Logistics"

        Public ->
            "Public"

        ProductDistribution ->
            "Product Distribution"

        SiteService ->
            "Site Service"

        SiteSpares ->
            "Site Spares"

        FLEX ->
            "FLEX"

        SelfStorage ->
            "Self Storage"

        FulfillmentCenter ->
            "Fulfillment Center"

        SupermarketDistribution ->
            "Supermarket Distribution"


fromString : String -> Result String Activity
fromString activityStr =
    case activityStr of
        "ThirdPartyLogistics" ->
            Ok ThirdPartyLogistics

        "Public" ->
            Ok Public

        "ProductDistribution" ->
            Ok ProductDistribution

        "SiteService" ->
            Ok SiteService

        "SiteSpares" ->
            Ok SiteSpares

        "FLEX" ->
            Ok FLEX

        "SelfStorage" ->
            Ok SelfStorage

        "FulfillmentCenter" ->
            Ok FulfillmentCenter

        "SupermarketDistribution" ->
            Ok SupermarketDistribution

        tag ->
            Err <|
                "Unexpected tag `"
                    ++ tag
                    ++ "` for type `Activity`."


encode : Activity -> Value
encode =
    toJsonString >> Encode.string


decoder : Decoder Activity
decoder =
    Decode.string
        |> Decode.andThen (fromString >> resultToDecoder)

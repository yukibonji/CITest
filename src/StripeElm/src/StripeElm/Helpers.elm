module Helpers
    exposing
        ( maybe
        , isJust
        , isNothing
        , (=>)
        , camelCase
        , encodeDU
        , decodeDU
        , encodeTuple2
        , decodeTuple2
        , currency
        , simpleCurrency
        , jsonParam
        , decodeDate
        , encodeDate
        , resultToDecoder
        , pluralize
        )

import Date exposing (Date)
import Dict exposing (Dict)
import Regex exposing (replace, HowMany(All), regex)
import Json.Encode as Encode exposing (Value)
import Json.Decode as Decode exposing (Decoder)
import Json.Decode.Pipeline as Pipeline exposing (decode, required)
import UrlParser


pluralize : String -> String -> Int -> String
pluralize singular plural count =
    if count == 1 then
        (toString count) ++ " " ++ singular
    else
        (toString count) ++ " " ++ plural


jsonParam : String -> Decoder a -> UrlParser.QueryParser (Maybe a -> b) b
jsonParam name =
    UrlParser.customParam name << jsonParamHelp


jsonParamHelp : Decoder a -> Maybe String -> Maybe a
jsonParamHelp decoder maybeStr =
    case maybeStr of
        Just str ->
            Decode.decodeString decoder str
                |> Result.toMaybe

        _ ->
            Nothing


maybe : a -> (b -> a) -> Maybe b -> a
maybe whenNothing withJust maybeVal =
    case maybeVal of
        Just val ->
            withJust val

        _ ->
            whenNothing


isJust : Maybe a -> Bool
isJust m =
    case m of
        Just _ ->
            True

        _ ->
            False


isNothing : Maybe a -> Bool
isNothing =
    not << isJust


(=>) : a -> b -> ( a, b )
(=>) =
    (,)


{-| infixl 0 means the (=>) operator has the same precedence as (<|) and (|>),
meaning you can use it at the end of a pipeline and have the precedence work out.
-}
infixl 0 =>


camelCase : String -> String
camelCase =
    replace All (regex "\\s+") (\_ -> "")
        << replace
            All
            (regex "(?:^\\w|[A-Z]|\\b\\w)")
            (\{ match, index } ->
                if index == 0 then
                    String.toLower match
                else
                    String.toUpper match
            )


resultToDecoder : Result b a -> Decoder a
resultToDecoder result =
    case result of
        Ok r ->
            Decode.succeed r

        Err e ->
            Decode.fail <| toString e


encodeDU : (a -> ( String, Value )) -> String -> a -> Value
encodeDU encode ty du =
    let
        ( tag, value ) =
            encode du
    in
        Encode.object
            [ "type" => Encode.string ty
            , "tag" => Encode.string tag
            , "value" => value
            ]


decodeDU : String -> (String -> Maybe ( Decoder a, a -> t )) -> Decoder t
decodeDU tyName decodeCases =
    Decode.field "type" Decode.string
        |> Decode.andThen
            (\ty ->
                if ty == tyName then
                    Decode.field "tag" Decode.string
                        |> Decode.andThen
                            (\tag ->
                                case decodeCases tag of
                                    Just ( decoder, constr ) ->
                                        Decode.map constr <|
                                            Decode.field "value" decoder

                                    _ ->
                                        Decode.fail <| "Unknown tag `" ++ tag ++ "` for type `" ++ ty ++ "`."
                            )
                else
                    Decode.fail <| "Wrong type `" ++ ty ++ "`; expected `" ++ tyName ++ "`."
            )


encodeTuple2 : (a -> Value) -> (b -> Value) -> ( a, b ) -> Value
encodeTuple2 encodeFirst encodeSecond ( a, b ) =
    Encode.object
        [ "1" => encodeFirst a
        , "2" => encodeSecond b
        ]


decodeDate : Decoder Date
decodeDate =
    Decode.andThen
        (\str ->
            case Date.fromString str of
                Ok date ->
                    Decode.succeed date

                Err err ->
                    Decode.fail err
        )
        Decode.string


encodeDate : Date -> Value
encodeDate =
    toString >> Encode.string


decodeTuple2 : Decoder a -> Decoder a1 -> Decoder ( a, a1 )
decodeTuple2 decodeFirst decodeSecond =
    decode (=>)
        |> required "1" decodeFirst
        |> required "2" decodeSecond


simpleCurrency : String -> Float -> String
simpleCurrency symbol =
    currency { symbol = symbol, abbreviateUnits = True, isAccounting = False, sep = ',', decimalPoint = '.' }


currency :
    { a
        | abbreviateUnits : Bool
        , decimalPoint : Char
        , isAccounting : Bool
        , sep : Char
        , symbol : String
    }
    -> Float
    -> String
currency { symbol, abbreviateUnits, isAccounting, sep, decimalPoint } =
    let
        toCurrencyStr x =
            if abbreviateUnits then
                let
                    ( x2, b ) =
                        numberWithBase x

                    suffix =
                        Dict.get b currencySuffix

                    dp =
                        if (toFloat <| round x2) == x2 then
                            0
                        else
                            2
                in
                    prettyFloatHelper (Just symbol) suffix isAccounting sep decimalPoint dp x2
            else
                prettyFloatHelper (Just symbol) Nothing isAccounting sep decimalPoint 2 x
    in
        toCurrencyStr


numberWithBase : Float -> ( Float, Int )
numberWithBase x =
    let
        n =
            logBase 10 x |> floor

        b =
            n - (n % 3)
    in
        ( x / (10.0 ^ toFloat b), b )


currencySuffix : Dict Int String
currencySuffix =
    Dict.fromList
        [ ( 3, "k" )
        , ( 6, "mn" )
        , ( 9, "bn" )
        , ( 12, "tn" )
        , ( 15, "qn" )
        ]


prettyFloatHelper :
    Maybe String
    -> Maybe String
    -> Bool
    -> Char
    -> Char
    -> Int
    -> Float
    -> String
prettyFloatHelper prefix suffix showPos sep decimalPoint decimalPlaces val =
    let
        absVal =
            abs val

        sign =
            if val < 0.0 then
                "-"
            else if showPos then
                "+"
            else
                ""

        absStr =
            toString absVal

        ( lhs, rhs ) =
            case String.split "." absStr of
                [ lhs, rhs ] ->
                    ( lhs, rhs )

                _ ->
                    ( absStr, "00" )

        lhss =
            List.map (String.fromList) <| chunksOf 3 (String.toList lhs)

        rhsTrunc =
            String.padRight decimalPlaces '0' <|
                String.left decimalPlaces rhs

        sepStr =
            String.fromChar sep

        dpStr =
            if decimalPlaces == 0 then
                ""
            else
                String.fromChar decimalPoint

        pfxStr =
            Maybe.withDefault "" prefix

        sfxStr =
            Maybe.withDefault "" suffix
    in
        sign ++ pfxStr ++ String.join sepStr lhss ++ dpStr ++ rhsTrunc ++ sfxStr


chunksOf : Int -> List a -> List (List a)
chunksOf chunkSize xs =
    chunksOfHelper chunkSize [] [] 0 (List.reverse xs)


chunksOfHelper :
    Int
    -> List (List a)
    -> List a
    -> Int
    -> List a
    -> List (List a)
chunksOfHelper chunkSize accu currentChunk currentChunkSize xs =
    case xs of
        [] ->
            case currentChunk of
                [] ->
                    accu

                _ ->
                    currentChunk :: accu

        next :: rest ->
            if currentChunkSize == chunkSize then
                chunksOfHelper chunkSize (currentChunk :: accu) [ next ] 1 rest
            else
                chunksOfHelper chunkSize accu (next :: currentChunk) (currentChunkSize + 1) rest

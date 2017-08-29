module Data.Building.Lighting exposing (Lighting(..), encode)

import Json.Encode as Encode exposing (Value)
import Json.Decode as Decode exposing (Decoder)
import Helpers exposing (resultToDecoder, (=>))


type Lighting
    = HID { isProtected : Bool }
    | Flourescent
    | LED


toString : Lighting -> String
toString lighting =
    case lighting of
        HID { isProtected } ->
            "HID"
                ++ (if isProtected then
                        " (protected)"
                    else
                        " (unprotected)"
                   )

        Flourescent ->
            "Flourescent"

        LED ->
            "LED"


fromString : String -> Result String Lighting
fromString lightingStr =
    case lightingStr of
        "Flourescent" ->
            Ok Flourescent

        "LED" ->
            Ok LED

        "HID (protected)" ->
            Ok <| HID { isProtected = True }

        "HID (unprotected)" ->
            Ok <| HID { isProtected = False }

        tag ->
            Err <|
                "Unexpected tag `"
                    ++ tag
                    ++ "` for type `ForkLift`."


encode : Lighting -> Value
encode lighting =
    case lighting of
        HID { isProtected } ->
            Encode.object
                [ "isProtected" => Encode.bool isProtected ]

        _ ->
            Encode.string <|
                toString lighting


decoder : Decoder Lighting
decoder =
    Decode.oneOf
        [ Decode.map (\isProtected -> HID { isProtected = isProtected }) <|
            Decode.field "HID" Decode.bool
        , Decode.string
            |> Decode.andThen (fromString >> resultToDecoder)
        ]

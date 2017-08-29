module View.Icon exposing (view, Icon(..), Direction(..))

import Color exposing (Color)
import Svg exposing (Svg)
import Svg.Attributes exposing (cx, cy, d, fillOpacity, r)


type Icon
    = Search { size : Int, color : Color }
    | Bookmark { size : Int, color : Color, filled : Bool }
    | EditLocation { size : Int, color : Color }
    | MyLocation { size : Int, color : Color }
    | Error { size : Int, color : Color, filled : Bool }
    | Warning { size : Int, color : Color }
    | Assessment { size : Int, color : Color }
    | Assignment { size : Int, color : Color }
    | Logo { size : Int, color : Color }
    | Settings { size : Int, color : Color }
    | User { size : Int, color : Color }
    | Chevron { size : Int, color : Color, direction : Direction }
    | AddLocation { size : Int, color : Color }
    | Delete { size : Int, color : Color }
    | Edit { size : Int, color : Color }
    | PageView { size : Int, color : Color }
    | Close { size : Int, color : Color }
    | Cancel { size : Int, color : Color }


type Direction
    = Left
    | Right
    | Up
    | Down


view : Icon -> Svg msg
view icon =
    case icon of
        Search { size, color } ->
            search color size

        Bookmark { size, color, filled } ->
            if filled then
                bookmark color size
            else
                bookmarkBorder color size

        Error { size, color, filled } ->
            if filled then
                error color size
            else
                errorOutline color size

        Warning { size, color } ->
            warning color size

        Assessment { size, color } ->
            assessment color size

        Assignment { size, color } ->
            assignment color size

        AddLocation { size, color } ->
            addLocation color size

        EditLocation { size, color } ->
            editLocation color size

        MyLocation { size, color } ->
            myLocation color size

        Logo { size, color } ->
            logo color size

        Settings { size, color } ->
            settings color size

        User { size, color } ->
            account color size

        Delete { size, color } ->
            delete color size

        Close { size, color } ->
            close color size

        Cancel { size, color } ->
            cancel color size

        Edit { size, color } ->
            edit color size

        PageView { size, color } ->
            pageview color size

        Chevron { size, color, direction } ->
            case direction of
                Left ->
                    chevronLeft color size

                Right ->
                    chevronRight color size

                Up ->
                    chevronUp color size

                Down ->
                    chevronDown color size



-- Icons --


{-| -}
cancel : Color -> Int -> Svg msg
cancel =
    icon "0 0 48 48" [ Svg.path [ d "M24 4C12.95 4 4 12.95 4 24s8.95 20 20 20 20-8.95 20-20S35.05 4 24 4zm10 27.17L31.17 34 24 26.83 16.83 34 14 31.17 21.17 24 14 16.83 16.83 14 24 21.17 31.17 14 34 16.83 26.83 24 34 31.17z" ] [] ]


{-| -}
close : Color -> Int -> Svg msg
close =
    icon "0 0 48 48" [ Svg.path [ d "M38 12.83L35.17 10 24 21.17 12.83 10 10 12.83 21.17 24 10 35.17 12.83 38 24 26.83 35.17 38 38 35.17 26.83 24z" ] [] ]


pageview : Color -> Int -> Svg msg
pageview =
    icon "0 0 48 48" [ Svg.path [ d "M23 18c-2.76 0-5 2.24-5 5s2.24 5 5 5 5-2.24 5-5-2.24-5-5-5zM40 8H8c-2.21 0-4 1.79-4 4v24c0 2.21 1.79 4 4 4h32c2.21 0 4-1.79 4-4V12c0-2.21-1.79-4-4-4zm-6.41 28.41l-5.81-5.81c-1.39.88-3.02 1.4-4.78 1.4-4.97 0-9-4.03-9-9s4.03-9 9-9 9 4.03 9 9c0 1.76-.52 3.39-1.4 4.77l5.82 5.8-2.83 2.84z" ] [] ]


edit : Color -> Int -> Svg msg
edit =
    icon "0 0 48 48" [ Svg.path [ d "M6 34.5V42h7.5l22.13-22.13-7.5-7.5L6 34.5zm35.41-20.41c.78-.78.78-2.05 0-2.83l-4.67-4.67c-.78-.78-2.05-.78-2.83 0l-3.66 3.66 7.5 7.5 3.66-3.66z" ] [] ]


delete : Color -> Int -> Svg msg
delete =
    icon "0 0 48 48" [ Svg.path [ d "M12 38c0 2.21 1.79 4 4 4h16c2.21 0 4-1.79 4-4V14H12v24zM38 8h-7l-2-2H19l-2 2h-7v4h28V8z" ] [] ]


{-| -}
chevronLeft : Color -> Int -> Svg msg
chevronLeft =
    icon "0 0 48 48" [ Svg.path [ d "M30.83 14.83L28 12 16 24l12 12 2.83-2.83L21.66 24z" ] [] ]


{-| -}
chevronRight : Color -> Int -> Svg msg
chevronRight =
    icon "0 0 48 48" [ Svg.path [ d "M20 12l-2.83 2.83L26.34 24l-9.17 9.17L20 36l12-12z" ] [] ]


chevronUp : Color -> Int -> Svg msg
chevronUp =
    icon "0 0 48 48" [ Svg.path [ d "M24 16L12 28l2.83 2.83L24 21.66l9.17 9.17L36 28z" ] [] ]


{-| -}
chevronDown : Color -> Int -> Svg msg
chevronDown =
    icon "0 0 48 48" [ Svg.path [ d "M33.17 17.17L24 26.34l-9.17-9.17L12 20l12 12 12-12z" ] [] ]


logo : Color -> Int -> Svg msg
logo =
    icon "-3.6043875 -3.6043875 174.857515 127.355025"
        [ Svg.path
            [ d "M 14.46875,0 0.0025,14.4725 45.60375,60.07375 91.2125,14.4725 76.74125,0.00375 45.60375,31.135 14.46875,0 z m 88.685,0.1375 0,64.49 64.495,0.005 -0.002,-20.46375 -44.02875,-0.005 -10e-4,-44.03125 -20.4625,0.005 z M 0,105.675 14.4725,120.14375 45.6075,89.0125 76.7425,120.14625 91.21,105.675 45.6075,60.0725 0,105.675 z"
            ]
            []
        ]


settings : Color -> Int -> Svg msg
settings =
    icon "0 0 48 48" [ Svg.path [ d "M38.86 25.95c.08-.64.14-1.29.14-1.95s-.06-1.31-.14-1.95l4.23-3.31c.38-.3.49-.84.24-1.28l-4-6.93c-.25-.43-.77-.61-1.22-.43l-4.98 2.01c-1.03-.79-2.16-1.46-3.38-1.97L29 4.84c-.09-.47-.5-.84-1-.84h-8c-.5 0-.91.37-.99.84l-.75 5.3c-1.22.51-2.35 1.17-3.38 1.97L9.9 10.1c-.45-.17-.97 0-1.22.43l-4 6.93c-.25.43-.14.97.24 1.28l4.22 3.31C9.06 22.69 9 23.34 9 24s.06 1.31.14 1.95l-4.22 3.31c-.38.3-.49.84-.24 1.28l4 6.93c.25.43.77.61 1.22.43l4.98-2.01c1.03.79 2.16 1.46 3.38 1.97l.75 5.3c.08.47.49.84.99.84h8c.5 0 .91-.37.99-.84l.75-5.3c1.22-.51 2.35-1.17 3.38-1.97l4.98 2.01c.45.17.97 0 1.22-.43l4-6.93c.25-.43.14-.97-.24-1.28l-4.22-3.31zM24 31c-3.87 0-7-3.13-7-7s3.13-7 7-7 7 3.13 7 7-3.13 7-7 7z" ] [] ]


account : Color -> Int -> Svg msg
account =
    icon "0 0 48 48" [ Svg.path [ d "M6 10v28c0 2.21 1.79 4 4 4h28c2.21 0 4-1.79 4-4V10c0-2.21-1.79-4-4-4H10c-2.21 0-4 1.79-4 4zm24 8c0 3.32-2.69 6-6 6s-6-2.68-6-6c0-3.31 2.69-6 6-6s6 2.69 6 6zM12 34c0-4 8-6.2 12-6.2S36 30 36 34v2H12v-2z" ] [] ]



-- xl color size =
--     Svg.svg
--         [ SvgAttr.width <| toString size
--         , SvgAttr.viewBox "-3.6043875 -3.6043875 174.857515 127.355025"
--         ]
--         [ Svg.path
--             [ SvgAttr.d "M 14.46875,0 0.0025,14.4725 45.60375,60.07375 91.2125,14.4725 76.74125,0.00375 45.60375,31.135 14.46875,0 z m 88.685,0.1375 0,64.49 64.495,0.005 -0.002,-20.46375 -44.02875,-0.005 -10e-4,-44.03125 -20.4625,0.005 z M 0,105.675 14.4725,120.14375 45.6075,89.0125 76.7425,120.14625 91.21,105.675 45.6075,60.0725 0,105.675 z"
--             ], SvgAttr.style <| "fill:" ++ hexString color ++ ";fill-opacity:1;fill-rule:nonzero;stroke:none"
--             ]
--             []
--           -- d="M 14.46875,0 0.0025,14.4725 45.60375,60.07375 91.2125,14.4725 76.74125,0.00375 45.60375,31.135 14.46875,0 z m 88.685,0.1375 0,64.49 64.495,0.005 -0.002,-20.46375 -44.02875,-0.005 -10e-4,-44.03125 -20.4625,0.005 z M 0,105.675 14.4725,120.14375 45.6075,89.0125 76.7425,120.14625 91.21,105.675 45.6075,60.0725 0,105.675 z" id="path21908" style="fill:#000000;fill-opacity:1;fill-rule:nonzero;stroke:none"
--         ]


{-| -}
error : Color -> Int -> Svg msg
error =
    icon "0 0 48 48" [ Svg.path [ d "M24 4C12.96 4 4 12.95 4 24s8.96 20 20 20 20-8.95 20-20S35.04 4 24 4zm2 30h-4v-4h4v4zm0-8h-4V14h4v12z" ] [] ]


{-| -}
errorOutline : Color -> Int -> Svg msg
errorOutline =
    icon "0 0 48 48" [ Svg.path [ d "M22 30h4v4h-4zm0-16h4v12h-4zm1.99-10C12.94 4 4 12.95 4 24s8.94 20 19.99 20S44 35.05 44 24 35.04 4 23.99 4zM24 40c-8.84 0-16-7.16-16-16S15.16 8 24 8s16 7.16 16 16-7.16 16-16 16z" ] [] ]


{-| -}
warning : Color -> Int -> Svg msg
warning =
    icon "0 0 48 48" [ Svg.path [ d "M2 42h44L24 4 2 42zm24-6h-4v-4h4v4zm0-8h-4v-8h4v8z" ] [] ]


addLocation : Color -> Int -> Svg msg
addLocation =
    icon "0 0 48 48" [ Svg.path [ d "M24 4c-7.72 0-14 6.28-14 14 0 10.5 14 26 14 26s14-15.5 14-26c0-7.72-6.28-14-14-14zm8 16h-6v6h-4v-6h-6v-4h6v-6h4v6h6v4z" ] [] ]


editLocation : Color -> Int -> Svg msg
editLocation =
    icon "0 0 48 48" [ Svg.path [ d "M24 4c-7.72 0-14 6.28-14 14 0 10.5 14 26 14 26s14-15.5 14-26c0-7.72-6.28-14-14-14zm-3.12 20H18v-2.88l6.7-6.68 2.86 2.86-6.68 6.7zm8.9-8.9l-1.4 1.4-2.88-2.88 1.4-1.4c.3-.3.78-.3 1.08 0l1.8 1.8c.3.3.3.78 0 1.08z" ] [] ]


myLocation : Color -> Int -> Svg msg
myLocation =
    icon "0 0 48 48" [ Svg.path [ d "M24 16c-4.42 0-8 3.58-8 8s3.58 8 8 8 8-3.58 8-8-3.58-8-8-8zm17.88 6C40.96 13.66 34.34 7.04 26 6.12V2h-4v4.12C13.66 7.04 7.04 13.66 6.12 22H2v4h4.12c.92 8.34 7.54 14.96 15.88 15.88V46h4v-4.12c8.34-.92 14.96-7.54 15.88-15.88H46v-4h-4.12zM24 38c-7.73 0-14-6.27-14-14s6.27-14 14-14 14 6.27 14 14-6.27 14-14 14z" ] [] ]


{-| -}
assessment : Color -> Int -> Svg msg
assessment =
    icon "0 0 48 48" [ Svg.path [ d "M38 6H10c-2.21 0-4 1.79-4 4v28c0 2.21 1.79 4 4 4h28c2.21 0 4-1.79 4-4V10c0-2.21-1.79-4-4-4zM18 34h-4V20h4v14zm8 0h-4V14h4v20zm8 0h-4v-8h4v8z" ] [] ]


{-| -}
assignment : Color -> Int -> Svg msg
assignment =
    icon "0 0 48 48" [ Svg.path [ d "M38 6h-8.37c-.82-2.32-3.02-4-5.63-4s-4.81 1.68-5.63 4H10c-2.21 0-4 1.79-4 4v28c0 2.21 1.79 4 4 4h28c2.21 0 4-1.79 4-4V10c0-2.21-1.79-4-4-4zM24 6c1.1 0 2 .89 2 2s-.9 2-2 2-2-.89-2-2 .9-2 2-2zm4 28H14v-4h14v4zm6-8H14v-4h20v4zm0-8H14v-4h20v4z" ] [] ]


search : Color -> Int -> Svg msg
search =
    icon
        "0 0 48 48"
        [ Svg.path
            [ d "M31 28h-1.59l-.55-.55C30.82 25.18 32 22.23 32 19c0-7.18-5.82-13-13-13S6 11.82 6 19s5.82 13 13 13c3.23 0 6.18-1.18 8.45-3.13l.55.55V31l10 9.98L40.98 38 31 28zm-12 0c-4.97 0-9-4.03-9-9s4.03-9 9-9 9 4.03 9 9-4.03 9-9 9z" ]
            []
        ]


{-| -}
bookmark : Color -> Int -> Svg msg
bookmark =
    icon "0 0 48 48" [ Svg.path [ d "M34 6H14c-2.21 0-3.98 1.79-3.98 4L10 42l14-6 14 6V10c0-2.21-1.79-4-4-4z" ] [] ]


{-| -}
bookmarkBorder : Color -> Int -> Svg msg
bookmarkBorder =
    icon "0 0 48 48" [ Svg.path [ d "M34 6H14c-2.21 0-3.98 1.79-3.98 4L10 42l14-6 14 6V10c0-2.21-1.79-4-4-4zm0 30l-10-4.35L14 36V10h20v26z" ] [] ]



-- Internal --


icon : String -> List (Svg msg) -> Color -> Int -> Svg msg
icon viewBox children color size =
    let
        stringSize =
            toString size

        stringColor =
            toRgbaString color
    in
        Svg.svg
            [ Svg.Attributes.width stringSize
            , Svg.Attributes.height stringSize
            , Svg.Attributes.viewBox viewBox
            ]
            [ Svg.g
                [ Svg.Attributes.fill stringColor ]
                children
            ]


toRgbaString : Color -> String
toRgbaString color =
    let
        { red, green, blue, alpha } =
            Color.toRgb color
    in
        "rgba("
            ++ toString red
            ++ ","
            ++ toString green
            ++ ","
            ++ toString blue
            ++ ","
            ++ toString alpha
            ++ ")"

module Data.UUID exposing (UUID, uuid)

import Bitwise exposing (and, or)
import Random exposing (Generator)
import Array
import String


type alias UUID =
    String


uuid : Generator UUID
uuid =
    Random.map5
        (\a b c d e ->
            (String.fromList a) ++ "-" ++ (String.fromList b) ++ "-4" ++ (String.fromList c) ++ "-" ++ (String.fromList d) ++ "-" ++ (String.fromList e)
        )
        (hexList 8)
        (hexList 4)
        (hexList 3)
        (Random.map2
            (\x xs -> (hexChar <| constrain x) :: xs)
            hex
            (hexList 3)
        )
        (hexList 12)


hexList : Int -> Generator (List Char)
hexList n =
    Random.list n <| Random.map hexChar hex


constrain : Int -> Int
constrain i =
    or (and i 3) 8


hexChar : Int -> Char
hexChar c =
    Array.get c hexDigit |> Maybe.withDefault 'x'


hex : Generator Int
hex =
    Random.int
        0
        15


hexDigit : Array.Array Char
hexDigit =
    Array.fromList
        [ '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' ]

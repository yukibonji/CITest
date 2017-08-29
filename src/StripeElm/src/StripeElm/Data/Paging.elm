module Data.Paging exposing (Paging)


type alias Paging =
    { offset : Int
    , fetchNext : Int
    }

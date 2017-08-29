module View.Intent exposing (Intent(..), toString)


type Intent
    = Primary
    | Success
    | Warning
    | Danger


toString : Intent -> String
toString intent =
    case intent of
        Primary ->
            "--info"

        Success ->
            "--success"

        Warning ->
            "--warning"

        Danger ->
            "--error"

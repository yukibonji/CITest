namespace XLCatlin.DataLab.XCBRA.DomainModel

// Authentication and users

type Username = Username of string
    with
    member this.Value = (this |> function Username id -> id)

type AuthToken = string //TODO: Authentication

type User = {
    Email : string
    Token : AuthToken
    Username : Username
    Bio : string option
    }

// ==========================
// Authorization input
// ==========================

/// Passed in to commands and queries
type ApiAuthorization = {
    Username : Username 
    AuthToken : string
    }
    with
    /// The domain for HTTP Authorization 
    static member Domain =
        "XLCatlin.DataLab.XCBRA"
    /// convert to a HTTP Authorization header
    member this.AuthHeader =
        sprintf "%s %s %s" ApiAuthorization.Domain this.Username.Value this.AuthToken

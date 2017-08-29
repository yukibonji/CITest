module Query.Helpers exposing (apiUrl)


apiUrl :
    { r
        | apiHost : String
        , apiPort : Int
        , apiProtocol : String
        , apiVersion : Int
    }
    -> String
    -> String
apiUrl { apiProtocol, apiHost, apiPort, apiVersion } str =
    apiProtocol
        ++ "://"
        ++ apiHost
        ++ ":"
        ++ toString apiPort
        ++ str



--
-- ++ "/api/v"
-- ++ toString apiVersion
-- ++ str

module Data.Country exposing (Country, alpha2, encode, decoder)

import Dict exposing (Dict)
import Json.Encode as Encode exposing (Value)
import Json.Decode as Decode exposing (Decoder)
import Json.Decode.Pipeline as Pipeline exposing (decode, required, optional)
import Helpers exposing ((=>))


type alias Country =
    { name : String
    , alpha2 : String
    , alpha3 : String
    , numeric : Int
    }


alpha2 : Dict String Country
alpha2 =
    case raw of
        Ok xs ->
            Dict.fromList <| List.map (\ctry -> ( ctry.alpha2, ctry )) xs

        _ ->
            Dict.empty


raw : Result String (List Country)
raw =
    Decode.decodeString (Decode.list decoder) <|
        """
            [
            {"name":"AALAND ISLANDS","alpha2":"AX","alpha3":"ALA","numeric":248},
            {"name":"AFGHANISTAN","alpha2":"AF","alpha3":"AFG","numeric":4},
            {"name":"ALBANIA","alpha2":"AL","alpha3":"ALB","numeric":8},
            {"name":"ALGERIA","alpha2":"DZ","alpha3":"DZA","numeric":12},
            {"name":"AMERICAN SAMOA","alpha2":"AS","alpha3":"ASM","numeric":16},
            {"name":"ANDORRA","alpha2":"AD","alpha3":"AND","numeric":20},
            {"name":"ANGOLA","alpha2":"AO","alpha3":"AGO","numeric":24},
            {"name":"ANGUILLA","alpha2":"AI","alpha3":"AIA","numeric":660},
            {"name":"ANTARCTICA","alpha2":"AQ","alpha3":"ATA","numeric":10},
            {"name":"ANTIGUA AND BARBUDA","alpha2":"AG","alpha3":"ATG","numeric":28},
            {"name":"ARGENTINA","alpha2":"AR","alpha3":"ARG","numeric":32},
            {"name":"ARMENIA","alpha2":"AM","alpha3":"ARM","numeric":51},
            {"name":"ARUBA","alpha2":"AW","alpha3":"ABW","numeric":533},
            {"name":"AUSTRALIA","alpha2":"AU","alpha3":"AUS","numeric":36},
            {"name":"AUSTRIA","alpha2":"AT","alpha3":"AUT","numeric":40},
            {"name":"AZERBAIJAN","alpha2":"AZ","alpha3":"AZE","numeric":31},
            {"name":"BAHAMAS","alpha2":"BS","alpha3":"BHS","numeric":44},
            {"name":"BAHRAIN","alpha2":"BH","alpha3":"BHR","numeric":48},
            {"name":"BANGLADESH","alpha2":"BD","alpha3":"BGD","numeric":50},
            {"name":"BARBADOS","alpha2":"BB","alpha3":"BRB","numeric":52},
            {"name":"BELARUS","alpha2":"BY","alpha3":"BLR","numeric":112},
            {"name":"BELGIUM","alpha2":"BE","alpha3":"BEL","numeric":56},
            {"name":"BELIZE","alpha2":"BZ","alpha3":"BLZ","numeric":84},
            {"name":"BENIN","alpha2":"BJ","alpha3":"BEN","numeric":204},
            {"name":"BERMUDA","alpha2":"BM","alpha3":"BMU","numeric":60},
            {"name":"BHUTAN","alpha2":"BT","alpha3":"BTN","numeric":64},
            {"name":"BOLIVIA","alpha2":"BO","alpha3":"BOL","numeric":68},
            {"name":"BOSNIA AND HERZEGOWINA","alpha2":"BA","alpha3":"BIH","numeric":70},
            {"name":"BOTSWANA","alpha2":"BW","alpha3":"BWA","numeric":72},
            {"name":"BOUVET ISLAND","alpha2":"BV","alpha3":"BVT","numeric":74},
            {"name":"BRAZIL","alpha2":"BR","alpha3":"BRA","numeric":76},
            {"name":"BRITISH INDIAN OCEAN TERRITORY","alpha2":"IO","alpha3":"IOT","numeric":86},
            {"name":"BRUNEI DARUSSALAM","alpha2":"BN","alpha3":"BRN","numeric":96},
            {"name":"BULGARIA","alpha2":"BG","alpha3":"BGR","numeric":100},
            {"name":"BURKINA FASO","alpha2":"BF","alpha3":"BFA","numeric":854},
            {"name":"BURUNDI","alpha2":"BI","alpha3":"BDI","numeric":108},
            {"name":"CAMBODIA","alpha2":"KH","alpha3":"KHM","numeric":116},
            {"name":"CAMEROON","alpha2":"CM","alpha3":"CMR","numeric":120},
            {"name":"CANADA","alpha2":"CA","alpha3":"CAN","numeric":124},
            {"name":"CAPE VERDE","alpha2":"CV","alpha3":"CPV","numeric":132},
            {"name":"CAYMAN ISLANDS","alpha2":"KY","alpha3":"CYM","numeric":136},
            {"name":"CENTRAL AFRICAN REPUBLIC","alpha2":"CF","alpha3":"CAF","numeric":140},
            {"name":"CHAD","alpha2":"TD","alpha3":"TCD","numeric":148},
            {"name":"CHILE","alpha2":"CL","alpha3":"CHL","numeric":152},
            {"name":"CHINA","alpha2":"CN","alpha3":"CHN","numeric":156},
            {"name":"CHRISTMAS ISLAND","alpha2":"CX","alpha3":"CXR","numeric":162},
            {"name":"COCOS (KEELING) ISLANDS","alpha2":"CC","alpha3":"CCK","numeric":166},
            {"name":"COLOMBIA","alpha2":"CO","alpha3":"COL","numeric":170},
            {"name":"COMOROS","alpha2":"KM","alpha3":"COM","numeric":174},
            {"name":"DEMOCRATIC REPUBLIC OF CONGO","alpha2":"CD","alpha3":"COD","numeric":180},
            {"name":"REPUBLIC OF CONGO","alpha2":"CG","alpha3":"COG","numeric":178},
            {"name":"COOK ISLANDS","alpha2":"CK","alpha3":"COK","numeric":184},
            {"name":"COSTA RICA","alpha2":"CR","alpha3":"CRI","numeric":188},
            {"name":"COTE D'IVOIRE","alpha2":"CI","alpha3":"CIV","numeric":384},
            {"name":"CROATIA","alpha2":"HR","alpha3":"HRV","numeric":191},
            {"name":"CUBA","alpha2":"CU","alpha3":"CUB","numeric":192},
            {"name":"CYPRUS","alpha2":"CY","alpha3":"CYP","numeric":196},
            {"name":"CZECH REPUBLIC","alpha2":"CZ","alpha3":"CZE","numeric":203},
            {"name":"DENMARK","alpha2":"DK","alpha3":"DNK","numeric":208},
            {"name":"DJIBOUTI","alpha2":"DJ","alpha3":"DJI","numeric":262},
            {"name":"DOMINICA","alpha2":"DM","alpha3":"DMA","numeric":212},
            {"name":"DOMINICAN REPUBLIC","alpha2":"DO","alpha3":"DOM","numeric":214},
            {"name":"ECUADOR","alpha2":"EC","alpha3":"ECU","numeric":218},
            {"name":"EGYPT","alpha2":"EG","alpha3":"EGY","numeric":818},
            {"name":"EL SALVADOR","alpha2":"SV","alpha3":"SLV","numeric":222},
            {"name":"EQUATORIAL GUINEA","alpha2":"GQ","alpha3":"GNQ","numeric":226},
            {"name":"ERITREA","alpha2":"ER","alpha3":"ERI","numeric":232},
            {"name":"ESTONIA","alpha2":"EE","alpha3":"EST","numeric":233},
            {"name":"ETHIOPIA","alpha2":"ET","alpha3":"ETH","numeric":231},
            {"name":"FALKLAND ISLANDS (MALVINAS)","alpha2":"FK","alpha3":"FLK","numeric":238},
            {"name":"FAROE ISLANDS","alpha2":"FO","alpha3":"FRO","numeric":234},
            {"name":"FIJI","alpha2":"FJ","alpha3":"FJI","numeric":242},
            {"name":"FINLAND","alpha2":"FI","alpha3":"FIN","numeric":246},
            {"name":"FRANCE","alpha2":"FR","alpha3":"FRA","numeric":250},
            {"name":"FRENCH GUIANA","alpha2":"GF","alpha3":"GUF","numeric":254},
            {"name":"FRENCH POLYNESIA","alpha2":"PF","alpha3":"PYF","numeric":258},
            {"name":"FRENCH SOUTHERN TERRITORIES","alpha2":"TF","alpha3":"ATF","numeric":260},
            {"name":"GABON","alpha2":"GA","alpha3":"GAB","numeric":266},
            {"name":"GAMBIA","alpha2":"GM","alpha3":"GMB","numeric":270},
            {"name":"GEORGIA","alpha2":"GE","alpha3":"GEO","numeric":268},
            {"name":"GERMANY","alpha2":"DE","alpha3":"DEU","numeric":276},
            {"name":"GHANA","alpha2":"GH","alpha3":"GHA","numeric":288},
            {"name":"GIBRALTAR","alpha2":"GI","alpha3":"GIB","numeric":292},
            {"name":"GREECE","alpha2":"GR","alpha3":"GRC","numeric":300},
            {"name":"GREENLAND","alpha2":"GL","alpha3":"GRL","numeric":304},
            {"name":"GRENADA","alpha2":"GD","alpha3":"GRD","numeric":308},
            {"name":"GUADELOUPE","alpha2":"GP","alpha3":"GLP","numeric":312},
            {"name":"GUAM","alpha2":"GU","alpha3":"GUM","numeric":316},
            {"name":"GUATEMALA","alpha2":"GT","alpha3":"GTM","numeric":320},
            {"name":"GUINEA","alpha2":"GN","alpha3":"GIN","numeric":324},
            {"name":"GUINEA-BISSAU","alpha2":"GW","alpha3":"GNB","numeric":624},
            {"name":"GUYANA","alpha2":"GY","alpha3":"GUY","numeric":328},
            {"name":"HAITI","alpha2":"HT","alpha3":"HTI","numeric":332},
            {"name":"HEARD AND MC DONALD ISLANDS","alpha2":"HM","alpha3":"HMD","numeric":334},
            {"name":"HONDURAS","alpha2":"HN","alpha3":"HND","numeric":340},
            {"name":"HONG KONG","alpha2":"HK","alpha3":"HKG","numeric":344},
            {"name":"HUNGARY","alpha2":"HU","alpha3":"HUN","numeric":348},
            {"name":"ICELAND","alpha2":"IS","alpha3":"ISL","numeric":352},
            {"name":"INDIA","alpha2":"IN","alpha3":"IND","numeric":356},
            {"name":"INDONESIA","alpha2":"ID","alpha3":"IDN","numeric":360},
            {"name":"IRAN","alpha2":"IR","alpha3":"IRN","numeric":364},
            {"name":"IRAQ","alpha2":"IQ","alpha3":"IRQ","numeric":368},
            {"name":"IRELAND","alpha2":"IE","alpha3":"IRL","numeric":372},
            {"name":"ISRAEL","alpha2":"IL","alpha3":"ISR","numeric":376},
            {"name":"ITALY","alpha2":"IT","alpha3":"ITA","numeric":380},
            {"name":"JAMAICA","alpha2":"JM","alpha3":"JAM","numeric":388},
            {"name":"JAPAN","alpha2":"JP","alpha3":"JPN","numeric":392},
            {"name":"JORDAN","alpha2":"JO","alpha3":"JOR","numeric":400},
            {"name":"KAZAKHSTAN","alpha2":"KZ","alpha3":"KAZ","numeric":398},
            {"name":"KENYA","alpha2":"KE","alpha3":"KEN","numeric":404},
            {"name":"KIRIBATI","alpha2":"KI","alpha3":"KIR","numeric":296},
            {"name":"DEMOCRATIC PEOPLE'S REPUBLIC OF KOREA","alpha2":"KP","alpha3":"PRK","numeric":408},
            {"name":"REPUBLIC OF KOREA","alpha2":"KR","alpha3":"KOR","numeric":410},
            {"name":"KUWAIT","alpha2":"KW","alpha3":"KWT","numeric":414},
            {"name":"KYRGYZSTAN","alpha2":"KG","alpha3":"KGZ","numeric":417},
            {"name":"LAO PEOPLE'S DEMOCRATIC REPUBLIC","alpha2":"LA","alpha3":"LAO","numeric":418},
            {"name":"LATVIA","alpha2":"LV","alpha3":"LVA","numeric":428},
            {"name":"LEBANON","alpha2":"LB","alpha3":"LBN","numeric":422},
            {"name":"LESOTHO","alpha2":"LS","alpha3":"LSO","numeric":426},
            {"name":"LIBERIA","alpha2":"LR","alpha3":"LBR","numeric":430},
            {"name":"LIBYAN ARAB JAMAHIRIYA","alpha2":"LY","alpha3":"LBY","numeric":434},
            {"name":"LIECHTENSTEIN","alpha2":"LI","alpha3":"LIE","numeric":438},
            {"name":"LITHUANIA","alpha2":"LT","alpha3":"LTU","numeric":440},
            {"name":"LUXEMBOURG","alpha2":"LU","alpha3":"LUX","numeric":442},
            {"name":"MACAU","alpha2":"MO","alpha3":"MAC","numeric":446},
            {"name":"THE FORMER YUGOSLAV REPUBLIC OF MACEDONIA","alpha2":"MK","alpha3":"MKD","numeric":807},
            {"name":"MADAGASCAR","alpha2":"MG","alpha3":"MDG","numeric":450},
            {"name":"MALAWI","alpha2":"MW","alpha3":"MWI","numeric":454},
            {"name":"MALAYSIA","alpha2":"MY","alpha3":"MYS","numeric":458},
            {"name":"MALDIVES","alpha2":"MV","alpha3":"MDV","numeric":462},
            {"name":"MALI","alpha2":"ML","alpha3":"MLI","numeric":466},
            {"name":"MALTA","alpha2":"MT","alpha3":"MLT","numeric":470},
            {"name":"MARSHALL ISLANDS","alpha2":"MH","alpha3":"MHL","numeric":584},
            {"name":"MARTINIQUE","alpha2":"MQ","alpha3":"MTQ","numeric":474},
            {"name":"MAURITANIA","alpha2":"MR","alpha3":"MRT","numeric":478},
            {"name":"MAURITIUS","alpha2":"MU","alpha3":"MUS","numeric":480},
            {"name":"MAYOTTE","alpha2":"YT","alpha3":"MYT","numeric":175},
            {"name":"MEXICO","alpha2":"MX","alpha3":"MEX","numeric":484},
            {"name":"FEDERATED STATES OF MICRONESIA","alpha2":"FM","alpha3":"FSM","numeric":583},
            {"name":"REPUBLIC OF MOLDOVA","alpha2":"MD","alpha3":"MDA","numeric":498},
            {"name":"MONACO","alpha2":"MC","alpha3":"MCO","numeric":492},
            {"name":"MONGOLIA","alpha2":"MN","alpha3":"MNG","numeric":496},
            {"name":"MONTSERRAT","alpha2":"MS","alpha3":"MSR","numeric":500},
            {"name":"MOROCCO","alpha2":"MA","alpha3":"MAR","numeric":504},
            {"name":"MOZAMBIQUE","alpha2":"MZ","alpha3":"MOZ","numeric":508},
            {"name":"MYANMAR","alpha2":"MM","alpha3":"MMR","numeric":104},
            {"name":"NAMIBIA","alpha2":"NA","alpha3":"NAM","numeric":516},
            {"name":"NAURU","alpha2":"NR","alpha3":"NRU","numeric":520},
            {"name":"NEPAL","alpha2":"NP","alpha3":"NPL","numeric":524},
            {"name":"NETHERLANDS","alpha2":"NL","alpha3":"NLD","numeric":528},
            {"name":"NETHERLANDS ANTILLES","alpha2":"AN","alpha3":"ANT","numeric":530},
            {"name":"NEW CALEDONIA","alpha2":"NC","alpha3":"NCL","numeric":540},
            {"name":"NEW ZEALAND","alpha2":"NZ","alpha3":"NZL","numeric":554},
            {"name":"NICARAGUA","alpha2":"NI","alpha3":"NIC","numeric":558},
            {"name":"NIGER","alpha2":"NE","alpha3":"NER","numeric":562},
            {"name":"NIGERIA","alpha2":"NG","alpha3":"NGA","numeric":566},
            {"name":"NIUE","alpha2":"NU","alpha3":"NIU","numeric":570},
            {"name":"NORFOLK ISLAND","alpha2":"NF","alpha3":"NFK","numeric":574},
            {"name":"NORTHERN MARIANA ISLANDS","alpha2":"MP","alpha3":"MNP","numeric":580},
            {"name":"NORWAY","alpha2":"NO","alpha3":"NOR","numeric":578},
            {"name":"OMAN","alpha2":"OM","alpha3":"OMN","numeric":512},
            {"name":"PAKISTAN","alpha2":"PK","alpha3":"PAK","numeric":586},
            {"name":"PALAU","alpha2":"PW","alpha3":"PLW","numeric":585},
            {"name":"PALESTINIAN TERRITORY","alpha2":"PS","alpha3":"PSE","numeric":275},
            {"name":"PANAMA","alpha2":"PA","alpha3":"PAN","numeric":591},
            {"name":"PAPUA NEW GUINEA","alpha2":"PG","alpha3":"PNG","numeric":598},
            {"name":"PARAGUAY","alpha2":"PY","alpha3":"PRY","numeric":600},
            {"name":"PERU","alpha2":"PE","alpha3":"PER","numeric":604},
            {"name":"PHILIPPINES","alpha2":"PH","alpha3":"PHL","numeric":608},
            {"name":"PITCAIRN","alpha2":"PN","alpha3":"PCN","numeric":612},
            {"name":"POLAND","alpha2":"PL","alpha3":"POL","numeric":616},
            {"name":"PORTUGAL","alpha2":"PT","alpha3":"PRT","numeric":620},
            {"name":"PUERTO RICO","alpha2":"PR","alpha3":"PRI","numeric":630},
            {"name":"QATAR","alpha2":"QA","alpha3":"QAT","numeric":634},
            {"name":"REUNION","alpha2":"RE","alpha3":"REU","numeric":638},
            {"name":"ROMANIA","alpha2":"RO","alpha3":"ROU","numeric":642},
            {"name":"RUSSIAN FEDERATION","alpha2":"RU","alpha3":"RUS","numeric":643},
            {"name":"RWANDA","alpha2":"RW","alpha3":"RWA","numeric":646},
            {"name":"SAINT HELENA","alpha2":"SH","alpha3":"SHN","numeric":654},
            {"name":"SAINT KITTS AND NEVIS","alpha2":"KN","alpha3":"KNA","numeric":659},
            {"name":"SAINT LUCIA","alpha2":"LC","alpha3":"LCA","numeric":662},
            {"name":"SAINT PIERRE AND MIQUELON","alpha2":"PM","alpha3":"SPM","numeric":666},
            {"name":"SAINT VINCENT AND THE GRENADINES","alpha2":"VC","alpha3":"VCT","numeric":670},
            {"name":"SAMOA","alpha2":"WS","alpha3":"WSM","numeric":882},
            {"name":"SAN MARINO","alpha2":"SM","alpha3":"SMR","numeric":674},
            {"name":"SAO TOME AND PRINCIPE","alpha2":"ST","alpha3":"STP","numeric":678},
            {"name":"SAUDI ARABIA","alpha2":"SA","alpha3":"SAU","numeric":682},
            {"name":"SENEGAL","alpha2":"SN","alpha3":"SEN","numeric":686},
            {"name":"SERBIA AND MONTENEGRO","alpha2":"CS","alpha3":"SCG","numeric":891},
            {"name":"SEYCHELLES","alpha2":"SC","alpha3":"SYC","numeric":690},
            {"name":"SIERRA LEONE","alpha2":"SL","alpha3":"SLE","numeric":694},
            {"name":"SINGAPORE","alpha2":"SG","alpha3":"SGP","numeric":702},
            {"name":"SLOVAKIA","alpha2":"SK","alpha3":"SVK","numeric":703},
            {"name":"SLOVENIA","alpha2":"SI","alpha3":"SVN","numeric":705},
            {"name":"SOLOMON ISLANDS","alpha2":"SB","alpha3":"SLB","numeric":90},
            {"name":"SOMALIA","alpha2":"SO","alpha3":"SOM","numeric":706},
            {"name":"SOUTH AFRICA","alpha2":"ZA","alpha3":"ZAF","numeric":710},
            {"name":"SOUTH GEORGIA AND THE SOUTH SANDWICH ISLANDS","alpha2":"GS","alpha3":"SGS","numeric":239},
            {"name":"SPAIN","alpha2":"ES","alpha3":"ESP","numeric":724},
            {"name":"SRI LANKA","alpha2":"LK","alpha3":"LKA","numeric":144},
            {"name":"SUDAN","alpha2":"SD","alpha3":"SDN","numeric":736},
            {"name":"SURINAME","alpha2":"SR","alpha3":"SUR","numeric":740},
            {"name":"SVALBARD AND JAN MAYEN ISLANDS","alpha2":"SJ","alpha3":"SJM","numeric":744},
            {"name":"SWAZILAND","alpha2":"SZ","alpha3":"SWZ","numeric":748},
            {"name":"SWEDEN","alpha2":"SE","alpha3":"SWE","numeric":752},
            {"name":"SWITZERLAND","alpha2":"CH","alpha3":"CHE","numeric":756},
            {"name":"SYRIAN ARAB REPUBLIC","alpha2":"SY","alpha3":"SYR","numeric":760},
            {"name":"TAIWAN","alpha2":"TW","alpha3":"TWN","numeric":158},
            {"name":"TAJIKISTAN","alpha2":"TJ","alpha3":"TJK","numeric":762},
            {"name":"UNITED REPUBLIC OF TANZANIA","alpha2":"TZ","alpha3":"TZA","numeric":834},
            {"name":"THAILAND","alpha2":"TH","alpha3":"THA","numeric":764},
            {"name":"TIMOR-LESTE","alpha2":"TL","alpha3":"TLS","numeric":626},
            {"name":"TOGO","alpha2":"TG","alpha3":"TGO","numeric":768},
            {"name":"TOKELAU","alpha2":"TK","alpha3":"TKL","numeric":772},
            {"name":"TONGA","alpha2":"TO","alpha3":"TON","numeric":776},
            {"name":"TRINIDAD AND TOBAGO","alpha2":"TT","alpha3":"TTO","numeric":780},
            {"name":"TUNISIA","alpha2":"TN","alpha3":"TUN","numeric":788},
            {"name":"TURKEY","alpha2":"TR","alpha3":"TUR","numeric":792},
            {"name":"TURKMENISTAN","alpha2":"TM","alpha3":"TKM","numeric":795},
            {"name":"TURKS AND CAICOS ISLANDS","alpha2":"TC","alpha3":"TCA","numeric":796},
            {"name":"TUVALU","alpha2":"TV","alpha3":"TUV","numeric":798},
            {"name":"UGANDA","alpha2":"UG","alpha3":"UGA","numeric":800},
            {"name":"UKRAINE","alpha2":"UA","alpha3":"UKR","numeric":804},
            {"name":"UNITED ARAB EMIRATES","alpha2":"AE","alpha3":"ARE","numeric":784},
            {"name":"UNITED KINGDOM","alpha2":"GB","alpha3":"GBR","numeric":826},
            {"name":"UNITED STATES","alpha2":"US","alpha3":"USA","numeric":840},
            {"name":"UNITED STATES MINOR OUTLYING ISLANDS","alpha2":"UM","alpha3":"UMI","numeric":581},
            {"name":"URUGUAY","alpha2":"UY","alpha3":"URY","numeric":858},
            {"name":"UZBEKISTAN","alpha2":"UZ","alpha3":"UZB","numeric":860},
            {"name":"VANUATU","alpha2":"VU","alpha3":"VUT","numeric":548},
            {"name":"VATICAN CITY STATE (HOLY SEE)","alpha2":"VA","alpha3":"VAT","numeric":336},
            {"name":"VENEZUELA","alpha2":"VE","alpha3":"VEN","numeric":862},
            {"name":"VIET NAM","alpha2":"VN","alpha3":"VNM","numeric":704},
            {"name":"VIRGIN ISLANDS (BRITISH)","alpha2":"VG","alpha3":"VGB","numeric":92},
            {"name":"VIRGIN ISLANDS (U.S.)","alpha2":"VI","alpha3":"VIR","numeric":850},
            {"name":"WALLIS AND FUTUNA ISLANDS","alpha2":"WF","alpha3":"WLF","numeric":876},
            {"name":"WESTERN SAHARA","alpha2":"EH","alpha3":"ESH","numeric":732},
            {"name":"YEMEN","alpha2":"YE","alpha3":"YEM","numeric":887},
            {"name":"ZAMBIA","alpha2":"ZM","alpha3":"ZMB","numeric":894},
            {"name":"ZIMBABWE","alpha2":"ZW","alpha3":"ZWE","numeric":716}
            ]
            """



-- Serialization ---------------------------------------------------------------


encode :
    { a | alpha2 : String, alpha3 : String, name : String, numeric : Int }
    -> Value
encode { name, alpha2, alpha3, numeric } =
    Encode.object
        [ "name" => Encode.string name
        , "alpha2" => Encode.string alpha2
        , "alpha2" => Encode.string alpha3
        , "numeric" => Encode.int numeric
        ]


decoder : Decoder Country
decoder =
    decode Country
        |> required "name" Decode.string
        |> required "alpha2" Decode.string
        |> required "alpha3" Decode.string
        |> required "numeric" Decode.int

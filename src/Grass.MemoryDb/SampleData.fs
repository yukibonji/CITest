/// Sample data used to populate the event store and read model
module XLCatlin.DataLab.XCBRA.MemoryDb.SampleData

open System
open XLCatlin.DataLab.XCBRA
open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA.EventStore
open XLCatlin.DataLab.XCBRA.ReadModelStore
open XLCatlin.DataLab.XCBRA.Command

type SampleData = 
    { name: string 
    ; addressLines : string 
    ; city :string 
    ; state:string 
    ; postalCode : string 
    ; country: string 
    ; latLong : Option<float * float> 
    ; size : int option 
    ; description: string option 
    }

let swap (a: _[]) x y =
    let tmp = a.[x]
    a.[x] <- a.[y]
    a.[y] <- tmp

// shuffle an array (in-place)
let shuffle (rng:Random) a =
    Array.iteri 
        (fun i _ -> swap a i (rng.Next(i, Array.length a)))
        a


let sampleData : SampleData [] = 
    [|
        {name="YYZ1";   addressLines= "6363 Millcreek Drive";   city= "Mississauga";    state ="ON";    postalCode ="L5N 1L8";  country = "Canada";     latLong = Some (43.5920225,-79.7397711);    size=Some 501700;   description = None }
        {name="YYZ6";   addressLines= "8050 Heritage Road";     city= "Brampton";   state ="ON";    postalCode ="L6Y 0C9";  country = "Canada";     latLong = Some (43.6184196,-79.7927516);    size=Some 868100;   description = None }
        {name="YYZ3";   addressLines= "7995 Winston Churchill Blvd.";   city= "Brampton";   state ="ON";    postalCode ="L6Y 5Z4";  country = "Canada";     latLong = Some (43.608036,-79.7969265);     size=Some 521600;   description = None }
        {name="YYZ2";   addressLines= "2750 Peddie Rd";     city= "Milton";     state ="ON";    postalCode ="L9T 6Y9";  country = "Canada";     latLong = Some (43.5419096,-79.9199261);    size=Some 375200;   description = None }
        {name="YVR3";   addressLines= "109 Braid Street";   city= "New Westminster";    state ="BC";    postalCode ="V3L 5H4";  country = "Canada";     latLong = Some (49.233074,-122.8862376);    size=Some 548000;   description = None }
        {name="YVR2";   addressLines= "450 Derwent Pl.";    city= "Delta";  state ="BC";    postalCode ="V3M 5Y9";  country = "Canada";     latLong = Some (49.1609089,-122.9515654);   size=Some 193500;   description = None }
        {name="CTU1";   addressLines= "ProLogis Industrial Park in the west park of Chengdu Hi-Tech Zone";  city= "Chengdu";    state ="Sichuan";   postalCode ="";     country = "China";  latLong = None;     size=Some 193700;   description = None }
        {name="SHA1";   addressLines= "GLP Park Suzhou Industrial Park";    city= "Suzhou";     state ="Jiangsu";   postalCode ="";     country = "China";  latLong = None;     size=Some 118400;   description = None }
        {name="HRB1";   addressLines= "Haerbin, Heilongjiang";  city= "Haerbin";    state ="Heilongjiang";  postalCode ="";     country = "China";  latLong = None;     size=Some 86000;    description = None }
        {name="CAN4";   addressLines= "Huangpu Development Zone";   city= "Guangzhou";  state ="Guangdong";     postalCode ="";     country = "China";  latLong = None;     size=Some 86000;    description = None }
        {name="SHA3";   addressLines= "China (Shanghai) Pilot Free-Trade Zone";     city= "Shanghai";   state ="";  postalCode ="";     country = "China";  latLong = None;     size=None;  description = None }
        {name="WUH1";   addressLines= "Wuhan, Hubei";   city= "Wuhan";  state ="Hubei";     postalCode ="";     country = "China";  latLong = None;     size=Some 300000;   description = None }
        {name="PEK5";   addressLines= "Tongzhou. Beijing";  city= "Tongzhou";   state ="Beijing";   postalCode ="";     country = "China";  latLong = None;     size=Some 322800;   description = None }
        {name="CAN2";   addressLines= "Guangdong Champion Valley Park";     city= "Guangzhou";  state ="Guangdong";     postalCode ="";     country = "China";  latLong = None;     size=Some 1829200;  description = None }
        {name="CAN1";   addressLines= "Huangpu District, Guangzhou, Guangdong";     city= "Guangzhou";  state ="Guangdong";     postalCode ="";     country = "China";  latLong = None;     size=Some 118400;   description = None }
        {name="XMN1";   addressLines= "Xiamen, Fujian";     city= "Xiamen";     state ="Fujian";    postalCode ="";     country = "China";  latLong = None;     size=Some 170000;   description = None }
        {name="XMN2";   addressLines= "Xiamen";     city= "Fujian";     state ="";  postalCode ="";     country = "China";  latLong = None;     size=Some 170000;   description = None }
        {name="PEK3";   addressLines= "Yizhuang. Daxing. Beijing";  city= "Yizhuang. Daxing";   state ="Beijing";   postalCode ="";     country = "China";  latLong = None;     size=Some 538000;   description = None }
        {name="CTU2";   addressLines= "Tianfu New Area";    city= "Chengdu";    state ="Sichuan";   postalCode ="";     country = "China";  latLong = None;     size=Some 538000;   description = None }
        {name="TSN2";   addressLines= "Wuqing. Tianjin";    city= "Wuqing";     state ="Tianjin";   postalCode ="";     country = "China";  latLong = None;     size=Some 17646;    description = None }
        {name="SHE1";   addressLines= "Shenyang. Liaoning";     city= "Shenyang";   state ="Liaoning";  postalCode ="";     country = "China";  latLong = None;     size=Some 625800;   description = None }
        {name="XIY1";   addressLines= "Xi'ian, Shaanxi";    city= "Xi'ian";     state ="Shaanxi";   postalCode ="";     country = "China";  latLong = None;     size=Some 625800;   description = None }
        {name="SHA2";   addressLines= "HuaQiao Near Shanghai";  city= "Kunshan";    state ="Jiangsu";   postalCode ="";     country = "China";  latLong = None;     size=Some 1291200;  description = None }
        {name="NNG1";   addressLines= "Beibu Gulf Technopark";  city= "Nanning";    state ="Guangxi";   postalCode ="";     country = "China";  latLong = None;     size=Some 538000;   description = None }
        {name="PRG2";   addressLines= "K Amazonu 256";  city= "Dobrovfz";   state ="";  postalCode ="25261";    country = "Czech Republic";     latLong = Some (50.1072186,14.2135569);     size=Some 1022200;  description = None }
        {name="PRG1";   addressLines= "U Trati 216";    city= "Dobrovfz";   state ="";  postalCode ="25261";    country = "Czech Republic";     latLong = Some (50.1077771,14.2119517);     size=Some 269000;   description = None }
        {name="MRS1";   addressLines= "Building 2, Rue Joseph Garde. ZAC Les Portes de Provence";   city= "Montelimar";     state ="Drome";     postalCode ="26200";    country = "France";     latLong = Some (44.5284451,4.737516);   size=Some 495000;   description = None }
        {name="ORY1";   addressLines= "Pole 45 1401 Rue Champ Rouge";   city= "Saran";  state ="Loiret";    postalCode ="45770";    country = "France";     latLong = Some (47.9620428,1.84207);    size=Some 753200;   description = None }
        {name="LIL1";   addressLines= "Rue de la Plaine 59553";     city= "Nord-Pas-de-Calais";     state ="Nord-Pas-de-Calais";    postalCode ="59553";    country = "France";     latLong = Some (50.392496,3.0205861);   size=Some 968400;   description = None }
        {name="LYS1";   addressLines= "Distripole Chalon, ZAC du Parc d'Activite du Val de Bourgogne, 1 Rue Ama";   city= "Sevrey, Saone-et-Loire";     state ="Burgundy";  postalCode ="71100";    country = "France";     latLong = Some (46.7370666,4.8485756);  size=Some 430400;   description = None }
        {name="LEJ1";   addressLines= "Bücherstraße 1";     city= "Leipzig";    state ="Saxony";    postalCode ="4347";     country = "Germany";    latLong = Some (51.36255,12.4504813);   size=Some 807000;   description = None }
        {name="BER3";   addressLines= "Amazon Logistik Potsdam GmbH HavellandsraRe 5";  city= "Brieselang";     state ="Brandenburg";   postalCode ="14656";    country = "Germany";    latLong = Some (52.6059735,12.9744685);     size=Some 742400;   description = None }
        {name="HAM1";   addressLines= "Benzstraße 10,";     city= "Winsen";     state ="Lower Saxony";  postalCode ="21423";    country = "Germany";    latLong = Some (53.3316971,10.2197618);     size=Some 688600;   description = None }
        {name="FRA1";   addressLines= "Am Schloss Eichhof 1, Bad Hersfeld -Schloss Eichhof";    city= "Bad Hersfeld";   state ="Hessen";    postalCode ="36251";    country = "Germany";    latLong = Some (50.8458102,9.6763725);  size=Some 451900;   description = None }
        {name="FRA3";   addressLines= "Amazonstrasse 1, Bad Hersfeld -Obere Kuehnbach";     city= "Bad Hersfeld";   state ="Hessen";    postalCode ="36251";    country = "Germany";    latLong = Some (50.8453138,9.675892);   size=Some 11836;    description = None }
        {name="DUS2";   addressLines= "Amazonstrasse 1 / Alte Landstrasse";     city= "Rheinberg";  state ="North Rhine-Westphalia";    postalCode ="47495";    country = "Germany";    latLong = Some (51.5367188,6.5849559);  size=Some 11836;    description = None }
        {name="EDE6";   addressLines= "Wahrbrink 25";   city= "Werne";  state ="North Rhine-Westphalia";    postalCode ="59368";    country = "Germany";    latLong = Some (51.6591841,7.6049675);  size=Some 10357;    description = None }
        {name="STR1";   addressLines= "Amazonstrasse 1 / Bauschlotter Strasse";     city= "Pforzheim";  state ="Baden-Wurttemberg";     postalCode ="75177";    country = "Germany";    latLong = Some (48.9238404,8.7222521);  size=Some 11836;    description = None }
        {name="MUC3";   addressLines= "Amazonstrasse 1 / Zeppelinstrasse 2";    city= "Graben";     state ="Bavaria";   postalCode ="86836";    country = "Germany";    latLong = Some (48.1997193,10.8476601);     size=Some 11836;    description = None }
        {name="SDEA";   addressLines= "49, Block B, Mohan Cooperative Industrial Estate";   city= "Badarpur, South Delhi";  state ="Delhii";    postalCode ="110076";   country = "India";  latLong = Some (28.5182586,77.2894889);     size=Some 52500;    description = None }
        {name="DEL2";   addressLines= "50, Patparganj Industrial Area";     city= "Patparganj";     state ="Delhi,";    postalCode ="110092";   country = "India";  latLong = Some (28.6386039,77.3030877);     size=Some 52500;    description = None }
        {name="DEL3";   addressLines= "Unit No. 1, Khewat/ Khata No: 373/ 400 Mustatil No. 31, Major District Road 132,";   city= "Tauru";  state ="Haryana";   postalCode ="122015";   country = "India";  latLong = Some (28.225675,76.943822);   size=Some 52500;    description = None }
        {name="LUH1";   addressLines= "";   city= "Ludhiana";   state ="Punjab";    postalCode ="141418";   country = "India";  latLong = Some (30.830944,76.0717015);  size=Some 50000;    description = None }
        {name="BLR6";   addressLines= "Brigade Gateway, 8th Floor, 26/1, Dr. Rajkumar Road, Malleshwaram West";     city= "Bengaluru, Bangalore";   state ="Karnataka";     postalCode ="560055";   country = "India";  latLong = Some (13.01218,77.5550697);   size=Some 52500;    description = None }
        {name="SMAA";   addressLines= "78/26, Azad Nagar";  city= "Chennai";    state ="Tamil Nadu";    postalCode ="600014";   country = "India";  latLong = Some (13.0499852,80.2585602);     size=Some 52500;    description = None }
        {name="HYD1";   addressLines= "Penjrala. Mahbubnagar District";     city= "Kothur, Hyderabad";  state ="Telangana";     postalCode ="";     country = "India";  latLong = Some (17.1504648,78.2872663);     size=Some 280000;   description = None }
        {name="SJAA";   addressLines= "Kumasiya, Jhotwara Industrial Area, Jhotwara";   city= "Jaipur";     state ="Rajasthan";     postalCode ="";     country = "India";  latLong = Some (26.9489339,75.7590576);     size=Some 7000;     description = None }
        {name="MXP5";   addressLines= "Amazon Italia Logistica S.r.L, Strada Dogana Po 2 U";    city= "Castel San Giovanni";    state ="Piacenza";  postalCode ="29015";    country = "Italy";  latLong = Some (45.0772546,9.4524898);  size=Some 925400;   description = None }
        {name="FSZ1";   addressLines= "4-5-1 Ogi-cho";  city= "Odawara-shi";    state ="Kanagawa";  postalCode ="250-0001";     country = "Japan";  latLong = Some (35.2701787,139.1626305);    size=Some 2132000;  description = None }
        {name="NRT1";   addressLines= "Amazon Ichikawa FC, 2-13-1 Shiohama";    city= "Ichikawa-shi";   state ="Chiba";     postalCode ="272-0127";     country = "Japan";  latLong = Some (35.6685842,139.9240895);    size=Some 670300;   description = None }
        {name="NRT2";   addressLines= "2036 Kamikoya";  city= "Yachiyo-shi";    state ="Chiba";     postalCode ="276-0022";     country = "Japan";  latLong = Some (35.7317713,140.1252669);    size=Some 367400;   description = None }
        {name="NRT5";   addressLines= "1-10-15 Minamidai";  city= "Kawagoe-shi";    state ="Saitama";   postalCode ="350-1165";     country = "Japan";  latLong = Some (35.8852562,139.4481222);    size=Some 418900;   description = None }
        {name="KIX2";   addressLines= "2-1-1 Midorigaoka";  city= "Daito-shi";  state ="Osaka";     postalCode ="574-8531";     country = "Japan";  latLong = Some (34.7127877,135.6314895);    size=Some 271200;   description = None }
        {name="KIX1";   addressLines= "138-7 Chikkoyawata-machi";   city= "Sakai-Ku, Sakai-shi";    state ="Osaka";     postalCode ="590-0901";     country = "Japan";  latLong = Some (34.6043274,135.4504894);    size=Some 730900;   description = None }
        {name="HSG1";   addressLines= "3-1-3 Yayoigaoka";   city= "Tosu-shi";   state ="Saga";  postalCode ="841-0005";     country = "Japan";  latLong = Some (33.4105801,130.5200856);    size=Some 244000;   description = None }
        {name="MEX1";   addressLines= "Camino a Coacalco 11A";  city= "Cuautitlan Izcalli";     state ="Estado de Mexico";  postalCode ="54715";    country = "Mexico";     latLong = Some (19.6967373,-99.2175383);    size=Some 400000;   description = None }
        {name="P0Z1";   addressLines= "ul. Jezynowa. 62-080";   city= "Sady";   state ="Poznan";    postalCode ="";     country = "Poland";     latLong = Some (52.4491637,16.7046046);     size=Some 1081800;  description = None }
        {name="WR02";   addressLines= "ul. Logistyczna, 55-040";    city= "Bielany Wroclawskie";    state ="Wroclaw";   postalCode ="";     country = "Poland";     latLong = Some (51.037354,16.943174);   size=Some 12804;    description = None }
        {name="BCN1";   addressLines= "Carrer Alta Ribagorça, 10";  city= "El Prat de Llobregat";   state ="Barcelona";     postalCode ="8820";     country = "Spain";  latLong = Some (41.3123515,2.0726746);  size=Some 645600;   description = None }
        {name="MAD4";   addressLines= "Avenida de la Astronomia 24, Madrid Puerta de San Fernando";     city= "San Fernando de Henares";    state ="Madrid";    postalCode ="28830";    country = "Spain";  latLong = Some (40.4463509,-3.4999451);  size=Some 827700;   description = None }
        {name="DCR1";   addressLines= "Units 2 - 4, Access 23, Purley Way, 4-8 Queensway";  city= "Croydon";    state ="South London";  postalCode ="CRO 4BD";  country = "United Kingdom";     latLong = Some (51.3609746,-0.1214652);     size=Some 74500;    description = None }
        {name="LBA3";   addressLines= "time business centre, 1 Watervole Way";  city= "Doncaster";  state ="South Yorkshire";   postalCode ="DN4 5JP";  country = "United Kingdom";     latLong = Some (53.5013328,-1.1337778);     size=Some 250000;   description = None }
        {name="LBA1";   addressLines= "Firstpoint Business Park, Unit 1, Balby Carr Bank,";     city= "Doncaster";  state ="South Yorkshire";   postalCode ="DN4 5JS";  country = "United Kingdom";     latLong = Some (53.5010524,-1.1335251);     size=Some 415000;   description = None }
        {name="LBA2";   addressLines= "Balby Car Bank";     city= "Doncaster ";     state ="South Yorkshire";   postalCode ="DN4 5JT";  country = "United Kingdom";     latLong = Some (53.499678,-1.1254468);  size=None;  description = None }
        {name="LTN2";   addressLines= "Boundary Way";   city= "Hemel Hempsted";     state ="Hertfordshire";     postalCode ="HP2 7LF";  country = "United Kingdom";     latLong = Some (51.7619229,-0.4279945);     size=Some 450000;   description = None }
        {name="EDI4";   addressLines= "Amazon Way";     city= "Dunfermline";    state ="Fife (Glenrothes)";     postalCode ="KY11 8ST";     country = "United Kingdom";     latLong = Some (56.0627169,-3.3915339);     size=Some 1000;     description = None }
        {name="DNG1";   addressLines= "Interlink Way E, Bardon Hill";   city= "Coalville";  state ="Leicestershire";    postalCode ="LE67 1LB";     country = "United Kingdom";     latLong = Some (52.6976411,-1.3348203);     size=Some 1000000;  description = None }
        {name="LTN4";   addressLines= "Boscombe Road, DC1, Prologis Park Dunstable";    city= "Dunstable";  state ="Bedfordshire";  postalCode ="LU5 4RW";  country = "United Kingdom";     latLong = Some (51.891504,-0.5083366);  size=Some 310300;   description = None }
        {name="LTN1";   addressLines= "Ridgmont, Marston Gate";     city= "Ridgmont, Marston Gate";     state ="Bedfordshire";  postalCode ="MK43 0ZA";     country = "United Kingdom";     latLong = Some (52.0292962,-0.5944669);     size=Some 500000;   description = None }
        {name="BHX3";   addressLines= "Royal Oak Distribution Centre,  4 Royal Oak Way";    city= "Daventry";   state ="East Midlands";     postalCode ="NN11 8QL";     country = "United Kingdom";     latLong = Some (52.2656504,-1.1877364);     size=Some 600000;   description = None }
        {name="GLA1";   addressLines= "2 Cloch Road";   city= "Faulds Park, Gourock";   state ="Inverclyde";    postalCode ="PA19 1BQ";     country = "United Kingdom";     latLong = Some (55.94388,-4.8674583);   size=Some 300000;   description = None }
        {name="EUK5";   addressLines= "Phase 2. Kingston Park Flaxley Road";    city= "Peterborough";   state ="Cambridgeshire";    postalCode ="PE29EN";   country = "United Kingdom";     latLong = Some (52.5425746,-0.2395319);     size=Some 545000;   description = None }
        {name="LCY1";   addressLines= "London Distribution Park, Windrush Road";    city= "Tilbury";    state ="Essex";     postalCode ="RM18 7AN";     country = "United Kingdom";     latLong = Some (51.467747,0.3491533);   size=Some 200000;   description = None }
        {name="CWL1";   addressLines= "Ffordd Amazon, Crymlyn Burrows";     city= "Jersey Marine";  state ="Swansea Bay";   postalCode ="SA1 8QX";  country = "United Kingdom";     latLong = Some (51.6244725,-3.8662725);     size=Some 800000;   description = None }
        {name="MAN1";   addressLines= "6 Sunbank Ln";   city= "Altrincham";     state ="Manchester";    postalCode ="WA15 0PT";     country = "United Kingdom";     latLong = Some (53.3602412,-2.3023936);     size=Some 654000;   description = None }
        {name="DXB1";   addressLines= "Britania point, Patent Dr";  city= "Wednesbury"; state ="West Midlands"; postalCode ="WS10 7XB"; country = "United Kingdom"; latLong = Some (52.555266,-2.0374012);  size=Some 45500; description=None }
        {name="BHX1";   addressLines= "Towers Business Park, Wheelhouse Road";  city= "Rugeley";    state ="Staffordshire"; postalCode ="WS15 1LX"; country = "United Kingdom"; latLong = Some (52.7538179,-1.9227874); size=Some 700000;   description=None }
        {name="LB A";   addressLines= "Balby Car Bank";  city= "Doncaster";  state ="South Yorkshire";   postalCode ="DN4 5JT"; country = "United Kingdom"; latLong = Some (53.4927706,-1.1249685); size=Some 1000000;  description = None  }
        {name="DB02";   addressLines= "500 Sprague St.";    city= "Dedham"; state ="MA";    postalCode ="02026";    country = "United States";  latLong = Some (42.2341006,-71.1426264);    size=Some 60500;    description= Some "Delivery Station for Boston" }
        {name="BOS5";   addressLines= "1000 Technology Center Drive";   city= "Stoughton";  state ="MA";    postalCode ="02072";    country = "United States";  latLong = Some (42.148944,-71.0640117); size=Some 332700;   description= Some "Sortation Center for Boston Market" }
        {name="DB01";   addressLines= "201 Beacham Street"; city= "Everett (Chelsea)";  state ="MA";    postalCode ="02149";    country = "United States";  latLong = Some (42.3939114,-71.054881); size=Some 96600;    description= Some "Delivery Station for Boston" }
        {name="BOS6";   addressLines= "201 Beachman St";    city= "Everett";    state ="MA";    postalCode ="02149";    country = "United States";  latLong = Some (42.3943148,-71.0551717);    size=Some 96600;    description= Some "Amazon.Fresh for Boston Market" }
        {name="BOS7";   addressLines= "1180 Innovation Way";    city= "Fall River"; state ="MA";    postalCode ="02722";    country = "United States";  latLong = Some (41.7627676,-71.1032802);    size=Some 1000000;  description= Some "Small Sortable" }
        {name="BOS1";   addressLines= "10 State St";    city= "Nashua"; state ="NH";    postalCode ="03063";    country = "United States";  latLong = Some (42.791972,-71.5317197); size=Some 63750;    description = Some" Small Sortable" }
        {name="BDL1";   addressLines= "200 Old Iron Ore Rd";    city= "Windsor";    state ="CT";    postalCode ="06095";    country = "United States";  latLong = Some (41.8724489,-72.7103621);    size=Some 1017500;  description= Some "Large Items" }
        {name="BDL5";   addressLines= "29 Research Parkway";    city= "Wallingford";    state ="CT";    postalCode ="06492";    country = "United States";  latLong = Some (41.4959958,-72.7620238);    size=Some 184100;   description= Some "Sortation Center for the Windsor CT Market" }
        {name="EWR6";   addressLines= "275 Omar Avenue";    city= "Avenel"; state ="NJ";    postalCode ="07001";    country = "United States";  latLong = Some (40.5831076,-74.2593812);    size=Some 391700;   description= Some "Amazon.Fresh Distribution Center" }
        {name="EWR5";   addressLines= "301 Blair Road #100";    city= "Avenel"; state ="NJ";    postalCode ="07001";    country = "United States";  latLong = Some (40.5829485,-74.2575607);    size=Some 562200;   description= Some "Sortation Center and Returns Center" }
        {name="EWR9";   addressLines= "8003 Industrial Highway";    city= "Carteret";   state ="NJ";    postalCode ="07008";    country = "United States";  latLong = Some (40.5932827,-74.2260783);    size=Some 1017500;  description= Some "Amazon Pantry Formerly a Wakefern full-line grocery distribution center. 500 associates" }
        {name="LGA8";   addressLines= "380 Middlesex Avenue";   city= "Carteret";   state ="NJ";    postalCode ="07008";    country = "United States";  latLong = Some (40.5640444,-74.2232193);    size=Some 161400;   description= Some "Amazon Pantry (FDFC) Formerly a Goya Distribution Center" }
        {name="LGA7";   addressLines= "380 Middlesex Avenue";   city= "Carteret";   state ="NJ";    postalCode ="07008";    country = "United States";  latLong = Some (40.5640444,-74.2232193);    size=Some 794400;   description= Some "Amazon.Fresh Formerly a White Rose Grocery Distribution Center." }
        {name="DEW1";   addressLines= "2 Empire Boulevard"; city= "Moonachie";  state ="NJ";    postalCode ="07074";    country = "United States";  latLong = Some (40.8337734,-74.0528511);    size=Some 75000;    description= Some "Delivery Station for New Jersey" }
        {name="EWR8";   addressLines= "698 Route 46 West";  city= "Teterboro";  state ="NJ";    postalCode ="07608";    country = "United States";  latLong = Some (40.8645071,-74.0603802);    size=Some 617000;   description= Some "Not Open" }
        {name="ACY5";   addressLines= "2277 Center Square Rd";  city= "Logan Township"; state ="NJ";    postalCode ="08085";    country = "United States";  latLong = Some (39.7759758,-75.3796274);    size=Some 202900;   description = Some" Sortation Center for New Jersey Market" }
        {name="ABE8";   addressLines= "309 Cedar Lane"; city= "Florence";   state ="NJ";    postalCode ="08518";    country = "United States";  latLong = Some (40.1108611,-74.798848); size=Some 613000;   description = None  }
        {name="EWR4";   addressLines= "50 New Canton Way";  city= "Robbinsville";   state ="NJ";    postalCode ="08691";    country = "United States";  latLong = Some (40.1957862,-74.5676958);    size=Some 1200000;  description = Some" Books and DVDs" }
        {name="JFK7";   addressLines= "7 W. 34th St.";  city= "New York";   state ="NY";    postalCode ="10001";    country = "United States";  latLong = Some (40.7491533,-73.9873529);    size=Some 40000;    description = Some" Prime Now Hub for Manhattan" }
        {name="PIT5";   addressLines= "2250 Roswell Drive"; city= "Pittsburg";  state ="PA";    postalCode ="15205";    country = "United States";  latLong = Some (40.4440196,-80.0844278);    size=Some 250000;   description = Some" Sortation Center for Pittsburg Region" }
        {name="PHL6";   addressLines= "675 Allen Rd";   city= "Carlisle";   state ="PA";    postalCode ="17015";    country = "United States";  latLong = Some (40.1821236,-77.2329923);    size=Some 1206500;  description = Some" Large Non-Sortable" }
        {name="MDT1";   addressLines= "2 Ames Drive";   city= "Carlisle";   state ="PA";    postalCode ="17015";    country = "United States";  latLong = Some (40.1663469,-77.2438681);    size=Some 750000;   description = Some" Large Non-Sortable" }
        {name="PHL4";   addressLines= "21 Roadway Dr";  city= "Carlisle";   state ="PA";    postalCode ="17015";    country = "United States";  latLong = Some (40.2270866,-77.115469); size=Some 558700;   description = Some" Large Non-Sortable" }
        {name="PHL5";   addressLines= "500 McCarthy Dr. Fairview Business Park";    city= "Lewisberry"; state ="PA";    postalCode ="17339";    country = "United States";  latLong = Some (40.1714603,-76.8390548);    size=Some 750000;   description = Some" Large Non-Sortable" }
        {name="ABE2";   addressLines= "705 Boulder Drive";  city= "Breinigsville";  state ="PA";    postalCode ="18031";    country = "United States";  latLong = Some (40.5578407,-75.6174766);    size=Some 600000;   description = Some" Large Sortable" }
        {name="ABE4";   addressLines= "1610 Van Buren Road";    city= "Easton"; state ="PA";    postalCode ="18045";    country = "United States";  latLong = Some (40.7396822,-75.2791324);    size=Some 1106400;  description = Some" Large Sortable" }
        {name="AVP1";   addressLines= "550 Oak Ridge Road"; city= "Hazleton";   state ="PA";    postalCode ="18202";    country = "United States";  latLong = Some (40.9222117,-76.0513544);    size=Some 630000;   description = Some" Redistribution Center" }
        {name="AVP6";   addressLines= "1 Commerce Rd";  city= "Pittston";   state ="PA";    postalCode ="18640";    country = "United States";  latLong = Some (41.3280871,-75.7228564);    size=Some 437440;   description = Some" Fulfillment Center" }
        {name="DPH1";   addressLines= "4219 Richmond Street";   city= "Philadelphia";   state ="PA";    postalCode ="19137";    country = "United States";  latLong = Some (39.9945063,-75.0780535);    size=Some 65500;    description = Some" Delivery Station for Portland" }
        {name="PHL7";   addressLines= "560 Merrimac Ave";   city= "Middletown"; state ="DE";    postalCode ="19709";    country = "United States";  latLong = Some (39.439333,-75.7339491);   size=Some 1200000;  description = None  }
        {name="BWI1";   addressLines= "45121 Global Plaza"; city= "Sterling";   state ="VA";    postalCode ="20166";    country = "United States";  latLong = Some (38.9860544,-77.4420369);    size=Some 80900;    description = Some" Amazon Pantry (FDFC)" }
        {name="DDC2";   addressLines= "861 E Gude Drive";   city= "Rockville";  state ="MD";    postalCode ="20850";    country = "United States";  latLong = Some (39.0996728,-77.1431878);    size=Some 65000;    description = Some" Delivery Station Washington DC" }
        {name="BWI2";   addressLines= "2010 Broening Highway";  city= "Baltimore";  state ="MD";    postalCode ="21224";    country = "United States";  latLong = Some (39.2672765,-76.5508294);    size=Some 1017550;  description = Some" Small Sortable" }
        {name="BWI5";   addressLines= "5001 Holabird Ave."; city= "Baltimore";  state ="MD";    postalCode ="21224";    country = "United States";  latLong = Some (39.2705556,-76.550079); size=Some 345000;   description = Some" Sortation Center connected by conveyor to the BWI2 Fulfillment Center" }
        {name="BWI6";   addressLines= "5617 Industrial Drive";  city= "Springfield";    state ="VA";    postalCode ="22151";    country = "United States";  latLong = Some (38.798454,-77.1718596); size=Some 127300;   description = Some" Prime Now Hub for Washington DC" }
        {name="UVA3";   addressLines= "2034 Dabney Road";   city= "Richmond";   state ="VA";    postalCode ="23230";    country = "United States";  latLong = Some (37.5837327,-77.4838813);    size=Some 11900;    description = Some" Prime Now Hub for Richmond, VA" }
        {name="RIC1";   addressLines= "5000 Commerce Way";  city= "Petersburg"; state ="VA";    postalCode ="23803";    country = "United States";  latLong = Some (37.1947281,-77.4951982);    size=Some 1100000;  description = Some" Large Packages" }
        {name="RIC2";   addressLines= "1901 Meadowville Technology Pkwy";   city= "Chester";    state ="VA";    postalCode ="23836";    country = "United States";  latLong = Some (37.352147,-77.3290513); size=Some 1100000;  description = Some" Auto parts, electronics, office supplies" }
        {name="UNC2";   addressLines= "3200 Bush Street";   city= "Raleigh";    state ="NC";    postalCode ="27609";    country = "United States";  latLong = Some (35.823934,-78.6155307); size=Some 30000;    description = Some" Prime Now Hub for Raleigh" }
        {name="RDU5";   addressLines= "1805 TW Alexander Drive";    city= "Durham"; state ="NC";    postalCode ="27703";    country = "United States";  latLong = Some (35.922731,-78.8341396); size=Some 324500;   description = Some" Sortation Center for North Carolina" }
        {name="CLT5";   addressLines= "1745 Derita Rd.";    city= "Concord";    state ="NC";    postalCode ="28027";    country = "United States";  latLong = Some (35.3891364,-80.720459); size=Some 222500;   description = Some" Sortation Center for North Carolina 360 people employed" }
        {name="CLT2";   addressLines= "10240 Old Dowd Road";    city= "Charlotte";  state ="NC";    postalCode ="28214";    country = "United States";  latLong = Some (35.24593,-80.9989317);  size=Some 397800;   description = Some" Inbound Cross Dock" }
        {name="CAE1";   addressLines= "4400 12 Street Extension";   city= "West Columbia";  state ="SC";    postalCode ="29172";    country = "United States";  latLong = Some (33.9113987,-81.0523143);    size=Some 1250000;  description = Some" Small Sortable" }
        {name="GSP1";   addressLines= "402 John Dodd Rd";   city= "Spartanburg";    state ="SC";    postalCode ="29303";    country = "United States";  latLong = Some (35.0080756,-82.037581); size=Some 1016100;  description = Some" Large Non-Sortable" }
        {name="CHS1";   addressLines= "7290 Investment Drive";  city= "N Charleston";   state ="SC";    postalCode ="29418";    country = "United States";  latLong = Some (32.92479,-80.0618587);  size=Some 128200;   description = Some" Food Distibrution" }
        {name="ATL8";   addressLines= "2201 Thorton Road";  city= "Lithia Springs"; state ="GA";    postalCode ="30122";    country = "United States";  latLong = Some (33.7444627,-84.5946557);    size=Some 733200;   description = Some" Sortation Center" }
        {name="ATL7";   addressLines= "6855 Shannon Pkwy";  city= "Union City"; state ="GA";    postalCode ="30291";    country = "United States";  latLong = Some (33.5641417,-84.5439556);    size=Some 517100;   description = Some" Amazon Pantry" }
        {name="VUAT";   addressLines= "6855 Shannon Pkwy";  city= "Union City"; state ="GA";    postalCode ="30291";    country = "United States";  latLong = Some (33.5641417,-84.5439556);    size=Some 744000;   description = Some" Supplemental" }
        {name="UGA1";   addressLines= "2302 Marietta Blvd NW";  city= "Atlanta";    state ="GA";    postalCode ="30318";    country = "United States";  latLong = Some (33.8180172,-84.4516111);    size=None;  description = Some" Prime Now Hub Atlanta" }
        {name="ATL6";   addressLines= "4200 North Commerce Dr.";    city= "East Point"; state ="GA";    postalCode ="30344";    country = "United States";  latLong = Some (33.643533,-84.5023884); size=Some 301200;   description = Some" Sortation Center to service Atlanta market" }
        {name="MGE1";   addressLines= "650 Broadway Ave";   city= "Braselton";  state ="GA";    postalCode ="30517";    country = "United States";  latLong = Some (34.1057472,-83.7755657);    size=Some 600000;   description = Some" Large Non-Sortable" }
        {name="UFL4";   addressLines= "7469 Kingspointe Parkway";   city= "Orlando";    state ="FL";    postalCode ="32819";    country = "United States";  latLong = Some (28.4594663,-81.4417144);    size=Some 96000;    description = Some" Prime Now Hub/Delivery Station for Orlando" }
        {name="DMI3";   addressLines= "3200 Northwest 67th Avenue"; city= "Miami";  state ="FL";    postalCode ="33122";    country = "United States";  latLong = Some (25.8115485,-80.3083123);    size=Some 117200;   description = Some" Sortation Center located next to Miami International Airport Regional Air Hub Facility" }
        {name="DM11";   addressLines= "15600 NW 15th Ave."; city= "Miami Gardens";  state ="FL";    postalCode ="33169";    country = "United States";  latLong = Some (25.9175682,-80.2287831);    size=Some 37400;    description = Some" Delivery Station for Miami area" }
        {name="MIA5";   addressLines= "1900 NW 132nd Place";    city= "Miami";  state ="FL";    postalCode ="33182";    country = "United States";  latLong = Some (25.7925672,-80.4122867);    size=Some 335800;   description = Some" Sortation Center located due west of Miami International Airport" }
        {name="TPA1";   addressLines= "3350 Laurel Ridge Ave";  city= "Ruskin"; state ="FL";    postalCode ="33570";    country = "United States";  latLong = Some (27.727387,-82.390111);  size=Some 1100000;  description = Some" Small Sortable" }
        {name="TPA2";   addressLines= "1760 County Line Rd.";   city= "Lakeland";   state ="FL";    postalCode ="33811";    country = "United States";  latLong = Some (28.0202158,-82.0559576);    size=Some 1000000;  description = Some" Large non-sortable" }
        {name="MC05";   addressLines= "205 Deen Still Road";    city= "Davenport";  state ="FL";    postalCode ="33897";    country = "United States";  latLong = Some (28.2513925,-81.6648558);    size=Some 270000;   description = Some" Sortation Center to service Tampa Bay & Lakeland FCs" }
        {name="BNA2";   addressLines= "500 Duke Dr";    city= "Lebanon";    state ="TN";    postalCode ="37090";    country = "United States";  latLong = Some (36.1202013,-86.4118266);    size=Some 1200000;  description = Some" Small Sortable" }
        {name="BNA3";   addressLines= "2020 Joe B Jackson Pkwy";    city= "Murfreesboro";   state ="TN";    postalCode ="37127";    country = "United States";  latLong = Some (35.7778345,-86.367011); size=Some 1000000;  description = Some" Larger Sortable" }
        {name="BNA5";   addressLines= "50 Airways Blvd.";   city= "Nashville";  state ="TN";    postalCode ="37217";    country = "United States";  latLong = Some (36.1299286,-86.6939972);    size=Some 214000;   description = Some" Sortation Center for Nashville" }
        {name="CHA2";   addressLines= "225 Infinity Dr NW"; city= "Charleston"; state ="TN";    postalCode ="37310";    country = "United States";  latLong = Some (35.282446,-84.814381);  size=Some 1200000;  description = Some" Large Non-Sortable" }
        {name="CHA1";   addressLines= "7200 Discovery Drive";   city= "Chattanooga";    state ="TN";    postalCode ="37416";    country = "United States";  latLong = Some (35.0682171,-85.1434756);    size=Some 1020000;  description = Some" Large Sortable" }
        {name="SDF9";   addressLines= "100 W. Thomas P. Echols Lane";   city= "Shepherdsville"; state ="KY";    postalCode ="40165";    country = "United States";  latLong = Some (37.9835977,-85.6907576);    size=Some 600000;   description = Some" Returns Center" }
        {name="SDF6";   addressLines= "271 Omega Pkwy"; city= "Shepherdsville"; state ="KY";    postalCode ="40165";    country = "United States";  latLong = Some (37.9783371,-85.6903498);    size=Some 118000;   description = Some" Fashion, Returns Center" }
        {name="SDF4";   addressLines= "376 Zappos.com Blvd";    city= "Shepherdsville"; state ="KY";    postalCode ="40165";    country = "United States";  latLong = Some (37.9801038,-85.6871537);    size=Some 823000;   description = Some" Zappos.com" }
        {name="LEX1";   addressLines= "1850 Mercer Drive";  city= "Lexington";  state ="KY";    postalCode ="40511";    country = "United States";  latLong = Some (38.076042,-84.5357608); size=Some 604000;   description = Some" Large Sortable; Returns Center" }
        {name="LEX2";   addressLines= "172 Trade St.";  city= "Lexington";  state ="KY";    postalCode ="40511";    country = "United States";  latLong = Some (38.074561,-84.551372);  size=Some 380000;   description = Some" Returns Center" }
        {name="CVG1";   addressLines= "1155 Worldwide Blvd.";   city= "Hebron"; state ="KY";    postalCode ="41048";    country = "United States";  latLong = Some (39.0849703,-84.7210432);    size=Some 484000;   description = Some" Shoes & Purses, Endless.com" }
        {name="CVG3";   addressLines= "3680 Langley Dr.";   city= "Hebron"; state ="KY";    postalCode ="41048";    country = "United States";  latLong = Some (39.062105,-84.7186916); size=Some 711400;   description = Some" Redistribution Center" }
        {name="CVG2";   addressLines= "1600 Worldwide Blvd.";   city= "Hebron"; state ="KY";    postalCode ="41048";    country = "United States";  latLong = Some (39.0866701,-84.7282505);    size=Some 543000;   description = Some" Returns Center" }
        {name="SDF1";   addressLines= "1050 South Columbia";    city= "Campbellsville"; state ="KY";    postalCode ="42718";    country = "United States";  latLong = Some (37.3253867,-85.3525207);    size=Some 770000;   description = Some" Large Sortable" }
        {name="CMH1";   addressLines= "11999 National Road SW"; city= "Etna";   state ="OH";    postalCode ="43062";    country = "United States";  latLong = Some (39.9536204,-82.7144206);    size=Some 855000;   description = Some" Small Sortable" }
        {name="CMH2";   addressLines= "6050 Gateway Court"; city= "Groveport";  state ="OH";    postalCode ="43125";    country = "United States";  latLong = Some (39.8992454,-82.962122); size=Some 1000000;  description = Some" Large Sortable" }
        {name="UOH2";   addressLines= "3563 Interchange Rd";    city= "Columbus";   state ="OH";    postalCode ="43204";    country = "United States";  latLong = Some (39.972207,-83.1024887); size=Some 45700;    description = Some" Prime Now Hub for Columbus, OH" }
        {name="CLE5";   addressLines= "8685 Independence Parkway";  city= "Twinsburg";  state ="OH";    postalCode ="44087";    country = "United States";  latLong = Some (41.3058204,-81.4691873);    size=Some 248000;   description = Some" Sortation Center for Cleveland 300 people to be employed" }
        {name="IND1";   addressLines= "4255 Anson Blvd";    city= "Whitestown"; state ="IN";    postalCode ="46075";    country = "United States";  latLong = Some (39.9796565,-86.381657); size=Some 1000000;  description = Some" Large & Small Sortable" }
        {name="XUSE";   addressLines= "5460 Industial Court Suite 300"; city= "Whitestown"; state ="IN";    postalCode ="46075";    country = "United States";  latLong = Some (39.9661812,-86.3841775);    size=Some 624000;   description = Some" Large Non-sortable" }
        {name="IND5";   addressLines= "800 Perry Road"; city= "Plainfield"; state ="IN";    postalCode ="46168";    country = "United States";  latLong = Some (39.6947616,-86.3597319);    size=Some 925800;   description = Some" Large Non-Sortable" }
        {name="IND2";   addressLines= "715 Airtech Pkwy";   city= "Plainfield"; state ="IN";    postalCode ="46168";    country = "United States";  latLong = Some (39.7002576,-86.3446763);    size=Some 947300;   description = Some" Large Non-Sortable" }
        {name="IND4";   addressLines= "710 S. Girls School Rd"; city= "Indianapolis";   state ="IN";    postalCode ="46231";    country = "United States";  latLong = Some (39.7563552,-86.2920375);    size=Some 902850;   description = Some" Text Books" }
        {name="SDF8";   addressLines= "900 Patrol Rd";  city= "Jeffersonville"; state ="IN";    postalCode ="47130";    country = "United States";  latLong = Some (38.3787703,-85.6889709);    size=Some 1200000;  description = Some" Specialty Apparel Site; shoes, watches & jewelry" }
        {name="DTW1";   addressLines= "13000 Eckles Road";  city= "Livonia";    state ="MI";    postalCode ="48150";    country = "United States";  latLong = Some (42.3868071,-83.4330708);    size=Some 1000000;  description = Some" Small Sortable" }
        {name="DTW5";   addressLines= "19991 Brownstown Center Drive";  city= "Trenton";    state ="MI";    postalCode ="48183";    country = "United States";  latLong = Some (42.1596338,-83.2395458);    size=Some 210000;   description = Some" Sortation Center for Detroit Market" }
        {name="DML1";   addressLines= "3935 W. Mitchell St.";   city= "Milwaukee";  state ="WI";    postalCode ="53125";    country = "United States";  latLong = Some (43.0117858,-87.9654698);    size=Some 50000;    description = Some" Delivery Station for Milwaukee" }
        {name="MKE5";   addressLines= "11211 Burlington Road";  city= "Kenosha";    state ="WI";    postalCode ="53144";    country = "United States";  latLong = Some (42.6108217,-87.9451565);    size=Some 600000;   description = Some" Sortation Center for Milwaukee Market 500 people employed" }
        {name="MKE1";   addressLines= "3501 120th Ave"; city= "Kenosha";    state ="WI";    postalCode ="53144";    country = "United States";  latLong = Some (42.6074522,-87.9501237);    size=Some 1100000;  description = Some" Small Sortable" }
        {name="DMN1";   addressLines= "2811 Beverly Drive"; city= "Eagen";  state ="MN";    postalCode ="55121";    country = "United States";  latLong = Some (44.852868,-93.1375311); size=Some 142000;   description = Some" Delivery Station for Minneapolis/St. Paul" }
        {name="MSP5";   addressLines= "5825 11th Ave. East";    city= "Shakopee";   state ="MN";    postalCode ="55379";    country = "United States";  latLong = Some (44.788284,-93.460413);  size=Some 162000;   description = Some" Sortation Center for Minneapolis Market" }
        {name="MSP1";   addressLines= "2601 4th Avenue East";   city= "Shakopee";   state ="MN";    postalCode ="55379";    country = "United States";  latLong = Some (44.7952313,-93.486695); size=Some 820000;   description = Some" Small Sortable" }
        {name="UMN1";   addressLines= "763 Kasota Ave.";    city= "Minneapolis";    state ="MN";    postalCode ="55414";    country = "United States";  latLong = Some (44.9826163,-93.2172444);    size=Some 35000;    description = Some" Prime Now Hub for Minneapolis/St Paul" }
        {name="DCH2";   addressLines= "8220 Austin Ave.";   city= "Morton Grove";   state ="IL";    postalCode ="60053";    country = "United States";  latLong = Some (42.0317982,-87.7834101);    size=Some 39300;    description = Some" Delivery Station for North Chicago area" }
        {name="ORD6";   addressLines= "1250 Mittel Blvd";   city= "Wood Dale";  state ="IL";    postalCode ="60191";    country = "United States";  latLong = Some (41.9870119,-87.9896398);    size=Some 82100;    description = Some" Amazon.Fresh Refrigerated Food Distribution Center" }
        {name="MDW4";   addressLines= "201 EMERALD DR"; city= "Joliet"; state ="IL";    postalCode ="60433";    country = "United States";  latLong = Some (41.4864293,-88.0731184);    size=Some 1000000;  description = Some" New $75M project to open in 2017" }
        {name="MDW2";   addressLines= "250 Emerald Drive";  city= "Joliet"; state ="IL";    postalCode ="60433";    country = "United States";  latLong = Some (41.483053,-88.0720149); size=Some 475104;   description = Some" Redistribution Center" }
        {name="MDW6";   addressLines= "1125 W REMINGTON BLVD";  city= "Romeoville"; state ="IL";    postalCode ="60446";    country = "United States";  latLong = Some (41.6721579,-88.1176978);    size=Some 767273;   description = None  }
        {name="MDW7";   addressLines= "6521 W Monee Manhattan Road";    city= "Monee";  state ="IL";    postalCode ="60449";    country = "United States";  latLong = Some (41.4251238,-87.7725009);    size=Some 856600;   description = Some" Small Sortable" }
        {name="DCH3";   addressLines= "4500 Western Ave.";  city= "Lisle";  state ="IL";    postalCode ="60532";    country = "United States";  latLong = Some (41.8032735,-88.0993532);    size=Some 68000;    description = Some" Delivery Station for Chicago" }
        {name="DCH1";   addressLines= "2801 S. Western Ave.";   city= "Chicago";    state ="IL";    postalCode ="60608";    country = "United States";  latLong = Some (41.8404604,-87.6857453);    size=Some 150000;   description = Some" Delivery Station service Chicago market" }
        {name="UIL1";   addressLines= "1111 N. Cherry Ave.";    city= "Goose Island";   state ="IL";    postalCode ="60642";    country = "United States";  latLong = Some (41.9019965,-87.654422); size=Some 52000;    description = Some" Amazon Prime" }
        {name="STL6";   addressLines= "3931 Lakeview Corporate Drive";  city= "Edwardsville";   state ="IL";    postalCode ="62025";    country = "United States";  latLong = Some (38.786073,-90.0778638); size=Some 700000;   description = Some" Large Non-Sortable" }
        {name="MKC4";   addressLines= "19645 Waverly Rd";   city= "Edgerton";   state ="KS";    postalCode ="66021";    country = "United States";  latLong = Some (38.7759366,-94.9470341);    size=Some 822100;   description = Some" Large Non-sortable" }
        {name="MCI7";   addressLines= "27200 W 157th St";   city= "New Century";    state ="KS";    postalCode ="66031";    country = "United States";  latLong = Some (38.8449862,-94.9036866);    size=Some 446500;   description = None  }
        {name="MCI5";   addressLines= "16851 W 113th St";   city= "Lenexa"; state ="KS";    postalCode ="66219";    country = "United States";  latLong = Some (38.921271,-94.7848777); size=None;  description = Some" Not Open" }
        {name="DFW6";   addressLines= "940 W Bethel Road";  city= "Coppell";    state ="TX";    postalCode ="75019";    country = "United States";  latLong = Some (32.9533723,-97.024299); size=Some 1000000;  description = Some" Large Non-Sortable, smaller items 500,000 sq ft." }
        {name="DFW8";   addressLines= "2700 Regent Blvd.";  city= "Irving"; state ="TX";    postalCode ="75063";    country = "United States";  latLong = Some (32.9396797,-97.0305082);    size=Some 428500;   description = Some" Sortation Center for Dallas/Fort Worth" }
        {name="DDC1";   addressLines= "12401 North Stemmons Fwy.";  city= "Farmers Branch"; state ="TX";    postalCode ="75234";    country = "United States";  latLong = Some (32.9189669,-96.9027847);    size=Some 183000;   description = Some" Delivery Station for Dalllas" }
        {name="FTW1";   addressLines= "33333 LBJ Freeway";  city= "Dallas"; state ="TX";    postalCode ="75241";    country = "United States";  latLong = Some (32.6610034,-96.7382958);    size=Some 500000;   description = Some" Redistribution Center" }
        {name="FTW2";   addressLines= "2701 Bethel Road";   city= "DeSoto"; state ="TX";    postalCode ="75261";    country = "United States";  latLong = Some (32.9524902,-97.025672); size=Some 1000000;  description = Some" Small Sortable" }
        {name="FTW4";   addressLines= "4601 Gold Spike Drive";  city= "Fort Worth"; state ="TX";    postalCode ="76106";    country = "United States";  latLong = Some (32.8292565,-97.3573214);    size=Some 318500;   description = Some" Amazon.Fresh Distribution Center" }
        {name="FTW3";   addressLines= "15201 Heritage Pkwy";    city= "Fort Worth"; state ="TX";    postalCode ="76177";    country = "United States";  latLong = Some (33.0090143,-97.2973912);    size=Some 1000000;  description = Some" Small Sortable" }
        {name="DFW7";   addressLines= "700 Westport Parkway";   city= "Fort Worth"; state ="TX";    postalCode ="76177";    country = "United States";  latLong = Some (32.9705437,-97.3379442);    size=Some 1100000;  description = Some" Kiva, Small Sortable" }
        {name="HOU1";   addressLines= "8120 Humble Westfield Rd.";  city= "Humble"; state ="TX";    postalCode ="77338";    country = "United States";  latLong = Some (29.9998579,-95.3044506);    size=Some 240000;   description = Some" Sortation Center for Houston Market" }
        {name="SAT1";   addressLines= "6000 Enterprise Avenue"; city= "Schertz";    state ="TX";    postalCode ="78154";    country = "United States";  latLong = Some (29.5994668,-98.2918586);    size=Some 1260000;  description = Some" Larege Sortable" }
        {name="SAT5";   addressLines= "1410 S. Callaghan Road"; city= "Bexar. San Antonio"; state ="TX";    postalCode ="78227";    country = "United States";  latLong = Some (29.4206276,-98.5996772);    size=Some 193900;   description = Some" Sortation Center for San Antonio Market" }
        {name="SAT2";   addressLines= "1401 E McCarty Lane";    city= "San Marcos"; state ="TX";    postalCode ="78666";    country = "United States";  latLong = Some (29.8356458,-97.9672401);    size=Some 885000;   description = Some" Fulfillment Center" }
        {name="DAU1";   addressLines= "2093-2209 Rutland Dr";   city= "Austin"; state ="TX";    postalCode ="78758";    country = "United States";  latLong = Some (30.3806257,-97.7181183);    size=None;  description = Some" Delivery Station" }
        {name="DEN5";   addressLines= "19799 E. 36th Drive";    city= "Aurora"; state ="CO";    postalCode ="80011";    country = "United States";  latLong = Some (39.7668311,-104.757924);    size=Some 452400;   description = Some" First Amazon facility in Colorado" }
        {name="DPX1";   addressLines= "500 S. 48th Street"; city= "Phoenix";    state ="AZ";    postalCode ="85034";    country = "United States";  latLong = Some (33.4430978,-111.9810869);   size=Some 62900;    description = Some" Delivery Station for Phoenix" }
        {name="PHX3";   addressLines= "6835 West Buckeye Road"; city= "Pheonix";    state ="AZ";    postalCode ="85043";    country = "United States";  latLong = Some (33.433327,-112.2099731);    size=Some 1005000;  description = Some" Large Sortable" }
        {name="PHX7";   addressLines= "800 N 75th Ave"; city= "Pheonix";    state ="AZ";    postalCode ="85043";    country = "United States";  latLong = Some (33.4570939,-112.2275611);   size=Some 1260000;  description = Some" Small Sortable and Non-Sortable" }
        {name="PHX6";   addressLines= "4750 West Mohave St";    city= "Pheonix";    state ="AZ";    postalCode ="85043";    country = "United States";  latLong = Some (33.4325959,-112.1669107);   size=Some 1207000;  description = Some" Large Sortable" }
        {name="XUSH";   addressLines= "7037 West Van Buren Street"; city= "Pheonix";    state ="AZ";    postalCode ="85043";    country = "United States";  latLong = Some (33.450087,-112.2122697);    size=None;  description = None  }
        {name="PHX5";   addressLines= "16980 W Commerce Drive"; city= "Goodyear";   state ="AZ";    postalCode ="85338";    country = "United States";  latLong = Some (33.4095704,-112.4241542);   size=Some 1200000;  description = Some" Non-Sortable" }
        {name="PHX9";   addressLines= "777 S 79th Ave"; city= "Tolleson";   state ="AZ";    postalCode ="85353";    country = "United States";  latLong = Some (33.4413095,-112.2310927);   size=None;  description = None  }
        {name="LAS2";   addressLines= "3837 Bay Lake Trail, Suite 111 North";   city= "Las Vegas";  state ="NV";    postalCode ="89030";    country = "United States";  latLong = Some (36.2286233,-115.1075518);   size=Some 283920;   description = Some" Small Sortable, Returns Center" }
        {name="RNO1";   addressLines= "1600 East Newlands Drive";   city= "Fernley";    state ="NV";    postalCode ="89408";    country = "United States";  latLong = Some (39.6103605,-119.2145092);   size=Some 786000;   description = Some" Big Sortable" }
        {name="RNO4";   addressLines= "8000 North Virginia Street"; city= "Reno";   state ="NV";    postalCode ="89506";    country = "United States";  latLong = Some (39.6038771,-119.8505984);   size=Some 634000;   description = Some" Big Sortable, Small Sortable" }
        {name="UCA3";   addressLines= "11800 W Olympic Blvd.";  city= "Los Angeles";    state ="CA";    postalCode ="90064";    country = "United States";  latLong = Some (34.0338721,-118.4471838);   size=Some 28800;    description = Some" Prime Now Hub for Santa Monica" }
        {name="DLA8";   addressLines= "2815 W. El Segundo Blvd.";   city= "Hawthorne";  state ="CA";    postalCode ="90250";    country = "United States";  latLong = Some (33.9180008,-118.3269552);   size=Some 170000;   description = Some" Delivery Station for Los Angeles" }
        {name="DLA1";   addressLines= "900 W Florence Ave.";    city= "Inglewood";  state ="CA";    postalCode ="90301";    country = "United States";  latLong = Some (33.9620691,-118.3726662);   size=Some 27740;    description = Some" Delivery Station for Los Angeles" }
        {name="DLA2";   addressLines= "5650 Dolly Ave.";    city= "Buena Park"; state ="CA";    postalCode ="90621";    country = "United States";  latLong = Some (33.880313,-118.0096117);    size=Some 330000;   description = Some" Delivery Station for Anaheim" }
        {name="VUKF";   addressLines= "18120 Bishop Ave";   city= "Carson"; state ="CA";    postalCode ="90746";    country = "United States";  latLong = Some (33.865608,-118.244509); size=None;  description = None  }
        {name="DLA4";   addressLines= "9031 Lurline Ave.";  city= "Chatsworth"; state ="CA";    postalCode ="91311";    country = "United States";  latLong = Some (34.2343899,-118.5853907);   size=Some 29200;    description = Some" Delivery Station for Burbank area" }
        {name="DSD2";   addressLines= "2777 Loker Ave W.";  city= "Carlsbad";   state ="CA";    postalCode ="92010";    country = "United States";  latLong = Some (33.1310808,-117.2567962);   size=Some 39700;    description = Some" Delivery Station for San Diego" }
        {name="UCA6";   addressLines= "2727 Kurtz St."; city= "San Diego";  state ="CA";    postalCode ="92110";    country = "United States";  latLong = Some (32.7496358,-117.2017215);   size=Some 37800;    description = Some" Prime Now Hub for San Diego" }
        {name="DSD1";   addressLines= "7130 Miramar Rd. 300A";  city= "San Diego";  state ="CA";    postalCode ="92121";    country = "United States";  latLong = Some (32.88161,-117.1615884); size=Some 58400;    description = Some" Delivery Station for NorthEast Bay Area" }
        {name="VUPQ";   addressLines= "9211 Kaiser Way";    city= "Fontana";    state ="CA";    postalCode ="92335";    country = "United States";  latLong = Some (34.085363,-117.5200589);    size=None;  description = None  }
        {name="ONT9";   addressLines= "2125 West San Bernardino Ave";   city= "Redlands";   state ="CA";    postalCode ="92374";    country = "United States";  latLong = Some (34.0757543,-117.2305927);   size=Some 700000;   description = Some" Non-Sortable" }
        {name="SNA4";   addressLines= "2496 W Walnut Ave";  city= "Rialto"; state ="CA";    postalCode ="92376";    country = "United States";  latLong = Some (34.1313068,-117.4225899);   size=Some 882200;   description = Some" Large Non-Sortable" }
        {name="ONT2";   addressLines= "1910 E Central Ave"; city= "San Bernardino"; state ="CA";    postalCode ="92408";    country = "United States";  latLong = Some (34.0894698,-117.2427208);   size=Some 951700;   description = Some" Small Sortable" }
        {name="ONT5";   addressLines= "2020 E. Central Ave. Bldg. 4";   city= "San Bernardino"; state ="CA";    postalCode ="92408";    country = "United States";  latLong = Some (34.0884272,-117.2466121);   size=Some 514600;   description = Some" ONT5 is a Sortation Center for the Los Angeles region" }
        {name="DLA5";   addressLines= "6250 Sycamore Canyon Blvd."; city= "Riverside";  state ="CA";    postalCode ="92507";    country = "United States";  latLong = Some (33.9181798,-117.2979505);   size=Some 36000;    description = Some" Delivery Station for Riverside area" }
        {name="ONT8";   addressLines= "24300 Nandina Ave";  city= "Moreno Valley";  state ="CA";    postalCode ="92551";    country = "United States";  latLong = Some (33.8691564,-117.2378397);   size=Some 769300;   description = Some" Redistribution Center" }
        {name="ONT6";   addressLines= "24208 San Michele Rd";   city= "Moreno Valley";  state ="CA";    postalCode ="92551";    country = "United States";  latLong = Some (33.872071,-117.2389735);    size=Some 1250000;  description = Some" Small Sortable" }
        {name="UCA4";   addressLines= "2006 McGaw Ave.";    city= "Irvine"; state ="CA";    postalCode ="92614";    country = "United States";  latLong = Some (33.69054,-117.8468731); size=Some 45300;    description = Some" Prime Now Hub for Irvine, CA" }
        {name="SNA6";   addressLines= "5250 Goodman Road";  city= "Eastvale";   state ="CA";    postalCode ="92880";    country = "United States";  latLong = Some (33.9933257,-117.5550004);   size=Some 1007700;  description = None  }
        {name="DSF5";   addressLines= "250 Utah Avenue";    city= "South San Francisco";    state ="CA";    postalCode ="94080";    country = "United States";  latLong = Some (37.6463483,-122.3975331);   size=Some 188000;   description = Some" Delivery Station for South San Francisco" }
        {name="DSF6";   addressLines= "6015 Giant Road";    city= "Richmond";   state ="CA";    postalCode ="94080";    country = "United States";  latLong = Some (37.9986783,-122.3511247);   size=Some 224200;   description = Some" Delivery Station for NorthEast Bay Area" }
        {name="UCA7";   addressLines= "222 Commercial Street";  city= "Sunnyvale";  state ="CA";    postalCode ="94085";    country = "United States";  latLong = Some (37.3785196,-122.0094164);   size=Some 24000;    description = Some" Prime Now Hub for San Jose" }
        {name="UCA1";   addressLines= "888 Tennessee Street";   city= "San Francisco";  state ="CA";    postalCode ="94112";    country = "United States";  latLong = Some (37.7604332,-122.391828);    size=Some 39000;    description = Some" Prime Now Hub for San Francisco" }
        {name="OAK8";   addressLines= "1 Middleton Way";    city= "American Canyon";    state ="CA";    postalCode ="94503";    country = "United States";  latLong = Some (38.2052664,-122.2669937);   size=None;  description = None  }
        {name="OAK5";   addressLines= "38811 Cherry Street";    city= "Newark"; state ="CA";    postalCode ="94560";    country = "United States";  latLong = Some (37.5197389,-122.0174759);   size=Some 574650;   description = Some" Sortation Center to service the San Francisco Bay area." }
        {name="OAK7";   addressLines= "38811 Cherry St";    city= "Newark"; state ="CA";    postalCode ="94560";    country = "United States";  latLong = Some (37.5197389,-122.0174759);   size=None;  description = Some" Amazon Prime Pantry 200 people" }
        {name="DSF1";   addressLines= "990 Beecher St.";    city= "San Leandro";    state ="CA";    postalCode ="94577";    country = "United States";  latLong = Some (37.7203121,-122.1854414);   size=Some 42400;    description = Some" Delivery Station for Oakland" }
        {name="DSF3";   addressLines= "1700 Montague Expy.";    city= "San Jose";   state ="CA";    postalCode ="95131";    country = "United States";  latLong = Some (37.4028955,-121.8987148);   size=Some 25800;    description = Some" Delivery Station for San Jose" }
        {name="XUSD";   addressLines= "1909 Zephyr St"; city= "Stockton";   state ="CA";    postalCode ="95206";    country = "United States";  latLong = Some (37.9180245,-121.2530862);   size=Some 508000;   description = Some" Large Non-sortable" }
        {name="OAK4";   addressLines= "1350 N. MacArthur Drive";    city= "Tracy";  state ="CA";    postalCode ="95304";    country = "United States";  latLong = Some (37.7442387,-121.407982);    size=None;  description = Some" OAK6 is Amazon.Fresh for the San Francisco Bay Area Sq. Ft. Listed in OAK4" }
        {name="OAK3";   addressLines= "255 Park Center Drive";  city= "Patterson";  state ="CA";    postalCode ="95363";    country = "United States";  latLong = Some (37.4689743,-121.1687386);   size=Some 1000000;  description = Some" Kiva Automated, Large Sortable" }
        {name="PCA1";   addressLines= "1565 N MacArthur Drive"; city= "Tracy";  state ="CA";    postalCode ="95376";    country = "United States";  latLong = Some (37.7426467,-121.4112818);   size=None;  description = None  }
        {name="SJC7";   addressLines= "188 Mountain House Parkway"; city= "Tracy";  state ="CA";    postalCode ="95391";    country = "United States";  latLong = Some (37.7248766,-121.5315946);   size=None;  description = None  }
        {name="UCA9";   addressLines= "2934 Ramona Avenue"; city= "Sacramento"; state ="CA";    postalCode ="95826";    country = "United States";  latLong = Some (38.54644,-121.4178602); size=Some 20000;    description = Some" Prime Now Hub for Sacramento" }
        {name="PDX5";   addressLines= "5647 NW Huffman Street"; city= "Washington, Hillsboro";  state ="OR";    postalCode ="97124";    country = "United States";  latLong = Some (45.5575756,-122.9221144);   size=Some 303000;   description = Some" Sortation Center for Portland. OR" }
        {name="UOR1";   addressLines= "3610 NW St Helens Rd";   city= "Portland";   state ="OR";    postalCode ="97210";    country = "United States";  latLong = Some (45.5487199,-122.7253089);   size=None;  description = Some" Prime Now Hub for Portland. OR" }
        {name="BFI4";   addressLines= "21005 64th Ave S";   city= "Kent";   state ="WA";    postalCode ="98032";    country = "United States";  latLong = Some (47.4142267,-122.2581874);   size=Some 1000000;  description = Some" Small Sortable" }
        {name="BFI5";   addressLines= "20529 59th Place South, Building B"; city= "Kent";   state ="WA";    postalCode ="98032";    country = "United States";  latLong = Some (47.4185933,-122.2621502);   size=Some 318200;   description = Some" Sortation Center for Seattle Market" }
        {name="UWA1";   addressLines= "11710 118th Ave. N.E., Building B";  city= "Kirkland";   state ="WA";    postalCode ="98034";    country = "United States";  latLong = Some (47.7052495,-122.1848041);   size=Some 38300;    description = Some" Prime Now Hub for East Seattle" }
        {name="DSE2";   addressLines= "6705 E Marginal Way";    city= "Seattle";    state ="WA";    postalCode ="98108";    country = "United States";  latLong = Some (47.5419975,-122.3285355);   size=Some 26200;    description = Some" Delivery Station for Seattle" }
        {name="UWA3";   addressLines= "2121 6th Ave.";  city= "Seattle";    state ="WA";    postalCode ="98121";    country = "United States";  latLong = Some (47.6162691,-122.3402042);   size=Some 18900;    description = Some" Prime Now Hub for Seattle" }
        {name="UWA2";   addressLines= "13537 Aurora Ave. N.";   city= "North Seattle";  state ="WA";    postalCode ="98133";    country = "United States";  latLong = Some (47.728052,-122.3465077);    size=Some 38400;    description = Some" Prime Now Hub for North Seattle" }
        {name="SEA6";   addressLines= "2646 Rainier Ave. South";    city= "Seattle";    state ="WA";    postalCode ="98144";    country = "United States";  latLong = Some (47.5797029,-122.2993064);   size=Some 313300;   description = Some" Amazon Pantry (FDFC)" }
        {name="BFI3";   addressLines= "2700 Center Drive";  city= "DuPont"; state ="WA";    postalCode ="98327";    country = "United States";  latLong = Some (47.1125829,-122.641148);    size=Some 1000000;  description = Some" Larger Non-Sortable, KIVA Automation" }
        {name="BFI1";   addressLines= "1800 140th Avenue Suite A";  city= "Sumner"; state ="WA";    postalCode ="98390";    country = "United States";  latLong = Some (47.2409972,-122.245727);    size=Some 492000;   description = Some" Small Sortable" }
    |]      










let data : (Location * Warehouse list ) list=
  let rng = System.Random(180180)


  let randomQR = 
    let d = [|Poor;Fair;Good;Excellent|]
    fun i ->
      if i > 3 then d.[3]
      else d.[i]

  let randomOWN = 
    let d = [|Owned;OwnedOccupant;Leased;Tenant|]
    fun i ->
      if i > 3 then d.[3]
      else d.[i]

  do 
      shuffle rng sampleData

  sampleData   
  |> Seq.groupBy (fun d -> d.addressLines, d.city , d.state , d.postalCode , d.country,d.latLong)
  |> Seq.map 
        (fun ((addressLines,city,state,postalCode,country,latLong) , locs) -> 
            let nm = 
                Seq.map (fun d -> d.name) locs 
                |> Seq.reduce (sprintf "%s / %s")

            let desc = 
                Seq.map (fun d -> d.description) locs
                |> Seq.choose id
                |> Seq.tryHead
            
            let geocode = 
                Option.map 
                    (fun (lat , long) ->
                         {latitude  = decimal lat ; longitude = decimal long }

                    )
                    latLong 

            let addr = 
                { postalCountry = country
                ; addressLines = List.ofArray <| addressLines.Split([|','|])
                ; administrativeArea = state
                ; locality = city
                ; dependentLocality = None
                ; postalCode = postalCode
                ; sortingCode = None
                ; languageCode = None 
                }

            let locationId = 
                LocationId <| Guid.NewGuid().ToString()

            let loc = 
                      { id =  locationId 
                      ; name = nm
                      ; description = desc
                      ; address = addr
                      ; geocode = geocode
                      ; siteCondition = Some << randomQR <| rng.Next(4)
                      ; plantLayout = Some << randomQR <| rng.Next(4)
                      ; ownership = Some << randomOWN <| rng.Next(4)
                      ; lastSurveyDate = Some <| DateTime.Now.AddDays(-720.0 * rng.NextDouble())
                      }  

            let buildings = 
                locs 
                |> Seq.map 
                        (fun d -> 
                            let responses = 
                                match d.size with 
                                | Some sqft -> 

                                    let lower = float sqft
                                    let upper = lower + rng.NextDouble() * 0.2 * lower 
                                    let median = lower  + (upper - lower ) * rng.NextDouble()

                                    { WarehouseUserInput.empty with 
                                        floorArea =  
                                            Answered 
                                                { upperBound = 1M<Metre^2> * decimal upper 
                                                ; lowerBound = 1M<Metre^2> *decimal sqft 
                                                ; median = 1M<Metre^2> * decimal median 
                                                }
                                    }

                                | _ -> 
                                    WarehouseUserInput.empty

                            { id = BuildingId <| Guid.NewGuid().ToString()
                            ; locationId = locationId
                            ; name = d.name 
                            ; occupancy = Warehouse
                            ; userInput = responses 
                            }
                        )
                |> Seq.toList 

            loc, buildings 
        )
    |> Seq.toList




// ===================================
// WarehouseEvents
// ===================================

let warehouseCreated id locationId name = 
    WarehouseEvent.Created(id locationId name)


// ===================================
// EventStore/Readmodel set up with this data
// ===================================

let appendLocationEvents (eventStore:IEventStore) locationId (events:LocationEvent list) = 
    let streamId = Command.Location.idToEventStream locationId 
    let writeEvents = events |> List.map Command.Location.toWriteEvent
    let expectedVersion = ExpectedVersion.Any 
    let result = eventStore.AppendEventsToStream streamId expectedVersion writeEvents  
    match result with
    | Ok _ -> 
        ()  //ignore
    | Error data -> 
        let msg = sprintf "Error appendLocationEvents '%A'" data
        failwith msg // should never happen

let appendWarehouseEvents (eventStore:IEventStore) warehouseId (events:WarehouseEvent list) = 
    let streamId = Command.Warehouse.idToEventStream warehouseId  
    let writeEvents = events |> List.map Command.Warehouse.toWriteEvent
    let expectedVersion = ExpectedVersion.Any 
    let result = eventStore.AppendEventsToStream streamId expectedVersion writeEvents  
    match result with
    | Ok _ -> 
        ()  //ignore
    | Error data -> 
        let msg = sprintf "Error appendWarehouseEvents '%A'" data
        failwith msg // should never happen




/// set up an event store initialized with the above events
let eventStore() =
    let utcNow() = DateTime.UtcNow
    let eventStore = MemoryDb.EventStore(utcNow) :> IEventStore
        
    // Create & update each of the fake locations
    do 
        data
        |> Seq.iter 
            (fun (loc, warehouses ) -> 
                let locationEvents = 
                    List.choose id 
                        [ Some <| LocationEvent.Created(loc.id, loc.name, loc.address, loc.geocode)
                        ; Option.map (fun desc -> LocationEvent.DescriptionUpdated(loc.id , desc)) loc.description
                        ; Option.map (fun ownership -> LocationEvent.OwnershipUpdated(loc.id, ownership)) loc.ownership
                        ; Option.map (fun cond -> LocationEvent.SiteConditionUpdated(loc.id, cond)) loc.siteCondition
                        ; Option.map (fun plantLayout -> LocationEvent.PlantLayoutUpdated(loc.id, plantLayout)) loc.plantLayout
                        ; Option.map (fun date -> LocationEvent.LastSurveyDateUpdated(loc.id, date)) loc.lastSurveyDate
                        ]

                let warehouseEvents = 
                    warehouses 
                    |> List.iter 
                            (fun warehouse -> 
                                appendWarehouseEvents eventStore warehouse.id
                                    [ WarehouseEvent.Created(warehouse.id, loc.id, warehouse.name)
                                    ; WarehouseEvent.FloorAreaAnswered(warehouse.id ,warehouse.userInput.floorArea)
                                    ]
                            )
                    
                     
                appendLocationEvents eventStore loc.id locationEvents

            )


    //// Building101
    //let events = [
    //    warehouseCreated buildingA101.Id buildingA101.LocationId buildingA101.Name
    //    ]
    //appendWarehouseEvents eventStore buildingA101.Id events

    //// Building102
    //let events = [
    //    warehouseCreated buildingA102.Id buildingA102.LocationId buildingA102.Name
    //    ]
    //appendWarehouseEvents eventStore buildingA102.Id events

    //// Building201
    //let events = [
    //    warehouseCreated buildingB201.Id buildingB201.LocationId buildingB201.Name
    //    ]
    //appendWarehouseEvents eventStore buildingB201.Id events

    //// Building202
    //let events = [
    //    warehouseCreated buildingB202.Id buildingB202.LocationId buildingB202.Name
    //    ]
    //appendWarehouseEvents eventStore buildingB202.Id events

    // return
    eventStore 




/// set up an event store initialized with the above events
let readModelStore (eventStore:IEventStore) =
    let readModelStore = 
        MemoryDb.ReadModelStore() :> IReadModelStore

    let result = 
        DomainCommandHandler.replayEventsForAllStreams eventStore readModelStore 
    
    match result with
    | Ok _ -> 
        ()  //ignore

    | Error data -> 
        let msg = sprintf "Error replayEventsForAllStreams '%A'" data
        failwith msg // should never happen

    readModelStore 
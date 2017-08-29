IF OBJECT_ID('dbo.LocationAddress', 'U') IS NOT NULL DROP TABLE dbo.LocationAddress; 
CREATE TABLE dbo.LocationAddress(
	-- not used
    LocationId nvarchar(100) NOT NULL
    ,PostalCountry nvarchar(100) NOT NULL
    ,AddressLine1 nvarchar(100) NOT NULL
	,AddressLine2 nvarchar(100) NULL
	,AddressLine3 nvarchar(100) NULL
	,AddressLine4 nvarchar(100) NULL
	,AddressLine5 nvarchar(100) NULL
    ,AdministrativeArea nvarchar(100) NULL
    ,Locality nvarchar(100) NULL
    ,DependentLocality nvarchar(100) NULL
    ,PostalCode nvarchar(100) NOT NULL
    ,SortingCode nvarchar(100) NULL
    ,LanguageCode nvarchar(100) NULL
	,LastUpdated datetime NOT NULL
)


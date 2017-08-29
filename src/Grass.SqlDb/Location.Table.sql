IF OBJECT_ID('dbo.Location', 'U') IS NOT NULL DROP TABLE dbo.Location; 
CREATE TABLE dbo.Location (
    LocationId nvarchar(100) NOT NULL
    ,Name nvarchar(100) NOT NULL
    ,Description nvarchar(100) NOT NULL
	-- Address included here because it's 1:1 with location and required
    ,Address_PostalCountry nvarchar(100) NOT NULL
    ,Address_AddressLine1 nvarchar(100) NOT NULL
	,Address_AddressLine2 nvarchar(100) NULL
	,Address_AddressLine3 nvarchar(100) NULL
	,Address_AddressLine4 nvarchar(100) NULL
	,Address_AddressLine5 nvarchar(100) NULL
    ,Address_AdministrativeArea nvarchar(100) NULL
    ,Address_Locality nvarchar(100) NULL
    ,Address_DependentLocality nvarchar(100) NULL
    ,Address_PostalCode nvarchar(100) NOT NULL
    ,Address_SortingCode nvarchar(100) NULL
    ,Address_LanguageCode nvarchar(100) NULL
	,Geocode_Latitude float NULL
    ,Geocode_Longitude float NULL
	-- BuildingValue in LocationAssetValue with AssetName="Building"
	-- MachineryValue in LocationAssetValue with AssetName="Machinery"
    -- StockValue in LocationAssetValue with AssetName="Stock"
    -- BusinessInterruptionValue in LocationAssetValue with AssetName="BusinessInterruption"
    ,TotalAreaSqM decimal NULL
    ,SiteCondition nvarchar(100) NULL
    ,PlantLayout nvarchar(100) NULL
    ,Ownership nvarchar(100) NULL
	,LastUpdated datetime NOT NULL
)


ALTER TABLE dbo.Location
ADD CONSTRAINT PK_Location PRIMARY KEY CLUSTERED (LocationId);  

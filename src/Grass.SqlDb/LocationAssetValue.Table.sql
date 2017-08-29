IF OBJECT_ID('dbo.LocationAssetValue', 'U') IS NOT NULL DROP TABLE dbo.LocationAssetValue; 
CREATE TABLE dbo.LocationAssetValue(
    LocationId nvarchar(100) NOT NULL
	,AssetName nvarchar(100) NOT NULL
	,CurrencyCode varchar(3) NOT NULL
    ,Value money NOT NULL
	,LastUpdated datetime NOT NULL
);

ALTER TABLE dbo.LocationAssetValue
ADD CONSTRAINT PK_LocationAssetValue PRIMARY KEY CLUSTERED (LocationId,AssetName);  



IF OBJECT_ID('dbo.WarehouseAssetValue', 'U') IS NOT NULL DROP TABLE dbo.WarehouseAssetValue; 
CREATE TABLE dbo.WarehouseAssetValue(
    WarehouseId nvarchar(100) NOT NULL
	,AssetName nvarchar(100) NOT NULL
	,CurrencyCode varchar(3) NOT NULL
    ,Value money NOT NULL
	,LastUpdated datetime NOT NULL
);

ALTER TABLE dbo.WarehouseAssetValue
ADD CONSTRAINT PK_WarehouseAssetValue PRIMARY KEY CLUSTERED (WarehouseId,AssetName);  

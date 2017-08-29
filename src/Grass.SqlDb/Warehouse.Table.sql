IF OBJECT_ID('dbo.Warehouse', 'U') IS NOT NULL DROP TABLE dbo.Warehouse; 
CREATE TABLE dbo.Warehouse(
	WarehouseId nvarchar(100) NOT NULL
	,LocationId nvarchar(100) NOT NULL
	,Name nvarchar(100) NOT NULL
	,Occupancy nvarchar(100) NOT NULL
	,YearBuilt int NULL
	,FloorAreaSqM decimal NULL
	,CompartmentationSqM decimal NULL
	,NumberOfStories int NULL
	,HeightM decimal NULL
	,NearestNeighbourM decimal NULL
	-- RoofConstruction in WarehouseConstructionScore with FeatureName="Roof"
	-- WallConstruction in WarehouseConstructionScore with FeatureName="Wall"
	-- BuildingValue in LocationAssetValue with AssetName="Building"
	-- MachineryValue in LocationAssetValue with AssetName="Machinery"
    -- StockValue in LocationAssetValue with AssetName="Stock"
    -- BusinessInterurptionValue in LocationAssetValue with AssetName="BusinessInterruption"
	,LastUpdated datetime NOT NULL
);

ALTER TABLE dbo.Warehouse
ADD CONSTRAINT PK_Warehouse PRIMARY KEY CLUSTERED (WarehouseId);  



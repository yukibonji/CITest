IF OBJECT_ID('dbo.WarehouseConstructionScore', 'U') IS NOT NULL DROP TABLE dbo.WarehouseConstructionScore; 
CREATE TABLE dbo.WarehouseConstructionScore(
    WarehouseId nvarchar(100) NOT NULL
	,FeatureName nvarchar(100) NOT NULL -- Wall, Roof, etc
	,ConstructionMethod nvarchar(100) NOT NULL -- FireResistant, Combustible, etc
    ,Score decimal NOT NULL
	,LastUpdated datetime NOT NULL
);

ALTER TABLE dbo.WarehouseConstructionScore
ADD CONSTRAINT PK_WarehouseConstructionScore PRIMARY KEY CLUSTERED (WarehouseId,FeatureName,ConstructionMethod);  





ALTER TABLE dbo.LocationAssetValue
ADD CONSTRAINT FK_LocationAssetValue_Location FOREIGN KEY (LocationId)     
    REFERENCES dbo.Location (LocationId)     

ALTER TABLE dbo.Warehouse
ADD CONSTRAINT FK_Warehouse_Location FOREIGN KEY (LocationId)     
    REFERENCES dbo.Location (LocationId)     

ALTER TABLE dbo.WarehouseAssetValue
ADD CONSTRAINT FK_WarehouseAssetValue_Warehouse FOREIGN KEY (WarehouseId)     
    REFERENCES dbo.Warehouse (WarehouseId)     

ALTER TABLE dbo.WarehouseConstructionScore
ADD CONSTRAINT FK_WarehouseConstructionScore_Warehouse FOREIGN KEY (WarehouseId)     
    REFERENCES dbo.Warehouse (WarehouseId)     

IF OBJECT_ID('dbo.RecordedEvent', 'U') IS NOT NULL DROP TABLE dbo.RecordedEvent; 
CREATE TABLE dbo.RecordedEvent (
    StreamId nvarchar(100) NOT NULL
    ,EventId uniqueidentifier NOT NULL
    ,EventNumber int NOT NULL
    ,EventType nvarchar(100) NOT NULL
    ,Data nvarchar(MAX) NOT NULL
    ,Metadata nvarchar(MAX) NOT NULL
    ,Created datetime NOT NULL
);

ALTER TABLE dbo.RecordedEvent 
ADD CONSTRAINT PK_RecordedEvent PRIMARY KEY CLUSTERED (StreamId,EventId);  


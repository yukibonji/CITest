IF OBJECT_ID('dbo.DazzleCalculation', 'U') IS NOT NULL DROP TABLE dbo.DazzleCalculation; 
CREATE TABLE dbo.DazzleCalculation (
    DazzleCalculationId nvarchar(100) NOT NULL
    ,Input nvarchar(MAX) NOT NULL
	,Status nvarchar(100) NOT NULL
    ,Output nvarchar(MAX) NOT NULL
    ,DurationMs int NOT NULL -- if status=InProcess this is time remaining
    ,Started datetime NOT NULL -- set when started
	,Completed datetime NULL -- set when completed
)


ALTER TABLE dbo.DazzleCalculation
ADD CONSTRAINT PK_DazzleCalculation PRIMARY KEY CLUSTERED (DazzleCalculationId);  

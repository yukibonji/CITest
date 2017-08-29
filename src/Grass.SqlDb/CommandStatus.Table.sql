IF OBJECT_ID('dbo.CommandStatus', 'U') IS NOT NULL DROP TABLE dbo.CommandStatus; 
CREATE TABLE dbo.CommandStatus (
    CommandId uniqueidentifier NOT NULL
    ,Status nvarchar(100) NOT NULL -- Pending, Success, Failure
    ,Data nvarchar(MAX) NOT NULL -- In the failure case
    ,Started datetime NOT NULL -- set when started
	,Completed datetime NULL -- set when completed
);

ALTER TABLE dbo.CommandStatus
ADD CONSTRAINT PK_CommandStatus PRIMARY KEY CLUSTERED (CommandId);  



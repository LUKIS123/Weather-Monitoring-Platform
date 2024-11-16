IF (NOT EXISTS (SELECT *
FROM sys.schemas
WHERE name = 'stationsAccess')) 
BEGIN
    EXEC ('CREATE SCHEMA [stationsAccess] AUTHORIZATION [dbo]')
END
GO

CREATE TABLE [stationsAccess].[StationPermissionRequests]
(
    Id INT IDENTITY(1,1),
    UserId NVARCHAR(255) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES [identity].[Users](Id)
    ON DELETE NO ACTION,
    DeviceId INT NOT NULL,
    FOREIGN KEY (DeviceId) REFERENCES [identity].[Devices](Id)
    ON DELETE NO ACTION,
    -- AWAITING, GRANTED, DENIED
    Status INT NOT NULL,
    ChangeDate DATETIME2(0) NOT NULL DEFAULT GETDATE()
)
GO

CREATE TABLE [stationsAccess].[StationsPermissions]
(
    Id INT IDENTITY(1,1),
    UserId NVARCHAR(255) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES [identity].[Users](Id)
    ON DELETE NO ACTION,
    DeviceId INT NOT NULL,
    FOREIGN KEY (DeviceId) REFERENCES [identity].[Devices](Id)
    ON DELETE NO ACTION,
)
GO

IF (NOT EXISTS (SELECT *
FROM sys.schemas
WHERE name = 'weatherData')) 
BEGIN
    EXEC ('CREATE SCHEMA [weatherData] AUTHORIZATION [dbo]')
END

CREATE TABLE [weatherData].[SensorsMeasurements]
(
    Id BIGINT PRIMARY KEY IDENTITY(1,1),
    ReceivedAt DATETIME2(0) NOT NULL DEFAULT GETDATE(),
    Humidity REAL NOT NULL,
    Temperature REAL NOT NULL,
    AirPressure REAL NOT NULL,
    Altitude REAL NOT NULL,
    PM1_0 REAL,
    PM2_5 REAL,
    PM10 REAL,
    DeviceId INT NOT NULL,
    FOREIGN KEY (DeviceId) REFERENCES [identity].[Devices](Id)
    ON DELETE CASCADE,
);
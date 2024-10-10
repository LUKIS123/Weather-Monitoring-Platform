IF (NOT EXISTS (SELECT *
FROM sys.schemas
WHERE name = 'identity')) 
BEGIN
    EXEC ('CREATE SCHEMA [identity] AUTHORIZATION [dbo]')
END

CREATE TABLE [identity].[MqttClients]
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Username NVARCHAR(100) NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    ClientId NVARCHAR(50) NOT NULL,
    IsSuperUser BIT NOT NULL DEFAULT 0,
);

CREATE TABLE [identity].[MqttTopics]
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Topic NVARCHAR(255) NOT NULL,
);

CREATE TABLE [identity].[MqttClientsAllowedTopics]
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ClientId UNIQUEIDENTIFIER NOT NULL,
    TopicId UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (ClientId) REFERENCES [identity].[MqttClients](Id)
    ON DELETE CASCADE,
    FOREIGN KEY (TopicId) REFERENCES [identity].[MqttTopics](Id)
    ON DELETE CASCADE,
);

CREATE TABLE [identity].[MqttClientsTopicsPermissions]
(
    AllowedTopicId UNIQUEIDENTIFIER NOT NULL,
    ActionType INT NOT NULL,
    FOREIGN KEY (AllowedTopicId) REFERENCES [identity].[MqttClientsAllowedTopics](Id)
    ON DELETE CASCADE,
);

CREATE TABLE [identity].[Users]
(
    Id NVARCHAR(255) PRIMARY KEY,
    Role INT NOT NULL,
    MqttClientId UNIQUEIDENTIFIER,
    FOREIGN KEY (MqttClientId) REFERENCES [identity].[MqttClients](Id)
    ON DELETE CASCADE,
);

CREATE TABLE [identity].[Devices]
(
    Id INT IDENTITY(1, 1) PRIMARY KEY,
    GoogleMapsPlusCode NVARCHAR(50) NOT NULL DEFAULT '4356+M6 Wrocław',
    MqttClientId UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (MqttClientId) REFERENCES [identity].[MqttClients](Id)
    ON DELETE CASCADE,
);

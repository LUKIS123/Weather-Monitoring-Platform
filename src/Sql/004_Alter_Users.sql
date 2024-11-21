ALTER TABLE [identity].[Users]
ADD 
    Email NVARCHAR(255) NOT NULL,
    Nickname NVARCHAR(255) NOT NULL DEFAULT 'User';
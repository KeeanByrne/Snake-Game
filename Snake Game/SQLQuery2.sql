CREATE TABLE [User]
(
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50),
    Password NVARCHAR(50), 
    Highscore INT
)
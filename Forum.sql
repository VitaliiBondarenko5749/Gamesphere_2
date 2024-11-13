CREATE DATABASE GameSphereForumDb
GO

USE GameSphereForumDb
GO

CREATE SCHEMA forum
GO

CREATE TABLE forum.Posts
(
Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
UserId NVARCHAR(MAX) NOT NULL,
GameId UNIQUEIDENTIFIER NULL,
Topic NVARCHAR(150) NOT NULL,
Content TEXT NOT NULL,
CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
Views INT NOT NULL DEFAULT 0,
);

CREATE TABLE forum.FavoritePosts
(
Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
UserId NVARCHAR(MAX) NOT NULL,
PostId UNIQUEIDENTIFIER NOT NULL,
FOREIGN KEY (PostId) REFERENCES forum.Posts(Id) ON DELETE CASCADE
);

CREATE TABLE forum.Replies
(
Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
UserId NVARCHAR(MAX) NOT NULL,
PostId UNIQUEIDENTIFIER NOT NULL,
Content TEXT NOT NULL,
CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
ReplyToId UNIQUEIDENTIFIER NULL,
FOREIGN KEY (PostId) REFERENCES forum.Posts(Id) ON DELETE CASCADE,
FOREIGN KEY (ReplyToId) REFERENCES forum.Replies(Id)
);

GO

CREATE TRIGGER TRG_DeleteReplyCascade
ON forum.Replies
AFTER DELETE
AS
BEGIN
    -- Видаляємо всі відповіді, у яких ReplyToId відповідає видаленим записам
    DELETE FROM forum.Replies
    WHERE ReplyToId IN (SELECT Id FROM DELETED);
END;

GO

-- Заповнення таблиці forum.Posts
-- Заповнення таблиці forum.Posts
DECLARE @i INT = 1
WHILE @i <= 20
BEGIN
    INSERT INTO forum.Posts (Id, UserId, GameId, Topic, Content, CreatedAt, Views)
    VALUES 
    (NEWID(), 'df7eae08-b7a7-4472-83de-98f22336ce6a', NULL, 
    CONCAT('Topic ', @i), CONCAT('Content for post number ', @i), GETDATE(), FLOOR(RAND() * 1000));

    SET @i = @i + 1;
END;

GO

-- Заповнення таблиці forum.FavoritePosts
DECLARE @i INT = 1
WHILE @i <= 20
BEGIN
    INSERT INTO forum.FavoritePosts (Id, UserId, PostId)
    VALUES 
    (NEWID(), CASE WHEN @i % 2 = 0 THEN 'df7eae08-b7a7-4472-83de-98f22336ce6a' ELSE NEWID() END, 
    (SELECT TOP 1 Id FROM forum.Posts ORDER BY NEWID()));

    SET @i = @i + 1;
END;

GO

-- Заповнення таблиці forum.Replies
DECLARE @i INT = 1
WHILE @i <= 20
BEGIN
    INSERT INTO forum.Replies (Id, UserId, PostId, Content, CreatedAt, ReplyToId)
    VALUES 
    (NEWID(), 'df7eae08-b7a7-4472-83de-98f22336ce6a', 
    (SELECT TOP 1 Id FROM forum.Posts ORDER BY NEWID()), 
    CONCAT('Reply content number ', @i), GETDATE(), (SELECT TOP 1 Id FROM forum.Replies ORDER BY NEWID()));

    SET @i = @i + 1;
END;

GO
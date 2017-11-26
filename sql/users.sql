CREATE TABLE [dbo].[Users]
(
  id       NVARCHAR(50) NOT NULL,
  username NVARCHAR(50) NOT NULL
);
INSERT INTO [dbo].[Users] VALUES ('1', 'igor');

ALTER TABLE [dbo].[Users]
    ADD CONSTRAINT id_pk  PRIMARY KEY (UserId);

EXEC sp_rename 'master.dbo.GameResults.id', GameResultId, 'COLUMN'

SELECT [gr.UserGameResults.GameResult].[GameResultId], [gr.UserGameResults.GameResult].[WinnerScore]
FROM [GameResults] AS [gr]
  INNER JOIN UserGameResult AS [gr.UserGameResults] ON [gr].[GameResultId] = [gr.UserGameResults].[GameResultId]
  INNER JOIN [GameResults] AS [gr.UserGameResults.GameResult] ON [gr.UserGameResults].[GameResultId] = [gr.UserGameResults.GameResult].[GameResultId]
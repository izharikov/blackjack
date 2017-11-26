SELECT * FROM users;

CREATE table game_result (
    id NVARCHAR(50) not null PRIMARY key,
    winner_score int 
);

create table user_game_res(
   user_id NVARCHAR(50) not null,
   game_id NVARCHAR(50) not null,
   is_winner BIT ,
   constraint user_game_res_pk PRIMARY KEY (user_id, game_id)
);

insert into GameResults values('123', 20) ;
insert into user_game_res values ('1', '123', 1);
insert into user_game_res values ('67deb53f-28ef-43b4-805a-896ae9c6ed2e', '123', 1);

select * from GameResults;
select * from user_game_res;

select user_id from user_game_res ugr 
    inner JOIN GameResults grb on ugr.game_id = grb.GameResultId
    where ugr.game_id = '123';

EXEC sp_rename 'game_result.winner_score', 'winner_score', 'COLUMN';  
select * from GameResults;

EXEC sp_rename 'game_result', 'game_result';

select * from GameResults;

-- alter table game_result

SELECT [g].[game_id], [g].[user_id], [g].[is_winner]
      FROM [user_game_res] AS [g];


alter table user_game_res 
   add CONSTRAINT UserId FOREIGN key (user_id) REFERENCES Users(UserId);

alter table user_game_res 
   add CONSTRAINT GameId FOREIGN key (game_id) REFERENCES GameResults(GameResultId);
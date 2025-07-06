INSERT INTO Sources ( Name, ApiKey, ApiUrl) VALUES
( 'NewsApiOrg', 'dfbe0c57160b48aabca15477250e85d6', 'https://newsapi.org/v2/top-headlines?country=us'),
( 'TheNewsApi', '0Emr4xnmh8AMvkMra0zDehqlBnJ8OJTXusc7JrhI', 'https://api.thenewsapi.com/v1/news/top?locale=us&limit=3');


INSERT INTO Categories (Name) VALUES
('unkown'),
('general'),
('science'),
('sports'),
('business'),
('health'),
('entertainment'),
('tech'),
('politics'),
('food'),
('travel');

SELECT * FROM Categories
select * from Users
update Users set Role= 'Admin' where Username = 'admin'

select * from articles where id = 93

--dotnet ef migrations add InitialCreate --project NewsAggregator.DAL --startup-project NewsAggregator.API

--dotnet ef database update --project NewsAggregator.DAL --startup-project NewsAggregator.API

INSERT INTO CategoryKeywords (Keyword, CategoryId) VALUES
('football', 3), -- sports
('stock', 4),    -- business
('covid', 5),    -- health
('movie', 6),    -- entertainment
('ai', 7),       -- tech
('election', 8), -- politics
('recipe', 9),   -- food
('travel', 10);

update categories set Name = 'unknown' where Name = 'unkown'
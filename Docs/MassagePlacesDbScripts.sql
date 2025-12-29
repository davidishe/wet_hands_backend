-- Создание таблицы каталога массажей (SQL Server)
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'MassagePlaces')
BEGIN
  CREATE TABLE dbo.MassagePlaces
  (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(256) NOT NULL,
    Country NVARCHAR(128) NULL,
    City NVARCHAR(128) NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Rating INT NOT NULL CONSTRAINT DF_MassagePlaces_Rating DEFAULT (0),
    MainImage NVARCHAR(MAX) NOT NULL,
    Gallery NVARCHAR(MAX) NOT NULL,
    Attributes NVARCHAR(MAX) NOT NULL,
    CONSTRAINT UQ_MassagePlaces_Name UNIQUE (Name)
  );
END
GO

-- Обновление существующей таблицы (если уже создана без новых полей)
IF COL_LENGTH('dbo.MassagePlaces', 'Gallery') IS NULL
BEGIN
  ALTER TABLE dbo.MassagePlaces ADD Gallery NVARCHAR(MAX) NOT NULL CONSTRAINT DF_MassagePlaces_Gallery DEFAULT ('[]');
END
GO
IF COL_LENGTH('dbo.MassagePlaces', 'Attributes') IS NULL
BEGIN
  ALTER TABLE dbo.MassagePlaces ADD Attributes NVARCHAR(MAX) NOT NULL CONSTRAINT DF_MassagePlaces_Attributes DEFAULT ('[]');
END
GO

-- Импорт старого JSON-файла в таблицу (путь до файла поменяйте при запуске)
-- Требует ввключённый BULK access и права на путь.
DECLARE @json NVARCHAR(MAX);
-- Пример чтения: 
-- SELECT @json = BulkColumn FROM OPENROWSET(BULK '/app/Data/massage_places.json', SINGLE_CLOB) as j;

-- Пример вставки в БД из JSON (раскомментируйте SELECT @json… выше):
-- INSERT INTO dbo.MassagePlaces (Name, Country, City, Description, Rating, MainImage, Gallery, Attributes)
-- SELECT 
--   ISNULL(JSON_VALUE(j.value, '$.Name'), '') AS Name,
--   NULLIF(JSON_VALUE(j.value, '$.Country'), '') AS Country,
--   NULLIF(JSON_VALUE(j.value, '$.City'), '') AS City,
--   ISNULL(JSON_VALUE(j.value, '$.Description'), '') AS Description,
--   TRY_CAST(JSON_VALUE(j.value, '$.Rating') AS INT) AS Rating,
--   ISNULL(JSON_VALUE(j.value, '$.MainImage'), '') AS MainImage,
--   ISNULL(JSON_QUERY(j.value, '$.Gallery'), '[]') AS Gallery,
--   ISNULL(JSON_QUERY(j.value, '$.Attributes'), '[]') AS Attributes
-- FROM OPENJSON(@json) AS j;
GO

USE Backlogger
GO

-- Creating database tables

IF OBJECT_ID('Materials', 'U') IS NOT NULL
DROP TABLE Materials
GO
CREATE TABLE Materials
(
	MaterialID INT NOT NULL IDENTITY(1,1), 
	HobbyID INT NOT NULL,
	Title NVARCHAR(MAX) NOT NULL,
	MaterialFormatID INT NOT NULL,
	Price MONEY,
	SubscriptionID INT,
	TimeSpent INT, -- in minutes
	Rating SMALLINT, -- 1-10 scale
	DateReleased DATE,
	Info NVARCHAR(MAX),
	CONSTRAINT MaterialsPK PRIMARY KEY(MaterialID)
)

IF OBJECT_ID('MaterialFormats', 'U') IS NOT NULL
DROP TABLE MaterialFormats
GO
CREATE TABLE MaterialFormats
(
	MaterialFormatID INT NOT NULL IDENTITY(1,1), 
	FormatType NVARCHAR(MAX),
	CONSTRAINT MaterialFormatsPK PRIMARY KEY(MaterialFormatID)
)

IF OBJECT_ID('MaterialGenres', 'U') IS NOT NULL
DROP TABLE MaterialGenres
GO
CREATE TABLE MaterialGenres
(
	MaterialID INT NOT NULL, 
	GenreID INT NOT NULL,
	CONSTRAINT MaterialGenresPK PRIMARY KEY(MaterialID, GenreID)
)

IF OBJECT_ID('Genres', 'U') IS NOT NULL
DROP TABLE Genres
GO
CREATE TABLE Genres
(
	GenreID INT NOT NULL IDENTITY(1,1),
	GenreName NVARCHAR(MAX),
	CONSTRAINT GenrePK PRIMARY KEY(GenreID)
)

IF OBJECT_ID('MaterialAuthors', 'U') IS NOT NULL
DROP TABLE MaterialAuthors
GO
CREATE TABLE MaterialAuthors
(
	MaterialID INT NOT NULL,
	AuthorID INT NOT NULL,
	CONSTRAINT MaterialAuthorsPK PRIMARY KEY(MaterialID, AuthorID)
)

IF OBJECT_ID('Authors', 'U') IS NOT NULL
DROP TABLE Authors
GO
CREATE TABLE Authors
(
	AuthorID INT NOT NULL IDENTITY(1,1),
	AuthorName NVARCHAR(MAX),
	CONSTRAINT AuthorPK PRIMARY KEY(AuthorID)
)

IF OBJECT_ID('Subscriptions', 'U') IS NOT NULL
DROP TABLE Subscriptions
GO
CREATE TABLE Subscriptions
(
	SubscriptionID INT NOT NULL IDENTITY(1,1),
	HobbyID INT NOT NULL,
	SubscriptionName NVARCHAR(MAX),
	Price MONEY NOT NULL,
	IsActive BIT NOT NULL,
	CONSTRAINT Subscription PRIMARY KEY(SubscriptionID)
)

IF OBJECT_ID('Hobbies', 'U') IS NOT NULL
DROP TABLE Hobbies
GO
CREATE TABLE Hobbies
(
	HobbyID INT NOT NULL IDENTITY(1,1),
	HobbyName NVARCHAR(MAX)
	CONSTRAINT HobbiesPK PRIMARY KEY(HobbyID)
)

IF OBJECT_ID('StatusUpdates', 'U') IS NOT NULL
DROP TABLE StatusUpdates
GO
CREATE TABLE StatusUpdates
(
	UpdateID INT NOT NULL IDENTITY(1,1),
	MaterialID INT NOT NULL,
	DateModified DATETIME NOT NULL,
	StatusID INT NOT NULL,
	CONSTRAINT StatusUpdatesPK PRIMARY KEY(UpdateID)
)

IF OBJECT_ID('Statuses', 'U') IS NOT NULL
DROP TABLE Statuses
GO
CREATE TABLE Statuses
(
	StatusID INT NOT NULL IDENTITY(1,1),
	StatusName VARCHAR(MAX) NOT NULL,
	CONSTRAINT StatusesPK PRIMARY KEY(StatusID)
)

IF OBJECT_ID('MaterialsHobbiesFK', 'F') IS NOT NULL
ALTER TABLE Materials
DROP CONSTRAINT MaterialsHobbiesFK
GO
ALTER TABLE Materials
ADD CONSTRAINT MaterialsHobbiesFK
FOREIGN KEY(HobbyID) REFERENCES Hobbies(HobbyID)
ON DELETE CASCADE;

IF OBJECT_ID('MaterialsSubscriptionsFK', 'F') IS NOT NULL
ALTER TABLE Materials
DROP CONSTRAINT MaterialsSubscriptionsFK
GO
ALTER TABLE Materials
ADD CONSTRAINT MaterialsSubscriptionsFK
FOREIGN KEY(SubscriptionID) REFERENCES Subscriptions(SubscriptionID)
ON DELETE CASCADE;

IF OBJECT_ID('SubscriptionsHobbiesFK', 'F') IS NOT NULL
ALTER TABLE Subscriptions
DROP CONSTRAINT SubscriptionsHobbiesFK
GO
ALTER TABLE Subscriptions
ADD CONSTRAINT SubscriptionsHobbiesFK
FOREIGN KEY(HobbyID) REFERENCES Hobbies(HobbyID)
ON DELETE NO ACTION;

IF OBJECT_ID('MaterialsMaterialFormatsFK', 'F') IS NOT NULL
ALTER TABLE Materials
DROP CONSTRAINT MaterialsMaterialFormatsFK
GO
ALTER TABLE Materials
ADD CONSTRAINT MaterialsMaterialFormatsFK
FOREIGN KEY(MaterialFormatID) REFERENCES MaterialFormats(MaterialFormatID)
ON DELETE CASCADE;

IF OBJECT_ID('MaterialGenresMaterialsFK', 'F') IS NOT NULL
ALTER TABLE MaterialGenres
DROP CONSTRAINT MaterialGenresMaterialsFK
GO
ALTER TABLE MaterialGenres
ADD CONSTRAINT MaterialGenresMaterialsFK
FOREIGN KEY(MaterialID) REFERENCES Materials(MaterialID)
ON DELETE CASCADE;

IF OBJECT_ID('MaterialGenresGenresFK', 'F') IS NOT NULL
ALTER TABLE MaterialGenres
DROP CONSTRAINT MaterialGenresGenresFK
GO
ALTER TABLE MaterialGenres
ADD CONSTRAINT MaterialGenresGenresFK
FOREIGN KEY(GenreID) REFERENCES Genres(GenreID)
ON DELETE CASCADE;

IF OBJECT_ID('MaterialAuthorsMaterialsFK', 'F') IS NOT NULL
ALTER TABLE MaterialAuthors
DROP CONSTRAINT MaterialAuthorsMaterialsFK
GO
ALTER TABLE MaterialAuthors
ADD CONSTRAINT MaterialAuthorsMaterialsFK
FOREIGN KEY(MaterialID) REFERENCES Materials(MaterialID)
ON DELETE CASCADE;

IF OBJECT_ID('MaterialAuthorsAuthorsFK', 'F') IS NOT NULL
ALTER TABLE MaterialAuthors
DROP CONSTRAINT MaterialAuthorsAuthorsFK
GO
ALTER TABLE MaterialAuthors
ADD CONSTRAINT MaterialAuthorsAuthorsFK
FOREIGN KEY(AuthorID) REFERENCES Authors(AuthorID)
ON DELETE CASCADE;

IF OBJECT_ID('StatusUpdatesMaterialsFK', 'F') IS NOT NULL
ALTER TABLE StatusUpdates
DROP CONSTRAINT StatusUpdatesMaterialsFK
GO
ALTER TABLE StatusUpdates
ADD CONSTRAINT StatusUpdatesMaterialsFK
FOREIGN KEY(MaterialID) REFERENCES Materials(MaterialID)
ON DELETE CASCADE;

IF OBJECT_ID('StatusUpdatesStatusesFK', 'F') IS NOT NULL
ALTER TABLE StatusUpdates
DROP CONSTRAINT StatusUpdatesStatusesFK
GO
ALTER TABLE StatusUpdates
ADD CONSTRAINT StatusUpdatesStatusesFK
FOREIGN KEY(StatusID) REFERENCES Statuses(StatusID)
ON DELETE CASCADE;

INSERT INTO Hobbies VALUES
('Books'), ('Movies'), ('Games')

INSERT INTO Statuses VALUES
('Added'), ('In progress'), ('Dropped'), ('On hold'), ('Finished');

IF OBJECT_ID('ConcatenateAuthors', 'IF') IS NOT NULL
DROP FUNCTION ConcatenateAuthors
GO

CREATE FUNCTION ConcatenateAuthors
(
	@MaterialID INT
)
RETURNS TABLE
AS
	RETURN
	(
		WITH AuthorsName AS
		(
			SELECT AuthorName
			FROM Authors 
			INNER JOIN MaterialAuthors
			ON MaterialAuthors.AuthorID = Authors.AuthorID
			WHERE MaterialAuthors.MaterialID = @MaterialID
		)
		SELECT STRING_AGG(AuthorName, ', ') AS AuthorsList
		FROM AuthorsName
	);
GO

IF OBJECT_ID('ConcatenateGenres', 'IF') IS NOT NULL
DROP FUNCTION ConcatenateGenres
GO

CREATE FUNCTION ConcatenateGenres
(
	@MaterialID INT
)
RETURNS TABLE
AS
	RETURN
	(
		WITH GenresName AS
		(
			SELECT GenreName
			FROM Genres
			INNER JOIN MaterialGenres
			ON MaterialGenres.GenreID = Genres.GenreID
			WHERE MaterialGenres.MaterialID = @MaterialID
		)
		SELECT STRING_AGG(GenreName, ', ') AS GenresList
		FROM GenresName
	);
GO

IF OBJECT_ID('LastStatusUpdate', 'IF') IS NOT NULL
DROP FUNCTION LastStatusUpdate
GO

CREATE FUNCTION LastStatusUpdate
(
	@MaterialID INT
)
RETURNS TABLE
AS
	RETURN
	(
		WITH UpdatesMaterial AS
		(
			SELECT Statuses.StatusID, Statuses.StatusName, StatusUpdates.DateModified, StatusUpdates.MaterialID
			FROM StatusUpdates
			INNER JOIN Statuses
			ON Statuses.StatusID = StatusUpdates.StatusID
			WHERE StatusUpdates.MaterialID = @MaterialID
		)
		SELECT TOP 1 *
		FROM UpdatesMaterial
		ORDER BY DateModified DESC
	);
GO

IF OBJECT_ID('Books', 'V') IS NOT NULL
DROP VIEW Books
GO
CREATE VIEW Books
AS
	SELECT MaterialID, Title, A.AuthorsList, G.GenresList, M.Price, TimeSpent, Rating, DateReleased, Info, M.MaterialFormatID, M.SubscriptionID
	FROM Materials M
	OUTER APPLY ConcatenateAuthors(M.MaterialID) A
	OUTER APPLY ConcatenateGenres(M.MaterialID) G
	WHERE M.HobbyID IN (SELECT HobbyID FROM Hobbies WHERE HobbyName = 'Books')
GO

IF OBJECT_ID('Movies', 'V') IS NOT NULL
DROP VIEW Movies
GO
CREATE VIEW Movies
AS
	SELECT MaterialID, Title, A.AuthorsList, G.GenresList, M.Price, TimeSpent, Rating, DateReleased, Info, M.MaterialFormatID, M.SubscriptionID
	FROM Materials M
	OUTER APPLY ConcatenateAuthors(M.MaterialID) A
	OUTER APPLY ConcatenateGenres(M.MaterialID) G
	WHERE M.HobbyID IN (SELECT HobbyID FROM Hobbies WHERE HobbyName = 'Movies')
GO


IF OBJECT_ID('Games', 'V') IS NOT NULL
DROP VIEW Games
GO
CREATE VIEW Games
AS
	SELECT MaterialID, Title, A.AuthorsList, G.GenresList, M.Price, TimeSpent, Rating, DateReleased, Info, M.MaterialFormatID, M.SubscriptionID
	FROM Materials M
	OUTER APPLY ConcatenateAuthors(M.MaterialID) A
	OUTER APPLY ConcatenateGenres(M.MaterialID) G
	WHERE M.HobbyID IN (SELECT HobbyID FROM Hobbies WHERE HobbyName = 'Games')
GO

IF OBJECT_ID('BooksSubscriptions', 'V') IS NOT NULL
DROP VIEW BooksSubscriptions
GO
CREATE VIEW BooksSubscriptions
AS
	SELECT SubscriptionID, SubscriptionName, Price, IsActive
	FROM Subscriptions
	WHERE HobbyID IN (SELECT HobbyID FROM Hobbies WHERE HobbyName = 'Books')
GO

IF OBJECT_ID('MoviesSubscriptions', 'V') IS NOT NULL
DROP VIEW MoviesSubscriptions
GO
CREATE VIEW MoviesSubscriptions
AS
	SELECT SubscriptionID, SubscriptionName, Price, IsActive
	FROM Subscriptions
	WHERE HobbyID IN (SELECT HobbyID FROM Hobbies WHERE HobbyName = 'Movies')
GO

IF OBJECT_ID('GamesSubscriptions', 'V') IS NOT NULL
DROP VIEW GamesSubscriptions
GO
CREATE VIEW GamesSubscriptions
AS
	SELECT SubscriptionID, SubscriptionName, Price, IsActive
	FROM Subscriptions
	WHERE HobbyID IN (SELECT HobbyID FROM Hobbies WHERE HobbyName = 'Games')
GO

IF OBJECT_ID('TopRatedAuthors', 'IF') IS NOT NULL
DROP FUNCTION TopRatedAuthors
GO

CREATE FUNCTION TopRatedAuthors
(
	@hobbyID INT
)
RETURNS TABLE
AS
	RETURN
	(
		WITH AuthorsByHobby AS
		(
			SELECT Authors.AuthorID
			FROM Authors
			INNER JOIN MaterialAuthors 
			ON MaterialAuthors.AuthorID = Authors.AuthorID
			INNER JOIN Materials
			ON Materials.MaterialID = MaterialAuthors.MaterialID
			WHERE HobbyID = @hobbyID
		),
		AuthorAverageRating AS
		(
			SELECT AuthorsByHobby.AuthorID, AVG(CAST(Rating AS DECIMAL(6,4))) AS AverageRating
			FROM AuthorsByHobby
			INNER JOIN MaterialAuthors 
			ON MaterialAuthors.AuthorID = AuthorsByHobby.AuthorID
			INNER JOIN Materials
			ON Materials.MaterialID = MaterialAuthors.MaterialID
			GROUP BY AuthorsByHobby.AuthorID
		)
		SELECT Authors.AuthorID, Authors.AuthorName, AuthorAverageRating.AverageRating
		FROM AuthorAverageRating
		INNER JOIN Authors
		ON Authors.AuthorID = AuthorAverageRating.AuthorID
	);
GO

IF OBJECT_ID('TopRatedGenres', 'IF') IS NOT NULL
DROP FUNCTION TopRatedGenres
GO

CREATE FUNCTION TopRatedGenres
(
	@hobbyID INT
)
RETURNS TABLE
AS
	RETURN
	(
		WITH GenresByHobby AS
		(
			SELECT Genres.GenreID
			FROM Genres
			INNER JOIN MaterialGenres
			ON MaterialGenres.GenreID = Genres.GenreID
			INNER JOIN Materials
			ON Materials.MaterialID = MaterialGenres.MaterialID
			WHERE HobbyID = @hobbyID
		),
		GenreAverageRating AS
		(
			SELECT GenresByHobby.GenreID, AVG(CAST(Rating AS DECIMAL(6,4))) AS AverageRating
			FROM GenresByHobby
			INNER JOIN MaterialGenres
			ON MaterialGenres.GenreID = GenresByHobby.GenreID
			INNER JOIN Materials
			ON Materials.MaterialID = MaterialGenres.MaterialID
			GROUP BY GenresByHobby.GenreID
		)
		SELECT Genres.GenreID, Genres.GenreName, GenreAverageRating.AverageRating
		FROM GenreAverageRating
		INNER JOIN Genres
		ON Genres.GenreID = GenreAverageRating.GenreID
	);
GO

IF OBJECT_ID('PartialTimeStatus', 'FN') IS NOT NULL
DROP FUNCTION PartialTimeStatus
GO

CREATE FUNCTION PartialTimeStatus
(
	@updateID INT
)
RETURNS INT
AS
	BEGIN

	DECLARE @sID INT, @dMod DATETIME, @mID INT, @dPrec DATETIME
	
	SELECT 
		@sID = StatusID,
		@dMod = DateModified,
		@mID = MaterialID
	FROM StatusUpdates
	WHERE StatusUpdates.UpdateID = @updateID

	IF (@sID <> 5)
	BEGIN
		RETURN 0
	END

	SELECT TOP 1 
		@dPrec = DateModified
	FROM StatusUpdates
	WHERE MaterialID = @mID AND StatusID = 2 AND DateModified < @dMod
	ORDER BY DateModified DESC

	RETURN DATEDIFF(s, @dPrec, @dMod)
	END
GO

IF OBJECT_ID('PartialTimeMaterial', 'IF') IS NOT NULL
DROP FUNCTION PartialTimeMaterial
GO

CREATE FUNCTION PartialTimeMaterial
(
	@MaterialID INT
)
RETURNS TABLE
AS
	RETURN
	(
		SELECT SUM(Backlogger.dbo.PartialTimeStatus(UpdateID)) AS TotalTime, MaterialID
		FROM StatusUpdates
		WHERE MaterialID = @MaterialID
		GROUP BY MaterialID
	);
GO


IF OBJECT_ID('ConvertTimeToHHMMSS', 'FN') IS NOT NULL
DROP FUNCTION ConvertTimeToHHMMSS
GO

CREATE FUNCTION ConvertTimeToHHMMSS
(
    @time DECIMAL(28,3), 
    @unit VARCHAR(20)
)
RETURNS VARCHAR(20)
AS
BEGIN

    declare @seconds decimal(18,3), @minutes int, @hours int, @temp varchar(20);

    if(@unit = 'hour' or @unit = 'hh' )
        set @seconds = @time * 60 * 60;
    else if(@unit = 'minute' or @unit = 'mi' or @unit = 'n')
        set @seconds = @time * 60;
    else if(@unit = 'second' or @unit = 'ss' or @unit = 's')
        set @seconds = @time;
    else set @seconds = 0; -- unknown time units

    set @hours = convert(int, @seconds /60 / 60);
    set @minutes = convert(int, (@seconds / 60) - (@hours * 60 ));
    set @seconds = @seconds % 60;

	set @temp = convert(varchar(9), convert(int, @hours)) + ':' +
        right('00' + convert(varchar(2), convert(int, @minutes)), 2) + ':' +
        right('00' + convert(varchar(6), @seconds), 6);

    return 
        left(@temp, len(@temp)-4)

END
GO

IF OBJECT_ID('TimeSpentOnMaterials', 'V') IS NOT NULL
DROP VIEW TimeSpentOnMaterials
GO
CREATE VIEW TimeSpentOnMaterials
AS
	SELECT M.MaterialID, Title, T.TotalTime
	FROM Materials M
	OUTER APPLY PartialTimeMaterial(M.MaterialID) T
GO

IF OBJECT_ID('TimeSpentOnHobbies', 'V') IS NOT NULL
DROP VIEW TimeSpentOnHobbies
GO
CREATE VIEW TimeSpentOnHobbies
AS
	SELECT H.HobbyID, MAX(H.HobbyName) AS HobbyName, SUM(TM.TotalTime) AS TotalTimeHobby
	FROM TimeSpentOnMaterials TM
	INNER JOIN Materials
	ON Materials.MaterialID = TM.MaterialID
	INNER JOIN Hobbies H
	ON Materials.HobbyID = H.HobbyID
	GROUP BY H.HobbyID
GO

IF OBJECT_ID('MaterialsFullReport', 'V') IS NOT NULL
DROP VIEW MaterialsFullReport
GO
CREATE VIEW MaterialsFullReport
AS
	SELECT Title, H.HobbyName, MF.FormatType, M.Price, CA.AuthorsList, CG.GenresList, LU.DateModified AS LastUpdateDate,
		S.StatusName AS LastUpdateStatus, SB.SubscriptionName, Rating, DateReleased, Backlogger.dbo.ConvertTimeToHHMMSS(T.TotalTime, 's') AS TotalTime
	FROM Materials M
	CROSS APPLY ConcatenateAuthors(M.MaterialID) CA
	CROSS APPLY ConcatenateGenres(M.MaterialID) CG
	CROSS APPLY LastStatusUpdate(M.MaterialID) LU
	CROSS APPLY PartialTimeMaterial(M.MaterialID) T
	INNER JOIN Statuses S
	ON S.StatusID = LU.StatusID
	LEFT JOIN Subscriptions SB
	ON SB.SubscriptionID = M.SubscriptionID
	INNER JOIN Hobbies H
	ON H.HobbyID = M.HobbyID
	LEFT JOIN MaterialFormats MF
	ON MF.MaterialFormatID = M.MaterialFormatID
GO
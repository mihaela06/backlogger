USE master
GO

DECLARE @ProjectPath NVARCHAR(MAX);
DECLARE @DataPath NVARCHAR(MAX);
DECLARE @SqlCode NVARCHAR(MAX);

-- Modify this variable to the actual path where you saved the project
SET @ProjectPath = N'E:\ATM\An III\Semestrul I\Aplicații baze de date\Proiect\Backlogger';

SET @DataPath = @ProjectPath + '\Database\Data\';
PRINT N'Installing database in ' + @DataPath;

IF EXISTS (SELECT * FROM sys.databases WHERE Name = 'Backlogger')
	PRINT N'The database already exists';
	-- DROP DATABASE Backlogger;
ELSE
	SET @SqlCode = 'CREATE DATABASE Backlogger
	ON PRIMARY
	(
		Name = BackloggerMasterData,
		FileName = ''' + @DataPath + 'BackloggerMasterData.mdf''
	),
	(
		Name = BackloggerNonMasterData,
		FileName = ''' + @DataPath + 'BackloggerNonMasterData.ndf''
	)
	LOG ON
	(
		Name = BackloggerLog,
		FileName = ''' + @DataPath + 'BackloggerLog.ldf''
	)'
	EXEC(@SqlCode)

	PRINT N'Database created in ' + @DataPath;
GO


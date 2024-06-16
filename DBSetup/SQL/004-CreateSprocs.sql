USE [RelationalSerializer]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DeleteField]
	@GUID nvarchar(36),
	@property nvarchar(max)
AS
BEGIN
	
	SET NOCOUNT ON;
	delete FROM [dbo].[CustomObjects]
	  where Property = @property and GUID = @guid
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DeleteObject]
	@guid nvarchar(36)
AS
BEGIN
	
	delete from [dbo].[CustomObjects] where GUID = @guid
	delete from [dbo].[CustomObjectMeta] where GUID = @guid
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DeleteProject] 
	@target nvarchar(36)
AS
BEGIN
	
	delete from [dbo].[ProjectMeta] where guid = @target
	delete from [dbo].[ProjectObjectTemplatePairs] where project_guid = @target

	declare @templateGUID nvarchar(36)
	select @templateguid=OBJECT_GUID from [db_a9769a_rs].[dbo].[ProjectObjectTemplatePairs] where project_guid = @target

	delete from [dbo].[ProjectObjectTemplatePairs] where project_guid = @target

	delete from [dbo].[TemplateModData] where guid = @templateGUID

	delete from [dbo].[CustomObjects] where GUID in (select guid from [db_a9769a_rs].[dbo].[CustomObjectMeta] where project = @target)
	delete from [dbo].[CustomObjectMeta] where project = @target
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetAllObjectGuids]
AS
BEGIN
	
	SET NOCOUNT ON;
	select guid, project, NAV_HIDDEN, EXPORT_EXCLUDE from [dbo].[CustomObjectMeta]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetEnumTypes]
AS
BEGIN
	
	SET NOCOUNT ON;

	SELECT distinct(enum) from [dbo].[Enums]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetEnumValues]
	@name nvarchar(max)
AS
BEGIN
	
	SET NOCOUNT ON;
	select label, value from [dbo].[Enums] where enum = @name
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetObjectFields]
	@guid NVARCHAR(36)
AS
BEGIN
	
	SET NOCOUNT ON;

	select PROPERTY, PROPERTY_VALUE, [type], IsArray from CustomObjects where GUID = @guid
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetProjects]
AS
BEGIN
	
	SET NOCOUNT ON;
	select GUID, NAME from ProjectMeta
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetSourceByName]
	@name nvarchar(max)
AS
BEGIN
	
	SET NOCOUNT ON;

	SELECT source, source_type, last_refresh from [dbo].[DataSources] where name = @name
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetTemplateMods]
	@guid nvarchar(36)
AS
BEGIN
	
	select field, value from TemplateModData where GUID = @GUID
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetTemplates]
AS
BEGIN
	
	select * from ProjectObjectTemplatePairs
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[InsertProject]
    @guid NVARCHAR(36),
    @name NVARCHAR(max)
AS
BEGIN
    
    INSERT INTO ProjectMeta (GUID, NAME)
    VALUES (@guid, @name)
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[InsertSource]
	@source nvarchar(max),
	@source_type nvarchar(max),
	@name nvarchar(max)
AS
BEGIN
	
	SET NOCOUNT ON;
	insert into [dbo].[DataSources] (source, source_type, LAST_REFRESH, [NAME]) values (@source, @source_type, GETDATE(), @name)
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[InsertTemplateMeta]
    @projectguid NVARCHAR(36),
    @objectguid NVARCHAR(36)
AS
BEGIN
    
    INSERT INTO ProjectObjectTemplatePairs(PROJECT_GUID,OBJECT_GUID)
    VALUES (@projectguid, @objectguid)
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SetEnum]
    @InsertValues EnumTable READONLY
AS
BEGIN
	
	DELETE FROM Enums
	WHERE Enum = (SELECT TOP 1 Enum FROM @InsertValues);

    INSERT INTO Enums (Enum, Label, Value)
    SELECT Enum, Label, Value FROM @InsertValues
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SetExportExclude]
	@GUID nvarchar(36),
	@newValue bit
AS
BEGIN
	
	SET NOCOUNT ON;
	update [dbo].[CustomObjectMeta] set EXPORT_EXCLUDE = @newValue where GUID = @GUID
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpsertField]
    @guid NVARCHAR(36),
    @field NVARCHAR(255),
    @value NVARCHAR(MAX),
	@type NVARCHAR(max),
	@IsArray bit
AS
BEGIN
	
    -- Check if the record exists
    IF EXISTS (SELECT 1 FROM CustomObjects WHERE GUID = @guid and PROPERTY = @field)
    BEGIN
        UPDATE CustomObjects SET PROPERTY_VALUE = @value, IsArray = @IsArray, type = @type WHERE GUID = @guid and PROPERTY = @field
    END
    ELSE
    BEGIN
        INSERT INTO CustomObjects (GUID, PROPERTY, PROPERTY_VALUE, LAST_EDITED, TYPE, IsArray) VALUES (@guid, @field, @value, GETDATE(), @type, @IsArray)
    END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpsertModifiers]
    @guid NVARCHAR(36),
	@field nvarchar(max),
    @value NVARCHAR(MAX)
AS
BEGIN
	
    -- Check if the record exists
    IF EXISTS (SELECT 1 FROM TemplateModData WHERE GUID = @guid and field = @field)
    BEGIN
        UPDATE TemplateModData SET VALUE = @value WHERE GUID = @guid and field = @field
    END
    ELSE
    BEGIN
        INSERT INTO TemplateModData (GUID, field, VALUE) VALUES (@guid, @field, @value)
    END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpsertObjectMeta]
    @guid NVARCHAR(36),
    @project NVARCHAR(max)
AS
BEGIN
    -- Check if the record exists
    IF EXISTS (SELECT 1 FROM CustomObjectMeta WHERE GUID = @guid)
    BEGIN
        -- Update existing record
        UPDATE CustomObjectMeta
        SET Project = @project
        WHERE GUID = @guid
    END
    ELSE
    BEGIN
        -- Insert new record
        INSERT INTO CustomObjectMeta (GUID, Project, TIME_CREATED)
        VALUES (@guid, @project, GETDATE())
    END
END
GO

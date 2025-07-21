USE [RelationalSerializer]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CustomObjectMeta](
	[GUID] [nvarchar](36) NOT NULL,
	[PROJECT] [nvarchar](36) NOT NULL,
	[TIME_CREATED] [datetime] NOT NULL,
	[EXPORT_EXCLUDE] [bit] NULL,
	[NAV_HIDDEN] [bit] NULL
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[CustomObjects](
	[GUID] [nvarchar](36) NOT NULL,
	[PROPERTY] [nvarchar](max) NOT NULL,
	[PROPERTY_VALUE] [nvarchar](max) NULL,
	[LAST_EDITED] [datetime] NOT NULL,
	[type] [nvarchar](max) NULL,
	[IsArray] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[DataSources](
	[SOURCE] [nvarchar](max) NOT NULL,
	[SOURCE_TYPE] [nvarchar](50) NOT NULL,
	[LAST_REFRESH] [datetime] NOT NULL,
	[NAME] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[Enums](
	[enum] [nvarchar](50) NOT NULL,
	[label] [nvarchar](50) NOT NULL,
	[value] [int] NOT NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ProjectMeta](
	[GUID] [nvarchar](36) NOT NULL,
	[NAME] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[ProjectObjectTemplatePairs](
	[PROJECT_GUID] [nvarchar](36) NOT NULL,
	[OBJECT_GUID] [nvarchar](36) NOT NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[TemplateModData](
	[guid] [nvarchar](36) NOT NULL,
	[field] [nvarchar](max) NOT NULL,
	[value] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
USE [RelationalSerializer]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TYPE [dbo].[EnumTable] AS TABLE(
	[Enum] [nvarchar](50) NULL,
	[Label] [nvarchar](50) NULL,
	[Value] [int] NULL
)
GO
USE [voddb]
GO
/****** Object:  Table [dbo].[ov_files]    Script Date: 06/03/2015 13:27:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ov_files](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[filecode] [nvarchar](16) NULL,
	[filesize] [bigint] NULL,
	[filename] [nvarchar](255) NULL,
	[stat] [int] NULL,
	[addtime] [smalldatetime] NULL,
	[adduser] [nvarchar](50) NULL,
	[addip] [nvarchar](50) NULL,
	[filedir] [nvarchar](50) NULL,
	[outfilename] [nvarchar](255) NULL,
	[flvfullpath] [nvarchar](255) NULL,
	[errcount] [int] NULL,
	[sendok] [int] NULL,
	[times] [int] NULL,
	[autoimg] [nvarchar](50) NULL,
	[flvsize] [bigint] NULL,
	[isdel] [int] NULL,
	[linkflv] [tinyint] NULL,
	[senderr] [int] NULL,
	[isrec] [tinyint] NULL,
	[truedir] [nvarchar](50) NULL,
	[webserver] [nvarchar](255) NULL,
	[filetype] [smallint] NULL,
	[userconfig] [nvarchar](255) NULL,
	[pagecount] [int] NULL,
	[prefilename] [nvarchar](255) NULL,
 CONSTRAINT [PK_ov_files] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FileHash]    Script Date: 06/03/2015 13:27:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FileHash](
	[FileHashID] [int] IDENTITY(1,1) NOT NULL,
	[FileHash] [varchar](255) NOT NULL,
	[VideoFileID] [int] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Default [DF_ov_files_stat]    Script Date: 06/03/2015 13:27:11 ******/
ALTER TABLE [dbo].[ov_files] ADD  CONSTRAINT [DF_ov_files_stat]  DEFAULT ((0)) FOR [stat]
GO
/****** Object:  Default [DF_ov_files_errcount]    Script Date: 06/03/2015 13:27:11 ******/
ALTER TABLE [dbo].[ov_files] ADD  CONSTRAINT [DF_ov_files_errcount]  DEFAULT ((0)) FOR [errcount]
GO
/****** Object:  Default [DF_ov_files_sendok]    Script Date: 06/03/2015 13:27:11 ******/
ALTER TABLE [dbo].[ov_files] ADD  CONSTRAINT [DF_ov_files_sendok]  DEFAULT ((0)) FOR [sendok]
GO
/****** Object:  Default [DF_ov_files_times]    Script Date: 06/03/2015 13:27:11 ******/
ALTER TABLE [dbo].[ov_files] ADD  CONSTRAINT [DF_ov_files_times]  DEFAULT ((0)) FOR [times]
GO
/****** Object:  Default [DF_ov_files_isdel]    Script Date: 06/03/2015 13:27:11 ******/
ALTER TABLE [dbo].[ov_files] ADD  CONSTRAINT [DF_ov_files_isdel]  DEFAULT ((0)) FOR [isdel]
GO
/****** Object:  Default [DF_ov_files_linkflv]    Script Date: 06/03/2015 13:27:11 ******/
ALTER TABLE [dbo].[ov_files] ADD  CONSTRAINT [DF_ov_files_linkflv]  DEFAULT ((0)) FOR [linkflv]
GO
/****** Object:  Default [DF_ov_files_senderr]    Script Date: 06/03/2015 13:27:11 ******/
ALTER TABLE [dbo].[ov_files] ADD  CONSTRAINT [DF_ov_files_senderr]  DEFAULT ((0)) FOR [senderr]
GO
/****** Object:  Default [DF_ov_files_isrec]    Script Date: 06/03/2015 13:27:11 ******/
ALTER TABLE [dbo].[ov_files] ADD  CONSTRAINT [DF_ov_files_isrec]  DEFAULT ((0)) FOR [isrec]
GO
/****** Object:  Default [DF_ov_files_pagecount]    Script Date: 06/03/2015 13:27:11 ******/
ALTER TABLE [dbo].[ov_files] ADD  CONSTRAINT [DF_ov_files_pagecount]  DEFAULT ((0)) FOR [pagecount]
GO

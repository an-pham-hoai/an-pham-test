/****** Object:  Table [dbo].[User]    Script Date: 7/13/2024 1:11:58 PM ******/

CREATE TABLE [dbo].[User](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedDate] [bigint] NOT NULL,
	[ModifiedDate] [bigint] NOT NULL,
	[Email] [nvarchar](256) NOT NULL,
	[PasswordHash] [varchar](50) NOT NULL,
	[Name] [nvarchar](256) NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO

/****** Object:  Table [dbo].[Question]    Script Date: 7/13/2024 2:02:02 PM ******/

CREATE TABLE [dbo].[Question](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedDate] [bigint] NOT NULL,
	[ModifiedDate] [bigint] NOT NULL,
	[Description] [nvarchar](512) NOT NULL,
	[AnswerA] [nvarchar](256) NOT NULL,
	[AnswerB] [nvarchar](256) NOT NULL,
	[AnswerC] [nvarchar](256) NOT NULL,
	[AnswerD] [nvarchar](256) NOT NULL,
	[Answer] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_Question] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Question] ADD  CONSTRAINT [DF_Question_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO


/****** Object:  Table [dbo].[Quiz]    Script Date: 7/13/2024 3:29:14 PM ******/

CREATE TABLE [dbo].[Quiz](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedDate] [bigint] NOT NULL,
	[ModifiedDate] [bigint] NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[Questions] [nvarchar](512) NULL,
 CONSTRAINT [PK_Quiz] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Quiz] ADD  CONSTRAINT [DF_Quiz_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO


/****** Object:  Table [dbo].[QuizSession]    Script Date: 7/13/2024 3:54:58 PM ******/

CREATE TABLE [dbo].[QuizSession](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedDate] [bigint] NOT NULL,
	[ModifiedDate] [bigint] NOT NULL,
	[QuizCode] [nvarchar](50) NOT NULL,
	[SessionId] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_QuizSession] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[QuizSession] ADD  CONSTRAINT [DF_QuizSession_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO


/****** Object:  Table [dbo].[Score]    Script Date: 7/13/2024 5:25:14 PM ******/

CREATE TABLE [dbo].[Score](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedDate] [bigint] NOT NULL,
	[ModifiedDate] [bigint] NOT NULL,
	[QuizSessionId] [bigint] NOT NULL,
	[UserId] [bigint] NOT NULL,
	[QuizScore] [int] NOT NULL,
 CONSTRAINT [PK_Score] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Score] ADD  CONSTRAINT [DF_Score_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO



-- ============================================================
-- WebhookForge — Schema Script 001: Create Tables
-- Target: SQL Server 2019+ / Azure SQL
-- Run order: 1st
-- ============================================================

USE WebhookForge;
GO

-- ── Users ────────────────────────────────────────────────────
CREATE TABLE [dbo].[Users] (
    [Id]           UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [Email]        NVARCHAR(256)    NOT NULL,
    [DisplayName]  NVARCHAR(100)    NOT NULL,
    [PasswordHash] NVARCHAR(256)    NOT NULL,
    [IsActive]     BIT              NOT NULL DEFAULT 1,
    [LastLoginAt]  DATETIME2        NULL,
    [CreatedAt]    DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]    DATETIME2        NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- ── Workspaces ────────────────────────────────────────────────
CREATE TABLE [dbo].[Workspaces] (
    [Id]          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [Name]        NVARCHAR(100)    NOT NULL,
    [Slug]        NVARCHAR(120)    NOT NULL DEFAULT '',
    [Description] NVARCHAR(500)    NULL,
    [OwnerId]     UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt]   DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]   DATETIME2        NULL,
    CONSTRAINT [PK_Workspaces]       PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Workspaces_Users] FOREIGN KEY ([OwnerId])
        REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION
);
GO

-- ── WorkspaceMembers ──────────────────────────────────────────
CREATE TABLE [dbo].[WorkspaceMembers] (
    [Id]          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [WorkspaceId] UNIQUEIDENTIFIER NOT NULL,
    [UserId]      UNIQUEIDENTIFIER NOT NULL,
    [Role]        NVARCHAR(20)     NOT NULL,
    [CreatedAt]   DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]   DATETIME2        NULL,
    CONSTRAINT [PK_WorkspaceMembers]           PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_WorkspaceMembers_Workspaces] FOREIGN KEY ([WorkspaceId])
        REFERENCES [dbo].[Workspaces] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_WorkspaceMembers_Users]      FOREIGN KEY ([UserId])
        REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION
);
GO

-- ── Endpoints ─────────────────────────────────────────────────
CREATE TABLE [dbo].[Endpoints] (
    [Id]          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [WorkspaceId] UNIQUEIDENTIFIER NOT NULL,
    [Token]       NVARCHAR(64)     NOT NULL,
    [Name]        NVARCHAR(100)    NOT NULL,
    [Description] NVARCHAR(500)    NULL,
    [IsActive]    BIT              NOT NULL DEFAULT 1,
    [ExpiresAt]   DATETIME2        NULL,
    [CreatedAt]   DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]   DATETIME2        NULL,
    CONSTRAINT [PK_Endpoints]            PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Endpoints_Workspaces] FOREIGN KEY ([WorkspaceId])
        REFERENCES [dbo].[Workspaces] ([Id]) ON DELETE CASCADE
);
GO

-- ── MockRules ─────────────────────────────────────────────────
CREATE TABLE [dbo].[MockRules] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [EndpointId]          UNIQUEIDENTIFIER NOT NULL,
    [Name]                NVARCHAR(100)    NOT NULL,
    [Priority]            INT              NOT NULL DEFAULT 0,
    [MatchMethod]         NVARCHAR(10)     NULL,
    [MatchPath]           NVARCHAR(500)    NULL,
    [MatchBodyExpression] NVARCHAR(1000)   NULL,
    [ResponseStatus]      SMALLINT         NOT NULL DEFAULT 200,
    [ResponseBody]        NVARCHAR(MAX)    NULL,
    [ResponseHeaders]     NVARCHAR(MAX)    NULL,  -- stored as JSON
    [DelayMs]             INT              NOT NULL DEFAULT 0,
    [IsActive]            BIT              NOT NULL DEFAULT 1,
    [CreatedAt]           DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]           DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_MockRules]          PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MockRules_Endpoints] FOREIGN KEY ([EndpointId])
        REFERENCES [dbo].[Endpoints] ([Id]) ON DELETE CASCADE
);
GO

-- ── IncomingRequests ─────────────────────────────────────────
CREATE TABLE [dbo].[IncomingRequests] (
    [Id]          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [EndpointId]  UNIQUEIDENTIFIER NOT NULL,
    [Method]      NVARCHAR(10)     NOT NULL,
    [Path]        NVARCHAR(2000)   NULL,
    [QueryString] NVARCHAR(2000)   NULL,
    [Headers]     NVARCHAR(MAX)    NULL,  -- stored as JSON
    [Body]        NVARCHAR(MAX)    NULL,
    [ContentType] NVARCHAR(200)    NULL,
    [IpAddress]   NVARCHAR(45)     NULL,
    [SizeBytes]   INT              NOT NULL DEFAULT 0,
    [ReceivedAt]  DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    [CreatedAt]   DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]   DATETIME2        NULL,
    CONSTRAINT [PK_IncomingRequests]          PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_IncomingRequests_Endpoints] FOREIGN KEY ([EndpointId])
        REFERENCES [dbo].[Endpoints] ([Id]) ON DELETE CASCADE
);
GO

-- ── RefreshTokens ─────────────────────────────────────────────
CREATE TABLE [dbo].[RefreshTokens] (
    [Id]         UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [UserId]     UNIQUEIDENTIFIER NOT NULL,
    [Token]      NVARCHAR(256)    NOT NULL,
    [IsRevoked]  BIT              NOT NULL DEFAULT 0,
    [ExpiresAt]  DATETIME2        NOT NULL,
    [CreatedAt]  DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]  DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_RefreshTokens]       PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_RefreshTokens_Users] FOREIGN KEY ([UserId])
        REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
);
GO

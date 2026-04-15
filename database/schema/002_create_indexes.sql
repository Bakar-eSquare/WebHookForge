-- ============================================================
-- WebhookForge — Schema Script 002: Indexes
-- Run order: 2nd (after 001_create_tables.sql)
-- ============================================================

USE WebhookForge;
GO

-- ── Users ────────────────────────────────────────────────────
CREATE UNIQUE NONCLUSTERED INDEX [UX_Users_Email]
    ON [dbo].[Users] ([Email] ASC);
GO

-- ── Workspaces ───────────────────────────────────────────────
CREATE NONCLUSTERED INDEX [IX_Workspaces_OwnerId]
    ON [dbo].[Workspaces] ([OwnerId] ASC);
GO

-- ── WorkspaceMembers ─────────────────────────────────────────
-- Enforce one membership record per user per workspace
CREATE UNIQUE NONCLUSTERED INDEX [UX_WorkspaceMembers_WorkspaceUser]
    ON [dbo].[WorkspaceMembers] ([WorkspaceId] ASC, [UserId] ASC);
GO

-- ── Endpoints ────────────────────────────────────────────────
-- Token lookup on every incoming webhook call — must be unique & fast
CREATE UNIQUE NONCLUSTERED INDEX [UX_Endpoints_Token]
    ON [dbo].[Endpoints] ([Token] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Endpoints_WorkspaceId]
    ON [dbo].[Endpoints] ([WorkspaceId] ASC);
GO

-- ── MockRules ────────────────────────────────────────────────
-- Ordered rule evaluation per endpoint
CREATE NONCLUSTERED INDEX [IX_MockRules_EndpointId_Priority]
    ON [dbo].[MockRules] ([EndpointId] ASC, [Priority] ASC, [IsActive] ASC);
GO

-- ── IncomingRequests ─────────────────────────────────────────
-- Primary query: list requests per endpoint ordered by date
CREATE NONCLUSTERED INDEX [IX_IncomingRequests_EndpointId_ReceivedAt]
    ON [dbo].[IncomingRequests] ([EndpointId] ASC, [ReceivedAt] DESC);
GO

-- ── RefreshTokens ────────────────────────────────────────────
CREATE UNIQUE NONCLUSTERED INDEX [UX_RefreshTokens_Token]
    ON [dbo].[RefreshTokens] ([Token] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_RefreshTokens_UserId]
    ON [dbo].[RefreshTokens] ([UserId] ASC);
GO

-- ============================================================
-- WebhookForge — Seed Script 001: Development Seed Data
-- WARNING: For local/development use only. Do NOT run in prod.
-- ============================================================

USE WebhookForge;
GO

-- ── Demo User ────────────────────────────────────────────────
-- Password: Admin@123  (BCrypt hash — regenerate in prod!)
DECLARE @UserId UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';

INSERT INTO [dbo].[Users] ([Id], [Email], [DisplayName], [PasswordHash])
VALUES (
    @UserId,
    'admin@webhookforge.local',
    'Admin User',
    '$2a$12$KIXzU5zMsGrWR3VWajlFZeH5OWdktZ5kBkWj3cgjq1r5Xyp9QpFEK'
);
GO

-- ── Demo Workspace ───────────────────────────────────────────
DECLARE @UserId      UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @WorkspaceId UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';

INSERT INTO [dbo].[Workspaces] ([Id], [Name], [Description], [OwnerId])
VALUES (
    @WorkspaceId,
    'My First Workspace',
    'Default workspace created by the seed script.',
    @UserId
);

INSERT INTO [dbo].[WorkspaceMembers] ([WorkspaceId], [UserId], [Role])
VALUES (@WorkspaceId, @UserId, 'Admin');
GO

-- ── Demo Endpoint ────────────────────────────────────────────
DECLARE @WorkspaceId UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';
DECLARE @EndpointId  UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333';

INSERT INTO [dbo].[Endpoints] ([Id], [WorkspaceId], [Token], [Name], [Description])
VALUES (
    @EndpointId,
    @WorkspaceId,
    'demo-token-abc123',
    'Demo Endpoint',
    'Send requests to /hooks/demo-token-abc123 to see them here.'
);
GO

-- ── Demo Mock Rule ───────────────────────────────────────────
DECLARE @EndpointId UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333';

INSERT INTO [dbo].[MockRules] (
    [EndpointId], [Name], [Priority],
    [MatchMethod], [ResponseStatus], [ResponseBody], [ResponseHeaders]
)
VALUES (
    @EndpointId,
    'Always return 200',
    10,
    'POST',
    200,
    N'{"message": "Received!"}',
    N'{"Content-Type": "application/json"}'
);
GO

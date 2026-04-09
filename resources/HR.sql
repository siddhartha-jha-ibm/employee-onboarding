
CREATE TABLE dbo.Employees (
    EmployeeId           INT IDENTITY PRIMARY KEY,
    EmployeeCode         NVARCHAR(50) NOT NULL,
    FirstName            NVARCHAR(100) NOT NULL,
    LastName             NVARCHAR(100) NOT NULL,
    Email                NVARCHAR(256) NOT NULL,
    Department           NVARCHAR(100) NOT NULL,
    JobTitle             NVARCHAR(100) NOT NULL,
    HireDate             DATE NOT NULL,
    CreatedAtUtc         DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy            NVARCHAR(100) NOT NULL
);



CREATE TABLE dbo.OnboardingProcesses (
    OnboardingProcessId  UNIQUEIDENTIFIER PRIMARY KEY,
    EmployeeId           INT NOT NULL,
    Status               NVARCHAR(50) NOT NULL, 
    -- Initiated | InProgress | Completed | Failed | Compensating

    CurrentStep          NVARCHAR(100) NULL,
    FailureReason        NVARCHAR(500) NULL,

    IsAccessProvisioned  BIT NOT NULL DEFAULT 0,
    IsFacilitiesAssigned BIT NOT NULL DEFAULT 0,
    IsPayrollActivated   BIT NOT NULL DEFAULT 0,

    StartedAtUtc         DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CompletedAtUtc       DATETIME2 NULL
);



CREATE TABLE dbo.OnboardingEvents (
    EventId              INT IDENTITY PRIMARY KEY,
    OnboardingProcessId  UNIQUEIDENTIFIER NOT NULL,
    EventType            NVARCHAR(100) NOT NULL,
    EventPayload         NVARCHAR(MAX) NULL,
    OccurredAtUtc        DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);



CREATE TABLE dbo.UserAccounts (
    UserAccountId        INT IDENTITY PRIMARY KEY,
    EmployeeId           INT NOT NULL,
    Username             NVARCHAR(100) NOT NULL,
    IsActive             BIT NOT NULL DEFAULT 1,
    CreatedAtUtc         DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    DisabledAtUtc        DATETIME2 NULL
);



CREATE TABLE dbo.UserRoles (
    UserRoleId           INT IDENTITY PRIMARY KEY,
    UserAccountId        INT NOT NULL,
    RoleName             NVARCHAR(100) NOT NULL
);



CREATE TABLE dbo.DeskAssignments (
    DeskAssignmentId     INT IDENTITY PRIMARY KEY,
    EmployeeId           INT NOT NULL,
    DeskCode             NVARCHAR(50) NOT NULL,
    IsActive             BIT NOT NULL DEFAULT 1,
    AssignedAtUtc        DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ReleasedAtUtc        DATETIME2 NULL
);



CREATE TABLE dbo.Badges (
    BadgeId              INT IDENTITY PRIMARY KEY,
    EmployeeId           INT NOT NULL,
    BadgeNumber          NVARCHAR(50) NOT NULL,
    IsActive             BIT NOT NULL DEFAULT 1,
    IssuedAtUtc          DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    RevokedAtUtc         DATETIME2 NULL
);



CREATE TABLE dbo.PayrollAccounts (
    PayrollAccountId     INT IDENTITY PRIMARY KEY,
    EmployeeId           INT NOT NULL,
    SalaryAmount         DECIMAL(18,2) NOT NULL,
    Currency             NVARCHAR(10) NOT NULL,
    IsActive             BIT NOT NULL DEFAULT 1,
    ActivatedAtUtc       DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    DeactivatedAtUtc     DATETIME2 NULL
);



CREATE TABLE dbo.OutboxMessages (
    MessageId            UNIQUEIDENTIFIER PRIMARY KEY,
    MessageType          NVARCHAR(100) NOT NULL,
    Payload              NVARCHAR(MAX) NOT NULL,
    OccurredAtUtc        DATETIME2 NOT NULL,
    ProcessedAtUtc       DATETIME2 NULL
);



CREATE TABLE dbo.InboxMessages (
    MessageId            UNIQUEIDENTIFIER PRIMARY KEY,
    Consumer             NVARCHAR(100) NOT NULL,
    ReceivedAtUtc        DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

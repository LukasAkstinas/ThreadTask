CREATE TABLE [dbo].ThreadData (
    [Id]       INT          IDENTITY (1, 1) NOT NULL,
    [ThreadID] INT          NOT NULL,
    [Time]     DATETIME     NULL DEFAULT getdate(),
    [Data]     VARCHAR (10) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

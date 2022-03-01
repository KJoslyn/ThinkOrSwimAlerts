IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220301060446_InitialCreate')
BEGIN
    CREATE TABLE [Positions] (
        [PositionId] bigint NOT NULL IDENTITY,
        [Symbol] nvarchar(max) NOT NULL,
        [Underlying] nvarchar(max) NOT NULL,
        [PutOrCall] int NOT NULL,
        [FirstBuy] datetimeoffset NOT NULL,
        [FinalSell] datetimeoffset NOT NULL,
        [Indicator] int NOT NULL,
        [IndicatorVersion] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Positions] PRIMARY KEY ([PositionId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220301060446_InitialCreate')
BEGIN
    CREATE TABLE [PositionUpdates] (
        [PositionUpdateId] bigint NOT NULL IDENTITY,
        [PositionId] bigint NULL,
        [SecondsAfterPurchase] int NOT NULL,
        [Mark] real NOT NULL,
        [GainOrLossPct] real NOT NULL,
        [IsNewHigh] bit NOT NULL,
        [IsNewLow] bit NOT NULL,
        CONSTRAINT [PK_PositionUpdates] PRIMARY KEY ([PositionUpdateId]),
        CONSTRAINT [FK_PositionUpdates_Positions_PositionId] FOREIGN KEY ([PositionId]) REFERENCES [Positions] ([PositionId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220301060446_InitialCreate')
BEGIN
    CREATE TABLE [Purchases] (
        [PurhaseId] bigint NOT NULL IDENTITY,
        [PositionId] bigint NULL,
        [BuyPrice] real NOT NULL,
        [Bought] datetimeoffset NOT NULL,
        [Bought15MinuteInterval] int NOT NULL,
        [Day] int NOT NULL,
        [Quantity] int NOT NULL,
        CONSTRAINT [PK_Purchases] PRIMARY KEY ([PurhaseId]),
        CONSTRAINT [FK_Purchases_Positions_PositionId] FOREIGN KEY ([PositionId]) REFERENCES [Positions] ([PositionId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220301060446_InitialCreate')
BEGIN
    CREATE INDEX [IX_PositionUpdates_PositionId] ON [PositionUpdates] ([PositionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220301060446_InitialCreate')
BEGIN
    CREATE INDEX [IX_Purchases_PositionId] ON [Purchases] ([PositionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220301060446_InitialCreate')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220301060446_InitialCreate', N'5.0.14');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220301064241_SecondSchema')
BEGIN
    EXEC sp_rename N'[Purchases].[PurhaseId]', N'PurchaseId', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220301064241_SecondSchema')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Positions]') AND [c].[name] = N'FinalSell');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Positions] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Positions] ALTER COLUMN [FinalSell] datetimeoffset NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220301064241_SecondSchema')
BEGIN
    ALTER TABLE [Positions] ADD [HighPrice] real NOT NULL DEFAULT CAST(0 AS real);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220301064241_SecondSchema')
BEGIN
    ALTER TABLE [Positions] ADD [LowPrice] real NOT NULL DEFAULT CAST(0 AS real);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220301064241_SecondSchema')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220301064241_SecondSchema', N'5.0.14');
END;
GO

COMMIT;
GO


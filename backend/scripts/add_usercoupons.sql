-- Notifications
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Notifications')
BEGIN
    CREATE TABLE [Notifications] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [UserId] INT NOT NULL,
        [Title] NVARCHAR(200) NOT NULL,
        [Content] NVARCHAR(MAX) NOT NULL,
        [Type] INT NOT NULL,
        [RelatedId] INT NULL,
        [IsRead] BIT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [FK_Notifications_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
    );
    CREATE INDEX [IX_Notifications_UserId_IsRead] ON [Notifications] ([UserId], [IsRead]);
END

-- Coupons
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Coupons')
BEGIN
    CREATE TABLE [Coupons] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(200) NOT NULL,
        [Type] INT NOT NULL,
        [Value] DECIMAL(18,4) NOT NULL,
        [MinOrderAmount] DECIMAL(18,2) NOT NULL,
        [ValidFrom] DATETIME2 NOT NULL,
        [ValidTo] DATETIME2 NOT NULL,
        [TotalQuantity] INT NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    CREATE INDEX [IX_Coupons_IsActive] ON [Coupons] ([IsActive]);
END

-- UserCoupons
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserCoupons')
BEGIN
    CREATE TABLE [UserCoupons] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [UserId] INT NOT NULL,
        [CouponId] INT NOT NULL,
        [IsUsed] BIT NOT NULL DEFAULT 0,
        [UsedAt] DATETIME2 NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [FK_UserCoupons_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]),
        CONSTRAINT [FK_UserCoupons_Coupons] FOREIGN KEY ([CouponId]) REFERENCES [Coupons]([Id])
    );
    CREATE INDEX [IX_UserCoupons_UserId_CouponId] ON [UserCoupons] ([UserId], [CouponId]);
END

-- Payments
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Payments')
BEGIN
    CREATE TABLE [Payments] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [AppointmentId] INT NOT NULL,
        [UserId] INT NOT NULL,
        [Amount] DECIMAL(18,2) NOT NULL,
        [DiscountAmount] DECIMAL(18,2) NOT NULL,
        [FinalAmount] DECIMAL(18,2) NOT NULL,
        [CouponId] INT NULL,
        [Status] INT NOT NULL DEFAULT 0,
        [PayMethod] INT NOT NULL,
        [PaidAt] DATETIME2 NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [FK_Payments_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]),
        CONSTRAINT [FK_Payments_Appointments] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments]([Id]),
        CONSTRAINT [FK_Payments_Coupons] FOREIGN KEY ([CouponId]) REFERENCES [Coupons]([Id])
    );
END

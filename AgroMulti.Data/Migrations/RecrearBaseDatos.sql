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
CREATE TABLE [EstadoEntrega] (
    [EstadoEntregaId] int NOT NULL IDENTITY,
    [Nombre] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_EstadoEntrega] PRIMARY KEY ([EstadoEntregaId])
);

CREATE TABLE [Producto] (
    [ProductoId] int NOT NULL IDENTITY,
    [Nombre] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_Producto] PRIMARY KEY ([ProductoId])
);

CREATE TABLE [Productor] (
    [ProductorId] int NOT NULL IDENTITY,
    [Codigo] nvarchar(50) NOT NULL,
    [Nombre] nvarchar(100) NOT NULL,
    [Apellido] nvarchar(100) NOT NULL,
    [Telefono] nvarchar(20) NULL,
    [Direccion] nvarchar(200) NULL,
    CONSTRAINT [PK_Productor] PRIMARY KEY ([ProductorId])
);

CREATE TABLE [SubProducto] (
    [SubProductoId] int NOT NULL IDENTITY,
    [ProductoId] int NOT NULL,
    [Nombre] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_SubProducto] PRIMARY KEY ([SubProductoId]),
    CONSTRAINT [FK_SubProducto_Producto] FOREIGN KEY ([ProductoId]) REFERENCES [Producto] ([ProductoId])
);

CREATE TABLE [Entrega] (
    [EntregaId] int NOT NULL IDENTITY,
    [NumeroEntrega] nvarchar(50) NOT NULL,
    [FechaEntrega] date NOT NULL,
    [ProductorId] int NOT NULL,
    [ProductoId] int NOT NULL,
    [SubProductoId] int NULL,
    [EstadoEntregaId] int NOT NULL,
    [Placa] nvarchar(20) NULL,
    [NombreConductor] nvarchar(100) NULL,
    [Kilos] decimal(10,2) NOT NULL,
    [Cajas] int NOT NULL,
    [Sacos] int NOT NULL,
    [KilosSecos] decimal(10,2) NULL,
    [Pasillo] nvarchar(50) NULL,
    [NumeroAnaquel] nvarchar(50) NULL,
    [Piso] nvarchar(50) NULL,
    [Observaciones] nvarchar(max) NULL,
    CONSTRAINT [PK_Entrega] PRIMARY KEY ([EntregaId]),
    CONSTRAINT [FK_Entrega_EstadoEntrega] FOREIGN KEY ([EstadoEntregaId]) REFERENCES [EstadoEntrega] ([EstadoEntregaId]),
    CONSTRAINT [FK_Entrega_Producto] FOREIGN KEY ([ProductoId]) REFERENCES [Producto] ([ProductoId]),
    CONSTRAINT [FK_Entrega_Productor] FOREIGN KEY ([ProductorId]) REFERENCES [Productor] ([ProductorId]),
    CONSTRAINT [FK_Entrega_SubProducto] FOREIGN KEY ([SubProductoId]) REFERENCES [SubProducto] ([SubProductoId])
);

CREATE TABLE [HistoricosEstadoEntrega] (
    [HistoricoEstadoEntregaId] int NOT NULL IDENTITY,
    [EntregaId] int NOT NULL,
    [EstadoEntregaId] int NOT NULL,
    [FechaCambio] datetime NOT NULL DEFAULT (GETDATE()),
    [Observaciones] nvarchar(500) NULL,
    CONSTRAINT [PK_HistoricosEstadoEntrega] PRIMARY KEY ([HistoricoEstadoEntregaId]),
    CONSTRAINT [FK_HistoricoEstadoEntrega_Entrega] FOREIGN KEY ([EntregaId]) REFERENCES [Entrega] ([EntregaId]) ON DELETE CASCADE,
    CONSTRAINT [FK_HistoricoEstadoEntrega_EstadoEntrega] FOREIGN KEY ([EstadoEntregaId]) REFERENCES [EstadoEntrega] ([EstadoEntregaId]) ON DELETE NO ACTION
);

CREATE INDEX [IX_Entrega_EstadoEntregaId] ON [Entrega] ([EstadoEntregaId]);

CREATE INDEX [IX_Entrega_ProductoId] ON [Entrega] ([ProductoId]);

CREATE INDEX [IX_Entrega_ProductorId] ON [Entrega] ([ProductorId]);

CREATE INDEX [IX_Entrega_SubProductoId] ON [Entrega] ([SubProductoId]);

CREATE UNIQUE INDEX [UQ_Entrega_NumeroEntrega] ON [Entrega] ([NumeroEntrega]);

CREATE INDEX [IX_HistoricoEstadoEntrega_EntregaId] ON [HistoricosEstadoEntrega] ([EntregaId]);

CREATE INDEX [IX_HistoricoEstadoEntrega_FechaCambio] ON [HistoricosEstadoEntrega] ([FechaCambio]);

CREATE INDEX [IX_HistoricosEstadoEntrega_EstadoEntregaId] ON [HistoricosEstadoEntrega] ([EstadoEntregaId]);

CREATE UNIQUE INDEX [UQ_Productor_Codigo] ON [Productor] ([Codigo]);

CREATE INDEX [IX_SubProducto_ProductoId] ON [SubProducto] ([ProductoId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260516184056_InitialCreate', N'10.0.7');

COMMIT;
GO


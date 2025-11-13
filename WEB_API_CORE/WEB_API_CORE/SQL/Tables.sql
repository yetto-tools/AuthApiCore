-- CREATE DATABASE DB_TEST;
-- GO


-- CREATE TABLE Usuarios (
--     Id INT IDENTITY(1,1) PRIMARY KEY,
--     UserRef UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
--     Nombre NVARCHAR(100) NOT NULL,
--     Email NVARCHAR(100) UNIQUE NOT NULL,
--     PasswordHash NVARCHAR(200) NOT NULL,
--     Rol NVARCHAR(50) DEFAULT 'User',
--     Activo BIT DEFAULT 1,
--     FechaCreacion DATETIME DEFAULT GETUTCDATE()
-- );


-- CREATE TABLE AuthTokens (
--     Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
--     UsuarioId INT NOT NULL,
--     AccessToken NVARCHAR(MAX) NULL,
--     RefreshToken NVARCHAR(200) NOT NULL,
--     ExpiraAccess DATETIME NULL,
--     ExpiraRefresh DATETIME NOT NULL,
--     Revocado BIT DEFAULT 0,
--     FechaCreacion DATETIME DEFAULT GETUTCDATE(),
--     FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
-- );

-- CREATE TABLE ResetTokens (
--     Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
--     UsuarioId INT NOT NULL,
--     Token NVARCHAR(200) NOT NULL,
--     Expira DATETIME NOT NULL,
--     Usado BIT DEFAULT 0,
--     FechaCreacion DATETIME DEFAULT GETUTCDATE(),
--     FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
-- );

-- GO

-- -- test get tabla de semana
-- CREATE OR ALTER PROCEDURE [dbo].[sp_GetWeek]
--     @pFechaInicio DATE,
--     @pFechaFin DATE
-- AS
-- BEGIN
--     SET NOCOUNT ON;

--     ;WITH Numeros AS (
--         SELECT 0 AS n
--         UNION ALL
--         SELECT n + 1 FROM Numeros
--         WHERE DATEADD(DAY, n + 1, @pFechaInicio) <= @pFechaFin
--     )
--     SELECT 
--           FORMAT(DATEADD(DAY, n, @pFechaInicio), 'dddd', 'es-GT') as dia_semana,
--          CAST(DATEADD(DAY, n, @pFechaInicio) AS datetimeoffset) AS fecha_calendario
--     FROM Numeros
--     OPTION (MAXRECURSION 32767);
--     SELECT 'OK' AS estado;
-- END;
-- GO


-- CREATE OR ALTER PROCEDURE [dbo].[sp_LoginUsuario]
--     @Email NVARCHAR(100)
-- AS
-- BEGIN
--     SET NOCOUNT ON;

--     SELECT TOP 1 Id, UserRef, Nombre, Email, PasswordHash, Rol, Activo
--     FROM Usuarios
--     WHERE Email = @Email;
-- END; 
-- GO

-- CREATE OR ALTER PROCEDURE [dbo].[sp_RegistrarUsuario]
--     @Nombre NVARCHAR(100),
--     @Email NVARCHAR(100),
--     @PasswordHash NVARCHAR(200)
-- AS
-- BEGIN
--     SET NOCOUNT ON;

--     IF EXISTS (SELECT 1 FROM Usuarios WHERE Email = @Email)
--     BEGIN
--         SELECT 'ERROR' AS Estado, 'El correo ya est� registrado.' AS Mensaje;
--         RETURN;
--     END;

--     INSERT INTO Usuarios (UserRef, Nombre, Email, PasswordHash)
--     VALUES (NEWID(), @Nombre, @Email, @PasswordHash);

--     SELECT 'OK' AS Estado, 'Usuario creado correctamente.' AS Mensaje;
-- END;
-- GO

-- CREATE OR ALTER PROCEDURE [dbo].[spUsuarios_ObtenerPorId]
--     @IdUsuario INT
-- AS
-- BEGIN
--     SET NOCOUNT ON;

--     SELECT 
--         Id,
--         UserRef,
--         Nombre,
--         Email,
--         Rol,
--         Activo
--     FROM dbo.Usuarios
--     WHERE Id = @IdUsuario;
-- END


USE [DB_TEST]
GO
/****** Object:  Table [dbo].[AuthTokens]    Script Date: 12/11/2025 18:12:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuthTokens](
	[Id] [uniqueidentifier] NOT NULL,
	[UsuarioId] [int] NOT NULL,
	[AccessToken] [nvarchar](max) NULL,
	[RefreshToken] [nvarchar](200) NOT NULL,
	[ExpiraAccess] [datetime] NULL,
	[ExpiraRefresh] [datetime] NOT NULL,
	[Revocado] [bit] NULL,
	[FechaCreacion] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ResetTokens]    Script Date: 12/11/2025 18:12:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResetTokens](
	[Id] [uniqueidentifier] NOT NULL,
	[UsuarioId] [int] NOT NULL,
	[Token] [nvarchar](200) NOT NULL,
	[Expira] [datetime] NOT NULL,
	[Usado] [bit] NULL,
	[FechaCreacion] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Usuarios]    Script Date: 12/11/2025 18:12:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Usuarios](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserRef] [uniqueidentifier] NOT NULL,
	[Nombre] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[PasswordHash] [nvarchar](200) NOT NULL,
	[Rol] [nvarchar](50) NULL,
	[Activo] [bit] NULL,
	[FechaCreacion] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AuthTokens] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[AuthTokens] ADD  DEFAULT ((0)) FOR [Revocado]
GO
ALTER TABLE [dbo].[AuthTokens] ADD  DEFAULT (getutcdate()) FOR [FechaCreacion]
GO
ALTER TABLE [dbo].[ResetTokens] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[ResetTokens] ADD  DEFAULT ((0)) FOR [Usado]
GO
ALTER TABLE [dbo].[ResetTokens] ADD  DEFAULT (getutcdate()) FOR [FechaCreacion]
GO
ALTER TABLE [dbo].[Usuarios] ADD  DEFAULT (newid()) FOR [UserRef]
GO
ALTER TABLE [dbo].[Usuarios] ADD  DEFAULT ('User') FOR [Rol]
GO
ALTER TABLE [dbo].[Usuarios] ADD  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[Usuarios] ADD  DEFAULT (getutcdate()) FOR [FechaCreacion]
GO
ALTER TABLE [dbo].[AuthTokens]  WITH CHECK ADD FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[ResetTokens]  WITH CHECK ADD FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
/****** Object:  StoredProcedure [dbo].[sp_GetWeek]    Script Date: 12/11/2025 18:12:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_GetWeek]
    @pFechaInicio DATE,
    @pFechaFin DATE
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH Numeros AS (
        SELECT 0 AS n
        UNION ALL
        SELECT n + 1 FROM Numeros
        WHERE DATEADD(DAY, n + 1, @pFechaInicio) <= @pFechaFin
    )
    SELECT 
          FORMAT(DATEADD(DAY, n, @pFechaInicio), 'dddd', 'es-GT') as dia_semana,
         CAST(DATEADD(DAY, n, @pFechaInicio) AS datetimeoffset) AS fecha_calendario
    FROM Numeros
    OPTION (MAXRECURSION 32767);
    SELECT 'OK' AS estado;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Listar_Usuarios]    Script Date: 12/11/2025 18:12:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_Listar_Usuarios]
    @pEstado INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        UserRef,
        Nombre,
        Email,
        Rol,
        Activo
    From Usuarios
    WHERE  
(@pEstado = -1 OR @pEstado IS NULL OR Activo = @pEstado);
END
GO
/****** Object:  StoredProcedure [dbo].[sp_LoginUsuario]    Script Date: 12/11/2025 18:12:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_LoginUsuario]
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 Id, UserRef, Nombre, Email, PasswordHash, Rol, Activo
    FROM Usuarios
    WHERE Email = @Email;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_RegistrarUsuario]    Script Date: 12/11/2025 18:12:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_RegistrarUsuario]
    @Nombre NVARCHAR(100),
    @Email NVARCHAR(100),
    @PasswordHash NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Usuarios WHERE Email = @Email)
    BEGIN
        SELECT 'ERROR' AS Estado, 'El correo ya está registrado.' AS Mensaje;
        RETURN;
    END;

    INSERT INTO Usuarios (UserRef, Nombre, Email, PasswordHash)
    VALUES (NEWID(), @Nombre, @Email, @PasswordHash);

    SELECT 'OK' AS Estado, 'Usuario creado correctamente.' AS Mensaje;
END
GO
/****** Object:  StoredProcedure [dbo].[spUsuarios_ObtenerPorId]    Script Date: 12/11/2025 18:12:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[spUsuarios_ObtenerPorId]
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        UserRef,
        Nombre,
        Email,
        Rol,
        Activo
    FROM dbo.Usuarios
    WHERE Id = @IdUsuario;
END
GO

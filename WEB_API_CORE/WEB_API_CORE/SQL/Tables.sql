CREATE DATABASE DB_TEST;
GO


CREATE TABLE Usuarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserRef UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    Nombre NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(200) NOT NULL,
    Rol NVARCHAR(50) DEFAULT 'User',
    Activo BIT DEFAULT 1,
    FechaCreacion DATETIME DEFAULT GETUTCDATE()
);


CREATE TABLE AuthTokens (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UsuarioId INT NOT NULL,
    AccessToken NVARCHAR(MAX) NULL,
    RefreshToken NVARCHAR(200) NOT NULL,
    ExpiraAccess DATETIME NULL,
    ExpiraRefresh DATETIME NOT NULL,
    Revocado BIT DEFAULT 0,
    FechaCreacion DATETIME DEFAULT GETUTCDATE(),
    FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
);

CREATE TABLE ResetTokens (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UsuarioId INT NOT NULL,
    Token NVARCHAR(200) NOT NULL,
    Expira DATETIME NOT NULL,
    Usado BIT DEFAULT 0,
    FechaCreacion DATETIME DEFAULT GETUTCDATE(),
    FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
);

GO

-- test get tabla de semana
CREATE OR ALTER PROCEDURE [dbo].[sp_GetWeek]
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
END;
GO


CREATE OR ALTER PROCEDURE [dbo].[sp_LoginUsuario]
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 Id, UserRef, Nombre, Email, PasswordHash, Rol, Activo
    FROM Usuarios
    WHERE Email = @Email;
END; 
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_RegistrarUsuario]
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
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[spUsuarios_ObtenerPorId]
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

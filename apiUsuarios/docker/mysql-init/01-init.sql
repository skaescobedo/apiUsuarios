SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS=0;

DROP TABLE IF EXISTS `Users`;
DROP TABLE IF EXISTS `Branches`;
DROP TABLE IF EXISTS `Roles`;
DROP TABLE IF EXISTS `__EFMigrationsHistory`;

CREATE TABLE `Roles` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) NOT NULL,
  `Description` varchar(250) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Roles_Name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `Branches` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(150) NOT NULL,
  `Street` varchar(150) NOT NULL,
  `ExteriorNumber` varchar(20) NOT NULL,
  `InteriorNumber` varchar(20) DEFAULT NULL,
  `Neighborhood` varchar(100) DEFAULT NULL,
  `City` varchar(100) NOT NULL,
  `State` varchar(100) NOT NULL,
  `PostalCode` varchar(20) NOT NULL,
  `Country` varchar(100) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Branches_Name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `Users` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `FirstName` varchar(100) NOT NULL,
  `LastName` varchar(100) NOT NULL,
  `SecondLastName` varchar(100) NOT NULL,
  `Email` varchar(150) NOT NULL,
  `Phone` varchar(20) NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `RoleId` int NOT NULL,
  `BranchId` int NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Users_Email` (`Email`),
  KEY `IX_Users_BranchId` (`BranchId`),
  KEY `IX_Users_RoleId` (`RoleId`),
  CONSTRAINT `FK_Users_Branches_BranchId` FOREIGN KEY (`BranchId`) REFERENCES `Branches` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Users_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Roles` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `__EFMigrationsHistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260418213539_InitialCreate', '8.0.0');

INSERT INTO `Roles` (`Id`, `Name`, `Description`) VALUES
(1, 'Administrador', 'Responsable de la administración general del sistema'),
(2, 'Supervisor', 'Encargado de supervisar usuarios de sucursal'),
(3, 'Operador', 'Usuario operativo con acceso limitado');

INSERT INTO `Branches` (`Id`, `Name`, `Street`, `ExteriorNumber`, `InteriorNumber`, `Neighborhood`, `City`, `State`, `PostalCode`, `Country`) VALUES
(1, 'Sucursal Los Mochis Centro', 'Blvd. Antonio Rosales', '985', 'Local 3', 'Centro', 'Los Mochis', 'Sinaloa', '81200', 'México'),
(2, 'Sucursal Culiacán Norte', 'Blvd. Universitarios', '2100', 'Oficina 5', 'Tres Ríos', 'Culiacán', 'Sinaloa', '80020', 'México'),
(3, 'Sucursal Mazatlán Centro', 'Av. Del Mar', '450', 'Piso 2', 'Centro', 'Mazatlán', 'Sinaloa', '82000', 'México');

INSERT INTO `Users` (`Id`, `FirstName`, `LastName`, `SecondLastName`, `Email`, `Phone`, `IsActive`, `RoleId`, `BranchId`) VALUES
(1, 'Luis', 'Gómez', 'Martínez', 'luis.gomez@empresa.com', '6671234567', 1, 1, 1),
(2, 'Ana', 'López', 'Sánchez', 'ana.lopez@empresa.com', '6682345678', 1, 2, 1),
(3, 'Carlos', 'Pérez', 'Ramírez', 'carlos.perez@empresa.com', '6693456789', 0, 3, 2),
(4, 'María', 'Torres', 'Ruiz', 'maria.torres@empresa.com', '6674567890', 1, 2, 3),
(5, 'José', 'Valdez', 'Castro', 'jose.valdez@empresa.com', '6685678901', 1, 3, 2);

SET FOREIGN_KEY_CHECKS=1;

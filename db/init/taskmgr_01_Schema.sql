-- phpMyAdmin SQL Dump
-- version 5.1.2
-- https://www.phpmyadmin.net/
--
-- Host: localhost:8889
-- Tempo de geraÃ§Ã£o: 11-Nov-2025 Ã s 20:40
-- VersÃ£o do servidor: 5.7.24
-- versÃ£o do PHP: 7.1.23

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Banco de dados: `taskmgr`
--
CREATE DATABASE IF NOT EXISTS `taskmgr` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci;
USE `taskmgr`;

-- --------------------------------------------------------

--
-- Estrutura da tabela `project`
--

DROP TABLE IF EXISTS `project`;
CREATE TABLE IF NOT EXISTS `project` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Title` varchar(200) NOT NULL,
  `Description` text,
  `OwnerId` int(11) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Project_User` (`OwnerId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Estrutura da tabela `taskcomment`
--

DROP TABLE IF EXISTS `taskcomment`;
CREATE TABLE IF NOT EXISTS `taskcomment` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `TaskItemId` int(11) NOT NULL,
  `Message` text NOT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedByUserId` int(11) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_TaskComment_Task` (`TaskItemId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Estrutura da tabela `taskhistory`
--

DROP TABLE IF EXISTS `taskhistory`;
CREATE TABLE IF NOT EXISTS `taskhistory` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `TaskItemId` int(11) NOT NULL,
  `Field` varchar(255) NOT NULL,
  `ChangedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ChangedByUserId` int(11) NOT NULL,
  `ModifiedAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `ModifiedByUserId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_TaskHistory_Task` (`TaskItemId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Estrutura da tabela `taskitem`
--

DROP TABLE IF EXISTS `taskitem`;
CREATE TABLE IF NOT EXISTS `taskitem` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ProjectId` int(11) NOT NULL,
  `Title` varchar(200) NOT NULL,
  `Description` text,
  `DueDate` date DEFAULT NULL,
  `Status` int(11) NOT NULL DEFAULT '0',
  `Priority` int(11) NOT NULL DEFAULT '0',
  `CreatedAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdatedAt` datetime(6) DEFAULT NULL,
  `CompletedAt` datetime(6) DEFAULT NULL,
  `CreatedByUserId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_TaskItem_Project` (`ProjectId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Estrutura da tabela `user`
--

DROP TABLE IF EXISTS `user`;
CREATE TABLE IF NOT EXISTS `user` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(120) NOT NULL,
  `Email` varchar(180) NOT NULL,
  `Role` varchar(20) NOT NULL DEFAULT 'user',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Email` (`Email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- RestriÃ§Ãµes para despejos de tabelas
--

--
-- Limitadores para a tabela `project`
--
ALTER TABLE `project`
  ADD CONSTRAINT `FK_Project_User` FOREIGN KEY (`OwnerId`) REFERENCES `user` (`Id`) ON UPDATE CASCADE;

--
-- Limitadores para a tabela `taskcomment`
--
ALTER TABLE `taskcomment`
  ADD CONSTRAINT `FK_TaskComment_Task` FOREIGN KEY (`TaskItemId`) REFERENCES `taskitem` (`Id`);

--
-- Limitadores para a tabela `taskhistory`
--
ALTER TABLE `taskhistory`
  ADD CONSTRAINT `FK_TaskHistory_Task` FOREIGN KEY (`TaskItemId`) REFERENCES `taskitem` (`Id`);

--
-- Limitadores para a tabela `taskitem`
--
ALTER TABLE `taskitem`
  ADD CONSTRAINT `FK_TaskItem_Project` FOREIGN KEY (`ProjectId`) REFERENCES `project` (`Id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;



-- Inclusão de dados para a tabela `user` - seed
INSERT INTO `user` (`Id`, `Name`, `Email`)
VALUES
  (1, 'Admin', 'admin@taskmgr.local'),
  (2, 'Gerente', 'gerente@taskmgr.local'),
  (3, 'Colaborador', 'colab@taskmgr.local');

update `user` set Role = 'manager' where Id = 1;


 Script de seed para tabela `taskitem`

INSERT INTO `taskitem`
  (`ProjectId`,`Title`,`Description`,`DueDate`,`Status`,`Priority`,`CreatedAt`,`UpdatedAt`,`CompletedAt`,`CreatedByUserId`)
VALUES
  (1, 'Implement login', 'Criar autenticação e fluxo de login', NOW() + INTERVAL 7 DAY, 2, 2, NOW() - INTERVAL 20 DAY, NOW() - INTERVAL 5 DAY, NOW() - INTERVAL 5 DAY, 1),
  (1, 'Add user profile', 'Página de perfil do usuário com edição', NOW() + INTERVAL 10 DAY, 2, 1, NOW() - INTERVAL 15 DAY, NOW() - INTERVAL 3 DAY, NOW() - INTERVAL 3 DAY, 1),
  (1, 'Database backup', 'Agendar backup automático diário', NOW() + INTERVAL 3 DAY, 1, 0, NOW() - INTERVAL 4 DAY, NOW() - INTERVAL 1 DAY, NULL, 2),
  (2, 'Create project template', 'Modelo padrão para novos projetos', NOW() + INTERVAL 14 DAY, 0, 1, NOW() - INTERVAL 2 DAY, NOW() - INTERVAL 2 DAY, NULL, 2),
  (2, 'API: Get tasks', 'Endpoint para listar tarefas paginadas', NOW() + INTERVAL 5 DAY, 2, 1, NOW() - INTERVAL 30 DAY, NOW() - INTERVAL 20 DAY, NOW() - INTERVAL 18 DAY, 3),
  (3, 'Fix bug #423', 'Corrigir exceção ao salvar comentário', NOW() + INTERVAL 1 DAY, 2, 2, NOW() - INTERVAL 7 DAY, NOW() - INTERVAL 2 DAY, NOW() - INTERVAL 1 DAY, 2),
  (3, 'UI: Dashboard charts', 'Adicionar gráficos de desempenho', NOW() + INTERVAL 12 DAY, 1, 1, NOW() - INTERVAL 6 DAY, NOW() - INTERVAL 1 DAY, NULL, 1),
  (1, 'Write tests', 'Cobertura unitária para handlers', NOW() + INTERVAL 9 DAY, 0, 1, NOW() - INTERVAL 1 DAY, NOW() - INTERVAL 1 DAY, NULL, 3),
  (2, 'CI pipeline', 'Configurar CI para build e testes', NOW() + INTERVAL 4 DAY, 1, 2, NOW() - INTERVAL 10 DAY, NOW() - INTERVAL 2 DAY, NULL, 1),
  (3, 'Documentation', 'Atualizar README e API docs', NOW() + INTERVAL 20 DAY, 0, 0, NOW() - INTERVAL 3 DAY, NOW() - INTERVAL 3 DAY, NULL, 2);

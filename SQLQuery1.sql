CREATE DATABASE TaskManagementDB;
GO

USE TaskManagementDB;
GO

CREATE TABLE Assignee (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL
);
GO

CREATE TABLE Status (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL
);
GO

-- Insert sample status values
INSERT INTO Status (Name) VALUES ('Not Started'), ('In Progress'), ('Completed');
GO

CREATE TABLE Task (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    AssigneeId INT FOREIGN KEY REFERENCES Assignee(Id),
    DueDate DATETIME NOT NULL,
    StatusId INT FOREIGN KEY REFERENCES Status(Id)
);
GO

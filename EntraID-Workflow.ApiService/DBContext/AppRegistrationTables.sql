-- Create AppType table
CREATE TABLE AppType (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL
);

-- Insert values into AppType table
INSERT INTO AppType (Name) VALUES ('Web'), ('SPA');

-- Create Status table
CREATE TABLE Status (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL
);

-- Insert values into Status table
INSERT INTO Status (Name) VALUES ('Submitted'), ('In progress'), ('Completed');

-- Create AppRegistration table
CREATE TABLE AppRegistration (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    AppName NVARCHAR(100) NOT NULL,
    OwnerEmail NVARCHAR(100) NOT NULL,
    AppTypeId INT NOT NULL,
    RedirectUrl NVARCHAR(200) NOT NULL,
    StatusId INT NOT NULL,
    FOREIGN KEY (AppTypeId) REFERENCES AppType(Id),
    FOREIGN KEY (StatusId) REFERENCES Status(Id)
);
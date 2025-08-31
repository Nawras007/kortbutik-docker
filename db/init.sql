CREATE DATABASE AppDb;
GO
USE AppDb;

CREATE TABLE Cards (
  CardId INT PRIMARY KEY IDENTITY,
  Name NVARCHAR(100) NOT NULL,
  Type NVARCHAR(50) NOT NULL,
  ManaCost NVARCHAR(50) NOT NULL,
  Description NVARCHAR(MAX) NULL,
  Price DECIMAL(18,2) NOT NULL
);

INSERT INTO Cards (Name, Type, ManaCost, Description, Price)
VALUES ('Fireball', 'Spell', '3R', 'Deals 3 damage', 2.99),
       ('Healing Touch', 'Spell', '2G', 'Restores 5 HP', 1.99);

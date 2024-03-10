SELECT * FROM AspNetUsers;
SELECT * FROM AspNetRoles;
SELECT * FROM AspNetUserRoles;

-- Create Admin and Customer Roles
INSERT INTO AspNetRoles (Id, Name, NormalizedName) 
VALUES 
('1', 'Administrator', 'Administrator'), ('2', 'Customer', 'Customer');

-- Update current user to become admin
DECLARE @userId VARCHAR(255);
SET @userId = '<get user id from AspNetUsers table>';
INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@userId, 1);

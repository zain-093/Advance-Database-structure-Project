-- database/security/roles.sql

-- 1. Create the required database roles
CREATE ROLE db_student;
CREATE ROLE db_faculty;
CREATE ROLE db_admin;
CREATE ROLE db_finance;
GO

-- 2. Restrict direct table access for students (Forcing them to use Stored Procedures or Views)
DENY SELECT ON dbo.Grades TO db_student;
DENY SELECT ON dbo.FeePayments TO db_student;
DENY SELECT ON dbo.Enrollments TO db_student;
GO

-- 3. Grant EXECUTE permissions on our existing stored procedures
GRANT EXECUTE ON OBJECT::dbo.EnrollInCourse TO db_student;
GRANT EXECUTE ON OBJECT::dbo.RegisterStudent TO db_admin;
GO
-- database/security/rls_policy.sql

CREATE SCHEMA Security;
GO

-- A. Predicate Function for Student Access (Enrollments & FeePayments)
CREATE OR ALTER FUNCTION Security.fn_StudentAccessPredicate(@StudentID INT)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN SELECT 1 AS fn_accessResult
WHERE 
    -- Admins and Finance can see all rows
    CAST(SESSION_CONTEXT(N'UserRole') AS NVARCHAR(20)) IN ('Admin', 'Finance')
    OR
    -- Students can only see rows matching their own StudentID
    (CAST(SESSION_CONTEXT(N'UserRole') AS NVARCHAR(20)) = 'Student' 
     AND @StudentID = CAST(SESSION_CONTEXT(N'StudentID') AS INT));
GO

-- B. Predicate Function for Faculty Access (Sections)
CREATE OR ALTER FUNCTION Security.fn_FacultySectionPredicate(@FacultyID INT)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN SELECT 1 AS fn_accessResult
WHERE 
    CAST(SESSION_CONTEXT(N'UserRole') AS NVARCHAR(20)) = 'Admin'
    OR
    (CAST(SESSION_CONTEXT(N'UserRole') AS NVARCHAR(20)) = 'Faculty' 
     AND @FacultyID = CAST(SESSION_CONTEXT(N'FacultyID') AS INT))
    OR
    -- Students need to see all sections so they can enroll
    CAST(SESSION_CONTEXT(N'UserRole') AS NVARCHAR(20)) = 'Student';
GO

-- C. Apply the Security Policies to the Tables
CREATE SECURITY POLICY Security.StudentEnrollmentsPolicy
ADD FILTER PREDICATE Security.fn_StudentAccessPredicate(StudentID) ON dbo.Enrollments,
ADD BLOCK PREDICATE Security.fn_StudentAccessPredicate(StudentID) ON dbo.Enrollments
WITH (STATE = ON);
GO

CREATE SECURITY POLICY Security.StudentFeePaymentsPolicy
ADD FILTER PREDICATE Security.fn_StudentAccessPredicate(StudentID) ON dbo.FeePayments,
ADD BLOCK PREDICATE Security.fn_StudentAccessPredicate(StudentID) ON dbo.FeePayments
WITH (STATE = ON);
GO

CREATE SECURITY POLICY Security.FacultySectionsPolicy
ADD FILTER PREDICATE Security.fn_FacultySectionPredicate(FacultyID) ON dbo.Sections,
ADD BLOCK PREDICATE Security.fn_FacultySectionPredicate(FacultyID) ON dbo.Sections
WITH (STATE = ON);
GO
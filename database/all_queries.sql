-- =========================================================================
-- HITEC UNIVERSITY - ACADEMIC DATA MANAGEMENT SYSTEM (HiSUP)
-- CONSOLIDATED SQL QUERIES FILE (all_queries.sql)
--
-- This file contains ALL SQL queries, DDL schemas, advanced DB objects, 
-- DCL security policies, seed data, and application-level SQL statements 
-- utilized in this project.
-- =========================================================================

USE HiSUP_DB;
GO

-- =========================================================================
-- SECTION 1: DATABASE & SCHEMA DDL (TABLE CREATIONS & CONSTRAINTS)
-- =========================================================================

-- A. Departments Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Departments')
BEGIN
    CREATE TABLE Departments (
        DepartmentID INT PRIMARY KEY IDENTITY(1,1),
        DeptName NVARCHAR(100) NOT NULL UNIQUE,
        DeptCode NVARCHAR(10) NOT NULL UNIQUE,
        EstablishedYear INT CHECK (EstablishedYear >= 1990),
        CreatedAt DATETIME DEFAULT GETDATE()
    );
END
GO

-- B. Programs Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Programs')
BEGIN
    CREATE TABLE Programs (
        ProgramID INT PRIMARY KEY IDENTITY(1,1),
        ProgramName NVARCHAR(100) NOT NULL UNIQUE,
        DepartmentID INT NOT NULL,
        TotalSemesters INT CHECK (TotalSemesters > 0),
        FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID)
    );
END
GO

-- C. Students Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Students')
BEGIN
    CREATE TABLE Students (
        StudentID INT PRIMARY KEY IDENTITY(1000,1),
        FirstName NVARCHAR(50) NOT NULL,
        LastName NVARCHAR(50) NOT NULL,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        CNIC_ClearText NVARCHAR(15) NULL,
        RegistrationNo NVARCHAR(20) NULL,
        Phone NVARCHAR(20) NULL,
        DepartmentID INT NOT NULL,
        ProgramID INT NULL,
        EnrollmentDate DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID),
        FOREIGN KEY (ProgramID) REFERENCES Programs(ProgramID)
    );
END
GO

-- D. Faculty Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Faculty')
BEGIN
    CREATE TABLE Faculty (
        FacultyID INT PRIMARY KEY IDENTITY(1,1),
        FirstName NVARCHAR(50) NOT NULL,
        LastName NVARCHAR(50) NOT NULL,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        DepartmentID INT NOT NULL,
        HireDate DATE DEFAULT GETDATE(),
        FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID)
    );
END
GO

-- E. Staff Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Staff')
BEGIN
    CREATE TABLE Staff (
        StaffID INT PRIMARY KEY IDENTITY(1,1),
        FirstName NVARCHAR(50) NOT NULL,
        LastName NVARCHAR(50) NOT NULL,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        Role NVARCHAR(50) NOT NULL,
        DepartmentID INT NULL, 
        HireDate DATE DEFAULT GETDATE(),
        FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID)
    );
END
GO

-- F. Courses Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Courses')
BEGIN
    CREATE TABLE Courses (
        CourseID INT PRIMARY KEY IDENTITY(1,1),
        CourseCode NVARCHAR(20) NOT NULL UNIQUE,
        CourseName NVARCHAR(100) NOT NULL,
        CreditHours INT CHECK (CreditHours BETWEEN 1 AND 4),
        DepartmentID INT NOT NULL,
        FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID)
    );
END
GO

-- G. CoursePrerequisites Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CoursePrerequisites')
BEGIN
    CREATE TABLE CoursePrerequisites (
        CourseID INT NOT NULL,
        PrerequisiteCourseID INT NOT NULL,
        PRIMARY KEY (CourseID, PrerequisiteCourseID),
        FOREIGN KEY (CourseID) REFERENCES Courses(CourseID),
        FOREIGN KEY (PrerequisiteCourseID) REFERENCES Courses(CourseID)
    );
END
GO

-- H. Sections Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Sections')
BEGIN
    CREATE TABLE Sections (
        SectionID INT PRIMARY KEY IDENTITY(1,1),
        CourseID INT NOT NULL,
        FacultyID INT NOT NULL,
        Semester NVARCHAR(20) NOT NULL,
        SectionName NVARCHAR(10) NOT NULL,
        AvailableSeats INT DEFAULT 40 CHECK (AvailableSeats >= 0),
        FOREIGN KEY (CourseID) REFERENCES Courses(CourseID),
        FOREIGN KEY (FacultyID) REFERENCES Faculty(FacultyID),
        CONSTRAINT UQ_Course_Section UNIQUE (CourseID, Semester, SectionName)
    );
END
GO

-- I. Enrollments Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Enrollments')
BEGIN
    CREATE TABLE Enrollments (
        EnrollmentID INT PRIMARY KEY IDENTITY(1,1),
        StudentID INT NOT NULL,
        CourseID INT NULL,
        Semester NVARCHAR(20) NULL,
        SectionID INT NULL,
        EnrollmentDate DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (StudentID) REFERENCES Students(StudentID) ON DELETE CASCADE,
        FOREIGN KEY (CourseID) REFERENCES Courses(CourseID),
        FOREIGN KEY (SectionID) REFERENCES Sections(SectionID)
    );
END
GO

-- J. Grades Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Grades')
BEGIN
    CREATE TABLE Grades (
        GradeID INT PRIMARY KEY IDENTITY(1,1),
        EnrollmentID INT NOT NULL UNIQUE,
        MarksObtained DECIMAL(5,2) CHECK (MarksObtained >= 0 AND MarksObtained <= 100),
        LetterGrade NVARCHAR(2),
        FOREIGN KEY (EnrollmentID) REFERENCES Enrollments(EnrollmentID) ON DELETE CASCADE
    );
END
GO

-- K. AttendanceRecords Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AttendanceRecords')
BEGIN
    CREATE TABLE AttendanceRecords (
        AttendanceID INT PRIMARY KEY IDENTITY(1,1),
        EnrollmentID INT NULL,
        StudentID INT NULL,
        SectionID INT NULL,
        ClassDate DATE NOT NULL,
        Status NVARCHAR(10) CHECK (Status IN ('Present', 'Absent', 'Leave', 'Late')),
        FOREIGN KEY (EnrollmentID) REFERENCES Enrollments(EnrollmentID) ON DELETE CASCADE,
        FOREIGN KEY (StudentID) REFERENCES Students(StudentID),
        FOREIGN KEY (SectionID) REFERENCES Sections(SectionID)
    );
END
GO

-- L. FeeStructure Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'FeeStructure')
BEGIN
    CREATE TABLE FeeStructure (
        FeeID INT PRIMARY KEY IDENTITY(1,1),
        ProgramID INT NOT NULL,
        Semester NVARCHAR(20) NOT NULL,
        TuitionFee DECIMAL(10,2) CHECK (TuitionFee >= 0),
        LibraryFee DECIMAL(10,2) DEFAULT 0,
        FOREIGN KEY (ProgramID) REFERENCES Programs(ProgramID)
    );
END
GO

-- M. FeePayments Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'FeePayments')
BEGIN
    CREATE TABLE FeePayments (
        PaymentID INT PRIMARY KEY IDENTITY(1,1),
        StudentID INT NOT NULL,
        FeeID INT NOT NULL,
        AmountPaid DECIMAL(10,2) CHECK (AmountPaid > 0),
        PaymentDate DATETIME DEFAULT GETDATE(),
        PaymentMethod NVARCHAR(50),
        FOREIGN KEY (StudentID) REFERENCES Students(StudentID) ON DELETE CASCADE,
        FOREIGN KEY (FeeID) REFERENCES FeeStructure(FeeID)
    );
END
GO

-- N. LibraryItems Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LibraryItems')
BEGIN
    CREATE TABLE LibraryItems (
        ItemID INT PRIMARY KEY IDENTITY(1,1),
        Title NVARCHAR(200) NOT NULL,
        Author NVARCHAR(100),
        ISBN NVARCHAR(20) UNIQUE,
        TotalCopies INT CHECK (TotalCopies >= 0),
        AvailableCopies INT CHECK (AvailableCopies >= 0),
        CONSTRAINT CHK_Copies CHECK (AvailableCopies <= TotalCopies)
    );
END
GO

-- O. LibraryIssues Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LibraryIssues')
BEGIN
    CREATE TABLE LibraryIssues (
        IssueID INT PRIMARY KEY IDENTITY(1,1),
        ItemID INT NOT NULL,
        StudentID INT NOT NULL,
        IssueDate DATETIME DEFAULT GETDATE(),
        DueDate DATETIME NOT NULL,
        ReturnDate DATETIME NULL,
        FineAmount DECIMAL(10,2) DEFAULT 0 CHECK (FineAmount >= 0),
        FOREIGN KEY (ItemID) REFERENCES LibraryItems(ItemID),
        FOREIGN KEY (StudentID) REFERENCES Students(StudentID) ON DELETE CASCADE
    );
END
GO

-- P. Hostels Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Hostels')
BEGIN
    CREATE TABLE Hostels (
        HostelID INT PRIMARY KEY IDENTITY(1,1),
        HostelName NVARCHAR(100) NOT NULL UNIQUE,
        Capacity INT CHECK (Capacity > 0),
        AvailableRooms INT CHECK (AvailableRooms >= 0)
    );
END
GO

-- Q. HostelAllotments Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HostelAllotments')
BEGIN
    CREATE TABLE HostelAllotments (
        AllotmentID INT PRIMARY KEY IDENTITY(1,1),
        HostelID INT NOT NULL,
        StudentID INT NOT NULL UNIQUE, 
        RoomNumber NVARCHAR(10) NOT NULL,
        AllotmentDate DATE DEFAULT GETDATE(),
        FOREIGN KEY (HostelID) REFERENCES Hostels(HostelID),
        FOREIGN KEY (StudentID) REFERENCES Students(StudentID) ON DELETE CASCADE
    );
END
GO

-- R. ExamSchedule Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ExamSchedule')
BEGIN
    CREATE TABLE ExamSchedule (
        ExamID INT PRIMARY KEY IDENTITY(1,1),
        SectionID INT NOT NULL,
        CourseID INT NULL,
        ExamDate DATE NOT NULL,
        StartTime TIME NOT NULL,
        EndTime TIME NOT NULL,
        Venue NVARCHAR(50) NOT NULL,
        FOREIGN KEY (SectionID) REFERENCES Sections(SectionID) ON DELETE CASCADE,
        FOREIGN KEY (CourseID) REFERENCES Courses(CourseID)
    );
END
GO

-- S. Results Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Results')
BEGIN
    CREATE TABLE Results (
        ResultID INT PRIMARY KEY IDENTITY(1,1),
        StudentID INT NOT NULL,
        Semester NVARCHAR(20) NOT NULL,
        SGPA DECIMAL(3,2) CHECK (SGPA >= 0.00 AND SGPA <= 4.00),
        CGPA DECIMAL(3,2) CHECK (CGPA >= 0.00 AND CGPA <= 4.00),
        FOREIGN KEY (StudentID) REFERENCES Students(StudentID) ON DELETE CASCADE,
        CONSTRAINT UQ_Student_Semester_Result UNIQUE (StudentID, Semester)
    );
END
GO

-- T. AuditLog Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AuditLog')
BEGIN
    CREATE TABLE AuditLog (
        LogID INT PRIMARY KEY IDENTITY(1,1),
        TableName NVARCHAR(50) NOT NULL,
        Action NVARCHAR(10) NOT NULL,
        OldValue NVARCHAR(MAX),
        NewValue NVARCHAR(MAX),
        DatabaseUser NVARCHAR(100) DEFAULT SYSTEM_USER,
        LogTimestamp DATETIME DEFAULT GETDATE()
    );
END
GO

-- U. UserAccounts Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserAccounts')
BEGIN
    CREATE TABLE UserAccounts (
        UserID INT PRIMARY KEY IDENTITY(1,1),
        Username NVARCHAR(50) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        Role NVARCHAR(20) CHECK (Role IN ('Admin', 'Student', 'Faculty', 'Finance', 'Library')),
        ReferenceID INT NOT NULL DEFAULT 0
    );
END
GO

-- V. Notifications Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Notifications')
BEGIN
    CREATE TABLE Notifications (
        NotificationID INT PRIMARY KEY IDENTITY(1,1),
        Message NVARCHAR(500) NOT NULL,
        CreatedAt DATETIME DEFAULT GETDATE()
    );
END
GO

-- W. StudentDocuments Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'StudentDocuments')
BEGIN
    CREATE TABLE StudentDocuments (
        DocumentID INT PRIMARY KEY IDENTITY(1,1),
        StudentID INT NOT NULL,
        DocumentName NVARCHAR(200) NOT NULL,
        UploadedAt DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (StudentID) REFERENCES Students(StudentID) ON DELETE CASCADE
    );
END
GO


-- =========================================================================
-- SECTION 2: ADVANCED DATABASE PROGRAMMATIC OBJECTS (VIEWS, PROCS, FUNCTIONS, TRIGGERS)
-- =========================================================================

-- A. View: Student Dashboard (Includes Window Functions)
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_StudentDashboard]'))
    DROP VIEW [dbo].[vw_StudentDashboard];
GO

CREATE VIEW vw_StudentDashboard
WITH SCHEMABINDING
AS
SELECT 
    s.StudentID,
    s.FirstName,
    s.LastName,
    e.Semester,
    c.CourseName,
    -- Window Function: Count total courses enrolled in the current semester
    COUNT(e.CourseID) OVER (PARTITION BY s.StudentID, e.Semester) AS TotalSemesterCourses,
    -- Window Function: Running total of fees paid by the student
    ISNULL(SUM(fp.AmountPaid) OVER (PARTITION BY s.StudentID ORDER BY fp.PaymentDate), 0) AS RunningTotalFeesPaid
FROM dbo.Students s
LEFT JOIN dbo.Enrollments e ON s.StudentID = e.StudentID
LEFT JOIN dbo.Courses c ON e.CourseID = c.CourseID
LEFT JOIN dbo.FeePayments fp ON s.StudentID = fp.StudentID;
GO


-- B. Stored Procedure: EnrollInCourse (Includes Explicit Transaction & Concurrent Locking)
IF EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[EnrollInCourse]'))
    DROP PROCEDURE [dbo].[EnrollInCourse];
GO

CREATE PROCEDURE EnrollInCourse
    @StudentID INT,
    @CourseID INT,
    @Semester NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        -- Explicit Transaction
        BEGIN TRANSACTION;

        DECLARE @AvailableSeats INT;
        
        -- Acquire a Shared Update Lock (UPDLOCK) under Serializable isolation level (SERIALIZABLE)
        -- to prevent phantom reads or double bookings during high-concurrency registration
        SELECT @AvailableSeats = SUM(AvailableSeats)
        FROM Sections WITH (UPDLOCK, SERIALIZABLE)
        WHERE CourseID = @CourseID AND Semester = @Semester;

        IF @AvailableSeats IS NULL OR @AvailableSeats <= 0
            THROW 50002, 'Enrollment Failed: No available seats for this course in the current semester.', 1;

        -- Insert the enrollment
        INSERT INTO Enrollments (StudentID, CourseID, Semester, EnrollmentDate)
        VALUES (@StudentID, @CourseID, @Semester, GETDATE());

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        -- Rollback on failure
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO


-- C. Trigger: Student Audit Log (trg_AuditStudentUpdate)
IF EXISTS (SELECT * FROM sys.triggers WHERE name = N'trg_AuditStudentUpdate')
    DROP TRIGGER trg_AuditStudentUpdate;
GO

CREATE TRIGGER trg_AuditStudentUpdate
ON Students
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Action NVARCHAR(10);
    
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted)
        SET @Action = 'UPDATE';
    ELSE IF EXISTS (SELECT * FROM inserted)
        SET @Action = 'INSERT';
    ELSE
        SET @Action = 'DELETE';

    -- Log inserts or updates
    IF @Action IN ('INSERT', 'UPDATE')
    BEGIN
        INSERT INTO AuditLog (TableName, Action, NewValue, DatabaseUser)
        SELECT 'Students', @Action, CONCAT('StudentID: ', StudentID, ', Name: ', FirstName, ' ', LastName), SYSTEM_USER
        FROM inserted;
    END

    -- Log deletes
    IF @Action = 'DELETE'
    BEGIN
        INSERT INTO AuditLog (TableName, Action, OldValue, DatabaseUser)
        SELECT 'Students', @Action, CONCAT('StudentID: ', StudentID, ', Name: ', FirstName, ' ', LastName), SYSTEM_USER
        FROM deleted;
    END
END;
GO


-- D. Trigger: Populate Enrollment Details from Sections
IF EXISTS (SELECT * FROM sys.triggers WHERE name = N'trg_PopulateEnrollmentDetails')
    DROP TRIGGER trg_PopulateEnrollmentDetails;
GO

CREATE TRIGGER trg_PopulateEnrollmentDetails
ON Enrollments
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE e
    SET e.CourseID = s.CourseID,
        e.Semester = s.Semester
    FROM Enrollments e
    JOIN inserted i ON e.EnrollmentID = i.EnrollmentID
    JOIN Sections s ON i.SectionID = s.SectionID
    WHERE i.SectionID IS NOT NULL;
END;
GO


-- E. Trigger: Populate Attendance Record Linkage
IF EXISTS (SELECT * FROM sys.triggers WHERE name = N'trg_PopulateAttendanceEnrollment')
    DROP TRIGGER trg_PopulateAttendanceEnrollment;
GO

CREATE TRIGGER trg_PopulateAttendanceEnrollment
ON AttendanceRecords
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE ar
    SET ar.EnrollmentID = e.EnrollmentID
    FROM AttendanceRecords ar
    JOIN inserted i ON ar.AttendanceID = i.AttendanceID
    JOIN Enrollments e ON i.StudentID = e.StudentID AND e.SectionID = i.SectionID
    WHERE i.EnrollmentID IS NULL OR i.EnrollmentID = 0;
END;
GO


-- F. Function: Calculate Student CGPA (fn_CalculateCGPA)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fn_CalculateCGPA]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
    DROP FUNCTION [dbo].[fn_CalculateCGPA];
GO

CREATE FUNCTION dbo.fn_CalculateCGPA (@StudentID INT)
RETURNS DECIMAL(3,2)
AS
BEGIN
    DECLARE @CGPA DECIMAL(3,2);
    SELECT TOP 1 @CGPA = CGPA
    FROM Results
    WHERE StudentID = @StudentID
    ORDER BY ResultID DESC;

    IF @CGPA IS NULL
    BEGIN
        DECLARE @TotalPoints DECIMAL(10,2) = 0;
        DECLARE @TotalCredits INT = 0;
        
        SELECT 
            @TotalPoints = SUM(
                CASE 
                    WHEN g.LetterGrade = 'A' THEN 4.0
                    WHEN g.LetterGrade = 'B' THEN 3.0
                    WHEN g.LetterGrade = 'C' THEN 2.0
                    WHEN g.LetterGrade = 'D' THEN 1.0
                    ELSE 0.0
                END * ISNULL(c.CreditHours, 3)
            ),
            @TotalCredits = SUM(ISNULL(c.CreditHours, 3))
        FROM Enrollments e
        JOIN Grades g ON e.EnrollmentID = g.EnrollmentID
        JOIN Sections s ON e.SectionID = s.SectionID
        JOIN Courses c ON s.CourseID = c.CourseID
        WHERE e.StudentID = @StudentID;
        
        IF @TotalCredits > 0
            SET @CGPA = @TotalPoints / @TotalCredits;
        ELSE
            SET @CGPA = 0.0;
    END

    RETURN @CGPA;
END;
GO


-- G. Function: Get Student Outstanding Fee (fn_GetOutstandingFee)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fn_GetOutstandingFee]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
    DROP FUNCTION [dbo].[fn_GetOutstandingFee];
GO

CREATE FUNCTION dbo.fn_GetOutstandingFee (@StudentID INT)
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @TotalFee DECIMAL(10,2) = 0;
    DECLARE @TotalPaid DECIMAL(10,2) = 0;
    DECLARE @ProgramID INT;

    SELECT @ProgramID = ProgramID FROM Students WHERE StudentID = @StudentID;

    SELECT @TotalFee = ISNULL(SUM(fs.TuitionFee + fs.LibraryFee), 0)
    FROM FeeStructure fs
    WHERE fs.ProgramID = @ProgramID
      AND fs.Semester IN (SELECT DISTINCT Semester FROM Enrollments WHERE StudentID = @StudentID);

    IF @TotalFee = 0
    BEGIN
        SELECT @TotalFee = ISNULL(SUM(fs.TuitionFee + fs.LibraryFee), 0)
        FROM FeeStructure fs
        WHERE fs.ProgramID = @ProgramID;
    END

    SELECT @TotalPaid = ISNULL(SUM(AmountPaid), 0)
    FROM FeePayments
    WHERE StudentID = @StudentID;

    RETURN CASE WHEN (@TotalFee - @TotalPaid) > 0 THEN (@TotalFee - @TotalPaid) ELSE 0.0 END;
END;
GO


-- H. Function: Get Student Attendance Percentage (fn_GetAttendancePercentage)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fn_GetAttendancePercentage]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
    DROP FUNCTION [dbo].[fn_GetAttendancePercentage];
GO

CREATE FUNCTION dbo.fn_GetAttendancePercentage (@StudentID INT)
RETURNS DECIMAL(5,2)
AS
BEGIN
    DECLARE @TotalRecords INT;
    DECLARE @PresentRecords INT;
    DECLARE @Percentage DECIMAL(5,2) = 0;

    SELECT @TotalRecords = COUNT(*) 
    FROM AttendanceRecords 
    WHERE StudentID = @StudentID;

    IF @TotalRecords > 0
    BEGIN
        SELECT @PresentRecords = COUNT(*) 
        FROM AttendanceRecords 
        WHERE StudentID = @StudentID AND Status IN ('Present', 'Leave', 'Late');

        SET @Percentage = CAST(@PresentRecords AS DECIMAL(10,2)) * 100.0 / @TotalRecords;
    END

    RETURN @Percentage;
END;
GO


-- =========================================================================
-- SECTION 3: DATABASE ROLES, PRIVILEGES, & ROW-LEVEL SECURITY (RLS)
-- =========================================================================

-- A. Database Roles Setup
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'db_student' AND type = 'R')
    CREATE ROLE db_student;
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'db_faculty' AND type = 'R')
    CREATE ROLE db_faculty;
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'db_admin' AND type = 'R')
    CREATE ROLE db_admin;
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'db_finance' AND type = 'R')
    CREATE ROLE db_finance;
GO

-- Deny direct table access for students (forcing View and Stored Procedure usage)
DENY SELECT ON dbo.Grades TO db_student;
DENY SELECT ON dbo.FeePayments TO db_student;
DENY SELECT ON dbo.Enrollments TO db_student;

-- Grant EXECUTE privileges
GRANT EXECUTE ON OBJECT::dbo.EnrollInCourse TO db_student;
GO

-- B. RLS Predicate Functions
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Security')
    EXEC('CREATE SCHEMA Security');
GO

CREATE OR ALTER FUNCTION Security.fn_StudentAccessPredicate(@StudentID INT)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN SELECT 1 AS fn_accessResult
WHERE 
    -- Admins and Finance can see all records
    CAST(SESSION_CONTEXT(N'UserRole') AS NVARCHAR(20)) IN ('Admin', 'Finance')
    OR
    -- Students can only see their own records
    (CAST(SESSION_CONTEXT(N'UserRole') AS NVARCHAR(20)) = 'Student' 
     AND @StudentID = CAST(SESSION_CONTEXT(N'StudentID') AS INT));
GO

CREATE OR ALTER FUNCTION Security.fn_FacultySectionPredicate(@FacultyID INT)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN SELECT 1 AS fn_accessResult
WHERE 
    CAST(SESSION_CONTEXT(N'UserRole') AS NVARCHAR(20)) = 'Admin'
    OR
    -- Faculty members can only see sections they teach
    (CAST(SESSION_CONTEXT(N'UserRole') AS NVARCHAR(20)) = 'Faculty' 
     AND @FacultyID = CAST(SESSION_CONTEXT(N'FacultyID') AS INT))
    OR
    -- Students can see all sections to facilitate course enrollment
    CAST(SESSION_CONTEXT(N'UserRole') AS NVARCHAR(20)) = 'Student';
GO

-- C. Apply Row-Level Security Policies
IF EXISTS (SELECT * FROM sys.security_policies WHERE name = 'StudentEnrollmentsPolicy')
    DROP SECURITY POLICY Security.StudentEnrollmentsPolicy;
GO
CREATE SECURITY POLICY Security.StudentEnrollmentsPolicy
ADD FILTER PREDICATE Security.fn_StudentAccessPredicate(StudentID) ON dbo.Enrollments,
ADD BLOCK PREDICATE Security.fn_StudentAccessPredicate(StudentID) ON dbo.Enrollments
WITH (STATE = ON);
GO

IF EXISTS (SELECT * FROM sys.security_policies WHERE name = 'StudentFeePaymentsPolicy')
    DROP SECURITY POLICY Security.StudentFeePaymentsPolicy;
GO
CREATE SECURITY POLICY Security.StudentFeePaymentsPolicy
ADD FILTER PREDICATE Security.fn_StudentAccessPredicate(StudentID) ON dbo.FeePayments,
ADD BLOCK PREDICATE Security.fn_StudentAccessPredicate(StudentID) ON dbo.FeePayments
WITH (STATE = ON);
GO

IF EXISTS (SELECT * FROM sys.security_policies WHERE name = 'FacultySectionsPolicy')
    DROP SECURITY POLICY Security.FacultySectionsPolicy;
GO
CREATE SECURITY POLICY Security.FacultySectionsPolicy
ADD FILTER PREDICATE Security.fn_FacultySectionPredicate(FacultyID) ON dbo.Sections,
ADD BLOCK PREDICATE Security.fn_FacultySectionPredicate(FacultyID) ON dbo.Sections
WITH (STATE = ON);
GO


-- =========================================================================
-- SECTION 4: SEED SAMPLE DATA (INITIAL SEEDING)
-- =========================================================================

-- Set connection context temporarily to 'Admin' to bypass RLS filters
EXEC sp_set_session_context N'UserRole', 'Admin';
GO

-- A. Seed Departments
IF NOT EXISTS (SELECT * FROM Departments)
BEGIN
    INSERT INTO Departments (DeptName, DeptCode, EstablishedYear) VALUES 
    ('Computer Science', 'CS', 2005),
    ('Electrical Engineering', 'EE', 2007),
    ('Management Sciences', 'MS', 2010);
END
GO

-- B. Seed Programs
IF NOT EXISTS (SELECT * FROM Programs)
BEGIN
    DECLARE @CS_DeptID INT = (SELECT DepartmentID FROM Departments WHERE DeptCode = 'CS');
    DECLARE @EE_DeptID INT = (SELECT DepartmentID FROM Departments WHERE DeptCode = 'EE');
    INSERT INTO Programs (ProgramName, DepartmentID, TotalSemesters) VALUES 
    ('BS Computer Science', @CS_DeptID, 8),
    ('BS Software Engineering', @CS_DeptID, 8),
    ('BS Electrical Engineering', @EE_DeptID, 8);
END
GO

-- C. Seed Students (Student ID starts at 1000)
IF NOT EXISTS (SELECT * FROM Students WHERE StudentID = 1000)
BEGIN
    DECLARE @CS_DeptID INT = (SELECT DepartmentID FROM Departments WHERE DeptCode = 'CS');
    SET IDENTITY_INSERT Students ON;
    INSERT INTO Students (StudentID, FirstName, LastName, Email, CNIC_ClearText, RegistrationNo, Phone, DepartmentID, ProgramID) VALUES
    (1000, 'Muhammad', 'Asad', 'student@hitec.edu', '37405-1234567-1', '22-CS-1000', '0300-1234567', @CS_DeptID, 1);
    SET IDENTITY_INSERT Students OFF;
END
GO

-- D. Seed Courses
IF NOT EXISTS (SELECT * FROM Courses)
BEGIN
    DECLARE @CS_DeptID INT = (SELECT DepartmentID FROM Departments WHERE DeptCode = 'CS');
    INSERT INTO Courses (CourseCode, CourseName, CreditHours, DepartmentID) VALUES
    ('CS-101', 'Introduction to Computing', 3, @CS_DeptID),
    ('CS-202', 'Database Systems', 4, @CS_DeptID),
    ('CS-303', 'Software Engineering', 3, @CS_DeptID),
    ('CS-404', 'Artificial Intelligence', 3, @CS_DeptID);
END
GO

-- E. Seed Faculty
IF NOT EXISTS (SELECT * FROM Faculty)
BEGIN
    DECLARE @CS_DeptID INT = (SELECT DepartmentID FROM Departments WHERE DeptCode = 'CS');
    INSERT INTO Faculty (FirstName, LastName, Email, DepartmentID, HireDate) VALUES
    ('Jane', 'Smith', 'jane.smith@hitec.edu', @CS_DeptID, '2020-01-15');
END
GO

-- F. Seed Sections
IF NOT EXISTS (SELECT * FROM Sections)
BEGIN
    DECLARE @FacultyID INT = (SELECT TOP 1 FacultyID FROM Faculty);
    DECLARE @CS101 INT = (SELECT CourseID FROM Courses WHERE CourseCode = 'CS-101');
    DECLARE @CS202 INT = (SELECT CourseID FROM Courses WHERE CourseCode = 'CS-202');
    DECLARE @CS303 INT = (SELECT CourseID FROM Courses WHERE CourseCode = 'CS-303');
    DECLARE @CS404 INT = (SELECT CourseID FROM Courses WHERE CourseCode = 'CS-404');
    
    INSERT INTO Sections (CourseID, FacultyID, Semester, SectionName, AvailableSeats) VALUES
    (@CS101, @FacultyID, 'Spring 2025', 'A', 35),
    (@CS202, @FacultyID, 'Spring 2025', 'A', 40),
    (@CS303, @FacultyID, 'Spring 2025', 'A', 30),
    (@CS404, @FacultyID, 'Spring 2025', 'A', 25);
END
GO

-- G. Seed FeeStructure
IF NOT EXISTS (SELECT * FROM FeeStructure)
BEGIN
    DECLARE @BSCS INT = (SELECT ProgramID FROM Programs WHERE ProgramName = 'BS Computer Science');
    INSERT INTO FeeStructure (ProgramID, Semester, TuitionFee, LibraryFee) VALUES
    (@BSCS, 'Spring 2025', 95000.00, 5000.00);
END
GO

-- H. Seed FeePayments
IF NOT EXISTS (SELECT * FROM FeePayments)
BEGIN
    DECLARE @FeeID INT = (SELECT TOP 1 FeeID FROM FeeStructure);
    INSERT INTO FeePayments (StudentID, FeeID, AmountPaid, PaymentDate, PaymentMethod) VALUES
    (1000, @FeeID, 100000.00, GETDATE(), 'HBL Mobile App');
END
GO

-- I. Seed Enrollments
IF NOT EXISTS (SELECT * FROM Enrollments)
BEGIN
    DECLARE @CS101 INT = (SELECT CourseID FROM Courses WHERE CourseCode = 'CS-101');
    DECLARE @CS202 INT = (SELECT CourseID FROM Courses WHERE CourseCode = 'CS-202');
    DECLARE @Sec101 INT = (SELECT SectionID FROM Sections WHERE CourseID = @CS101 AND Semester = 'Spring 2025');
    DECLARE @Sec202 INT = (SELECT SectionID FROM Sections WHERE CourseID = @CS202 AND Semester = 'Spring 2025');
    
    INSERT INTO Enrollments (StudentID, CourseID, Semester, SectionID, EnrollmentDate) VALUES
    (1000, @CS101, 'Spring 2025', @Sec101, GETDATE()),
    (1000, @CS202, 'Spring 2025', @Sec202, GETDATE());
END
GO

-- J. Seed UserAccounts
IF NOT EXISTS (SELECT * FROM UserAccounts)
BEGIN
    DECLARE @FacultyID INT = (SELECT TOP 1 FacultyID FROM Faculty);
    INSERT INTO UserAccounts (Username, PasswordHash, Role, ReferenceID) VALUES
    ('admin', 'password', 'Admin', 1),
    ('student@hitec.edu', 'password', 'Student', 1000),
    ('jane.smith@hitec.edu', 'password', 'Faculty', ISNULL(@FacultyID, 1)),
    ('finance', 'password', 'Finance', 2),
    ('librarian', 'password', 'Library', 3);
END
GO


-- =========================================================================
-- SECTION 5: INLINE SQL QUERIES EXECUTED BY ASP.NET CORE C# CODE
-- =========================================================================

/* 
  1. Connection context initialization (Executed on every DB connection open in SessionConnectionInterceptor.cs)
     Used to set the context parameters for Row-Level Security evaluation.
*/
-- EXEC sp_set_session_context N'UserRole', @UserRole; 
-- EXEC sp_set_session_context N'StudentID', @StudentID;
-- EXEC sp_set_session_context N'FacultyID', @FacultyID;

-- Example:
EXEC sp_set_session_context N'UserRole', N'Student';
EXEC sp_set_session_context N'StudentID', 1000;
EXEC sp_set_session_context N'FacultyID', 0;
GO

/*
  2. Student Statistics Queries (Executed in DashboardController.cs for student dashboard widgets)
*/
-- Fetch calculated CGPA for student 1000:
SELECT dbo.fn_CalculateCGPA(1000) AS CGPA;

-- Fetch calculated Outstanding Fee balance for student 1000:
SELECT dbo.fn_GetOutstandingFee(1000) AS OutstandingFee;

-- Fetch calculated Attendance Percentage for student 1000:
SELECT dbo.fn_GetAttendancePercentage(1000) AS AttendancePercentage;
GO


-- =========================================================================
-- SECTION 6: CORE DYNAMIC QUERIES TRANSLATED FROM APPLICATION CONTROLLERS (EF Core equivalents)
-- =========================================================================

-- A. Account Login (AccountController.cs)
-- Validates login credentials and determines user role
SELECT UserID, Username, Role, ReferenceID 
FROM UserAccounts 
WHERE Username = 'student@hitec.edu' AND PasswordHash = 'password';

-- B. Fetch Student Dashboard Details (DashboardController.cs)
-- 1. Student Profile
SELECT s.*, p.ProgramName, d.DeptName 
FROM Students s
LEFT JOIN Programs p ON s.ProgramID = p.ProgramID
LEFT JOIN Departments d ON s.DepartmentID = d.DepartmentID
WHERE s.StudentID = 1000;

-- 2. Enrollments Detail List
SELECT e.EnrollmentID, c.CourseCode, c.CourseName AS CourseTitle, c.CreditHours, 
       CONCAT(f.FirstName, ' ', f.LastName) AS FacultyName, s.Semester, g.MarksObtained AS Marks, g.LetterGrade
FROM Enrollments e
INNER JOIN Sections s ON e.SectionID = s.SectionID
INNER JOIN Courses c ON s.CourseID = c.CourseID
INNER JOIN Faculty f ON s.FacultyID = f.FacultyID
LEFT JOIN Grades g ON g.EnrollmentID = e.EnrollmentID
WHERE e.StudentID = 1000;

-- 3. Fee Payment History
SELECT PaymentID, AmountPaid, PaymentDate, PaymentMethod 
FROM FeePayments 
WHERE StudentID = 1000 
ORDER BY PaymentDate DESC;

-- 4. Library Borrow Issues
SELECT li.IssueID, b.Title, b.Author, li.IssueDate, li.DueDate, li.ReturnDate, li.FineAmount
FROM LibraryIssues li
INNER JOIN LibraryItems b ON li.ItemID = b.ItemID
WHERE li.StudentID = 1000;

-- 5. Attendance Log
SELECT ar.AttendanceID, c.CourseName AS CourseTitle, c.CourseCode, ar.ClassDate AS AttendanceDate, ar.Status
FROM AttendanceRecords ar
INNER JOIN Sections s ON ar.SectionID = s.SectionID
INNER JOIN Courses c ON s.CourseID = c.CourseID
WHERE ar.StudentID = 1000
ORDER BY ar.ClassDate DESC;

-- 6. Exam Results Record
SELECT ResultID, Semester, SGPA, CGPA 
FROM Results 
WHERE StudentID = 1000 
ORDER BY Semester DESC;

-- C. Course Enrollment Listing (EnrollmentController.cs)
-- Fetches available course sections for student enrollment
SELECT s.SectionID, s.Semester, s.SectionName, s.AvailableSeats,
       c.CourseCode, c.CourseName AS CourseTitle, c.CreditHours,
       CONCAT(f.FirstName, ' ', f.LastName) AS FacultyName
FROM Sections s
INNER JOIN Courses c ON s.CourseID = c.CourseID
INNER JOIN Faculty f ON s.FacultyID = f.FacultyID
WHERE s.Semester = 'Spring 2025';

-- Check if student is already enrolled in a section
SELECT COUNT(*) 
FROM Enrollments 
WHERE StudentID = 1000 AND SectionID = 1; -- @SectionID parameter
GO

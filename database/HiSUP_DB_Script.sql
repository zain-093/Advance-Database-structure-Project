CREATE DATABASE HiSUP_DB;
GO
USE HiSUP_DB;
GO

-- 1. Departments Table
CREATE TABLE Departments (
    DepartmentID INT PRIMARY KEY IDENTITY(1,1),
    DeptName NVARCHAR(100) NOT NULL UNIQUE,
    DeptCode NVARCHAR(10) NOT NULL UNIQUE,
    EstablishedYear INT CHECK (EstablishedYear >= 1990),
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

-- 2. Programs Table
CREATE TABLE Programs (
    ProgramID INT PRIMARY KEY IDENTITY(1,1),
    ProgramName NVARCHAR(100) NOT NULL UNIQUE,
    DepartmentID INT NOT NULL,
    TotalSemesters INT CHECK (TotalSemesters > 0),
    FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID)
);
GO

-- 3. Students Table
CREATE TABLE Students (
    StudentID INT PRIMARY KEY IDENTITY(1000,1),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    CNIC VARBINARY(MAX) NOT NULL, -- Encrypted column requirement
    DepartmentID INT NOT NULL,
    EnrollmentDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID) ON DELETE NO ACTION ON UPDATE CASCADE
);
GO

-- 4. Faculty Table
CREATE TABLE Faculty (
    FacultyID INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    DepartmentID INT NOT NULL,
    HireDate DATE DEFAULT GETDATE(),
    FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID)
);
GO

-- 5. Staff Table
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
GO

-- 6. Courses Table
CREATE TABLE Courses (
    CourseID INT PRIMARY KEY IDENTITY(1,1),
    CourseCode NVARCHAR(20) NOT NULL UNIQUE,
    CourseName NVARCHAR(100) NOT NULL,
    CreditHours INT CHECK (CreditHours BETWEEN 1 AND 4),
    DepartmentID INT NOT NULL,
    FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID)
);
GO

-- 7. Sections Table
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
GO

-- 8. Enrollments Table
CREATE TABLE Enrollments (
    EnrollmentID INT PRIMARY KEY IDENTITY(1,1),
    StudentID INT NOT NULL,
    CourseID INT NOT NULL,
    Semester NVARCHAR(20) NOT NULL,
    EnrollmentDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT UQ_Student_Course UNIQUE (StudentID, CourseID, Semester),
    FOREIGN KEY (StudentID) REFERENCES Students(StudentID) ON DELETE CASCADE,
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID) ON DELETE CASCADE
);
GO

-- 9. Grades Table
CREATE TABLE Grades (
    GradeID INT PRIMARY KEY IDENTITY(1,1),
    EnrollmentID INT NOT NULL UNIQUE,
    MarksObtained DECIMAL(5,2) CHECK (MarksObtained >= 0 AND MarksObtained <= 100),
    LetterGrade NVARCHAR(2),
    FOREIGN KEY (EnrollmentID) REFERENCES Enrollments(EnrollmentID) ON DELETE CASCADE
);
GO

-- 10. AttendanceRecords Table
CREATE TABLE AttendanceRecords (
    AttendanceID INT PRIMARY KEY IDENTITY(1,1),
    EnrollmentID INT NOT NULL,
    ClassDate DATE NOT NULL,
    Status NVARCHAR(10) CHECK (Status IN ('Present', 'Absent', 'Leave', 'Late')),
    FOREIGN KEY (EnrollmentID) REFERENCES Enrollments(EnrollmentID) ON DELETE CASCADE,
    CONSTRAINT UQ_Attendance_Date UNIQUE (EnrollmentID, ClassDate)
);
GO

-- 11. FeeStructure Table
CREATE TABLE FeeStructure (
    FeeID INT PRIMARY KEY IDENTITY(1,1),
    ProgramID INT NOT NULL,
    Semester NVARCHAR(20) NOT NULL,
    TuitionFee DECIMAL(10,2) CHECK (TuitionFee >= 0),
    LibraryFee DECIMAL(10,2) DEFAULT 0,
    FOREIGN KEY (ProgramID) REFERENCES Programs(ProgramID)
);
GO

-- 12. FeePayments Table
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
GO

-- 13. LibraryItems Table
CREATE TABLE LibraryItems (
    ItemID INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    Author NVARCHAR(100),
    ISBN NVARCHAR(20) UNIQUE,
    TotalCopies INT CHECK (TotalCopies >= 0),
    AvailableCopies INT CHECK (AvailableCopies >= 0),
    CONSTRAINT CHK_Copies CHECK (AvailableCopies <= TotalCopies)
);
GO

-- 14. LibraryIssues Table
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
GO

-- 15. Hostels Table
CREATE TABLE Hostels (
    HostelID INT PRIMARY KEY IDENTITY(1,1),
    HostelName NVARCHAR(100) NOT NULL UNIQUE,
    Capacity INT CHECK (Capacity > 0),
    AvailableRooms INT CHECK (AvailableRooms >= 0)
);
GO

-- 16. HostelAllotments Table
CREATE TABLE HostelAllotments (
    AllotmentID INT PRIMARY KEY IDENTITY(1,1),
    HostelID INT NOT NULL,
    StudentID INT NOT NULL UNIQUE, 
    RoomNumber NVARCHAR(10) NOT NULL,
    AllotmentDate DATE DEFAULT GETDATE(),
    FOREIGN KEY (HostelID) REFERENCES Hostels(HostelID),
    FOREIGN KEY (StudentID) REFERENCES Students(StudentID) ON DELETE CASCADE
);
GO

-- 17. ExamSchedule Table
CREATE TABLE ExamSchedule (
    ExamID INT PRIMARY KEY IDENTITY(1,1),
    SectionID INT NOT NULL,
    ExamDate DATE NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    Venue NVARCHAR(50) NOT NULL,
    FOREIGN KEY (SectionID) REFERENCES Sections(SectionID) ON DELETE CASCADE
);
GO

-- 18. Results Table 
CREATE TABLE Results (
    ResultID INT PRIMARY KEY IDENTITY(1,1),
    StudentID INT NOT NULL,
    Semester NVARCHAR(20) NOT NULL,
    SGPA DECIMAL(3,2) CHECK (SGPA >= 0.00 AND SGPA <= 4.00),
    CGPA DECIMAL(3,2) CHECK (CGPA >= 0.00 AND CGPA <= 4.00),
    FOREIGN KEY (StudentID) REFERENCES Students(StudentID) ON DELETE CASCADE,
    CONSTRAINT UQ_Student_Semester_Result UNIQUE (StudentID, Semester)
);
GO

-- 19. AuditLog Table
CREATE TABLE AuditLog (
    LogID INT PRIMARY KEY IDENTITY(1,1),
    TableName NVARCHAR(50) NOT NULL,
    Action NVARCHAR(10) NOT NULL,
    OldValue NVARCHAR(MAX),
    NewValue NVARCHAR(MAX),
    DatabaseUser NVARCHAR(100) DEFAULT SYSTEM_USER,
    LogTimestamp DATETIME DEFAULT GETDATE()
);
GO

-- 20. UserAccounts Table
CREATE TABLE UserAccounts (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) CHECK (Role IN ('Admin', 'Student', 'Faculty', 'Finance')),
    ReferenceID INT NOT NULL 
);
GO
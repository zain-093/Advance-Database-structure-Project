# Entity-Relationship (ER) Diagram - HiSUP Database

This document contains a comprehensive Entity-Relationship (ER) Diagram mapping out the relational schema for the HITEC University Academic Data Management System (HiSUP) database.

## 1. Database ER Diagram

The following interactive vector ER diagram displays all 23 database tables, their attributes (with data types, primary keys, and foreign keys), and the cardinalities between entities.

```mermaid
erDiagram
    Departments ||--o{ Programs : "offers"
    Departments ||--o{ Students : "enrolled_in"
    Departments ||--o{ Faculty : "employs"
    Departments ||--o{ Staff : "employs"
    Departments ||--o{ Courses : "contains"
    
    Programs ||--o{ Students : "has"
    Programs ||--o{ FeeStructure : "defines"

    Courses ||--o{ CoursePrerequisites : "requires"
    Courses ||--o{ Sections : "has sections"
    Courses ||--o{ Enrollments : "for course"
    Courses ||--o{ ExamSchedule : "has exams"

    Faculty ||--o{ Sections : "teaches"

    Sections ||--o{ Enrollments : "contains students"
    Sections ||--o{ AttendanceRecords : "tracks attendance"
    Sections ||--o{ ExamSchedule : "schedules exams"

    Students ||--o{ Enrollments : "registers"
    Students ||--o{ AttendanceRecords : "attends"
    Students ||--o{ FeePayments : "pays"
    Students ||--o{ LibraryIssues : "borrows"
    Students ||--o{ StudentDocuments : "uploads"
    Students ||--|| HostelAllotments : "stays_in"
    Students ||--o{ Results : "achieves"

    Enrollments ||--|| Grades : "receives"
    Enrollments ||--o{ AttendanceRecords : "checks"

    FeeStructure ||--o{ FeePayments : "billed via"

    LibraryItems ||--o{ LibraryIssues : "loaned as"

    Hostels ||--o{ HostelAllotments : "houses"

    Departments {
        int DepartmentID PK
        nvarchar DeptName
        nvarchar DeptCode
        int EstablishedYear
        datetime CreatedAt
    }

    Programs {
        int ProgramID PK
        nvarchar ProgramName
        int DepartmentID FK
        int TotalSemesters
    }

    Students {
        int StudentID PK
        nvarchar FirstName
        nvarchar LastName
        nvarchar Email
        nvarchar CNIC_ClearText
        nvarchar RegistrationNo
        nvarchar Phone
        int DepartmentID FK
        int ProgramID FK
        datetime EnrollmentDate
    }

    Faculty {
        int FacultyID PK
        nvarchar FirstName
        nvarchar LastName
        nvarchar Email
        int DepartmentID FK
        date HireDate
    }

    Staff {
        int StaffID PK
        nvarchar FirstName
        nvarchar LastName
        nvarchar Email
        nvarchar Role
        int DepartmentID FK
        date HireDate
    }

    Courses {
        int CourseID PK
        nvarchar CourseCode
        nvarchar CourseName
        int CreditHours
        int DepartmentID FK
    }

    CoursePrerequisites {
        int CourseID PK, FK
        int PrerequisiteCourseID PK, FK
    }

    Sections {
        int SectionID PK
        int CourseID FK
        int FacultyID FK
        nvarchar Semester
        nvarchar SectionName
        int AvailableSeats
    }

    Enrollments {
        int EnrollmentID PK
        int StudentID FK
        int CourseID FK
        nvarchar Semester
        int SectionID FK
        datetime EnrollmentDate
    }

    Grades {
        int GradeID PK
        int EnrollmentID FK
        decimal MarksObtained
        nvarchar LetterGrade
    }

    AttendanceRecords {
        int AttendanceID PK
        int EnrollmentID FK
        int StudentID FK
        int SectionID FK
        date ClassDate
        nvarchar Status
    }

    FeeStructure {
        int FeeID PK
        int ProgramID FK
        nvarchar Semester
        decimal TuitionFee
        decimal LibraryFee
    }

    FeePayments {
        int PaymentID PK
        int StudentID FK
        int FeeID FK
        decimal AmountPaid
        datetime PaymentDate
        nvarchar PaymentMethod
    }

    LibraryItems {
        int ItemID PK
        nvarchar Title
        nvarchar Author
        nvarchar ISBN
        int TotalCopies
        int AvailableCopies
    }

    LibraryIssues {
        int IssueID PK
        int ItemID FK
        int StudentID FK
        datetime IssueDate
        datetime DueDate
        datetime ReturnDate
        decimal FineAmount
    }

    Hostels {
        int HostelID PK
        nvarchar HostelName
        int Capacity
        int AvailableRooms
    }

    HostelAllotments {
        int AllotmentID PK
        int HostelID FK
        int StudentID FK
        nvarchar RoomNumber
        date AllotmentDate
    }

    ExamSchedule {
        int ExamID PK
        int SectionID FK
        int CourseID FK
        date ExamDate
        time StartTime
        time EndTime
        nvarchar Venue
    }

    Results {
        int ResultID PK
        int StudentID FK
        nvarchar Semester
        decimal SGPA
        decimal CGPA
    }

    AuditLog {
        int LogID PK
        nvarchar TableName
        nvarchar Action
        nvarchar OldValue
        nvarchar NewValue
        nvarchar DatabaseUser
        datetime LogTimestamp
    }

    UserAccounts {
        int UserID PK
        nvarchar Username
        nvarchar PasswordHash
        nvarchar Role
        int ReferenceID
    }

    Notifications {
        int NotificationID PK
        nvarchar Message
        datetime CreatedAt
    }

    StudentDocuments {
        int DocumentID PK
        int StudentID FK
        nvarchar DocumentName
        datetime UploadedAt
    }
```

### Static Image Version
For environments that do not automatically render Mermaid syntax, here is the generated PNG image of the ER Diagram:
![HiSUP Database ER Diagram](er_diagram.png)

## 2. Core Functional Modules


The database structure is organized into six major functional areas:

### 2.1 Academic Infrastructure
- **Departments & Programs**: Forms the hierarchical root. Departments offer multiple Programs.
- **Courses**: Belong to a Department and can have zero-to-many Prerequisites linked self-referentially via `CoursePrerequisites`.
- **Sections**: Represent specific instances of Courses being taught by a Faculty member in a given Semester.

### 2.2 Personnel & Users
- **Students, Faculty, & Staff**: Connect back to their host Departments.
- **UserAccounts**: Manages authentication credentials and role mappings (`Admin`, `Student`, `Faculty`, `Finance`, `Library`), where `ReferenceID` links back to the respective `StudentID`, `FacultyID`, or `StaffID`.

### 2.3 Registration, Attendance, & Grading
- **Enrollments**: Bridges `Students` to their selected `Sections`.
- **Grades**: Has a 1:1 relationship with `Enrollments` to store numeric marks and letter grades.
- **AttendanceRecords**: Tracks daily student presence status ('Present', 'Absent', 'Leave', 'Late') per section.

### 2.4 Finance & Billing
- **FeeStructure**: Defines semester billing limits mapped to degree `Programs`.
- **FeePayments**: Tracks invoices paid by `Students` applied against a specific `FeeStructure`.

### 2.5 Resources & Logistics
- **LibraryItems & Issues**: Manages physical book inventory and borrow logs containing check-out logs and calculated late fines.
- **Hostels & Allotments**: Models hostel building inventory and assigns rooms to students with a unique constraint ensuring a student is allotted to at most one room.

### 2.6 Examinations & Auditing
- **ExamSchedule**: Maps exam date, timing, and venue to academic `Sections` and `Courses`.
- **Results**: Archival ledger tracking semester-by-semester SGPA and CGPA results.
- **AuditLog**: Automated database-trigger logs tracking changes made to the `Students` table.

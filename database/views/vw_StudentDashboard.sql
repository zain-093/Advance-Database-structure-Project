CREATE OR ALTER VIEW vw_StudentDashboard
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
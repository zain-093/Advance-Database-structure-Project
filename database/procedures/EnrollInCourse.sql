CREATE OR ALTER PROCEDURE EnrollInCourse
    @StudentID INT,
    @CourseID INT,
    @Semester NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        -- Explicit transaction to satisfy Module 4 ACID requirements
        BEGIN TRANSACTION;

        -- 1. Check if the course exists and has available seats in ANY section this semester
        -- Using UPDLOCK and SERIALIZABLE to prevent concurrent reads grabbing the same last seat
        DECLARE @AvailableSeats INT;
        
        SELECT @AvailableSeats = SUM(AvailableSeats)
        FROM Sections WITH (UPDLOCK, SERIALIZABLE)
        WHERE CourseID = @CourseID AND Semester = @Semester;

        IF @AvailableSeats IS NULL OR @AvailableSeats <= 0
            THROW 50002, 'Enrollment Failed: No available seats for this course in the current semester.', 1;

        -- 2. Insert the enrollment (UQ_Student_Course constraint prevents duplicate enrollments)
        INSERT INTO Enrollments (StudentID, CourseID, Semester)
        VALUES (@StudentID, @CourseID, @Semester);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        -- Rollback on any failure (e.g., constraint violation or deadlock)
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
-- database/triggers/trg_AuditStudentUpdate.sql

CREATE OR ALTER TRIGGER trg_AuditStudentUpdate
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
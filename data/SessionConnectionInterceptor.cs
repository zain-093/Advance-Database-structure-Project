using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace HiSUP.Data
{
    public class SessionConnectionInterceptor : DbConnectionInterceptor
    {
        public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
        {
            SetSessionContext(connection);
            base.ConnectionOpened(connection, eventData);
        }

        public override async ValueTask ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
        {
            await SetSessionContextAsync(connection);
            await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
        }

        private void SetSessionContext(DbConnection connection)
        {
            using var command = connection.CreateCommand();
            // Injecting the variables our RLS policies are looking for
            command.CommandText = @"
                EXEC sp_set_session_context N'UserRole', 'Student'; 
                EXEC sp_set_session_context N'StudentID', 1000;
            ";
            command.ExecuteNonQuery();
        }

        private async Task SetSessionContextAsync(DbConnection connection)
        {
            using var command = connection.CreateCommand();
            command.CommandText = @"
                EXEC sp_set_session_context N'UserRole', 'Student'; 
                EXEC sp_set_session_context N'StudentID', 1000;
            ";
            await command.ExecuteNonQueryAsync();
        }
    }
}
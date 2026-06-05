using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace HiSUP.Data
{
    public class SessionConnectionInterceptor : DbConnectionInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionConnectionInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
        {
            SetSessionContext(connection);
            base.ConnectionOpened(connection, eventData);
        }

        public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
        {
            await SetSessionContextAsync(connection);
            await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
        }

        private void SetSessionContext(DbConnection connection)
        {
            var user = _httpContextAccessor?.HttpContext?.User;
            string role = user?.FindFirst(ClaimTypes.Role)?.Value ?? "Guest";
            string studentId = user?.FindFirst("StudentID")?.Value ?? "0";
            string facultyId = user?.FindFirst("FacultyID")?.Value ?? "0";

            using var command = connection.CreateCommand();
            command.CommandText = $@"
                EXEC sp_set_session_context N'UserRole', N'{role}'; 
                EXEC sp_set_session_context N'StudentID', {studentId};
                EXEC sp_set_session_context N'FacultyID', {facultyId};
            ";
            command.ExecuteNonQuery();
        }

        private async Task SetSessionContextAsync(DbConnection connection)
        {
            var user = _httpContextAccessor?.HttpContext?.User;
            string role = user?.FindFirst(ClaimTypes.Role)?.Value ?? "Guest";
            string studentId = user?.FindFirst("StudentID")?.Value ?? "0";
            string facultyId = user?.FindFirst("FacultyID")?.Value ?? "0";

            using var command = connection.CreateCommand();
            command.CommandText = $@"
                EXEC sp_set_session_context N'UserRole', N'{role}'; 
                EXEC sp_set_session_context N'StudentID', {studentId};
                EXEC sp_set_session_context N'FacultyID', {facultyId};
            ";
            await command.ExecuteNonQueryAsync();
        }
    }
}
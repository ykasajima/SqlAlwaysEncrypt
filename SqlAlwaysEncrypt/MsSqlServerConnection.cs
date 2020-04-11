using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;

namespace SqlAlwaysEncrypt
{
    /// <summary>
    /// SqlServerConnection for SQL Server 'Column Encryption Setting' support.
    /// see https://github.com/dotnet/SqlClient/issues/11.
    /// </summary>
    public class MsSqlServerConnection : SqlServerConnection
    {
        private bool? isMultipleActiveResultSetsEnabled;

        public MsSqlServerConnection(RelationalConnectionDependencies dependencies)
            : base(dependencies)
        {
        }

        public override bool IsMultipleActiveResultSetsEnabled
        {
            get
            {
                if (isMultipleActiveResultSetsEnabled == null)
                {
                    // System.ArgumentException: 'Keyword not supported: 'column encryption setting'
                    // System.Data.SqlClient.SqlConnectionStringBuilder not supported yet.
                    // use Microsoft.Data.SqlClient.SqlConnectionStringBuilder
                    var connStringBuilder = new SqlConnectionStringBuilder(ConnectionString);
                    isMultipleActiveResultSetsEnabled = connStringBuilder.MultipleActiveResultSets;
                }

                return isMultipleActiveResultSetsEnabled.Value;
            }
        }

        protected override DbConnection CreateDbConnection()
        {
            // System.ArgumentException: 'Keyword not supported: 'column encryption setting'
            // System.Data.SqlClient not supported yet.
            // use Microsoft.Data.SqlClient.SqlConnection
            return new SqlConnection(ConnectionString);
        }
    }
}

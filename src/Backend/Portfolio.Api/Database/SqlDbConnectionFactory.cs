using Microsoft.Extensions.Options;
using Portfolio.Api.AppSettings;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Portfolio.Api.Database
{
    public class SqlDbConnectionFactory : IDbConnectionFactory
    {
        private readonly IOptions<DatabaseSettings> _settings;

        public SqlDbConnectionFactory(IOptions<DatabaseSettings> settings)
        {
            this._settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public IDbConnection Create()
        {
            var connection = new SqlConnection(this._settings.Value.ConnectionString);
            connection.Open();
            return connection;
        }
    }
}
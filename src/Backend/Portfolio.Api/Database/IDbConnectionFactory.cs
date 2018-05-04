using System.Data;

namespace Portfolio.Api.Database
{
    public interface IDbConnectionFactory
    {
        IDbConnection Create();
    }
}
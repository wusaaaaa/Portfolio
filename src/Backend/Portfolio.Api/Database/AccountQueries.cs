using Dapper;
using Portfolio.Api.Entities;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Portfolio.Api.Database
{
    public static class AccountQueries
    {
        public static async Task<Account> QueryAccountByEmail(this IDbConnection self, string emailAddress, IDbTransaction transaction, CancellationToken cancellationToken = default(CancellationToken))
        {
            var sql = "SELECT Id, EmailAddress, PasswordHast FROM dbo.Accounts WHERE EmailAddress = @EmailAddress";
            var parameters = new
            {
                EmailAddress = emailAddress
            };
            var def = new CommandDefinition(commandText: sql, parameters: parameters, transaction: transaction, cancellationToken: cancellationToken);
            return (Account)await self.QueryFirstOrDefaultAsync(typeof(Account), def);
        }

        public static async Task CreateAccount(this IDbConnection self, Account account, IDbTransaction transaction, CancellationToken cancellationToken = default(CancellationToken))
        {
            var sql = "INSERT INTO dbo.Accounts(EmailADdress, PasswordHash) VALUES (@EmailAddress, @PasswordHash); SELECT SCOPE_IDENTITY();";
            var parameters = new
            {
                EmailAddress = account.EmailAddress,
                PasswordHash = account.PasswordHash
            };
            var def = new CommandDefinition(commandText: sql, parameters: parameters, transaction: transaction, cancellationToken: cancellationToken);
        }
    }
}
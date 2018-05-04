using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Database;
using Portfolio.Api.Entities;
using Portfolio.Api.Services;
using Portfolio.Shared.Accounts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Portfolio.Api.Controllers
{
    [Route("accounts")]
    public class AccountsController : Controller
    {
        private readonly IDbConnectionFactory _ddDbConnectionFactory;
        private readonly IJwtService _jwtService;

        public AccountsController(IDbConnectionFactory ddDbConnectionFactory, IJwtService jwtService)
        {
            this._ddDbConnectionFactory = ddDbConnectionFactory ?? throw new ArgumentNullException(nameof(ddDbConnectionFactory));
            this._jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterData data, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (data == null || string.IsNullOrWhiteSpace(data.EmailAddress) || string.IsNullOrWhiteSpace(data.Password))
                return this.BadRequest();

            data.EmailAddress = data.EmailAddress.Trim();

            using (var connection = this._ddDbConnectionFactory.Create())
            using (var transaction = connection.BeginTransaction())
            {
                var existingAccount = await connection.QueryAccountByEmail(data.EmailAddress, transaction, cancellationToken);

                if (existingAccount != null)
                    return this.BadRequest("Email address is already in use.");

                var account = new Account
                {
                    EmailAddress = data.EmailAddress,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(data.Password)
                };

                await connection.CreateAccount(account, transaction, cancellationToken);

                transaction.Commit();
            }

            return this.Ok();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginData data, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (data == null || string.IsNullOrWhiteSpace(data.EmailAddress) || string.IsNullOrWhiteSpace(data.Password))
                return this.BadRequest();

            data.EmailAddress.Trim();

            using (var connection = this._ddDbConnectionFactory.Create())
            using (var transaction = connection.BeginTransaction())
            {
                var account = await connection.QueryAccountByEmail(data.EmailAddress, transaction, cancellationToken);

                if (account == null)
                    return this.NotFound();

                if (BCrypt.Net.BCrypt.Verify(data.Password, account.PasswordHash) == false)
                    return this.Unauthorized();

                var result = new LoginResult
                {
                    Token = this._jwtService.Create(account.Id, account.EmailAddress)
                };

                return this.Ok(result);
            }
        }
    }
}
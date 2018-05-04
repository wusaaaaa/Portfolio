using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.Extensions.Options;
using Portfolio.Api.AppSettings;
using System;
using System.Collections.Generic;

namespace Portfolio.Api.Services
{
    public class JwtService : IJwtService
    {
        private readonly IOptions<AccountSettings> _accountSettings;
        private readonly JwtEncoder _encoder;
        private readonly JwtDecoder _decoder;

        public JwtService(IOptions<AccountSettings> accountSettings)
        {
            this._accountSettings = accountSettings ?? throw new ArgumentNullException(nameof(accountSettings));

            var algorithm = new HMACSHA256Algorithm();
            var serializer = new JsonNetSerializer();
            var urlEncoder = new JwtBase64UrlEncoder();
            var provider = new UtcDateTimeProvider();
            var validator = new JwtValidator(serializer, provider);

            this._encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            this._decoder = new JwtDecoder(serializer, validator, urlEncoder);

        }

        public string Create(int accountId, string emailAddress)
        {
            var payload = new Dictionary<string, object>
            {
                ["accout-id"] = accountId,
                ["email-address"] = emailAddress
            };

            return this._encoder.Encode(payload, this._accountSettings.Value.JwtSecret);
        }

        public bool Validate(string token)
        {
            try
            {
                this._decoder.Decode(token, this._accountSettings.Value.JwtSecret, verify: true);

                return true;
            }
            catch (TokenExpiredException)
            {
                return false;
            }
            catch (SignatureVerificationException)
            {
                return false;
            }
        }
    }
}
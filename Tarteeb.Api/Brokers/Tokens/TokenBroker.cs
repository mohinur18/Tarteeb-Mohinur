﻿//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Tarteeb.Api.Models;
using Tarteeb.Api.Models.Tokens;

namespace Tarteeb.Api.Brokers.Tokens
{
    public class TokenBroker : ITokenBroker
    {
        private readonly TokenConfiguration tokenConfiguration;

        public TokenBroker(IConfiguration configuration)
        {
            tokenConfiguration = new TokenConfiguration();
            configuration.Bind("Jwt",tokenConfiguration);
        }

        public string GenerateJWT(User user)
        {

            byte[] convertedKeyToBytes =
                Encoding.UTF8.GetBytes(tokenConfiguration.Key);

            var securityKey =
                new SymmetricSecurityKey(convertedKeyToBytes);

            var cridentials =
                new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                tokenConfiguration.Issuer,
                tokenConfiguration.Audience,
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cridentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
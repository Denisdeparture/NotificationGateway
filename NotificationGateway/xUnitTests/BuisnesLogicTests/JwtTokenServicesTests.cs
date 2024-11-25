using BuisnesLogic.Realization.Auth;
using BuisnesLogic.Models;
using FluentAssertions;
using MimeKit.Cryptography;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
using BuisnesLogic.ServicesInterface.ClientsInterfaces;
using Microsoft.Extensions.Configuration;
using BuisnesLogic.Models.Other;

namespace xUnitTests.BuisnesLogicTests
{
    public class JwtTokenServicesTests
    {
        [Fact]
        public void Test_When_Params_Was_Null()
        {
           
            //Act
            Action action = () => JwtCreator.CreateToken(null, null);
            //Asserts
            Assert.Throws<ArgumentNullException>(() => action());
          
        }
        [Fact]
        public void Test_Substitution_JwtManager()
        {
            //Arrange
            var manager = new JwtManager(new JwtManagerConfig()
            {
                Audince = "Token-JWT-Control",
                Isssuer = "http://localhost",
                SecurityKey = "https://localhost-JWT.05252125689478080880857",
                ExpirationTimeInMinutes = 60,
            });
            var model = new ServiceAuthModel()
            {
                Id = 1,
                SaltForPassword = "12221",
                Login = "login",
                Password = "pass",
            };
            
            //Act
            var token = JwtCreator.CreateToken(model, manager);
            //Assert
            token.GetType().Should().NotBeNull();
        }
    }
}

using Dapper;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using TimeTable.API.Controllers;
using TimeTable.DataContext.Data;
using TimeTable.DataContext.Models;
using TimeTable.Repository;
using TimeTable.Respository.Interfaces;

namespace TimeTable.UnitTest
{
    public class Tests
    {
        private UserRepository _userRepository;
        private IConfiguration _configuration;
        private ConnectToSql _connectToSql;

        [SetUp]
        public void Setup()
        {
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _connectToSql = new ConnectToSql(_configuration);
            _userRepository = new UserRepository(_connectToSql, _configuration);
        }

        [Test]
        public void Test1()
        {
            var signInModel = new SignInModel
            {
                Email = "user@eaut.edu.vn",
                PassWordHas = "hashedpassword"
            };

            var result = _userRepository.SignInAsync(signInModel);
            Assert.IsNotNull(result);
        }
    }
}
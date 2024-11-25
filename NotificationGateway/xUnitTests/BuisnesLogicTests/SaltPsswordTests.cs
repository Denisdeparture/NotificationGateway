using BuisnesLogic.Service.Safety;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xUnitTests.BuisnesLogicTests
{
    public class SaltPsswordTests
    {
        private SaltPassword _service;
        [Fact]
        public void Test_When_Params_Null_Compare()
        {
            //Arrange
            _service = new SaltPassword();
            //Act
            Action action = () => _service.Compare(string.Empty, string.Empty, string.Empty);
            //Assert
            Assert.Throws<ArgumentNullException>(action);
        }
        [Fact]
        public void Test_When_Params_Null_Salt()
        {
            //Arrange
            _service = new SaltPassword();
            //Act
            Action action = () => _service.Salt(string.Empty, 12);
            //Assert
            Assert.Throws<ArgumentNullException>(action);
        }
        [Fact]
        public void Test_Functional()
        {
            //Arrange
            Random random = new Random();
            _service = new SaltPassword();
            string password = string.Join(string.Empty, Enumerable.Range(0, 10).Select(x => (char)random.Next(97, 122)));
            // Act
            var res = _service.Salt(password, 12);
            // Assert
            Assert.True(_service.Compare(password, res.salt, res.hash));
        }
    }
}

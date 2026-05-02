using Microsoft.VisualStudio.TestTools.UnitTesting;
using GymnasticsScoringSystem.WPF.Services; 

namespace GymnasticsScoringSystem.Tests
{
    [TestClass]
    public class PasswordPolicyTests
    {
        private AuthenticationService _authService;

        [TestInitialize]
        public void Setup()
        {
            _authService = new AuthenticationService("test_data/test_users.json");
        }

        // Тест 1: Пароль короче 8 символов должен возвращать false
        [TestMethod]
        public void IsPasswordStrong_PasswordShorterThan8_ReturnsFalse()
        {
            // Arrange
            string weakPassword = "short"; 

            // Act
            bool result = _authService.IsPasswordStrong(weakPassword);

            // Assert
            Assert.IsFalse(result, "Пароль короче 8 символов не должен быть надёжным");
        }

        // Тест 2: Пароль длиной 8 символов, но без цифры — false
        [TestMethod]
        public void IsPasswordStrong_Length8_NoDigit_ReturnsFalse()
        {
            // Arrange
            string password = "Password"; // 8 символов, но нет цифры

            // Act
            bool result = _authService.IsPasswordStrong(password);

            // Assert
            Assert.IsFalse(result, "Пароль без цифры не должен быть надёжным");
        }

        // Тест 3: Пароль с цифрой, но короче 8 символов — false
        [TestMethod]
        public void IsPasswordStrong_Short_WithDigit_ReturnsFalse()
        {
            // Arrange
            string password = "Pass123"; // 7 символов, есть цифра

            // Act
            bool result = _authService.IsPasswordStrong(password);

            // Assert
            Assert.IsFalse(result, "Короткий пароль с цифрой не должен быть надёжным");
        }

        // Тест 4: Надёжный пароль — длина ≥8 И есть цифра
        [TestMethod]
        public void IsPasswordStrong_ValidPassword_ReturnsTrue()
        {
            // Arrange
            string strongPassword = "StrongPass1"; // 11 символов, есть цифра 1

            // Act
            bool result = _authService.IsPasswordStrong(strongPassword);

            // Assert
            Assert.IsTrue(result, "Надёжный пароль должен возвращать true");
        }

        //Тест 5: Граничный случай — ровно 8 символов с цифрой
        [TestMethod]
        public void IsPasswordStrong_Exactly8Chars_WithDigit_ReturnsTrue()
        {
            // Arrange
            string password = "Pass1234"; // ровно 8 символов, есть цифры

            // Act
            bool result = _authService.IsPasswordStrong(password);

            // Assert
            Assert.IsTrue(result);
        }

        // Тест 6: Граничный случай — 7 символов с цифрой (должен быть false)
        [TestMethod]
        public void IsPasswordStrong_7Chars_WithDigit_ReturnsFalse()
        {
            // Arrange
            string password = "Pass123"; // 7 символов

            // Act
            bool result = _authService.IsPasswordStrong(password);

            // Assert
            Assert.IsFalse(result);
        }

        //Тест 8: Null пароль
        [TestMethod]
        public void IsPasswordStrong_Null_ReturnsFalse()
        {
            // Arrange
            string password = null;

            // Act
            bool result = _authService.IsPasswordStrong(password);

            // Assert
            Assert.IsFalse(result);
        }

        // Тест 9: Пустой пароль
        [TestMethod]
        public void IsPasswordStrong_Empty_ReturnsFalse()
        {
            // Arrange
            string password = "";

            // Act
            bool result = _authService.IsPasswordStrong(password);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
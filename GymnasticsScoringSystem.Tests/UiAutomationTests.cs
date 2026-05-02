using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlaUI.Core;                  // Для Application
using FlaUI.Core.Capturing;
using FlaUI.UIA3;                  // Для UIA3Automation
using System;
using System.Diagnostics;
using System.Threading;
using GymnasticsScoringSystem.Tests.PageObjects;

namespace GymnasticsScoringSystem.Tests
{
    [TestClass]
    public class UiAutomationTests
    {
        private Process _appProcess;
        private UIA3Automation _automation;
        private Application _app; 

        [TestInitialize]
        public void Setup()
        {
            string exePath = @"F:\8 sem\ПОКПО (ДЗ) - Пестин\ЛР\7ЛР\Prog\Net\GymnasticsScoringSystem.WPF\GymnasticsScoringSystem.WPF\bin\Debug\net8.0-windows\GymnasticsScoringSystem.WPF.exe";

            _appProcess = Process.Start(exePath);
            Thread.Sleep(2000); 

            _automation = new UIA3Automation();
            _app = Application.Attach(_appProcess.Id); 
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (_appProcess != null && !_appProcess.HasExited)
            {
                _appProcess.Kill();
                _appProcess.WaitForExit();
            }
            _automation?.Dispose();
        }


        [TestMethod]
        public void Test_01_Positive_Login_Successful()
        {
            var mainWindow = _app.GetMainWindow(_automation); 
            var loginPage = new LoginPage(_automation, mainWindow);

            loginPage.LoginAs("admin", "AdminPass123!");
            Thread.Sleep(1000);

            var adminWindow = _app.GetMainWindow(_automation); 
            Assert.IsNotNull(adminWindow);
            Assert.IsTrue(adminWindow.Name.Contains("Панель администратора") || adminWindow.Name.Contains("Admin"));

            Console.WriteLine("Тест 1 пройден");
        }

        [TestMethod]
        public void Test_02_Negative_Login_WrongPassword()
        {
            var mainWindow = _app.GetMainWindow(_automation); 
            var loginPage = new LoginPage(_automation, mainWindow);

            loginPage.LoginAs("admin", "WrongPassword123");
            Thread.Sleep(1000);

            Assert.IsTrue(loginPage.IsErrorDisplayed(), "Должно отображаться сообщение об ошибке");
            Console.WriteLine("Тест 2 пройден");
        }

        [TestMethod]
        public void Test_03_Register_NewJudge_And_Login()
        {
            // 1. Вход как администратор
            var mainWindow = _app.GetMainWindow(_automation);
            var loginPage = new LoginPage(_automation, mainWindow);
            loginPage.LoginAs("admin", "AdminPass123!");
            Thread.Sleep(2000);

            // 2. Создание судьи
            var adminPage = new AdminPage(_automation, _app.GetMainWindow(_automation));
            string newUsername = "test_judge_" + DateTime.Now.Ticks;
            string newPassword = "TestPass123!";

            adminPage.CreateJudge(newUsername, newPassword);
            Thread.Sleep(2000); 

            // 3. Выход
            adminPage.Logout();
            Thread.Sleep(2000); 

            // 4. Перезапуск приложения
            if (_appProcess != null && !_appProcess.HasExited)
            {
                _appProcess.Kill();
                _appProcess.WaitForExit();
            }
            Thread.Sleep(2000); 

            _appProcess = Process.Start(@"F:\8 sem\ПОКПО (ДЗ) - Пестин\ЛР\7ЛР\Prog\Net\GymnasticsScoringSystem.WPF\GymnasticsScoringSystem.WPF\bin\Debug\net8.0-windows\GymnasticsScoringSystem.WPF.exe");
            Thread.Sleep(3000); 
            _app = Application.Attach(_appProcess.Id);

            // 5. Вход как новый судья
            var newLoginPage = new LoginPage(_automation, _app.GetMainWindow(_automation));
            newLoginPage.LoginAs(newUsername, newPassword);
            Thread.Sleep(2000);

            // 6. Проверка
            var judgeWindow = _app.GetMainWindow(_automation);
            Assert.IsNotNull(judgeWindow, "Окно судьи не открылось");
            Assert.IsTrue(judgeWindow.Name.Contains("Судья") || judgeWindow.Name.Contains("Judge"),
                $"Ожидается окно судьи, но открылось: {judgeWindow.Name}");

            Console.WriteLine("Тест 3 пройден");
        }
    }
}
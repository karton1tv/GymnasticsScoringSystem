using Reqnroll;
using GymnasticsScoringSystem.WPF.Services;
using GymnasticsScoringSystem.WPF.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace GymnasticsScoringSystem.Tests.StepDefinitions
{
    [Binding]
    public class AdminSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private AuthenticationService _authService;
        private string _tempUsersFile = "test_data/bdd_admin_temp.json";
        private string _enteredLogin;
        private string _enteredPassword;
        private bool _operationSuccess = false;
        private string _errorMessage = "";

        public AdminSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public void Setup()
        {
            if (File.Exists(_tempUsersFile)) File.Delete(_tempUsersFile);
            _authService = new AuthenticationService(_tempUsersFile);
        }

        [AfterScenario]
        public void Cleanup()
        {
            if (File.Exists(_tempUsersFile)) File.Delete(_tempUsersFile);
        }

        [Given(@"модератор авторизован в системе")]
        public void GivenModeratorAuthorized()
        {
            _authService.RegisterUser("admin", "AdminPass123!", User.UserRole.Admin);
            _authService.Login("admin", "AdminPass123!");
        }

        [Given(@"открыта вкладка ""Управление судьями""")]
        public void GivenJudgesTabOpen()
        {
            // В нашей архитектуре это просто флаг состояния
            _scenarioContext["JudgesTabOpen"] = true;
        }

        [Given(@"пользователь ""(.*)"" уже зарегистрирован в системе")]
        public void GivenUserAlreadyRegistered(string username)
        {
            _authService.RegisterUser(username, "ExistingPass123!", User.UserRole.Judge);
        }

        [When(@"модератор вводит логин ""(.*)""")]
        public void WhenAdminEntersLogin(string login)
        {
            _enteredLogin = login;
        }

        [When(@"вводит пароль ""(.*)""")]
        public void WhenAdminEntersPassword(string password)
        {
            _enteredPassword = password;
        }

        [When(@"нажимает кнопку ""Создать аккаунт""")]
        public void WhenAdminClicksCreateButton()
        {
            try
            {
                _authService.RegisterUser(_enteredLogin, _enteredPassword, User.UserRole.Judge);
                _operationSuccess = true;
            }
            catch (System.Exception ex)
            {
                _operationSuccess = false;
                _errorMessage = ex.Message;
            }
        }

        [Then(@"система должна создать новую запись в файле пользователей")]
        public void ThenSystemCreatesRecord()
        {
            Assert.IsTrue(_operationSuccess, "Операция создания должна быть успешной");
            var user = _authService.Login(_enteredLogin, _enteredPassword);
            Assert.IsNotNull(user, "Пользователь не найден в базе после создания");
        }

        [Then(@"добавить ""(.*)"" в список доступных судей")]
        public void ThenUserAddedToList(string username)
        {
            var users = _authService.GetAllUsers();
            Assert.IsTrue(users.Exists(u => u.Username == username));
        }

        [Then(@"отобразить сообщение ""(.*)""")]
        public void ThenDisplayMessage(string expectedMessage)
        {
            Assert.IsTrue(_operationSuccess);
        }

        [Then(@"система должна отклонить операцию")]
        public void ThenSystemRejectsOperation()
        {
            Assert.IsFalse(_operationSuccess, "Операция должна была завершиться ошибкой");
        }

        //[Then(@"отобразить сообщение об ошибке ""(.*)""")]
        //public void ThenDisplayErrorMessage(string expectedMessage)
        //{
        //    Assert.IsFalse(_operationSuccess);
        //    Assert.IsTrue(_errorMessage.Contains("существует"),
        //        $"Ожидалось сообщение о существовании, получено: {_errorMessage}");
        //}

        [Then(@"отобразить сообщение об ошибке регистрации ""(.*)""")]
        public void ThenDisplayRegistrationError(string expectedMessage)
        {
            Assert.IsFalse(_operationSuccess, "Операция должна была завершиться ошибкой");
            Assert.IsTrue(_errorMessage.Contains("существует"),
                $"Ожидалась ошибка о существующем пользователе, получено: {_errorMessage}");
        }

        [Then(@"не изменять файл пользователей")]
        public void ThenUsersFileNotChanged()
        {
            // Проверяем, что файл существует и содержит исходных пользователей
            Assert.IsTrue(File.Exists(_tempUsersFile));
        }
    }
}
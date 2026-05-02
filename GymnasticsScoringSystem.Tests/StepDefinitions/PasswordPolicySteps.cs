using Reqnroll;
using GymnasticsScoringSystem.WPF.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GymnasticsScoringSystem.Tests.StepDefinitions
{
    [Binding]
    public class PasswordPolicySteps
    {
        private readonly ScenarioContext _scenarioContext;
        private AuthenticationService _authService;
        private string _enteredPassword;
        private bool _isPasswordStrong;

        public PasswordPolicySteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _authService = new AuthenticationService("test_data/bdd_password_temp.json");
        }

        [Given(@"пользователь находится на форме регистрации или смены пароля")]
        public void GivenOnPasswordForm()
        {
            _scenarioContext["OnPasswordForm"] = true;
        }

        [When(@"пользователь вводит пароль ""(.*)""")]
        public void WhenEntersPassword(string password)
        {
            _enteredPassword = password;
            _isPasswordStrong = _authService.IsPasswordStrong(password);
        }


        [Then(@"система должна оценить пароль как ""(.*)""")]
        public void ThenPasswordEvaluatedAs(string result)
        {
            if (result == "надежный")
            {
                Assert.IsTrue(_isPasswordStrong, $"Пароль '{_enteredPassword}' должен быть надежным");
            }
            else
            {
                Assert.IsFalse(_isPasswordStrong, $"Пароль '{_enteredPassword}' не должен быть надежным");
            }
        }

        [Then(@"если оценка ""(.*)"", должно отобразиться сообщение ""(.*)""")]
        public void ThenDisplayMessage(string evaluation, string expectedMessage)
        {
            if (evaluation == "надежный")
            {
                Assert.IsTrue(_isPasswordStrong, $"Пароль '{_enteredPassword}' должен быть надежным");
            }
            else
            {
                Assert.IsFalse(_isPasswordStrong, $"Пароль '{_enteredPassword}' не должен быть надежным");
            }
        }
    }
}
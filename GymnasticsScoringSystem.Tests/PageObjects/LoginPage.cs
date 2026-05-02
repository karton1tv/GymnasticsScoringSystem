using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;

namespace GymnasticsScoringSystem.Tests.PageObjects
{
    public class LoginPage
    {
        private readonly UIA3Automation _automation;
        private readonly AutomationElement _window;

        // Локаторы элементов (AutomationId из XAML)
        private readonly string _usernameId = "txtUsername";
        private readonly string _passwordId = "txtPassword";
        private readonly string _loginButtonId = "btnLogin";
        private readonly string _errorLabelId = "lblError";

        public LoginPage(UIA3Automation automation, AutomationElement window)
        {
            _automation = automation;
            _window = window;
        }

        // Метод: ввести имя пользователя
        public LoginPage EnterUsername(string username)
        {
            var usernameBox = _window.FindFirstDescendant(cf => cf.ByAutomationId(_usernameId));
            if (usernameBox != null)
            {
                usernameBox.AsTextBox().Text = username;
            }
            return this;
        }

        // Метод: ввести пароль
        public LoginPage EnterPassword(string password)
        {
            var passwordBox = _window.FindFirstDescendant(cf => cf.ByAutomationId(_passwordId));
            if (passwordBox != null)
            {
                passwordBox.AsTextBox().Text = password;
            }
            return this;
        }

        // Метод: нажать кнопку "Войти"
        public void ClickLogin()
        {
            var loginButton = _window.FindFirstDescendant(cf => cf.ByAutomationId(_loginButtonId));
            if (loginButton != null)
            {
                loginButton.Click();
            }
        }

        // Шаг: полная авторизация (объединяет методы)
        public void LoginAs(string username, string password)
        {
            EnterUsername(username);
            EnterPassword(password);
            ClickLogin();
        }

        // Метод: проверить сообщение об ошибке
        public string GetErrorMessage()
        {
            var errorLabel = _window.FindFirstDescendant(cf => cf.ByAutomationId(_errorLabelId));
            if (errorLabel == null) return "";
            // В WPF TextBlock текст обычно доступен через свойство Name
            return errorLabel.Name ?? errorLabel.AsLabel()?.Text ?? "";
        }

        // Метод: проверить, что ошибка отображается
        public bool IsErrorDisplayed()
        {
            var errorLabel = _window.FindFirstDescendant(cf => cf.ByAutomationId(_errorLabelId));
            if (errorLabel == null) return false;

            string text = errorLabel.Name ?? errorLabel.AsLabel()?.Text ?? "";
            return !string.IsNullOrWhiteSpace(text);
        }
    }
}
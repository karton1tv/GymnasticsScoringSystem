using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using System.Runtime.Versioning;
using System.Threading;

namespace GymnasticsScoringSystem.Tests.PageObjects
{
    [SupportedOSPlatform("windows")]
    public class AdminPage
    {
        private readonly UIA3Automation _automation;
        private readonly AutomationElement _window;

        private readonly string _startVotingButtonId = "btnStartVoting";
        private readonly string _createJudgeButtonId = "btnCreateJudge";
        private readonly string _newJudgeUsernameId = "txtNewJudgeUsername";
        private readonly string _newJudgePasswordId = "txtNewJudgePassword";
        private readonly string _logoutButtonId = "btnLogout";
        private readonly string _tabJudgesId = "tabJudges";

        public AdminPage(UIA3Automation automation, AutomationElement window)
        {
            _automation = automation;
            _window = window;
        }

        // Метод: начать голосование
        public void StartVoting()
        {
            var button = _window.FindFirstDescendant(cf => cf.ByAutomationId(_startVotingButtonId));
            if (button != null)
            {
                button.Click();
            }
        }

        // Метод: создать нового судью
        public void CreateJudge(string username, string password)
        {
            // Перейдите на вкладку "Судьи"
            var tabJudges = _window.FindFirstDescendant(cf => cf.ByAutomationId(_tabJudgesId));
            if (tabJudges != null)
            {
                tabJudges.Click();
                Thread.Sleep(500);
            }

            // Введите имя пользователя
            var usernameBox = _window.FindFirstDescendant(cf => cf.ByAutomationId(_newJudgeUsernameId));
            var passwordBox = _window.FindFirstDescendant(cf => cf.ByAutomationId(_newJudgePasswordId));
            var createButton = _window.FindFirstDescendant(cf => cf.ByAutomationId(_createJudgeButtonId));

            if (usernameBox != null) usernameBox.AsTextBox().Text = username;
            if (passwordBox != null) passwordBox.AsTextBox().Text = password;
            if (createButton != null) createButton.Click();
        }

        // Метод: выйти из системы
        public void Logout()
        {
            var logoutButton = _window.FindFirstDescendant(cf => cf.ByAutomationId(_logoutButtonId));
            if (logoutButton != null)
            {
                logoutButton.Click();
                Thread.Sleep(1000);
            }
        }
    }
}
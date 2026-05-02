using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;

namespace GymnasticsScoringSystem.Tests.PageObjects
{
    public class JudgePage
    {
        private readonly UIA3Automation _automation;
        private readonly AutomationElement _window;

        private readonly string _scoreBoxId = "txtScore";
        private readonly string _submitScoreButtonId = "btnSubmitScore";
        private readonly string _logoutButtonId = "btnLogout";

        public JudgePage(UIA3Automation automation, AutomationElement window)
        {
            _automation = automation;
            _window = window;
        }

        // Метод: ввести оценку
        public void EnterScore(string score)
        {
            var scoreBox = _window.FindFirstDescendant(cf => cf.ByAutomationId(_scoreBoxId));
            if (scoreBox != null)
            {
                scoreBox.AsTextBox().Text = score;
            }
        }

        // Метод: отправить оценку
        public void SubmitScore()
        {
            var button = _window.FindFirstDescendant(cf => cf.ByAutomationId(_submitScoreButtonId));
            if (button != null)
            {
                button.Click();
            }
        }

        // Метод: выйти из системы
        public void Logout()
        {
            var logoutButton = _window.FindFirstDescendant(cf => cf.ByAutomationId(_logoutButtonId));
            if (logoutButton != null)
            {
                logoutButton.Click();
            }
        }
    }
}
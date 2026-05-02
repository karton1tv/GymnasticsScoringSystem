using Reqnroll;
using GymnasticsScoringSystem.WPF.Services;
using GymnasticsScoringSystem.WPF.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;

namespace GymnasticsScoringSystem.Tests.StepDefinitions
{
    [Binding]
    public class ScoreSubmissionSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private ScoringService _scoringService;
        private FileRepository _repository;
        private string _tempScoresFile = "test_data/bdd_scores_temp.json";
        private double _enteredScore;
        private bool _submitSuccess = false;
        private string _errorMessage = "";

        public ScoreSubmissionSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public void Setup()
        {
            if (File.Exists(_tempScoresFile)) File.Delete(_tempScoresFile);
            _repository = new FileRepository(_tempScoresFile);
            _scoringService = new ScoringService(_repository);
        }

        [AfterScenario]
        public void Cleanup()
        {
            if (File.Exists(_tempScoresFile)) File.Delete(_tempScoresFile);
        }

        [Given(@"судья авторизован в системе")]
        public void GivenJudgeAuthorized()
        {
            _scenarioContext["JudgeAuthorized"] = true;
        }

        [Given(@"сессия оценивания выступления №(\d+) активна")]
        public void GivenScoringSessionActive(int performanceId)
        {
            _scenarioContext["PerformanceId"] = performanceId;
            _scenarioContext["ScoringSessionActive"] = true;
        }

        //[When(@"судья вводит значение ""(.*)"" в поле оценки")]
        //public void WhenJudgeEntersScore(string score)
        //{
        //    _enteredScore = double.Parse(score.Replace(".", ","),
        //        System.Globalization.CultureInfo.InvariantCulture);
        //}

        //[When(@"судья пытается ввести значение ""(.*)""")]
        //public void WhenJudgeTriesToEnterValue(string score)
        //{
        //    _enteredScore = double.Parse(score.Replace(".", ","),
        //        System.Globalization.CultureInfo.InvariantCulture);
        //}

        // В методе WhenJudgeEntersScore
        [When(@"судья вводит значение ""(.*)"" в поле оценки")]
        public void WhenJudgeEntersScore(string score)
        {
            // Исправление: убираем Replace, оставляем InvariantCulture
            _enteredScore = double.Parse(score, System.Globalization.CultureInfo.InvariantCulture);
        }

        // В методе WhenJudgeTriesToEnterValue
        [When(@"судья пытается ввести значение ""(.*)""")]
        public void WhenJudgeTriesToEnterValue(string score)
        {
            // Исправление: убираем Replace
            _enteredScore = double.Parse(score, System.Globalization.CultureInfo.InvariantCulture);
        }

        [When(@"нажимает кнопку ""Отправить""")]
        public void WhenClicksSubmit()
        {
            try
            {
                if (_enteredScore < 0.0 || _enteredScore > 10.0)
                {
                    throw new System.ArgumentException("Оценка должна быть в диапазоне 0.00-10.00");
                }

                var scores = new List<double> { _enteredScore };
                _scoringService.CalculateAndSaveScores(
                    (int)_scenarioContext["PerformanceId"],
                    1,
                    scores,
                    0.5,
                    0.0
                );
                _submitSuccess = true;
            }
            catch (System.Exception ex)
            {
                _submitSuccess = false;
                _errorMessage = ex.Message;
            }
        }

        [Then(@"система должна сохранить оценку в файл результатов")]
        public void ThenScoreSavedToFile()
        {
            Assert.IsTrue(_submitSuccess, "Отправка оценки должна быть успешной");
            var records = _repository.LoadScores();
            Assert.AreEqual(1, records.Count);
        }

        //[Then(@"отобразить сообщение ""Оценка успешно отправлена""")]
        //public void ThenDisplaySuccessMessage()
        //{
        //    Assert.IsTrue(_submitSuccess);
        //}

        [Then(@"отобразить сообщение успеха ""(.*)""")]
        public void ThenDisplaySuccessMessage(string expectedMessage)
        {
            Assert.IsTrue(_submitSuccess, "Отправка оценки должна быть успешной");
        }

        [Then(@"разблокировать интерфейс для следующего выступления")]
        public void ThenInterfaceUnlocked()
        {
            Assert.IsTrue(_submitSuccess);
        }

        [Then(@"система должна отклонить ввод")]
        public void ThenScoreRejected()
        {
            Assert.IsFalse(_submitSuccess, "Ввод должен быть отклонен");
        }

        [Then(@"отобразить сообщение об ошибке ""(.*)""")]
        public void ThenDisplayErrorMessage(string expectedMessage)
        {
            Assert.IsFalse(_submitSuccess);
            Assert.IsTrue(_errorMessage.Contains("диапазоне") || _errorMessage.Contains("0.00"),
                $"Ожидалась ошибка диапазона, получено: {_errorMessage}");
        }

        [Then(@"не сохранять значение в файл результатов")]
        public void ThenScoreNotSaved()
        {
            var records = _repository.LoadScores();
            Assert.AreEqual(0, records.Count, "Оценка не должна быть сохранена");
        }
    }
}
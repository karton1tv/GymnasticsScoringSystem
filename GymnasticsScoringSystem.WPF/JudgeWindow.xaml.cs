using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using GymnasticsScoringSystem.WPF.Models;
using GymnasticsScoringSystem.WPF.Services;

namespace GymnasticsScoringSystem.WPF
{
    public partial class JudgeWindow : Window
    {
        private readonly ScoringService _scoringService;
        private readonly User _currentUser;

        public JudgeWindow(ScoringService scoringService, User currentUser)
        {
            InitializeComponent();

            _scoringService = scoringService;
            _currentUser = currentUser;

            lblJudgeInfo.Text = "Судья: " + currentUser.Username;
            UpdateVotingStatus();
        }

        private void UpdateVotingStatus()
        {
            // В реальном приложении здесь была бы проверка статуса голосования от сервера
            // Для демонстрации считаем, что голосование всегда активно
            lblVotingStatus.Text = "Статус: Голосование активно";
            lblVotingStatus.Foreground = System.Windows.Media.Brushes.Green;
        }

        private void txtScore_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры и точку
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnSubmitScore_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Парсинг входных данных
                int performanceId = int.Parse(txtPerformanceId.Text);
                int brigadeId = int.Parse(txtBrigadeId.Text);
                double score = double.Parse(txtScore.Text,
                    System.Globalization.CultureInfo.InvariantCulture);
                double maxDeviation = double.Parse(txtMaxDeviation.Text,
                    System.Globalization.CultureInfo.InvariantCulture);
                double penalty = double.Parse(txtPenalty.Text,
                    System.Globalization.CultureInfo.InvariantCulture);

                // Валидация оценки
                if (score < 0.0 || score > 10.0)
                {
                    lblResult.Text = "Ошибка: оценка должна быть от 0.00 до 10.00";
                    lblResult.Foreground = System.Windows.Media.Brushes.Red;
                    return;
                }

                // Для демонстрации: создаём список из одной оценки
                // В реальном приложении здесь была бы агрегация оценок от всех судей бригады
                List<double> scores = new List<double> { score };

                // Расчет и сохранение
                ScoringResult result = _scoringService.CalculateAndSaveScores(
                    performanceId,
                    brigadeId,
                    scores,
                    maxDeviation,
                    penalty);

                // Отображение результата
                lblResult.Text = string.Format(
                    "✓ Оценка сохранена!\nСредняя: {0}\nИтоговая: {1}\nКонсенсус: {2}",
                    result.AverageScore,
                    result.FinalScore,
                    result.IsConsensusReached ? "Да" : "Нет (требуется согласование)");
                lblResult.Foreground = System.Windows.Media.Brushes.Green;

                // Очистка поля оценки для следующего ввода
                txtScore.Clear();
                txtScore.Focus();
            }
            catch (FormatException)
            {
                lblResult.Text = "Ошибка: проверьте формат введённых данных";
                lblResult.Foreground = System.Windows.Media.Brushes.Red;
            }
            catch (ArgumentException ex)
            {
                lblResult.Text = "Ошибка: " + ex.Message;
                lblResult.Foreground = System.Windows.Media.Brushes.Red;
            }
            catch (Exception ex)
            {
                lblResult.Text = "Непредвиденная ошибка: " + ex.Message;
                lblResult.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
using System;
using System.Linq;
using System.Windows;
using GymnasticsScoringSystem.WPF.Models;
using GymnasticsScoringSystem.WPF.Services;

namespace GymnasticsScoringSystem.WPF
{
    public partial class AdminWindow : Window
    {
        private readonly AuthenticationService _authService;
        private readonly ScoringService _scoringService;
        private readonly FileRepository _repository;
        private readonly User _currentUser;
        private bool _isVotingActive = false;

        public AdminWindow(AuthenticationService authService,
            ScoringService scoringService,
            FileRepository repository,
            User currentUser)
        {
            InitializeComponent();

            _authService = authService;
            _scoringService = scoringService;
            _repository = repository;
            _currentUser = currentUser;

            lblWelcome.Text = "Добро пожаловать, " + currentUser.Username;
            LoadJudgesList();
        }

        private void btnStartVoting_Click(object sender, RoutedEventArgs e)
        {
            _isVotingActive = true;
            btnStartVoting.IsEnabled = false;
            btnStopVoting.IsEnabled = true;
            lblVotingStatus.Text = "Статус: Голосование активно";
            lblVotingStatus.Foreground = System.Windows.Media.Brushes.Green;

            MessageBox.Show("Голосование начато!\nСудьи могут вводить оценки.",
                "Информация",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void btnStopVoting_Click(object sender, RoutedEventArgs e)
        {
            _isVotingActive = false;
            btnStartVoting.IsEnabled = true;
            btnStopVoting.IsEnabled = false;
            lblVotingStatus.Text = "Статус: Голосование завершено";
            lblVotingStatus.Foreground = System.Windows.Media.Brushes.Gray;

            MessageBox.Show("Голосование завершено.\nРезультаты сохранены.",
                "Информация",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void btnCreateJudge_Click(object sender, RoutedEventArgs e)
        {
            string username = txtNewJudgeUsername.Text.Trim();
            string password = txtNewJudgePassword.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                lblJudgeResult.Text = "Заполните все поля";
                lblJudgeResult.Foreground = System.Windows.Media.Brushes.Red;
                return;
            }

            try
            {
                _authService.RegisterUser(username, password, User.UserRole.Judge);
                lblJudgeResult.Text = "Судья '" + username + "' успешно создан!";
                lblJudgeResult.Foreground = System.Windows.Media.Brushes.Green;

                txtNewJudgeUsername.Clear();
                txtNewJudgePassword.Clear();
                LoadJudgesList();
            }
            catch (InvalidOperationException ex)
            {
                lblJudgeResult.Text = "Ошибка: " + ex.Message;
                lblJudgeResult.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void LoadJudgesList()
        {
            lstJudges.Items.Clear();
            var judges = _authService.GetAllUsers()
                .Where(u => u.Role == User.UserRole.Judge)
                .Select(u => u.Username);

            foreach (var judge in judges)
            {
                lstJudges.Items.Add(judge);
            }
        }

        private void btnLoadResults_Click(object sender, RoutedEventArgs e)
        {
            var records = _repository.LoadScores();

            if (records.Count == 0)
            {
                txtResults.Text = "Нет сохранённых результатов.";
                return;
            }

            var output = new System.Text.StringBuilder();
            output.AppendLine("=== РЕЗУЛЬТАТЫ ОЦЕНИВАНИЯ ===\n");

            foreach (var record in records)
            {
                output.AppendLine("Выступление #" + record.PerformanceId);
                output.AppendLine("  Бригада: " + record.BrigadeId);
                output.AppendLine("  Оценки: " + string.Join(", ", record.Scores));
                output.AppendLine("  Средняя: " + record.AverageScore);
                output.AppendLine("  Итоговая: " + record.FinalScore);
                output.AppendLine("  Консенсус: " + (record.IsConsensusReached ? "Да" : "Нет"));
                output.AppendLine("  Разброс: " + record.Deviation);
                output.AppendLine();
            }

            txtResults.Text = output.ToString();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
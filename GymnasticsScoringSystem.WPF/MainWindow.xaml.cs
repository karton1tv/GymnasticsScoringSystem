using System;
using System.Windows;
using GymnasticsScoringSystem.WPF.Services;

namespace GymnasticsScoringSystem.WPF
{
    public partial class MainWindow : Window
    {
        private AuthenticationService _authService;
        private FileRepository _repository;
        private ScoringService _scoringService;

        public MainWindow()
        {
            InitializeComponent();

            // Укажите путь, где будет храниться файл пользователей
            string usersPath = "test_data/users.json";

            _repository = new FileRepository("test_data/scores.json");
            // Передаем путь в конструктор
            _authService = new AuthenticationService(usersPath);
            _scoringService = new ScoringService(_repository);

            InitializeTestUsers();
        }

        private void InitializeTestUsers()
        {
            // Администратор
            try
            {
                _authService.RegisterUser("admin", "AdminPass123!", Models.User.UserRole.Admin);
            }
            catch { } // Уже существует

            // Тестовые судьи
            try
            {
                _authService.RegisterUser("judge1", "JudgePass123!", Models.User.UserRole.Judge);
                _authService.RegisterUser("judge2", "JudgePass456!", Models.User.UserRole.Judge);
            }
            catch { }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ShowError("Введите имя пользователя и пароль");
                return;
            }

            try
            {
                Models.User user = _authService.Login(username, password);

                // Открытие соответствующего окна в зависимости от роли
                if (user.Role == Models.User.UserRole.Admin)
                {
                    var adminWindow = new AdminWindow(_authService, _scoringService, _repository, user);
                    adminWindow.Show();
                    this.Hide();

                    // Закрыть окно авторизации при закрытии админ-панели
                    adminWindow.Closed += (s, args) => this.Close();
                }
                else if (user.Role == Models.User.UserRole.Judge)
                {
                    var judgeWindow = new JudgeWindow(_scoringService, user);
                    judgeWindow.Show();
                    this.Hide();

                    judgeWindow.Closed += (s, args) => this.Close();
                }
            }
            catch (InvalidOperationException ex)
            {
                ShowError(ex.Message);
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visibility = Visibility.Visible;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
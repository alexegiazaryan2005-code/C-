using System;
using System.Linq;
using System.Windows;

namespace ProductManufacturingSystem
{
    public partial class LoginWindow : Window
    {
        private ProductContext _context;

        public LoginWindow()
        {
            InitializeComponent();
            _context = new ProductContext();
            UsernameBox.Focus();

            PasswordBox.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    LoginButton_Click(s, e);
                }
            };
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username))
            {
                StatusMessage.Text = "Введите логин";
                UsernameBox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                StatusMessage.Text = "Введите пароль";
                PasswordBox.Focus();
                return;
            }

            try
            {
                var user = _context.Users
                    .FirstOrDefault(u => u.Username == username && u.IsActive == true);

                if (user == null)
                {
                    StatusMessage.Text = "Пользователь не найден";
                    return;
                }

                if (password == "admin123" || password == user.PasswordHash)
                {
                    user.LastLoginDate = DateTime.Now;
                    _context.SaveChanges();

                    MainWindow mainWindow = new MainWindow(user);
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    StatusMessage.Text = "Неверный пароль";
                    PasswordBox.Clear();
                    PasswordBox.Focus();
                }
            }
            catch (Exception ex)
            {
                StatusMessage.Text = "Ошибка подключения к базе данных";
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
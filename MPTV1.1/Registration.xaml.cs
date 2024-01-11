using College_API_V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MPTV1._1
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        private readonly ApiClient apiClient;
        public Registration()
        {
            InitializeComponent();
            apiClient = new ApiClient();
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
        }

        private void Exite_Click(object sender, RoutedEventArgs e)
        {
            dopFuns dopFuns = new dopFuns();
            dopFuns.logOut();
        }

        private async void signUp_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(usEmail.Text) || !string.IsNullOrEmpty(usPass.Text) || !string.IsNullOrEmpty(usPassRepeat.Text))
            {
                if (IsValidEmail(usEmail.Text))
                {
                    if (usEmail.Text.Length >= 6)
                    {
                        if (usPass.Text == usPassRepeat.Text)
                        {
                            UserRegistrationRequest user = new UserRegistrationRequest
                            {
                                Email = usEmail.Text,
                                Password = usPass.Text,
                                UserType = "Abiturient"
                            };
                            try
                            {
                                string registrationResult = await apiClient.RegisterUserAsync(user);

                                // Проверяем, является ли результат ошибкой
                                if (!string.IsNullOrEmpty(registrationResult))
                                {
                                    // Ошибка регистрации
                                    MessageBox.Show($"{registrationResult}");
                                }
                                else
                                {
                                    // Успешная регистрация
                                    Login login = new Login();
                                    Close();
                                    login.ShowDialog();
                                }
                            }
                            catch (Exception ex)
                            {
                                // Обработка других исключений, если они возникнут
                                MessageBox.Show($"Произошла непредвиденная ошибка: {ex.Message}");
                            }
                        }
                        else
                            MessageBox.Show("Пароли не совпадают!");
                    }
                    else
                    {
                        MessageBox.Show("Слишком короткие данные!");
                    }
                }
                else
                {
                    MessageBox.Show("Неверный формат почты!");
                }
            }
            else
            {
                MessageBox.Show("Поля не заполнены!");
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            Close();
        }
    }
}

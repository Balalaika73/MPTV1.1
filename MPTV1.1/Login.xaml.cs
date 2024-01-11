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
using System.Net.Http;
using System.Net;
using System.Windows.Markup;
using College_API_V1.Models;
using Newtonsoft.Json.Linq;

namespace MPTV1._1
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    /// 
    public partial class Login : Window
    {
        HttpClient client = new HttpClient();
        private readonly ApiClient apiClient;

        public Login()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
            apiClient = new ApiClient("https://192.168.100.4:7107/api");
        }

        private void Exite_Click(object sender, RoutedEventArgs e)
        {
            dopFuns dopFuns = new dopFuns();
            dopFuns.logOut();
        }

        private async void enter_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(usEmail.Text) && !string.IsNullOrEmpty(usPass.Text))
            {
                User user = new User
                {
                    Email = usEmail.Text,
                    Password = usPass.Text
                };
                string role = null;
                try
                {
                    role = await apiClient.AuthenticateAsync(user);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Success", MessageBoxButton.OK);
                }


                if (role == "Сотрудник приемной комиссии")
                {
                    MainPriemComission mainPriem = new MainPriemComission();
                    App.Token = usEmail.Text;
                    mainPriem.Show();
                    Close();

                }
                else if (role == "Абитуриент")
                {
                    MainAbiturient mainAbiturient = new MainAbiturient();
                    App.Token = usEmail.Text;
                    mainAbiturient.Show();
                    Close();
                }
                else
                {
                    MessageBox.Show("Неверное имя пользователя или пароль.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Поля не заполнены!");
            }
            
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            Registration registration = new Registration();
            registration.Show();
            Close();
        }
    }
}

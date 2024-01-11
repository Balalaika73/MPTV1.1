using MPTV1._1.Models;
using System;
using System.IO;
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
using Path = System.IO.Path;
using College_API_V1.Models;
using MimeKit;
using MailKit.Net.Smtp;

namespace MPTV1._1
{
    /// <summary>
    /// Логика взаимодействия для InfoAbiturient.xaml
    /// </summary>
    public partial class InfoAbiturient : Window
    {
        string stateNumb;
        string directionName;
        abiturientModel ab = new abiturientModel();
        private readonly ApiClient apiClient;
        public InfoAbiturient(string stateNumb, string directionName)
        {
            InitializeComponent();
            this.stateNumb = stateNumb;
            this.directionName = directionName;
            apiClient = new ApiClient();
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
            pdfWebViewer.Navigate(new Uri("about:blank"));
            PopulateGroupsComboBox();
            LoadAbiturientInfo();
        }
        private async void PopulateGroupsComboBox()
        {
            try
            {
                var groupsList = await apiClient.GetMptGroupsAsync(directionName);

                if (groupsList != null)
                {
                    groups.ItemsSource = groupsList.Select(r => r.grNumb);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            groups.SelectedIndex = 0;
        }

        private async void LoadAbiturientInfo()
        {
            try
            {
                ab = await apiClient.GetAbiturientByStatement(stateNumb);
                FIO.Text = ab.FIO;
                abEmail.Text = ab.abGmail;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void addStudent_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите зачислить этого абитуриента?", "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var res = await apiClient.PostStudentAsync(new student
                {
                    stGmail = abEmail.Text,
                    freeEduc = true
                });

                if (res != null)
                {
                    bool exists = await apiClient.CheckIfStatementExists(abEmail.Text, directionName);
                    if (exists)
                    {
                        await apiClient.UpdateStatementStatusAsync("Одобрено", stateNumb);
                    }
                }
                    MessageBox.Show("Абитуриент успешно зачислен!");
                }
                else
                {
                    MessageBox.Show("Не удалось зачислить абитуриента.");
                }
            }

        private async void addStudentGroup_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите добавить этого абитуриента в группу " + groups.Text + "?", "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var res = await apiClient.PostStudentToGroupAsync(new stGroup
                {
                    stGmail = abEmail.Text,
                    stgroup1 = groups.Text
                });
                SendEmailAsync();
                MessageBox.Show("Абитуриент успешно зачислен!");
            }
        }

        private async Task SendEmailAsync()
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Приемная комиссия", "kiren1187@gmail.com"));
                email.To.Add(new MailboxAddress("empl", abEmail.Text));
                email.Subject = "Договор об оказании образовательных услуг";

                var builder = new BodyBuilder();
                // Добавьте текст или HTML в тело письма
                builder.TextBody = "Поздравляем с поступлением! Заполните договор";

                byte[] docBytes = await apiClient.SendDocAsync();

                // Добавление документа в тело письма
                builder.Attachments.Add("file.doc", docBytes, ContentType.Parse("application/msword"));

                email.Body = builder.ToMessageBody();

                var smtp = new SmtpClient();
                await smtp.ConnectAsync("smtp.gmail.com", 587, false);
                string savedToken = "kiren1187@gmail.com";
                await smtp.AuthenticateAsync(savedToken, "oredlmkwoyhokcxz");
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при отправке письма: " + ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            pcDirection main = new pcDirection(directionName);
            main.Show();
            Close();
        }

        private void Exite_Click(object sender, RoutedEventArgs e)
        {
            dopFuns dopFuns = new dopFuns();
            dopFuns.logOut();
        }

        private void selectCert_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tempFilePath = Path.GetTempFileName();
                File.WriteAllBytes(tempFilePath, ab.abCert);

                pdfWebViewer.Navigate(tempFilePath);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Ошибка: файл не существует");
            }
        }

        private void selectNotice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tempFilePath = Path.GetTempFileName();
                File.WriteAllBytes(tempFilePath, ab.abState);

                pdfWebViewer.Navigate(tempFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: файл не существует");
            }
        }

        private void selectSNILS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tempFilePath = Path.GetTempFileName();
                File.WriteAllBytes(tempFilePath, ab.abSNILS);

                pdfWebViewer.Navigate(tempFilePath);
            }
            catch(Exception ex) {
                MessageBox.Show($"Ошибка: файл не существует");
            }
        }

        private void selectPersAch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tempFilePath = Path.GetTempFileName();
                File.WriteAllBytes(tempFilePath, ab.abAchieve);

                pdfWebViewer.Navigate(tempFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: файл не существует");
            }
        }

        private void selectPassport_Click(object sender, RoutedEventArgs e)
        {
            if (ab.abPhoto != null)
            {
                string tempFilePath = Path.GetTempFileName();
                File.WriteAllBytes(tempFilePath, ab.abPhoto);

                pdfWebViewer.Navigate(tempFilePath);
            }
        }
    }
}

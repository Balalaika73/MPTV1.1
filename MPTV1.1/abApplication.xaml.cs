using System;
using System.ComponentModel;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;
using Path = System.IO.Path;
using MPTV1._1.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace MPTV1._1
{
    /// <summary>
    /// Логика взаимодействия для abApplication.xaml
    /// </summary>
    public partial class abApplication : Window
    {
        private readonly ApiClient apiClient;
        private string selectedDirection;
        private certificate certificate = new certificate();
        private persAchiefe persAchiefe = new persAchiefe();
        string certPath;
        string passpPath;
        string snilsPath;
        string statePath;
        string achievePath;
        abiturient updatedAbiturient = new abiturient();
        abStatement statement = new abStatement();
        AbDisplayAbiturient docs;
        public abApplication(string selectedDirection)
        {
            InitializeComponent();
            apiClient = new ApiClient("https://localhost:7107/api");
            Loaded += Window_Loaded;
            this.selectedDirection = selectedDirection;
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
            pdfWebViewer.Navigate(new Uri("about:blank"));
            LoadDocs();
        }

        private async void LoadDocs()
        {
            docs = await apiClient.GetAbAbiturientsByEmail(App.Token, selectedDirection);
            if (docs != null)
            {
                usName.Text = docs.AbName;
                usSur.Text = docs.AbSur;
                usMiddle.Text = docs.AbSecN;
                certificate.photoCert = docs.AbGPA;
                persAchiefe.achievePhoto = docs.AbAchieves;
                updatedAbiturient.passportPhoto = docs.Passport;
                updatedAbiturient.SNILSPhoto = docs.SNILS;
                statement.photoState = docs.Notice;
            }
        }

        private void SelectCert_Click(object sender, RoutedEventArgs e)
        {
            if (certificate == null || certificate.photoCert == null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "PDF Files|*.pdf";
                if (openFileDialog.ShowDialog() == true)
                {
                    string selectedCertPath = openFileDialog.FileName;
                    try
                    {
                        using (PdfReader reader = new PdfReader(selectedCertPath))
                        {
                            for (int pageNumb = 2; pageNumb <= reader.NumberOfPages; pageNumb++)
                            {
                                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                                string text = PdfTextExtractor.GetTextFromPage(reader, pageNumb, strategy);
                                text = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(text)));
                                MatchCollection matches = Regex.Matches(text, @"\b\d+\b");
                                List<int> ints = new List<int>();
                                foreach (Match match in matches)
                                {
                                    int parsedInt;
                                    if (int.TryParse(match.Value, out parsedInt))
                                    {
                                        ints.Add(parsedInt);
                                    }
                                }
                                double averageValue = ints.Average();
                                certificate.gpa = averageValue;
                                byte[] fileBytes = System.IO.File.ReadAllBytes(selectedCertPath);
                                certificate.photoCert = fileBytes;
                                selectCert.Content = System.IO.Path.GetFileName(selectedCertPath);
                                certPath = selectedCertPath;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Следуйте советам по загружке аттестата или другая общая ошибка");
                    }
                }
            }
            else if (certificate.photoCert != null)
            {
                try
                {
                    string tempFilePath = Path.GetTempFileName();
                    File.WriteAllBytes(tempFilePath, certificate.photoCert);

                    pdfWebViewer.Navigate(tempFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при открытии PDF-файла: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Файл не выбран.");
            }
        }

        private async void UploadFile_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на согласие на обработку персональных данных
            if (agreeChB.IsChecked != true)
            {
                MessageBox.Show("Дайте согласие на обработку персональных данных!");
                return;
            }
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите отправить эти данные?", "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                // Проверка наличия данных в полях ФИО
                if (string.IsNullOrEmpty(usName.Text) || string.IsNullOrEmpty(usSur.Text))
                {
                    MessageBox.Show("Заполните ФИО.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                abiturient fioAb = new abiturient
                {
                    abSur = usSur.Text,
                    abName = usName.Text,
                    abSecN = usMiddle.Text
                };
                string res = await apiClient.AddFIOAbAsync(fioAb);
                // Обработка уведомления
                statement.abGmail = App.Token;
                statement.stateDir = selectedDirection;
                statement.stateSt = "Зарегистрировано";
                if (budgetChB.IsChecked == true)
                    statement.freeEduc = true;
                else
                    statement.freeEduc = false;
                // Проверка наличия заявления
                bool exists = await apiClient.CheckIfStatementExists(App.Token, selectedDirection);
                if (exists)
                {
                    // Если заявление существует, обновляем его
                    statement.numbState = null;
                    statement.stateSt = "Зарегистрировано";
                    await apiClient.UpdateStatementAsync(App.Token, selectedDirection, statement);
                }
                else
                {
                    // Если заявление не существует, добавляем новое
                    await apiClient.AddStatementAsync(statement, App.Token);
                }

                // Обработка данных абитуриента
                updatedAbiturient.abName = usName.Text;
                updatedAbiturient.abSur = usSur.Text;
                updatedAbiturient.abSecN = usMiddle.Text;

                // Обработка сертификата
                if (certificate != null && certificate.photoCert != null)
                {
                    int certId = await apiClient.UploadCertificateAsync(certificate);
                    updatedAbiturient.certId = certId;
                }

                // Обработка достижений
                updatedAbiturient.acieveId = await apiClient.PostPersAchieveAsync(persAchiefe);
                if (updatedAbiturient.SNILSPhoto != null)
                {
                    string result1 = await apiClient.UpdateAbiturientSNILSAsync(updatedAbiturient.SNILSPhoto);
                }
                if (updatedAbiturient.passportPhoto != null)
                {
                    string result1 = await apiClient.UpdateAbiturientPassportAsync(updatedAbiturient.passportPhoto);
                }
                if (updatedAbiturient.certId != null)
                {
                    string result1 = await apiClient.UpdateAbiturientCertAsync((int)updatedAbiturient.certId);
                }

                MessageBox.Show("Зарегистрировано");
            }
        }

        public string ConvertPdfToBase64(string filePath)
        {
            try
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                return Convert.ToBase64String(fileBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
                return null;
            }
        }

        private void Exite_Click(object sender, RoutedEventArgs e)
        {
            dopFuns dopFuns = new dopFuns();
            dopFuns.logOut();
        }

        private void selectSNILS_Click(object sender, RoutedEventArgs e)
        {
            if (updatedAbiturient.SNILSPhoto == null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "PDF Files|*.pdf";
                if (openFileDialog.ShowDialog() == true)
                {
                    string selectedSNILSPath = openFileDialog.FileName;
                    try
                    {
                        byte[] fileBytes = System.IO.File.ReadAllBytes(selectedSNILSPath);
                        updatedAbiturient.SNILSPhoto = fileBytes;
                        selectSNILS.Content = System.IO.Path.GetFileName(selectedSNILSPath);
                        snilsPath = selectedSNILSPath;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка, проверьте файл");
                    }

                }
            }
            else if (updatedAbiturient.SNILSPhoto != null)
            {
                try
                {
                    pdfWebViewer.Navigate(snilsPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при открытии PDF-файла: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Файл не выбран.");
            }
        }

        private void selectPassport_Click(object sender, RoutedEventArgs e)
        {
            if (updatedAbiturient.passportPhoto == null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "PDF Files|*.pdf";
                if (openFileDialog.ShowDialog() == true)
                {
                    string selectedPassportPath = openFileDialog.FileName;
                    try
                    {
                        byte[] fileBytes = System.IO.File.ReadAllBytes(selectedPassportPath);
                        updatedAbiturient.passportPhoto = fileBytes;
                        selectPassport.Content = System.IO.Path.GetFileName(selectedPassportPath);
                        passpPath = selectedPassportPath;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка, проверьте файл");
                    }

                }
            }
            else if (updatedAbiturient.passportPhoto != null)
            {
                try
                {
                    string tempFilePath = Path.GetTempFileName();
                    File.WriteAllBytes(tempFilePath, updatedAbiturient.passportPhoto);

                    pdfWebViewer.Navigate(tempFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при открытии PDF-файла: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Файл не выбран.");
            }
        }

        private void selectNotice_Click(object sender, RoutedEventArgs e)
        {
            if (statement == null || statement.photoState == null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "PDF Files|*.pdf";
                if (openFileDialog.ShowDialog() == true)
                {
                    string selectedStatePath = openFileDialog.FileName;
                    byte[] fileBytes = System.IO.File.ReadAllBytes(selectedStatePath);
                    statement.photoState = fileBytes;
                    selectNotice.Content = System.IO.Path.GetFileName(selectedStatePath);
                    statePath = selectedStatePath;
                }
            }
            else if (statement.photoState != null)
            {
                try
                {
                    string tempFilePath = Path.GetTempFileName();
                    File.WriteAllBytes(tempFilePath, statement.photoState);

                    pdfWebViewer.Navigate(tempFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при открытии PDF-файла: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Файл не выбран.");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            abDirection abDirection = new abDirection(selectedDirection);
            abDirection.Show();
            Close();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            abiturient fullName = await apiClient.GetAbiturientByEmailAsync(App.Token);

            // Заполнение текстовых полей ФИО
            if (fullName!=null)
            {

                if (!String.IsNullOrEmpty(fullName.abName))
                {
                    usName.Text = fullName.abName;
                }

                if (!String.IsNullOrEmpty(fullName.abName))
                {
                    usSur.Text = fullName.abSur;
                }

                if (!String.IsNullOrEmpty(fullName.abName))
                {
                    usMiddle.Text = fullName.abSecN;
                }
            }
        }

        private void cancelCert_Click(object sender, RoutedEventArgs e)
        {
            certificate.gpa = null;
            certificate.photoCert = null;

            selectCert.Content = "Аттестат.pdf";
        }

        private void cancelSNILS_Click(object sender, RoutedEventArgs e)
        {
            updatedAbiturient.SNILSPhoto = null;
            selectSNILS.Content = "СНИЛС.pdf";
        }

        private void cancelPassport_Click(object sender, RoutedEventArgs e)
        {
            updatedAbiturient.passportPhoto = null;
            selectPassport.Content = "Паспорт.pdf";
        }

        private void cancelPersAch_Click(object sender, RoutedEventArgs e)
        {
            persAchiefe.achievePhoto = null;
            persAchiefe.pagesCount = null;
            selectPersAch.Content = "Достижения.pdf";
        }

        private void cancelNotice_Click(object sender, RoutedEventArgs e)
        {
            statement.photoState = null;
            selectNotice.Content = "Уведомление.pdf";
        }

        private void selectPersAch_Click(object sender, RoutedEventArgs e)
        {
            if (persAchiefe == null || persAchiefe.achievePhoto == null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "PDF Files|*.pdf";
                if (openFileDialog.ShowDialog() == true)
                {
                    string selectedAchivePath = openFileDialog.FileName;
                    try
                    {
                        using (PdfReader reader = new PdfReader(selectedAchivePath))
                        {
                            if (reader != null)
                            {
                                persAchiefe.pagesCount = reader.NumberOfPages;
                            }
                            byte[] fileBytes = System.IO.File.ReadAllBytes(selectedAchivePath);
                            persAchiefe.achievePhoto = fileBytes;
                            selectPersAch.Content = System.IO.Path.GetFileName(selectedAchivePath);
                            achievePath = selectedAchivePath;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Следуйте советам по загружке аттестата или другая общая ошибка");
                    }
                }
            }
            else if (persAchiefe.achievePhoto != null)
            {
                try
                {
                    string tempFilePath = Path.GetTempFileName();
                    File.WriteAllBytes(tempFilePath, persAchiefe.achievePhoto);

                    pdfWebViewer.Navigate(tempFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при открытии PDF-файла: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Файл не выбран.");
            }
        }

        private async void saveNotice_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF Files|*.pdf";
            saveFileDialog.Title = "Сохранить уведомление";

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                await apiClient.DownloadPDF(filePath);
            }

        }

        private async void LoadAbiturientInfo()
        {
            try
            {
                /*FIO.Text = ab.FIO;
                abEmail.Text = ab.abGmail;*/
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

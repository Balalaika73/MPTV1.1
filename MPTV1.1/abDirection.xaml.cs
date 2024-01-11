using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Логика взаимодействия для abDirection.xaml
    /// </summary>
    public partial class abDirection : Window, INotifyPropertyChanged
    {
        private ApiClient apiClient;
        private List<DisplayableAbiturient> allAbiturients;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<DisplayableAbiturient> Abiturients { get; set; }
        public abDirection(string directionName)
        {
            InitializeComponent();
            this.Title = directionName;
            apiClient = new ApiClient();
            Abiturients = new ObservableCollection<DisplayableAbiturient>();
            allAbiturients = new List<DisplayableAbiturient>();
            LoadApplyingAbiturients();
            DataContext = this;
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
            sortr.SelectedIndex = 0;
            Abiturients = new ObservableCollection<DisplayableAbiturient>(Abiturients.OrderBy(a => a.AbSur));

            abiturientsView.ItemsSource = Abiturients;
        }


        private void Exite_Click(object sender, RoutedEventArgs e)
        {
            dopFuns dopFuns = new dopFuns();
            dopFuns.logOut();
        }

        private void searchAb_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = searchAb.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Если строка поиска пуста, восстанавливаем все данные из временного списка
                Abiturients.Clear();
                foreach (var abiturient in allAbiturients)
                {
                    Abiturients.Add(abiturient);
                }
            }
            else
            {
                // Если строка поиска не пуста, фильтруем записи по тексту
                var filteredAbiturients = allAbiturients
                    .Where(abiturient =>
                        abiturient.AbSur.ToLower().Contains(searchText) ||
                        abiturient.AbName.ToLower().Contains(searchText) ||
                        abiturient.AbSecN.ToLower().Contains(searchText))
                    .ToList();

                // Обновляем отображаемые данные в DataGrid
                Abiturients.Clear();
                foreach (var abiturient in filteredAbiturients)
                {
                    Abiturients.Add(abiturient);
                }
            }
        }


        private void sortr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedSortOption = ((ComboBoxItem)sortr.SelectedItem).Content.ToString();

            switch (selectedSortOption)
            {
                case "А-Я":
                    Abiturients = new ObservableCollection<DisplayableAbiturient>(Abiturients.OrderBy(a => a.AbSur));
                    break;
                case "Я-А":
                    Abiturients = new ObservableCollection<DisplayableAbiturient>(Abiturients.OrderByDescending(a => a.AbSur));
                    break;
                case "Балл ↑":
                    Abiturients = new ObservableCollection<DisplayableAbiturient>(
        Abiturients
            .OrderBy(a => a.AbGPA)
    );
                    break;


                case "Балл ↓":
                    Abiturients = new ObservableCollection<DisplayableAbiturient>(
        Abiturients
            .OrderByDescending(a => a.AbGPA)
    );
                    break;

                default:
                    break;
            }

            abiturientsView.ItemsSource = Abiturients;
        }

        private void send_Click(object sender, RoutedEventArgs e)
        {
            abApplication abApplication = new abApplication(this.Title);
            abApplication.ShowDialog();
            Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainAbiturient main = new MainAbiturient();
            main.Show();
            Close();
        }

        private async void LoadApplyingAbiturients()
        {
            var abiturnentsByStatements = await apiClient.GetAbiturientsByStatementsAsync(this.Title);

            if (abiturnentsByStatements != null)
            {
                foreach (var abiturient in abiturnentsByStatements)
                {
                    var displayableAbiturient = new DisplayableAbiturient
                    {
                        AbSur = abiturient.AbSur,
                        AbName = abiturient.AbName,
                        AbSecN = abiturient.AbSecN,
                        AbGPA = abiturient.AbGPA ?? ""
                    };

                    Abiturients.Add(displayableAbiturient);
                    allAbiturients.Add(displayableAbiturient);
                }
            }
            else
            {
                // Обработка ошибки загрузки данных
            }
        }

    }
}

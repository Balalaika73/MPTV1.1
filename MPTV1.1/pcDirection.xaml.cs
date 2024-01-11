using MPTV1._1.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MPTV1._1
{
    /// <summary>
    /// Логика взаимодействия для pcDirection.xaml
    /// </summary>
    public partial class pcDirection : Window
    {
        private ApiClient apiClient;
        private List<PCDisplayAbiturients> allAbiturients;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<PCDisplayAbiturients> Abiturients { get; set; }
        public pcDirection(string directionName)
        {
            InitializeComponent();
            this.Title = directionName;
            apiClient = new ApiClient();
            Abiturients = new ObservableCollection<PCDisplayAbiturients>();
            allAbiturients = new List<PCDisplayAbiturients>();
            LoadApplyingAbiturients();
            DataContext = this;
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
            sortr.SelectedIndex = 0;
            Abiturients = new ObservableCollection<PCDisplayAbiturients>(Abiturients.OrderBy(a => a.AbSur));

            abiturientsView.ItemsSource = Abiturients;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void print_Click(object sender, RoutedEventArgs e)
        {

        }

        private void group_Click(object sender, RoutedEventArgs e)
        {

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

        private void Exite_Click(object sender, RoutedEventArgs e)
        {
            dopFuns dopFuns = new dopFuns();
            dopFuns.logOut();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainPriemComission main = new MainPriemComission();
            main.Show();
            Close();
        }

        private void sortr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedSortOption = ((ComboBoxItem)sortr.SelectedItem).Content.ToString();

            switch (selectedSortOption)
            {
                case "А-Я":
                    Abiturients = new ObservableCollection<PCDisplayAbiturients>(Abiturients.OrderBy(a => a.AbSur));
                    break;
                case "Я-А":
                    Abiturients = new ObservableCollection<PCDisplayAbiturients>(Abiturients.OrderByDescending(a => a.AbSur));
                    break;
                case "Балл ↑":
                    Abiturients = new ObservableCollection<PCDisplayAbiturients>(
                    Abiturients
                        .Where(a => !string.IsNullOrEmpty(a.AbGPA))
                        .OrderBy(a => double.Parse(a.AbGPA))
                        .Concat(Abiturients.Where(a => string.IsNullOrEmpty(a.AbGPA)))
                    );
                    break;
                case "Балл ↓":
                    Abiturients = new ObservableCollection<PCDisplayAbiturients>(
                    Abiturients
                        .OrderByDescending(a => !string.IsNullOrEmpty(a.AbGPA))
                        .ThenByDescending(a => string.IsNullOrEmpty(a.AbGPA) ? double.MaxValue : double.Parse(a.AbGPA))
                    );
                    break;
                default:
                    break;
            }

            abiturientsView.ItemsSource = Abiturients;
        }

        private void abiturientsView_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void autoAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void LoadApplyingAbiturients()
        {
            var abiturnentsByStatements = await apiClient.GetPCAbiturientsByStatementsAsync(this.Title);

            if (abiturnentsByStatements != null)
            {
                foreach (var abiturient in abiturnentsByStatements)
                {
                    var displayableAbiturient = new PCDisplayAbiturients
                    {
                        StNumb = abiturient.StNumb,
                        AbSur = abiturient.AbSur,
                        AbName = abiturient.AbName,
                        AbSecN = abiturient.AbSecN,
                        AbGPA = abiturient.AbGPA ?? "",
                        AbAchieves = abiturient.AbAchieves ?? ""
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

        private void abiturientsView_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (abiturientsView.SelectedItem != null)
            {
                var firstColumnValue = abiturientsView.SelectedCells[0].Column.GetCellContent(abiturientsView.SelectedItem) as TextBlock;
                if (firstColumnValue != null)
                {
                    string stateNumb = firstColumnValue.Text;

                    // Передаем почту в новое окно
                    InfoAbiturient info = new InfoAbiturient(stateNumb, this.Title);
                    info.Show();
                    Close();
                }
            }
        }
    }
}

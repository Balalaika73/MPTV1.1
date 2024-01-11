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
    /// Логика взаимодействия для MainAbiturient.xaml
    /// </summary>
    public partial class MainAbiturient : Window, INotifyPropertyChanged
    {
        private ObservableCollection<direction> _directions;
        private readonly ApiClient apiClient;

        public ObservableCollection<direction> Directions
        {
            get { return _directions; }
            set
            {
                if (_directions != value)
                {
                    _directions = value;
                    OnPropertyChanged(nameof(Directions));
                }
            }
        }

        public MainAbiturient()
        {
            InitializeComponent();
            DataContext = this;
            apiClient = new ApiClient();
            Loaded += Window_Loaded;
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
        }

        public async Task GetDirectionsAsync()
        {
            var directions = await apiClient.GetDirectionsAsync();

            if (directions != null)
            {
                Directions = new ObservableCollection<direction>(directions);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await GetDirectionsAsync();
        }

        private Expander lastExpandedExpander = null;

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            Expander expander = sender as Expander;

            if (expander != null)
            {
                if (lastExpandedExpander != null && lastExpandedExpander != expander)
                {
                    lastExpandedExpander.IsExpanded = false;
                }

                lastExpandedExpander = expander;
            }
        }

        private void Exite_Click(object sender, RoutedEventArgs e)
        {
            dopFuns dopFuns = new dopFuns();
            dopFuns.logOut();
        }

        private void GoToDirection_Click(object sender, RoutedEventArgs e)
        {
            if (lastExpandedExpander != null)
            {
                // Получаем выбранный объект направления
                direction selectedDirection = lastExpandedExpander.DataContext as direction;

                if (selectedDirection != null)
                {
                    // Используйте свойство Header, чтобы получить название
                    string directionName = selectedDirection.nameDir;

                    // Открывайте новое окно и передавайте название направления
                    abDirection abDirectionWindow = new abDirection(directionName);
                    abDirectionWindow.Show();
                    Close();
                }
            }
        }

    }
}

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cards
{
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<CardItem> _cards = new();
        private readonly ICollectionView _cardsView;
        private readonly string _jsonPath = Path.Combine(AppContext.BaseDirectory, "cards.json");
        private bool _isLoading;

        public MainWindow()
        {
            InitializeComponent();
            LoadCards();
            _cardsView = CollectionViewSource.GetDefaultView(_cards);
            cardsListBox.ItemsSource = _cardsView;
        }

        private void LoadCards()
        {
            if (!File.Exists(_jsonPath)) return;

            string json = File.ReadAllText(_jsonPath);
            List<CardItem>? cards = JsonSerializer.Deserialize<List<CardItem>>(json);

            if (cards == null) return;

            foreach (CardItem card in cards) _cards.Add(card);
        }

        private void SaveCards()
        {
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_cards, options);
            File.WriteAllText(_jsonPath, json);
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            CardItem card = new CardItem { Question = string.Empty, Answer = string.Empty };
            _cards.Add(card);
            cardsListBox.SelectedItem = card;
            SaveCards();
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (cardsListBox.SelectedItem is not CardItem card) return;

            _cards.Remove(card);
            questionTextBox.Text = string.Empty;
            answerTextBox.Text = string.Empty;
            SaveCards();
        }

        private void cardsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cardsListBox.SelectedItem is not CardItem card) return;

            _isLoading = true;
            questionTextBox.Text = card.Question;
            answerTextBox.Text = card.Answer;
            _isLoading = false;
        }

        private void questionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isLoading) return;
            if (cardsListBox.SelectedItem is not CardItem card) return;

            card.Question = questionTextBox.Text;
            _cardsView.Refresh();
        }

        private void answerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isLoading) return;
            if (cardsListBox.SelectedItem is not CardItem card) return;

            card.Answer = answerTextBox.Text;
            _cardsView.Refresh();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveCards();
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_cardsView == null) return;

            string search = searchTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(search))
            {
                _cardsView.Filter = null;
                return;
            }

            _cardsView.Filter = item =>
            {
                if (item is not CardItem card) return false;
                return card.Question.Contains(search, StringComparison.OrdinalIgnoreCase) || card.Answer.Contains(search, StringComparison.OrdinalIgnoreCase);
            };
        }
    }
}
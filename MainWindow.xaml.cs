using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProductManufacturingSystem
{
    public partial class MainWindow : Window
    {
        private ProductContext _context;
        private User _currentUser;

        public MainWindow(User currentUser)
        {
            InitializeComponent();
            _context = new ProductContext();
            _currentUser = currentUser;

            UserInfoText.Text = $"Добро пожаловать, {_currentUser.FullName} | Роль: {_currentUser.Role}";

            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => TimeText.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            timer.Start();

            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                ProductsPanel.Children.Clear();

                var products = _context.Products
                    .Include("WorkshopTimes")
                    .ToList();

                if (!products.Any())
                {
                    ShowNoDataMessage();
                    return;
                }

                foreach (var product in products)
                {
                    var productBlock = CreateProductBlock(product);
                    ProductsPanel.Children.Add(productBlock);
                }

                StatusText.Text = $"Загружено {products.Count} позиций | Последнее обновление: {DateTime.Now:HH:mm:ss}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "Ошибка загрузки продукции";
            }
        }

        private void ShowNoDataMessage()
        {
            var messageBlock = new TextBlock
            {
                Text = "Нет данных о продукции\nПожалуйста, добавьте продукцию в базу данных",
                FontSize = 16,
                Foreground = (Brush)new BrushConverter().ConvertFrom("#7F8C8D"),
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(50),
                FontWeight = FontWeights.SemiBold
            };
            ProductsPanel.Children.Add(messageBlock);
            StatusText.Text = "Нет данных о продукции";
        }

        private Border CreateProductBlock(Product product)
        {
            // Получаем времена по цехам (первые 3 цеха)
            var workshopTimes = product.WorkshopTimes?.OrderBy(w => w.WorkshopId).ToList();

            string timeWorkshop1 = (workshopTimes != null && workshopTimes.Count > 0) ? $"{workshopTimes[0].TimeInHours} ч." : "0 ч.";
            string timeWorkshop2 = (workshopTimes != null && workshopTimes.Count > 1) ? $"{workshopTimes[1].TimeInHours} ч." : "0 ч.";
            string timeWorkshop3 = (workshopTimes != null && workshopTimes.Count > 2) ? $"{workshopTimes[2].TimeInHours} ч." : "0 ч.";

            // Основной контейнер блока
            var blockBorder = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(8),
                Margin = new Thickness(0, 0, 0, 20),
                BorderBrush = (Brush)new BrushConverter().ConvertFrom("#DDD"),
                BorderThickness = new Thickness(1)
            };

            var mainGrid = new Grid();

            // 3 колонки
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.5, GridUnitType.Star) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.8, GridUnitType.Star) });

            // ========== СТРОКА 0 - ЗАГОЛОВКИ ==========
            AddHeaderCell(mainGrid, 0, 0, "Тип");
            AddHeaderCell(mainGrid, 0, 1, "Наименование продукта");
            AddHeaderCell(mainGrid, 0, 2, "Время изготовления");

            // ========== СТРОКА 1 - Артикул ==========
            AddLabelCell(mainGrid, 1, 0, "Артикул");
            AddValueCell(mainGrid, 1, 1, product.Article ?? "—");
            AddValueCell(mainGrid, 1, 2, timeWorkshop1, true);

            // ========== СТРОКА 2 - Минимальная стоимость ==========
            AddLabelCell(mainGrid, 2, 0, "Минимальная стоимость для партнера");
            AddValueCell(mainGrid, 2, 1, $"{product.MinPartnerPrice:N2} ₽");
            AddValueCell(mainGrid, 2, 2, timeWorkshop2, true);

            // ========== СТРОКА 3 - Основной материал ==========
            AddLabelCell(mainGrid, 3, 0, "Основной материал");
            AddValueCell(mainGrid, 3, 1, product.MainMaterial ?? "—");
            AddValueCell(mainGrid, 3, 2, timeWorkshop3, true);

            blockBorder.Child = mainGrid;
            return blockBorder;
        }

        // Добавление ячейки заголовка
        private void AddHeaderCell(Grid grid, int row, int col, string text)
        {
            var border = new Border
            {
                Background = (Brush)new BrushConverter().ConvertFrom("#3498DB"),
                Padding = new Thickness(12, 10, 12, 10),
                BorderBrush = (Brush)new BrushConverter().ConvertFrom("#2980B9"),
                BorderThickness = GetBorderThickness(col, row)
            };

            var textBlock = new TextBlock
            {
                Text = text,
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                FontSize = 13,
                HorizontalAlignment = col == 1 ? HorizontalAlignment.Left : HorizontalAlignment.Center
            };

            border.Child = textBlock;
            Grid.SetRow(border, row);
            Grid.SetColumn(border, col);
            grid.Children.Add(border);
        }

        // Добавление ячейки с меткой (первая колонка)
        private void AddLabelCell(Grid grid, int row, int col, string text)
        {
            Brush bgBrush = (row % 2 == 0)
                ? (Brush)new BrushConverter().ConvertFrom("#F9F9F9")
                : Brushes.White;

            var border = new Border
            {
                Background = bgBrush,
                Padding = new Thickness(12, 10, 12, 10),
                BorderBrush = (Brush)new BrushConverter().ConvertFrom("#DDD"),
                BorderThickness = GetBorderThickness(col, row)
            };

            var textBlock = new TextBlock
            {
                Text = text,
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)new BrushConverter().ConvertFrom("#555"),
                VerticalAlignment = VerticalAlignment.Center
            };

            border.Child = textBlock;
            Grid.SetRow(border, row);
            Grid.SetColumn(border, col);
            grid.Children.Add(border);
        }

        // Добавление ячейки со значением
        private void AddValueCell(Grid grid, int row, int col, string text, bool isCenter = false)
        {
            Brush bgBrush = (row % 2 == 0)
                ? (Brush)new BrushConverter().ConvertFrom("#F9F9F9")
                : Brushes.White;

            var border = new Border
            {
                Background = bgBrush,
                Padding = new Thickness(12, 10, 12, 10),
                BorderBrush = (Brush)new BrushConverter().ConvertFrom("#DDD"),
                BorderThickness = GetBorderThickness(col, row)
            };

            var textBlock = new TextBlock
            {
                Text = text,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center
            };

            if (isCenter || col == 2)
            {
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            }
            else if (col == 1)
            {
                textBlock.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else
            {
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            }

            border.Child = textBlock;
            Grid.SetRow(border, row);
            Grid.SetColumn(border, col);
            grid.Children.Add(border);
        }

        private Thickness GetBorderThickness(int col, int row)
        {
            double left = (col == 0) ? 0 : 1;
            double top = (row == 0) ? 0 : 1;
            double right = (col == 2) ? 0 : 1;
            double bottom = 0;

            return new Thickness(left, top, right, bottom);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            _context = new ProductContext();
            LoadProducts();
            StatusText.Text = $"Обновлено в {DateTime.Now:HH:mm:ss}";
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти из системы?",
                "Подтверждение выхода",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}
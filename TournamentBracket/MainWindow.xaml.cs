using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TournamentBracket.Model;

namespace TournamentBracket
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window//  , INotifyPropertyChanged
    {
        private readonly IBracketHolder bracketHolder;
        private List<StackPanel> panels;
        readonly int ButtonHeight = 40;
        public MainWindow()
        {
            bracketHolder=new BracketHolder(new DataProvider());
            this.DataContext = bracketHolder;
            InitializeComponent();
        }
        
        public void CreateColumnsAndPopulateThemWithNicknames(int numberOfColumns)
        {
            panels=new List<StackPanel>();
            for (int i = 1; i <= numberOfColumns; i++)
            {
                var columnToAdd = new ColumnDefinition {Width = new GridLength(150)};
                mainGrid.ColumnDefinitions.Add(columnToAdd);
               
                StackPanel stackPanel=new StackPanel{Name=$"StackPanel{i}"};
                panels.Add(stackPanel);
                stackPanel.Children.Add(new Label(){Content=$"Kolumna {i}" });
                mainGrid.Children.Add(stackPanel);
                Grid.SetColumn(stackPanel,i);              
            }
            PopulateStackPanels();
        }

        public void MakeBracketsScreenshot()
        {
            RenderTargetBitmap targetBitmap = new RenderTargetBitmap((int) mainGrid.ActualWidth,(int) mainGrid.ActualHeight,96,96,PixelFormats.Pbgra32);
            targetBitmap.Render(mainGrid);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(targetBitmap));
            targetBitmap.Render(panels[0]);
            encoder.Frames.Add(BitmapFrame.Create(targetBitmap));
            using (Stream fileStream =  File.Create("screenshot.png"))
            {
                encoder.Save(fileStream);
            }
        }

        private Binding ReturnButtonContentBinding(int columnNumber,int buttonNumberInOrder)
        {
            Binding buttonContentBinding =
                new Binding($"Brackets[{columnNumber}][{buttonNumberInOrder}]") {Source = bracketHolder};

            return buttonContentBinding;
        }

        private Binding ReturnButtonCommandBinding()
        {
            Binding buttonCommandBinding = new Binding("SetWinnerCommand") {Source = bracketHolder};

            return buttonCommandBinding;
        }
        private void PopulateStackPanels()
        {
            for (int columnNumber = 0; columnNumber < panels.Count; columnNumber++)
            {
                var currentPanel = panels[columnNumber];
                var currentBracket = bracketHolder.Brackets[columnNumber];
               
                for (int buttonNumberInOrder = 0; buttonNumberInOrder < currentBracket.Count; buttonNumberInOrder++)
                {
                    Binding buttonContentBinding = ReturnButtonContentBinding(columnNumber, buttonNumberInOrder);

                    Binding buttonCommandBinding = ReturnButtonCommandBinding();
                    
                    Thickness marginOfButtonToAdd = buttonNumberInOrder==0 ? ReturnThicknessOfFirstButtonInColumn(columnNumber) : ReturnThicknessOfButtonInColumn(buttonNumberInOrder,columnNumber);

                    
                    OrderedButton buttonToAdd = new OrderedButton()
                    {
                        //Style = ReturnDefaultButtonStyle()
                        Background = ReturnDefaultButtonStyle(),
                        Indices = new[]{columnNumber,buttonNumberInOrder},
                        Margin = marginOfButtonToAdd,
                        Height = ButtonHeight,
                        CommandParameter = new[]{columnNumber,buttonNumberInOrder},
                        IsEnabled =true
                    };

                    buttonToAdd.SetBinding(Button.CommandProperty, buttonCommandBinding);
                    buttonToAdd.SetBinding(ContentProperty,buttonContentBinding);
                    currentPanel.Children.Add(buttonToAdd);
                }
            }
        }

        private SolidColorBrush ReturnDefaultButtonStyle()
        {
            var colorToSet = new Color {R = 123, G = 50, B = 197};
            var background = new SolidColorBrush(colorToSet);
            return background;
        }

        private void LoadBracketFromFile()
        {
            bracketHolder.LoadBracket();
            CreateColumnsAndPopulateThemWithNicknames(bracketHolder.NumberOfColumns);
        }
        private Thickness ReturnThicknessOfButtonInColumn(int buttonNumberInOrder,int columnNumber)
        {
            double startingIndex = Math.Pow(2, columnNumber + 1)-1;
            return new Thickness(10,startingIndex*ButtonHeight,10,0);
        }

        private Thickness ReturnThicknessOfFirstButtonInColumn(int columnNumber)
        {
            if (columnNumber==0)
                return new Thickness(10,0,10,0);
            else
            {
                double startingIndex = Math.Pow(2, columnNumber)-1;
                return new Thickness(10,startingIndex*ButtonHeight,10,0);
            }
            
           

        }

        private void screenshotButton_Click(object sender, RoutedEventArgs e)
        {
            MakeBracketsScreenshot();
        }

        private void loadingButton_Click(object sender, RoutedEventArgs e)
        {
            bracketHolder.PopulateBracket();
            var numberOfColumns = bracketHolder.NumberOfColumns;
            CreateColumnsAndPopulateThemWithNicknames(numberOfColumns);
            DisableLoadingButtonsAfterLoadingBracket();
        }

        private void DisableLoadingButtonsAfterLoadingBracket()
        {
            loadingButton.IsEnabled = false;
            loadBracketButton.IsEnabled = false;
        }


        private void loadBracketButton_Click(object sender, RoutedEventArgs e)
        {
            LoadBracketFromFile();
            DisableLoadingButtonsAfterLoadingBracket();
        }
    }
}

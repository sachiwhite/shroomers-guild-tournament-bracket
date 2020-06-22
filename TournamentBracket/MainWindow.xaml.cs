using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TournamentBracket.Model;

namespace TournamentBracket
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window//  , INotifyPropertyChanged
    {
        private readonly IBracketHolder bracketHolder;
        private readonly  List<StackPanel> panels;
        readonly int ButtonHeight = 30;
        public MainWindow()
        {
            bracketHolder=new BracketHolder(new DebugDataProvider());
            panels=new List<StackPanel>();
            this.DataContext = bracketHolder;
            InitializeComponent();
        }

        public void CreateColumnsAndPopulateThemWithNicknames(int numberOfColumns)
        {
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
                        Indices = new[]{columnNumber,buttonNumberInOrder},
                        Margin = marginOfButtonToAdd,
                        Padding = new Thickness(10,0,10,0),
                        Height = ButtonHeight,
                        CommandParameter = new[]{columnNumber,buttonNumberInOrder}
                        //IsEnabled = ReturnIfThisButtonIsEnabled(Content)
                    };

                    buttonToAdd.SetBinding(Button.CommandProperty, buttonCommandBinding);
                    buttonToAdd.SetBinding(ContentProperty,buttonContentBinding);
                    currentPanel.Children.Add(buttonToAdd);
                }
            }
        }

        private void CreateBracketFromFile()
        {

        }
        private Thickness ReturnThicknessOfButtonInColumn(int buttonNumberInOrder,int columnNumber)
        {
            if (buttonNumberInOrder%2==0)
            {
                return new Thickness(10,columnNumber*30,10,0);
            }
            else
            {
                return new Thickness(10,columnNumber*30,10,0);
            }
            
        }

        private Thickness ReturnThicknessOfFirstButtonInColumn(int columnNumber)
        {
            return new Thickness(10,columnNumber*ButtonHeight+ButtonHeight/2,10,0);
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
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GraphDrawer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private void DimInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Button_Click(sender, e);
            }
        }


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string dimText = DimInput.Text;

            if (!string.IsNullOrEmpty(dimText) && !string.IsNullOrWhiteSpace(dimText))
            {
                int dim = int.Parse(DimInput.Text);

               
                Main.Children.Remove(input);
                input = new MatrixInput(dim) { HorizontalAlignment = HorizontalAlignment.Center };


 
                Main.Children.Add(input);

            }

        }

        private void Go_Button_Click(object sender, RoutedEventArgs e)
        {
            var canvasWindow = new CanvasWindow(input.To2DArr());

            canvasWindow.Show();


        }




        MatrixInput input;
        public MainWindow()
        {
            InitializeComponent();



            input = new MatrixInput(5) { HorizontalAlignment = HorizontalAlignment.Center };
            this.Main.Children.Add(input);
        }
    }
}

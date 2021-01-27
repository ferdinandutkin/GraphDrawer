using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GraphDrawer
{
    class MatrixInput : Grid
    {


        public event Action OnCellChange = () => { };
        public MatrixInput(int dim) : this(dim, dim)
        {

        }

        new public static int GetRow(UIElement element) => Grid.GetRow(element) - 1;

        new public static int GetColumn(UIElement element) => Grid.GetColumn(element) - 1;

        public string this[int row, int column]
        {
            get => Children.OfType<TextBox>().First(e => GetRow(e) == row && GetColumn(e) == column).Text;

            set => Children.OfType<TextBox>().First(e => GetRow(e) == row && GetColumn(e) == column).Text = value;

        }


        private void MakeGrid(int rows, int columns)
        {
            for (int i = 0; i < rows; i++)
            {

                RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            }


            for (int i = 0; i < columns; i++)
            {

                ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
            }
        }

        public int[,] To2DArr()
        {
            int cols = ColumnDefinitions.Count - 1;
            int rows = RowDefinitions.Count - 1;
            int[,] ret = new int[rows, cols];

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    ret[i, j] = int.Parse(this[i, j]);
                }

            }

            return ret;


        }


        public MatrixInput(int[,] init) : this(init.GetLength(0), init.GetLength(1))
        {
           
            int rows = init.GetLength(0);
            int cols = init.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    this[i, j] = init[i, j].ToString();
                }
            }
        }


        public MatrixInput(int rows, int columns)
        {



            MakeGrid(rows + 1, columns + 1);


            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {

                    var tb = new TextBox() { Text = "0", TextAlignment = TextAlignment.Center, Padding = new Thickness(10) };

                    tb.PreviewTextInput += (object sender, TextCompositionEventArgs e) =>
                    {
                        Regex regex = new Regex("[^0-9]+");
                        e.Handled = regex.IsMatch(e.Text);
                    };

                    tb.TextChanged += (object sender, TextChangedEventArgs e) => 
                    {
                        var tbx = sender as TextBox;
                        int row = GetRow(tbx);
                        int column = GetColumn(tbx);

                        this[column, row] = this[row, column];

                        OnCellChange();

                    };

                    tb.SetValue(RowProperty, i + 1);
                    tb.SetValue(ColumnProperty, j + 1);

                    Children.Add(tb);

                }
            }


            ColumnDefinitions[0].Width = new GridLength(20);

            for (int i = 1; i <= columns; i++)
            {
                var tb = new TextBlock() { Text = i.ToString(), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };


                tb.SetValue(ColumnProperty, i);
                tb.SetValue(RowProperty, 0);

                Children.Add(tb);

            }


            RowDefinitions[0].Height = new GridLength(20);

            for (int i = 1; i <= rows; i++)
            {
                var tb = new TextBlock() { Text = i.ToString(), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };


                tb.SetValue(ColumnProperty, 0);
                tb.SetValue(RowProperty, i);

                Children.Add(tb);

            }


        }


    }

}

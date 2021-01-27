using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphDrawer
{
    public partial class CanvasWindow : Window
    {
        IEnumerable<Point> GetRegularPolygonPoints(Point center, double radius, int count)
        {
            var point = new Point(radius, 0);

            var rot = DegreesToRadians(360) / count;

            for (int i = 0; i < count; i++)
            {

                point = RotatePoint(point, center, rot);

                yield return point;
            }
        }


        static Point RotatePoint(Point pointToRotate, Point centerPoint, double angleInRadians)
        {
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Point
            {
                X =
                    (int)
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                    sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y =
                    (int)
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                    cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }

        static double DegreesToRadians(double angleInDegrees) => angleInDegrees * (Math.PI / 180);

        class Vertex
        {
            private Grid container;
            public Vertex(Point center, string text)
            {
                container = new Grid() { Width = 30, Height = 30 };
                var ellipse = new Ellipse() { Width = 10, Height = 10, Stroke = new SolidColorBrush(Colors.Red), Fill = new SolidColorBrush(Colors.Red) };

                var textBlock = new TextBlock { Text = text, FontSize = 12 };
                container.Children.Add(ellipse);
                container.Children.Add(textBlock);

                container.MouseLeftButtonDown += new MouseButtonEventHandler(Control_MouseLeftButtonDown);
                container.MouseLeftButtonUp += new MouseButtonEventHandler(Control_MouseLeftButtonUp);
                container.MouseMove += new MouseEventHandler(Control_MouseMove);

                Canvas.SetLeft(container, center.X);
                Canvas.SetTop(container, center.Y);
                Canvas.SetZIndex(container, 2);



            }

            protected bool isDragging;


            private void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                isDragging = true;


                var draggableControl = sender as Grid;
                var clickPosition = new Point(CenterX, CenterY);
                draggableControl.CaptureMouse();
            }

            private void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            {
                isDragging = false;
                var draggable = sender as Grid;
                draggable.ReleaseMouseCapture();


            }

            private void Control_MouseMove(object sender, MouseEventArgs e)
            {
                var draggableControl = sender as Grid;

                if (isDragging && draggableControl != null)
                {
                    Point currentPosition = e.GetPosition(container.Parent as UIElement);

                    CenterX = currentPosition.X;
                    CenterY = currentPosition.Y;


                }



            }

            public double X
            {
                get => Canvas.GetLeft(container);

                set => Canvas.SetLeft(container, value);
            }

            public double CenterX
            {
                get => X + (container).Width / 2;
                set => X = value - (container).Width / 2;

            }

            public double CenterY
            {
                get => Y + container.Height / 2;
                set => Y = value - container.Height / 2;

            }

            public double Y
            {
                get => Canvas.GetTop(container);
                set => Canvas.SetTop(container, value);
            }


            public class CircleXConverter : IValueConverter
            {
                Vertex vertex;
                Ellipse ellipse;

                public CircleXConverter(Vertex vertex, Ellipse ellipse)
                {
                    this.vertex = vertex;
                    this.ellipse = ellipse;

                }
                public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
                {
                    return vertex.CenterX - ellipse.Width / 2;
                }

                public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                {
                    throw new NotImplementedException();
                }
            }




            public class CircleYConverter : IValueConverter
            {


                Vertex vertex;
                Ellipse ellipse;
                public CircleYConverter(Vertex vertex, Ellipse ellipse)
                {
                    this.vertex = vertex;
                    this.ellipse = ellipse;

                }
                public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
                {

                    return vertex.CenterY - ellipse.Height;
                }



                public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                {
                    throw new NotImplementedException();
                }
            }




            public class LineXConverter : IValueConverter
            {

                public object Convert(object value, Type targetType,
                    object parameter, CultureInfo culture)
                {

                    return (parameter as Vertex).CenterX;
                }

                public object ConvertBack(object value, Type targetType,
                    object parameter, CultureInfo culture)
                    => value;
            }



            //..................................................................

            public class LineYConverter : IValueConverter
            {

                public LineYConverter()
                {

                }

                public object Convert(object value, Type targetType,
                    object parameter, CultureInfo culture) => (parameter as Vertex).CenterY;

                public object ConvertBack(object value, Type targetType,
                    object parameter, CultureInfo culture)
                    => throw new NotImplementedException();
            }





            public Shape ConnectTo(Vertex v)

            {

                if (Equals(v))
                {
                    var ellipse = new Ellipse { Width = 40, Height = 40, Stroke = Brushes.LightSteelBlue, StrokeThickness = 2, };
                    Canvas.SetLeft(ellipse, CenterX - ellipse.Width / 2);
                    Canvas.SetTop(ellipse, CenterY - ellipse.Height);


                    ellipse.SetBinding(Canvas.LeftProperty, new Binding { Path = new PropertyPath("(Canvas.Left)"), Source = container, Converter = new CircleXConverter(this, ellipse) });

                    ellipse.SetBinding(Canvas.TopProperty, new Binding { Path = new PropertyPath("(Canvas.Top)"), Source = container, Converter = new CircleYConverter(this, ellipse) });


                    return ellipse;
                }
                else
                {


                    var line = new Line { StrokeThickness = 2, Stroke = Brushes.LightSteelBlue };


                    line.SetBinding(Line.X1Property, new Binding { Path = new PropertyPath("(Canvas.Left)"), Source = container, Converter = new LineXConverter(), ConverterParameter = this });


                    line.SetBinding(Line.X2Property, new Binding { Path = new PropertyPath("(Canvas.Left)"), Source = v.container, Converter = new LineXConverter(), ConverterParameter = v });


                    line.SetBinding(Line.Y1Property, new Binding { Path = new PropertyPath("(Canvas.Top)"), Source = container, Converter = new LineYConverter(), ConverterParameter = this });



                    line.SetBinding(Line.Y2Property, new Binding { Path = new PropertyPath("(Canvas.Top)"), Source = v.container, Converter = new LineYConverter(), ConverterParameter = v });






                    return line;
                }


            }





            public string Text
            {
                get => (container.Children[1] as TextBlock).Text;
                set => (container.Children[1] as TextBlock).Text = value;

            }

            public static implicit operator UIElement(Vertex v) => v.container;
        }


        public void Draw(int[,] matrix)
        {

            this.Graph.Children.Clear();

            int matrixLength = matrix.GetLength(1);

            var center = new Point(150, 150);

            var vertexes = GetRegularPolygonPoints(center, 100, matrixLength).Select((point, index) => new Vertex(point, (index + 1).ToString()));


            var vertexList = vertexes.ToList();


            vertexList.ForEach(v => Graph.Children.Add(v));


            for (int i = 0; i < matrixLength; i++)
            {
                for (int j = i; j < matrixLength; j++)
                {
                    for (int connection = 0; connection < matrix[i, j]; connection++)
                    {
                        Graph.Children.Add(vertexList[i].ConnectTo(vertexList[j]));
                    }

                }
            }



        }


        
        public CanvasWindow(int [,] matrix)
        {


            InitializeComponent();

            Draw(matrix);

            




        }
    }
}


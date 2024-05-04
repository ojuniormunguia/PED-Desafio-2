using System.Reflection.Emit;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace PED_Desafio_2
{
    public partial class MainWindow : Window
    {
        private Point? selectedPoint1;
        private Point? selectedPoint2;
        private Grafo grafo;

        public MainWindow()
        {
            InitializeComponent();
            CrearGrafo();
            AgregarElementos();
        }

        private Point lastMousePos;
        private bool isDragging;

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var zoom = e.Delta > 0 ? 1.1 : 0.9;
            scaleTransform.ScaleX *= zoom;
            scaleTransform.ScaleY *= zoom;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                lastMousePos = e.GetPosition(this);
                isDragging = true;
                MyCanvas.CaptureMouse();
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                Point clickPoint = e.GetPosition(MyCanvas);
                Ellipse nearestCircle = null;
                double minDistance = double.MaxValue;

                foreach (var child in MyCanvas.Children)
                {
                    if (child is Ellipse circle && circle.Fill == Brushes.Blue)
                    {
                        double dx = Canvas.GetLeft(circle) - clickPoint.X;
                        double dy = Canvas.GetTop(circle) - clickPoint.Y;
                        double distance = Math.Sqrt(dx * dx + dy * dy);

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nearestCircle = circle;
                        }
                    }
                }

                if (nearestCircle != null && minDistance <= nearestCircle.Width / 2)
                {
                    Point nearestPoint = new Point(Canvas.GetLeft(nearestCircle), Canvas.GetTop(nearestCircle));
                    if (selectedPoint1 == null)
                    {
                        selectedPoint1 = nearestPoint;
                    }
                    else if (selectedPoint2 == null)
                    {
                        selectedPoint2 = nearestPoint;
                        DrawLineAndDistance();
                        selectedPoint1 = null;
                        selectedPoint2 = null;
                    }
                }
            }
        }

        private void DrawLineAndDistance()
        {
            if (selectedPoint1 != null && selectedPoint2 != null)
            {
                Line line = new Line
                {
                    X1 = selectedPoint1.Value.X,
                    Y1 = selectedPoint1.Value.Y,
                    X2 = selectedPoint2.Value.X,
                    Y2 = selectedPoint2.Value.Y,
                    Stroke = Brushes.Red,
                    StrokeThickness = 2
                };
                MyCanvas.Children.Add(line);

                double distance = Math.Sqrt(Math.Pow(line.X2 - line.X1, 2) + Math.Pow(line.Y2 - line.Y1, 2));
                TextBlock distanceText = new TextBlock
                {
                    Text = distance.ToString("F2"),
                    Foreground = Brushes.Black,
                    Background = Brushes.White,
                    FontSize = 12
                };
                Canvas.SetLeft(distanceText, (line.X1 + line.X2) / 2);
                Canvas.SetTop(distanceText, (line.Y1 + line.Y2) / 2);
                MyCanvas.Children.Add(distanceText);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                var newPos = e.GetPosition(this);
                var dx = newPos.X - lastMousePos.X;
                var dy = newPos.Y - lastMousePos.Y;
                translateTransform.X += dx;
                translateTransform.Y += dy;
                lastMousePos = newPos;
            }
        }

        private void MyCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                isDragging = false;
                MyCanvas.ReleaseMouseCapture();
            }
        }

        private void CrearGrafo()
        {
            grafo = new Grafo();
            string[] labels = new string[]
            {
            "Ahuachapán",
            "Sonsonate",
            "Santa Ana",
            "La Libertad",
            "San Salvador",
            "Chalatenango",
            "Cuscatlán",
            "La Paz",
            "Cabañas",
            "San Vicente",
            "Usulután",
            "San Miguel",
            "Morazán",
            "La Unión"
            };

            Point[] puntos = new Point[]
            {
            new Point(104, 314),
            new Point(203, 388),
            new Point(263, 211),
            new Point(344, 368),
            new Point(424, 360),
            new Point(463, 178),
            new Point(479, 322),
            new Point(506, 479),
            new Point(615, 298),
            new Point(615, 404),
            new Point(689, 522),
            new Point(811, 455),
            new Point(869, 338),
            new Point(947, 455)
            };

            for (int i = 0; i < puntos.Length; i++)
            {
                Vertice vertice = new Vertice { Nombre = labels[i], Ubicacion = puntos[i] };
                grafo.AgregarVertice(vertice);
            }

            // Crear aristas entre vértices
            // Añade aquí las aristas que necesites

            // Crear el contorno
            Point[] puntos2 = new Point[]
            {
            new Point(20, 352),
            new Point(138, 424),
            new Point(149, 463),
            new Point(370, 479),
            new Point(712, 622),
            new Point(939, 622),
            new Point(994, 570),
            new Point(963, 522),
            new Point(971, 487),
            new Point(994, 514),
            new Point(1022, 498),
            new Point(979, 290),
            new Point(912, 306),
            new Point(869, 253),
            new Point(803, 261),
            new Point(697, 320),
            new Point(697, 261),
            new Point(593, 221),
            new Point(506, 162),
            new Point(455, 88),
            new Point(318, 71),
            new Point(239, 87),
            new Point(239, 123),
            new Point(266, 123),
            new Point(274, 159),
            new Point(205, 178),
            new Point(175, 237),
            new Point(127, 227),
            new Point(54, 285),
            new Point(20, 352)
            };

            AgregarPuntosYLineas(puntos2, Brushes.Gray, Brushes.Gray);
        }

        private void AgregarPuntosYLineas(Point[] puntos, Brush pointBrush, Brush lineBrush)
        {
            Polyline polyline = new Polyline
            {
                Stroke = lineBrush,
                StrokeThickness = 3
            };

            foreach (var point in puntos)
            {
                Ellipse circle = new Ellipse
                {
                    Fill = pointBrush,
                    Width = 6,
                    Height = 6
                };

                Canvas.SetLeft(circle, point.X - circle.Width / 2);
                Canvas.SetTop(circle, point.Y - circle.Height / 2);
                MyCanvas.Children.Add(circle);

                polyline.Points.Add(point);
            }

            MyCanvas.Children.Add(polyline);
        }


        private void AgregarElementos()
        {
            var vertices = grafo.Vertices.Keys.ToList();

            foreach (var vertice in vertices)
            {
                Ellipse circle = new Ellipse
                {
                    Fill = Brushes.Blue,
                    Width = 10,
                    Height = 10
                };

                Canvas.SetLeft(circle, vertice.Ubicacion.X);
                Canvas.SetTop(circle, vertice.Ubicacion.Y);
                MyCanvas.Children.Add(circle);

                TextBlock textBlock = new TextBlock
                {
                    Text = vertice.Nombre,
                    Foreground = Brushes.Black,
                    FontSize = 12
                };

                Canvas.SetLeft(textBlock, vertice.Ubicacion.X);
                Canvas.SetTop(textBlock, vertice.Ubicacion.Y + 20);
                MyCanvas.Children.Add(textBlock);
            }

            foreach (var vertice in vertices)
            {
                var closestVertices = GetThreeClosestVertices(vertice, vertices);
                foreach (var closestVertice in closestVertices)
                {
                    double distance = GetDistance(vertice, closestVertice);
                    DrawLineBetweenVertices(vertice, closestVertice, (int)distance);
                }
            }
        }
        private List<Vertice> GetThreeClosestVertices(Vertice source, List<Vertice> vertices)
        {
            return vertices
                .Where(v => v != source)
                .OrderBy(v => GetDistance(source, v))
                .Take(3)
                .ToList();
        }

        private double GetDistance(Vertice v1, Vertice v2)
        {
            return Math.Sqrt(Math.Pow(v1.Ubicacion.X - v2.Ubicacion.X, 2) +
                             Math.Pow(v1.Ubicacion.Y - v2.Ubicacion.Y, 2));
        }

        private void DrawLineBetweenVertices(Vertice origen, Vertice destino, int distancia)
        {
            Line line = new Line
            {
                X1 = origen.Ubicacion.X,
                Y1 = origen.Ubicacion.Y,
                X2 = destino.Ubicacion.X,
                Y2 = destino.Ubicacion.Y,
                Stroke = Brushes.Blue,
                StrokeThickness = 2
            };

            MyCanvas.Children.Add(line);

            TextBlock distanceText = new TextBlock
            {
                Text = distancia.ToString(),
                Foreground = Brushes.Black,
                Background = Brushes.White,
                FontSize = 12
            };

            Canvas.SetLeft(distanceText, (line.X1 + line.X2) / 2);
            Canvas.SetTop(distanceText, (line.Y1 + line.Y2) / 2);
            MyCanvas.Children.Add(distanceText);
        }
    }

    public class Vertice
    {
        public string Nombre { get; set; }
        public Point Ubicacion { get; set; }
    }

    public class Arista
    {
        public Vertice Destino { get; set; }
        public int Distancia { get; set; }
    }

    public class Grafo
    {
        public Dictionary<Vertice, List<Arista>> Vertices { get; } = new Dictionary<Vertice, List<Arista>>();

        public void AgregarVertice(Vertice vertice)
        {
            Vertices[vertice] = new List<Arista>();
        }

        public void AgregarArista(Vertice origen, Vertice destino, int distancia)
        {
            Vertices[origen].Add(new Arista { Destino = destino, Distancia = distancia });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TerceApp
{
    public partial class MainWindow : Window
    {
        private List<Terce> terceList;
        private Random random;
        private int score;

        public MainWindow()
        {
            InitializeComponent();

            terceList = new List<Terce>();
            random = new Random();
            score = 0;

            terceContainer.SizeChanged += TerceContainer_SizeChanged;
            canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;

            GenerateInitialTerce();
        }

        private void TerceContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (Terce terce in terceList)
            {
                terce.MoveToCenter(terceContainer.ActualWidth, terceContainer.ActualHeight);
            }
        }

        private void GenerateInitialTerce()
        {
            double width = terceContainer.ActualWidth;
            double height = terceContainer.ActualHeight;

            double size = random.Next(50, 150);
            double x = (width - size) / 2;
            double y = (height - size) / 2;

            Terce terce = new Terce(size, x, y);
            terce.Hit += Terce_Hit;
            terce.Draw(canvas);

            terceList.Add(terce);
        }

        private void GenerateNextTerce()
        {
            double width = terceContainer.ActualWidth;
            double height = terceContainer.ActualHeight;

            double size = random.Next(50, 150);
            double x = random.NextDouble() * (width - size);
            double y = random.NextDouble() * (height - size);

            Terce terce = new Terce(size, x, y);
            terce.Hit += Terce_Hit;
            terce.Draw(canvas);

            terceList.Add(terce);
        }

        private void Terce_Hit(object sender, EventArgs e)
        {
            Terce terce = (Terce)sender;
            terce.Hit -= Terce_Hit;
            terce.Clear(canvas);
            terceList.Remove(terce);

            score += 1;
            UpdateScore();

            GenerateNextTerce();
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(canvas);
            bool hit = false;

            foreach (Terce terce in terceList)
            {
                if (terce.IsHit(mousePosition))
                {
                    terce.OnHit();
                    hit = true;
                    score += 1;
                    UpdateScore();
                    break;
                }
            }

            if (!hit)
            {
                score -= 1;
                UpdateScore();
            }
        }

        private void UpdateScore()
        {
            scoreTextBlock.Text = "Score: " + score;
        }
    }

    public class Terce
    {
        private double size;
        private double x;
        private double y;
        private Canvas hitArea;

        public event EventHandler Hit;

        public Terce(double size, double x, double y)
        {
            this.size = size;
            this.x = x;
            this.y = y;

            hitArea = new Canvas();
            hitArea.Width = size;
            hitArea.Height = size;

            CreateCircles();
        }

        private void CreateCircles()
        {
            double centerX = size / 2;
            double centerY = size / 2;

            for (int i = 0; i < 5; i++)
            {
                double circleSize = size - i * (size / 5);

                Ellipse circle = new Ellipse
                {
                    Width = circleSize,
                    Height = circleSize,
                    StrokeThickness = 2,
                    Stroke = Brushes.Black,
                    Fill = Brushes.Red
                };

                double left = centerX - circleSize / 2;
                double top = centerY - circleSize / 2;

                Canvas.SetLeft(circle, left);
                Canvas.SetTop(circle, top);

                hitArea.Children.Add(circle);
            }
        }

        public void MoveToCenter(double containerWidth, double containerHeight)
        {
            x = (containerWidth - size) / 2;
            y = (containerHeight - size) / 2;

            Canvas.SetLeft(hitArea, x);
            Canvas.SetTop(hitArea, y);
        }

        public void Draw(Canvas canvas)
        {
            canvas.Children.Add(hitArea);
            Canvas.SetLeft(hitArea, x);
            Canvas.SetTop(hitArea, y);
        }

        public bool IsHit(Point point)
        {
            double centerX = x + size / 2;
            double centerY = y + size / 2;

            double distance = Math.Sqrt(Math.Pow(centerX - point.X, 2) + Math.Pow(centerY - point.Y, 2));

            return distance <= size / 2;
        }

        public void Clear(Canvas canvas)
        {
            canvas.Children.Remove(hitArea);
        }

        private void HitArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnHit();
        }

        public void OnHit()
        {
            Hit?.Invoke(this, EventArgs.Empty);
        }
    }
}

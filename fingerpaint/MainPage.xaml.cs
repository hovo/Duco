using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Ink;
using System.ComponentModel;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Coding4Fun.Toolkit.Controls;

namespace fingerpaint
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Point point;
        private Point old_point;
        private bool draw = false;

        Line line;
        private Color color;
        private List<int> _undoMemory = new List<int>();

        public MainPage()
        {
            InitializeComponent();

            old_point = point;
            
        }

        void SetAppBarButtonEnabled(PhoneApplicationPage page, int buttonIndex, bool isEnabled)
        {
            (page.ApplicationBar.Buttons[buttonIndex] as ApplicationBarIconButton).IsEnabled = isEnabled;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            paint.MouseMove += new MouseEventHandler(FingerMove);
            paint.MouseLeftButtonUp += new MouseButtonEventHandler(FingerUp);
            paint.MouseLeftButtonDown += new MouseButtonEventHandler(FingerDown);
        }

        void FingerMove(object sender, MouseEventArgs e)
        {
            if (draw)
            {
                SetAppBarButtonEnabled(this, 0, true);
                SetAppBarButtonEnabled(this, 1, true);

                point = e.GetPosition(paint);
                line = new Line();
                
                line.Stroke = new SolidColorBrush(color);

                line.X1 = point.X;
                line.Y1 = point.Y;

                line.X2 = old_point.X;
                line.Y2 = old_point.Y;

                line.StrokeStartLineCap = PenLineCap.Round;
                line.StrokeEndLineCap = PenLineCap.Round;

                line.StrokeThickness = brushSize.Value;

                //Make opacity changable
                line.Opacity = 1;

                paint.Children.Add(line);
                old_point = point;
            }
            old_point = point;
            
        }

        void FingerUp(object sender, MouseButtonEventArgs e)
        {
            draw = false;
        }

        void FingerDown(object sender, MouseButtonEventArgs e)
        {
            _undoMemory.Add(paint.Children.Count);
            point = e.GetPosition(paint);
            old_point = point;
            draw = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete the drawing?", "Confirm Delete", MessageBoxButton.OKCancel) != MessageBoxResult.Cancel)
                paint.Children.Clear();

            return;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            MediaLibrary library = new MediaLibrary();
            WriteableBitmap bitMap = new WriteableBitmap(paint, null);
            MemoryStream ms = new MemoryStream();
            Extensions.SaveJpeg(bitMap, ms, bitMap.PixelWidth,
                                bitMap.PixelHeight, 0, 100);
            ms.Seek(0, SeekOrigin.Begin);
            library.SavePicture(string.Format("Images\\{0}.jpg", Guid.NewGuid()), ms);

            var toast = new ToastPrompt { Title = "Drawing Saved" };
            toast.Show();

        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void btnImportImage_Click(object sender, EventArgs e)
        {
            var selectphoto = new PhotoChooserTask();
            selectphoto.ShowCamera = true;
            selectphoto.Completed += (s1, e1) =>
            {
                if (e1.TaskResult == TaskResult.OK)
                {
                    var bitmap = new BitmapImage();
                    bitmap.SetSource(e1.ChosenPhoto);

                    var writeableBitmap = new WriteableBitmap(bitmap);

                    var image = new Image()
                    {
                        Source = writeableBitmap,
                        Width = (int)paint.ActualWidth,
                        Height = (int)paint.ActualHeight,
                    };

                    Canvas.SetLeft(image, 0);
                    Canvas.SetTop(image, 0);

                    _undoMemory.Add(paint.Children.Count);
                    paint.Children.Add(image);
                }
            };
            selectphoto.Show();

        }

        private void ColorSlider_ColorChanged(object sender, Color color)
        {
            this.color = color;
        }

        private void btnUndo_Tap(object sender, GestureEventArgs e)
        {
            if (_undoMemory.Count == 0)
                return;

            int i = 0;
            foreach (var item in paint.Children.ToList())
            {
                if (i++ >= _undoMemory.Last())
                    paint.Children.Remove(item);
            }
            _undoMemory.Remove(_undoMemory.Last());
        }

        private void toggleElement(Control element) { 

             // If slider is currently visible, then hide
            if (element.Visibility == Visibility.Collapsed)
                element.Visibility = Visibility.Visible;
            else
                element.Visibility = Visibility.Collapsed;
        }

        private void btn_Tap(object sender, GestureEventArgs e)
        {
            toggleElement(brushSize);
        }

    }
}
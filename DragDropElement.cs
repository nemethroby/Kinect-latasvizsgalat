using System.Windows.Controls;
using Microsoft.Kinect.Toolkit.Input;
using Microsoft.Kinect.Wpf.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Diagnostics;
using KinectGame.DataModel;

namespace KinectGame
{
    public class DragDropElement : UserControl, IKinectControl
    {
        //az egérrel való mozgatáshoz
        private Point BasePoint = new Point(0.0, 0.0);
        private bool moving = false;
        Point _previous = new Point();
        private Rectangle finish;
        Stopwatch sw = new Stopwatch();

        //konstruktor
        public IKinectController CreateController(IInputModel inputModel, KinectRegion kinectRegion)
        {
            return new DragDropElementController(inputModel, kinectRegion);
        }

        public DragDropElement() {
           //a mozgatás eleje
            this.PreviewMouseLeftButtonDown += DragDropElement_PreviewMouseLeftButtonDown;
            //a mozgatás vége
            this.PreviewMouseLeftButtonUp += DragDropElement_PreviewMouseLeftButtonUp;
            //mozgatás közben
            this.PreviewMouseMove += DragDropElement_PreviewMouseMove;
        
        
        
        }

        void DragDropElement_PreviewMouseMove(object sender, MouseEventArgs e)
        {

            

       

            
            Canvas canvas = this.Parent as Canvas;
            finish = canvas.FindName("finish") as Rectangle;
           // Label carXLabel = canvas.FindName("carX") as Label;
            //Label carYLabel = canvas.FindName("carY") as Label;
            //Label finishXLabel = canvas.FindName("finishX") as Label;
            //Label finishYLabel = canvas.FindName("finishY") as Label;
            double finishX = Canvas.GetLeft(finish);
            double finishY = Canvas.GetTop(finish);
            double carX = Canvas.GetLeft(this);
            double carY = Canvas.GetTop(this);
           
            //var navigationRegion = Window.GetWindow(this).FindName("navigationRegion") as ContentControl;
            //var kinectRegionGrid = Window.GetWindow(this).FindName("kinectRegionGrid") as Grid;
            //var backButton = canvas.FindName("backButton") as Button;
            var finishdoboz = canvas.FindName("Finish") as Canvas;
            var finishlabel = canvas.FindName("Szoveg") as Label;


            if (moving)
            {
                //carXLabel.Content = carX + this.ActualWidth;
                //carYLabel.Content = carY + this.ActualHeight;
               // finishXLabel.Content = sw.Elapsed.Seconds;
               // finishYLabel.Content = finishY + finish.ActualWidth / 2;
                var element = sender as FrameworkElement;
                //jelenlegi pozíció elmentése egy pontba
                var currentPoint = e.GetPosition(element);

                //új pozíció
                (element as FrameworkElement).SetValue(Canvas.LeftProperty,
              e.GetPosition((element as FrameworkElement).Parent as FrameworkElement).X - _previous.X);

                (element as FrameworkElement).SetValue(Canvas.TopProperty,
                     e.GetPosition((element as FrameworkElement).Parent as FrameworkElement).Y - _previous.Y);
               
                
                if ( (carX+ActualWidth/2 >finishX+finish.ActualWidth/2) && carY-ActualHeight/2 < finishY+finish.ActualHeight/2) {

                    
                    
                    sw.Stop();
                    //MessageBoxResult result = MessageBox.Show("Az időd: "+sw.Elapsed.Seconds+"s Újra?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    finishdoboz.Visibility = System.Windows.Visibility.Visible;
                    finishlabel.Content = "Congratulations! Your time" + sw.Elapsed.Seconds + "s Replay?";


                    //if (result == MessageBoxResult.Yes)
                    //{
                    //     SampleDataItem sampleDataItem  = new SampleDataItem(typeof(Game));
                    //     navigationRegion.Content = Activator.CreateInstance(sampleDataItem.NavigationPage);
                    //}
                    //else {
                    //    backButton.Visibility = System.Windows.Visibility.Hidden;
                    //    navigationRegion.Content = kinectRegionGrid;
                   // }
                    


                }
               
            }
            
           

        }

        //mozgatás vége
        void DragDropElement_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (moving)
            {
                var element = sender as FrameworkElement;
                element.ReleaseMouseCapture();
                moving = false;
                e.Handled = true;
            }
        }

        //mozgatás eleje
        void DragDropElement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            _previous = e.GetPosition(element);
            //az egér mozgásának figyelése
            element.CaptureMouse();
            moving = true;
            e.Handled = true;

            sw.Start();

        }

        public bool IsManipulatable
        {
            get { return true; }
        }

        public bool IsPressable
        {
            get { return false; }
        }

        public void Finish() { 
            
        
        
        }

    }
}

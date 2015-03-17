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

namespace KinectGame
{
    public class DragDropElement : UserControl, IKinectControl
    {
        //az egérrel való mozgatáshoz
        private Point BasePoint = new Point(0.0, 0.0);
        private bool moving = false;
        Point _previous = new Point();

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
            if (moving)
            {
                var element = sender as FrameworkElement;
                //jelenlegi pozíció elmentése egy pontba
                var currentPoint = e.GetPosition(element);

                //új pozíció
                (element as FrameworkElement).SetValue(Canvas.LeftProperty,
              e.GetPosition((element as FrameworkElement).Parent as FrameworkElement).X - _previous.X);

                (element as FrameworkElement).SetValue(Canvas.TopProperty,
                     e.GetPosition((element as FrameworkElement).Parent as FrameworkElement).Y - _previous.Y);

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
        }

       

        public bool IsManipulatable
        {
            get { return true; }
        }

        public bool IsPressable
        {
            get { return false; }
        }
    }
}

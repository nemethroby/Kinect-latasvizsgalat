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
        private Point BasePoint = new Point(0.0, 0.0);
        private bool moving = false;
        Point _previous = new Point();
        public IKinectController CreateController(IInputModel inputModel, KinectRegion kinectRegion)
        {
            return new DragDropElementController(inputModel, kinectRegion);
        }

        public DragDropElement() {
           
            this.PreviewMouseLeftButtonDown += DragDropElement_PreviewMouseLeftButtonDown;
            this.PreviewMouseLeftButtonUp += DragDropElement_PreviewMouseLeftButtonUp;
            this.PreviewMouseMove += DragDropElement_PreviewMouseMove;
        
        
        
        }

        void DragDropElement_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (moving)
            {
                var element = sender as FrameworkElement;
                var currentPoint = e.GetPosition(element);

                (element as FrameworkElement).SetValue(Canvas.LeftProperty,
              e.GetPosition((element as FrameworkElement).Parent as FrameworkElement).X - _previous.X);

                (element as FrameworkElement).SetValue(Canvas.TopProperty,
                     e.GetPosition((element as FrameworkElement).Parent as FrameworkElement).Y - _previous.Y);

            }
        }

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

        void DragDropElement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            _previous = e.GetPosition(element);
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

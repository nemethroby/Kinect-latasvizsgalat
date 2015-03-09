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
        public IKinectController CreateController(IInputModel inputModel, KinectRegion kinectRegion)
        {
            return new DragDropElementController(inputModel, kinectRegion);
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

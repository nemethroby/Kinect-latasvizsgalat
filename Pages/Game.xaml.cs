//------------------------------------------------------------------------------
// <copyright file="ScrollViewerSample.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace KinectGame
{
    using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
    
    /// <summary>
    /// Interaction logic for ScrollViewerSample
    /// </summary>
    public partial class Game : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollViewerSample"/> class.
        /// </summary>
        /// 
        
        public Game()
        {
            this.InitializeComponent();
        

        }


        private Point BasePoint = new Point(0.0, 0.0);
        private bool moving = false;
        Point _previous = new Point();

        


        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            _previous = e.GetPosition(element);
            element.CaptureMouse();
            moving = true;
            e.Handled = true;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
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

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (moving)
            {
                var element = sender as FrameworkElement;
                element.ReleaseMouseCapture();
                moving = false;
                e.Handled = true;
            }
        }
        

       

       
        
        
    }

    


    }


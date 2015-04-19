namespace KinectGame
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;
  
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;
    using Microsoft.Kinect.Wpf.Controls;
    using KinectGame.DataModel;

    public partial class Profiles : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollViewerSample"/> class.
        /// </summary>
        /// 

        public Profiles()
        {
            this.InitializeComponent();


        }

        public void ProfileChange(object sender, RoutedEventArgs e)
        {


            var navigationRegion = Window.GetWindow(this).FindName("navigationRegion") as ContentControl;
            var kinectRegionGrid = Window.GetWindow(this).FindName("kinectRegionGrid") as Grid;

            var clickedButton = sender as Button;
            var button = Window.GetWindow(this).FindName("Profiles") as Button;
            var backButton = Window.GetWindow(this).FindName("backButton") as Button;
           
            button.Background = clickedButton.Background;
            
            var mainWindow = Window.GetWindow(this) as Window;
           
            //a vissza gomb elrejétése
            backButton.Visibility = System.Windows.Visibility.Hidden;
            //a főmenü megjelenítése
            
            
            navigationRegion.Content = kinectRegionGrid;
           
           



        }
    
       

      

    }




}


namespace KinectGame
{
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using KinectGame.DataModel;
    

    public partial class Game : UserControl
    {

        public Game()
        {
            this.InitializeComponent();
        

        }

        //nincs újrajátszás
        private void No_Click(object sender, RoutedEventArgs e)
        {
            var backButton = Window.GetWindow(this).FindName("backButton") as Button;
            var navigationRegion = Window.GetWindow(this).FindName("navigationRegion") as ContentControl;
            var kinectRegionGrid = Window.GetWindow(this).FindName("kinectRegionGrid") as Grid;

            //a vissza gomb elrejétése
            backButton.Visibility = System.Windows.Visibility.Hidden;
            //a főmenü megjelenítése
            navigationRegion.Content = kinectRegionGrid;
        }

        //újrajátszás
        private void Yes_Click(object sender, RoutedEventArgs e)
        {

            var navigationRegion = Window.GetWindow(this).FindName("navigationRegion") as ContentControl;

            SampleDataItem sampleDataItem = new SampleDataItem(typeof(Game));
            navigationRegion.Content = Activator.CreateInstance(sampleDataItem.NavigationPage);
        }    
    }

    }


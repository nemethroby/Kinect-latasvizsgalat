namespace KinectGame
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;
    using System.Windows.Controls;
    using Microsoft.Kinect.Wpf.Controls;
    using KinectGame.DataModel;


    //a MainWindow logikája
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        //aktív Kinect (azaz az első elérhető)
        private KinectSensor kinectSensor = null;

        //colorFrame olvasó
        private ColorFrameReader colorFrameReader = null;

        //színes bittérkép
        private WriteableBitmap colorBitmap = null;

        //státusz szövege
        private string statusText = null;

        //MainWindow iniciálizációja
        public MainWindow()
        {
            //alapértelmezett Kinect beállítása
            this.kinectSensor = KinectSensor.GetDefault();

            //az colorFrame olvasó megnyitása
            this.colorFrameReader = this.kinectSensor.ColorFrameSource.OpenReader();

            //ha adat érkezik, azt bepakoljuk a colorFrame olvasóba
            this.colorFrameReader.FrameArrived += this.Reader_ColorFrameArrived;

            // colorFrame leíró hozzáadása: Bgra formátum használata
            FrameDescription colorFrameDescription = this.kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);

            // bittérkép létehozása: 32 bites színmélység
            this.colorBitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);

            // Kinect állapot-észlelő: ha történik valami a Kinecttel, akkor ebbe megy az infó
            this.kinectSensor.IsAvailableChanged += this.Sensor_IsAvailableChanged;

            // Kinect elindítása
            this.kinectSensor.Open();

            // státusz szöveg beállítása
            this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.NoSensorStatusText;

            this.DataContext = this;

            // komponensek inicializálása az ablak irányításán belül (ez majd bővülni fog)
            this.InitializeComponent();

            KinectRegion.SetKinectRegion(this, kinectRegion);

            App app = ((App)Application.Current);
            app.KinectRegion = kinectRegion;

            //// Add in display content
            var sampleDataSource = SampleDataSource.GetGroup("Group-1");
            this.itemsControl.ItemsSource = sampleDataSource;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //ez az ImageSource van meghívva a XAML-en belül, ami visszaadja az aktuális bittérképet
        public ImageSource ImageSource
        {
            get
            {
                return this.colorBitmap;
            }
        }

        //státusz beállítása illetve lekérdezése
        public string StatusText
        {
            get
            {
                return this.statusText;
            }

            set
            {
                if (this.statusText != value)
                {
                    this.statusText = value;

                    // szöveg megváltoztatása értesítés esetén
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("StatusText"));
                    }
                }
            }
        }

        
        //leállító művelet: a főablak bezárása esetén leállítja először a colorFrame olvasót, utána a Kinectet
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.colorFrameReader != null)
            {
                // ColorFrameReder is IDisposable
                this.colorFrameReader.Dispose();
                this.colorFrameReader = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

        

        //színes képi adatok kezelése, ami a Kinect felől jön
        private void Reader_ColorFrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            // ColorFrame elérhető
            using (ColorFrame colorFrame = e.FrameReference.AcquireFrame())
            {
                if (colorFrame != null)
                {
                    FrameDescription colorFrameDescription = colorFrame.FrameDescription;

                    using (KinectBuffer colorBuffer = colorFrame.LockRawImageBuffer())
                    {
                        this.colorBitmap.Lock();

                        // adatok ellenőrzése és továbbítása a bittérképként a colorBitmap bufferbe
                        if ((colorFrameDescription.Width == this.colorBitmap.PixelWidth) && (colorFrameDescription.Height == this.colorBitmap.PixelHeight))
                        {
                            colorFrame.CopyConvertedFrameDataToIntPtr(
                                this.colorBitmap.BackBuffer,
                                (uint)(colorFrameDescription.Width * colorFrameDescription.Height * 4),
                                ColorImageFormat.Bgra);

                            this.colorBitmap.AddDirtyRect(new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight));
                        }

                        this.colorBitmap.Unlock();
                    }
                }
            }
        }


        /// azon események kezelése, mikor a Kinect nem elérhető valamilyek okból: megállítva, leállítva, kihúzva
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            // hiba esetén a státusz módosítása
            this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.SensorNotAvailableStatusText;
        }

      

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)e.OriginalSource;
            SampleDataItem sampleDataItem = button.DataContext as SampleDataItem;

            if (sampleDataItem != null && sampleDataItem.NavigationPage != null)
            {
                backButton.Visibility = System.Windows.Visibility.Visible;
                navigationRegion.Content = Activator.CreateInstance(sampleDataItem.NavigationPage);
            }
            else
            {
                var selectionDisplay = new SelectionDisplay(button.Content as string);
                this.kinectRegionGrid.Children.Add(selectionDisplay);

                // Selection dialog covers the entire interact-able area, so the current press interaction
                // should be completed. Otherwise hover capture will allow the button to be clicked again within
                // the same interaction (even whilst no longer visible).
                selectionDisplay.Focus();

                // Since the selection dialog covers the entire interact-able area, we should also complete
                // the current interaction of all other pointers.  This prevents other users interacting with elements
                // that are no longer visible.
                this.kinectRegion.InputPointerManager.CompleteGestures();

                e.Handled = true;
            }
        }

        /// <summary>
        /// Handle the back button click.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void GoBack(object sender, RoutedEventArgs e)
        {
            backButton.Visibility = System.Windows.Visibility.Hidden;
            navigationRegion.Content = this.kinectRegionGrid;
        }
    }
}

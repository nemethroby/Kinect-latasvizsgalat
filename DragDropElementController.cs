using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Kinect.Input;
using Microsoft.Kinect.Toolkit.Input;
using Microsoft.Kinect.Wpf.Controls;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;

namespace KinectGame
{
    public class DragDropElementController : IKinectManipulatableController
    {
        //Kinecttel való mozagtáshoz:
        private ManipulatableModel _inputModel;
        private KinectRegion _kinectRegion;
        private DragDropElement _dragDropElement;
        private bool _disposedValue;
       

        
        //a DragDropElement konstruktorával együtt jön létre
        public DragDropElementController(IInputModel inputModel, KinectRegion kinectRegion)
        {


            
            _inputModel = inputModel as ManipulatableModel;
            _kinectRegion = kinectRegion;
            _dragDropElement = _inputModel.Element as DragDropElement;
            

            //mozgatás kezdete
            _inputModel.ManipulationStarted += OnManipulationStarted;
            //mozgatás közben
            _inputModel.ManipulationUpdated += OnManipulationUpdated;
            //mozgatás végén
            _inputModel.ManipulationCompleted += OnManipulationCompleted;

        }


        
       


        //mozgatás végén
        private void OnManipulationCompleted(object sender,
            KinectManipulationCompletedEventArgs kinectManipulationCompletedEventArgs)
        {
        }

        //mozgatés közben
        private void OnManipulationUpdated(object sender, KinectManipulationUpdatedEventArgs e)
        {
            var parent = _dragDropElement.Parent as Canvas;
            if (parent != null)
            {
                //x,y változóba menti a _dragDropElement jelenlegi helyét
                //d változó a kéz kurzor jelenlegi pozíciója
                var d = e.Delta.Translation;
                var y = Canvas.GetTop(_dragDropElement);
                var x = Canvas.GetLeft(_dragDropElement);

                if (double.IsNaN(y)) y = 0;
                if (double.IsNaN(x)) x = 0;

                
                var yD = d.Y * _kinectRegion.ActualHeight;
                var xD = d.X * _kinectRegion.ActualWidth;

                //a Canvas mozgatása
                Canvas.SetTop(_dragDropElement, y + yD);
                Canvas.SetLeft(_dragDropElement, x + xD);
            }
        }

        //mozgatás kezdete
        private void OnManipulationStarted(object sender, KinectManipulationStartedEventArgs e)
        {
        }

        ManipulatableModel IKinectManipulatableController.ManipulatableInputModel
        {
            get { return _inputModel; }
        }

        FrameworkElement IKinectController.Element
        {
            get { return _inputModel.Element as FrameworkElement; }
        }

        //destruktor
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                _kinectRegion = null;
                _inputModel = null;
                _dragDropElement = null;

                _inputModel.ManipulationStarted -= OnManipulationStarted;
                _inputModel.ManipulationUpdated -= OnManipulationUpdated;
                _inputModel.ManipulationCompleted -= OnManipulationCompleted;

                _disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

   

        
    }
}
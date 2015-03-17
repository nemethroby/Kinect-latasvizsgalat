namespace KinectGame
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Kinect;
    using Microsoft.Kinect.Input;
    using Microsoft.Kinect.Toolkit.Input;
    using System.Diagnostics;

    public class HandInScreenEngagementModel : IKinectEngagementManager
    {
        bool stopped = true;
        bool engagementPeopleHaveChanged;
        List<BodyHandPair> handsToEngage;
        private int engagedPeopleAllowed;
        List<KinectPointerPoint> pointerPoints;
        TimeSpan lastFrameTime = TimeSpan.Zero;
        InputPointerManager inputPointerManager;

        internal HandInScreenEngagementModel(int engagedPeopleAllowed, InputPointerManager inputPointerManager)
        {
            this.EngagedPeopleAllowed = engagedPeopleAllowed;
            this.inputPointerManager = inputPointerManager;

            var kinectCoreWindow = KinectCoreWindow.GetForCurrentThread();
            kinectCoreWindow.PointerMoved += this.kinectCoreWindow_PointerMoved;
            this.handsToEngage = new List<BodyHandPair>();
            this.pointerPoints = new List<KinectPointerPoint>();
        }

        void kinectCoreWindow_PointerMoved(object sender, KinectPointerEventArgs e)
        {
            var kinectPointerPoint = e.CurrentPoint;
            var timeOfPointer = kinectPointerPoint.Properties.BodyTimeCounter;
            if (timeOfPointer == TimeSpan.Zero)
            {
                this.lastFrameTime = timeOfPointer;
            }
            else if (timeOfPointer == this.lastFrameTime)
            {
                this.pointerPoints.Add(kinectPointerPoint);
            }
            else
            {
                TrackEngagedPlayersViaHandInScreen();

                this.pointerPoints.Clear();
                this.pointerPoints.Add(kinectPointerPoint);
                this.lastFrameTime = timeOfPointer;
            }
        }

        public int EngagedPeopleAllowed
        {
            get
            {
                return this.engagedPeopleAllowed;
            }
            set
            {
                if (value > 2 || value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "This engagement manager requires 0 to 2 people to be set as the EngagedPeopleAllowed");
                }

                this.engagedPeopleAllowed = value;
            }
        }

        public bool EngagedBodyHandPairsChanged()
        {
            return this.engagementPeopleHaveChanged;
        }

        public IReadOnlyList<BodyHandPair> KinectManualEngagedHands
        {
            get
            {
                return KinectCoreWindow.KinectManualEngagedHands;
            }
        }

        public void StartManaging()
        {
            this.stopped = false;
        }

        public void StopManaging()
        {
            this.stopped = true;
        }

        private static bool IsHandInScreen(PointF unclampedHandPosition)
        {
            return (unclampedHandPosition.X >= 0.0 && unclampedHandPosition.X <= 1.0);
        }

        private bool IsHandBelowScreen(PointF unclampedHandPosition, uint pointerId)
        {
            ManipulatableModel manipulatableModel = this.inputPointerManager.GetCapturedInputModel(pointerId) as ManipulatableModel;
            return (unclampedHandPosition.Y > 1.1 && manipulatableModel == null);
        }

        private void TrackEngagedPlayersViaHandInScreen()
        {
            if (this.stopped)
            {
                return;
            }

            this.engagementPeopleHaveChanged = false;
            var currentlyEngagedHands = KinectCoreWindow.KinectManualEngagedHands;
            this.handsToEngage.Clear();

            foreach (var bodyHandPair in currentlyEngagedHands)
            {
                foreach (var kinectPointerPoint in this.pointerPoints)
                {
                    if (kinectPointerPoint.Properties.BodyTrackingId == bodyHandPair.BodyTrackingId
                        && kinectPointerPoint.Properties.HandType == bodyHandPair.HandType)
                    {
                        
                        bool toBeDisengaged = this.IsHandBelowScreen(kinectPointerPoint.Properties.UnclampedPosition, kinectPointerPoint.PointerId);

                        if (toBeDisengaged)
                        {
                            this.engagementPeopleHaveChanged = true;
                        }
                        else
                        {
                            this.handsToEngage.Add(bodyHandPair);
                        }
                    }
                }
            }

            
            foreach (var kinectPointerPoint in this.pointerPoints)
            {
                if (this.handsToEngage.Count < this.engagedPeopleAllowed)
                {
                    bool alreadyEngaged = false;
                    foreach (var bodyHandPair in this.handsToEngage)
                    {
                        alreadyEngaged = (kinectPointerPoint.Properties.BodyTrackingId == bodyHandPair.BodyTrackingId);
                    }

                    if (!alreadyEngaged)
                    {
                        
                        if (HandInScreenEngagementModel.IsHandInScreen(kinectPointerPoint.Properties.UnclampedPosition))
                        {
                            
                            this.handsToEngage.Add(
                                new BodyHandPair(kinectPointerPoint.Properties.BodyTrackingId, kinectPointerPoint.Properties.HandType));
                            this.engagementPeopleHaveChanged = true;
                        }
                    }
                }
            }

            if (this.engagementPeopleHaveChanged)
            {
                BodyHandPair firstPersonToEngage = null;
                BodyHandPair secondPersonToEngage = null;

                Debug.Assert(this.handsToEngage.Count <= 2, "handsToEngage should be <= 2");

                switch (this.handsToEngage.Count)
                {
                    case 0:
                        break;
                    case 1:
                        firstPersonToEngage = this.handsToEngage[0];
                        break;
                    case 2:
                        firstPersonToEngage = this.handsToEngage[0];
                        secondPersonToEngage = this.handsToEngage[1];
                        break;
                }

                switch (this.EngagedPeopleAllowed)
                {
                    case 0:
                    case 1:
                        KinectCoreWindow.SetKinectOnePersonManualEngagement(firstPersonToEngage);
                        break;
                    case 2:
                        KinectCoreWindow.SetKinectTwoPersonManualEngagement(firstPersonToEngage, secondPersonToEngage);
                        break;
                }
            }
        }
    }
}

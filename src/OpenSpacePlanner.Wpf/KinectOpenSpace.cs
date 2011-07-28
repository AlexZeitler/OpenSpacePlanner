using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using Microsoft.Research.Kinect.Nui;
using MessageBox = System.Windows.Forms.MessageBox;

namespace OpenSpacePlanner
{
  public class KinectOpenSpace
  {
    private readonly Runtime nui;
    private Point kinectScreen = new Point(640, 480);
    private int player = -1;
    private int inactivityCounter;
    // ReSharper disable NotAccessedField.Local
    private IDisposable inactivityTimer;
    // ReSharper restore NotAccessedField.Local

    public Point Screen { get; set; }
    public Boolean IsAvailable;

    public event KinectEnterEventHandler KinectEnter;
    public event KinectLeaveEventHandler KinectLeave;
    public event KinectPosEventHandler KinectPos;

    private void OnKinectPos(KinectPosEventHandlerArgs kinectPosEventHandlerArgs)
    {
      if ((KinectPos != null) && (IsAvailable))
        KinectPos(this, kinectPosEventHandlerArgs);
    }

    private void OnKinectLeave(KinectLeaveEventHandlerArgs kinectLeaveEventHandlerArgs)
    {
      if ((KinectLeave != null) && (IsAvailable))
        KinectLeave(this, kinectLeaveEventHandlerArgs);
    }

    private void OnKinectEnter(KinectEnterEventHandlerArgs kinectEnterEventHandlerArgs)
    {
      if ((KinectEnter != null) && (IsAvailable))
        KinectEnter(this, kinectEnterEventHandlerArgs);
    }

    public KinectOpenSpace(Point screenSize)
    {
      Screen = new Point(800, 600);
      if (screenSize != null)
        Screen = screenSize;

      nui = new Runtime();

      try
      {
        nui.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor);
      }
      catch (InvalidOperationException)
      {
        //MessageBox.Show("Runtime initialization failed. Please make sure Kinect device is plugged in.");
        return;
      }

      try
      {
        nui.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
        nui.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.DepthAndPlayerIndex);

      }
      catch (InvalidOperationException)
      {
        MessageBox.Show("Failed to open stream. Please make sure to specify a supported image type and resolution.");
        return;
      }

      nui.SkeletonFrameReady += NuiSkeletonFrameReady;
      inactivityTimer = Observable.Interval(TimeSpan.FromMilliseconds(200)).ObserveOnDispatcher().Subscribe(_ => OnInactivityTimer());
      IsAvailable = true;
    }

    private void OnInactivityTimer()
    {
      inactivityCounter++;
      if (inactivityCounter < 10)
        return;

      if (player == -1)
        return;

      player = -1;
      OnKinectLeave(new KinectLeaveEventHandlerArgs { Player = player });
    }

    private void NuiSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
    {
      SkeletonFrame skeletonFrame = e.SkeletonFrame;

      inactivityCounter = 0;
      if (skeletonFrame.Skeletons.Where(x => x.TrackingID == player).FirstOrDefault() == null)
      {
        player = -1;
        OnKinectLeave(new KinectLeaveEventHandlerArgs { Player = player });
      }
      foreach (var skeleton in skeletonFrame.Skeletons)
      {


        if (SkeletonTrackingState.Tracked == skeleton.TrackingState)
        {
          foreach (Joint joint in skeleton.Joints)
          {
            if (joint.ID == JointID.HandRight)
            {
              // Neuer Spieler gekommen
              if (player < 0)
              {
                player = skeleton.TrackingID;
                OnKinectEnter(new KinectEnterEventHandlerArgs { Player = player });
              }

              // Wenn aktueller Spieler, dann Event auslösen
              if (player == skeleton.TrackingID)
              {
                var screenPos = ReSizeForScreen(new Point(joint.Position.X, joint.Position.Y));
                //var screenPos = ReSizeForScreen(getDisplayPosition2(joint));
                OnKinectPos(
                    new KinectPosEventHandlerArgs
                        {
                          Player = player,
                          Point = screenPos
                        }
                        );
              }
            }
          }
        }
      }
    }

    private Point ReSizeForScreen(Point jointPos)
    {
      Console.WriteLine("Joint in x={0}, y={1}", jointPos.X, jointPos.Y);

      var x = Math.Min(Math.Max(jointPos.X * 1.5, -1), 1);
      var y = Math.Min(Math.Max(jointPos.Y * 1.5, 0), 1);
      return new Point((x * Screen.X) / 2 + (Screen.X / 2), Screen.Y - (y * Screen.Y));
    }

    // ReSharper disable UnusedMember.Local
    private Point GetDisplayPosition(Joint joint)
    {
      float depthX, depthY;
      nui.SkeletonEngine.SkeletonToDepthImage(joint.Position, out depthX, out depthY);
      depthX = Math.Max(0, Math.Min(depthX * 320, 320));  //convert to 320, 240 space
      depthY = Math.Max(0, Math.Min(depthY * 240, 240));  //convert to 320, 240 space
      int colorX, colorY;
      var imageViewArea = new ImageViewArea();
      // only ImageResolution.Resolution640x480 is supported at this point
      nui.NuiCamera.GetColorPixelCoordinatesFromDepthPixel(ImageResolution.Resolution640x480, imageViewArea, (int)depthX, (int)depthY, 0, out colorX, out colorY);

      // map back to skeleton.Width & skeleton.Height
      return new Point((int)(Screen.X * colorX / 640.0), (int)(Screen.Y * colorY / 480));
    }
  }
  // ReSharper restore UnusedMember.Local

  public delegate void KinectPosEventHandler(object sender, KinectPosEventHandlerArgs args);
  public delegate void KinectLeaveEventHandler(object sender, KinectLeaveEventHandlerArgs args);
  public delegate void KinectEnterEventHandler(object sender, KinectEnterEventHandlerArgs args);

  public class KinectPosEventHandlerArgs
  {
    public int Player { get; set; }
    public Point Point { get; set; }
  }

  public class KinectLeaveEventHandlerArgs
  {
    public int Player { get; set; }
  }

  public class KinectEnterEventHandlerArgs
  {
    public int Player { get; set; }
  }
}

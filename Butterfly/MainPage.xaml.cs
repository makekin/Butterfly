using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Butterfly
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Butterfly

        private Butterfly butterfly;

        // Flowers
        private List<Flower> flowers;

        // Which keys are pressed

        private bool UpPressed;
        private bool LeftPressed;
        private bool RightPressed;

        // game loop timer
        private DispatcherTimer timer;

        // audio
        private MediaElement mediaElement;

        
        public MainPage()
        {
            this.InitializeComponent();

            // add one butterfly
            butterfly = new Butterfly
            {
                LocationX = MyCanvas.Width / 2,
                LocationY = MyCanvas.Height / 2
            };
            MyCanvas.Children.Add(butterfly);

            //initialize list of flowers
            flowers = new List<Flower>();

            // Key listener
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;

            // Mouse Listener
            Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;

            // init audio
            InitAudio();

            // game loop timer
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 60);
            timer.Tick += Timer_Tick;
            timer.Start(); //start game

        }

        private async void InitAudio()
        {
            
            StorageFolder folder =
                await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            StorageFile file =
                await folder.GetFileAsync("ding.wav");
            var stream = await file.OpenAsync(FileAccessMode.Read);
            mediaElement = new MediaElement();
            mediaElement.AutoPlay = false;
            mediaElement.SetSource(stream, file.ContentType);
        }

        private void CoreWindow_PointerPressed(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.PointerEventArgs args)
        {
            // create a new flower
            Flower flower = new Flower();
            // set location with mouse position
            flower.LocationX = args.CurrentPoint.Position.X - flower.Width / 2;
            flower.LocationY = args.CurrentPoint.Position.Y - flower.Height / 2;
            // add to game canvas
            MyCanvas.Children.Add(flower);
            // set flower location in canvas
            flower.SetLocation();
            // add to flowers list (for collision checking)
            flowers.Add(flower);
        }

        /// <summary>
        /// Game Loop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, object e)
        {
            // Move Butterfly

            if (UpPressed) butterfly.Move();

            // Rotate Butterfly

            if (LeftPressed) butterfly.Rotate(-1);
            if (RightPressed) butterfly.Rotate(1);

            // Update location
            butterfly.SetLocation();

            // Collision to flowers
            CheckCollision();
        }

        private void CheckCollision()
        {
            // check all flowers
            foreach(Flower flower in flowers)
            {
                // get rects
                Rect Brect = new Rect(
                    butterfly.LocationX,
                    butterfly.LocationY,
                    butterfly.ActualWidth,
                    butterfly.ActualHeight
                    );

                Rect FRect = new Windows.Foundation.Rect(
                    flower.LocationX,
                    flower.LocationY,
                    flower.ActualWidth,
                    flower.ActualHeight
                    );
                // does these intersects?
                Brect.Intersect(FRect);
                // is Brect empty?
                if (!Brect.IsEmpty)
                {
                    // remove flower
                    MyCanvas.Children.Remove(flower);
                    flowers.Remove(flower);

                    // Play audio
                    mediaElement.Play();
                    break;
                }

            }
        }

        private void CoreWindow_KeyUp(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Up:
                    UpPressed = false;
                    break;
                case VirtualKey.Left:
                    LeftPressed = false;
                    break;
                case VirtualKey.Right:
                    RightPressed = false;
                    break;
            }
        }

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Up:
                    UpPressed = true;
                    break;
                case VirtualKey.Left:
                    LeftPressed = true;
                    break;
                case VirtualKey.Right:
                    RightPressed = true;
                    break;

                    
            }
        }
    }
}

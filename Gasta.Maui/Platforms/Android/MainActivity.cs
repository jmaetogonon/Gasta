using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;

namespace Gasta.Maui
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (Window is null) return;

            // Current Android versions draw apps edge-to-edge: the WebView would extend under
            // the status bar and the navigation buttons, which is what pushed headers into the
            // clock and the bottom nav / sheet buttons behind the system buttons. Take control
            // of the insets ourselves so the web page only ever sees the visible area, exactly
            // like the PWA in a normal browser tab.
            WindowCompat.SetDecorFitsSystemWindows(Window, false);
            if (OperatingSystem.IsAndroidVersionAtLeast(29))
                Window.NavigationBarContrastEnforced = false;

            var content = FindViewById(Android.Resource.Id.Content);
            if (content is not null)
                ViewCompat.SetOnApplyWindowInsetsListener(content, new SystemInsetsListener());
        }

        // Result of the "Save to device" dialog (see AndroidFileSaver).
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AndroidFileSaver.OnActivityResult(requestCode, resultCode, data);
        }
    }

    /// <summary>
    /// Pads the app by the status bar, the navigation bar / gesture area, any display cutout,
    /// and the on-screen keyboard (whichever is taller at the bottom), then consumes the
    /// insets so nothing below pads a second time. The WebView is simply resized by layout.
    /// </summary>
    internal class SystemInsetsListener : Java.Lang.Object, IOnApplyWindowInsetsListener
    { 
        public WindowInsetsCompat? OnApplyWindowInsets(Android.Views.View? v, WindowInsetsCompat? insets)
        {
            if (v is null || insets is null) return insets;

            var bars = insets.GetInsets(WindowInsetsCompat.Type.SystemBars() | WindowInsetsCompat.Type.DisplayCutout());
            var ime = insets.GetInsets(WindowInsetsCompat.Type.Ime());

            v.SetPadding(bars.Left, bars.Top, bars.Right, Math.Max(bars.Bottom, ime.Bottom));
            return WindowInsetsCompat.Consumed;
        }
    }
}

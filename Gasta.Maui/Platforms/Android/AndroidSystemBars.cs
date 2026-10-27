using Gasta.Maui.Services;
using AndroidX.Core.View;

namespace Gasta.Maui;

public class AndroidSystemBars : ISystemBars
{
    public void Apply(string hexColor, bool darkTheme)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
            var window = activity?.Window;
            if (activity is null || window is null) return;

            Android.Graphics.Color color;
            try { color = Android.Graphics.Color.ParseColor(hexColor); }
            catch (Java.Lang.IllegalArgumentException) { return; }

            // The app is padded by the bars' height; this colour is what shows in that padding.
            activity.FindViewById(Android.Resource.Id.Content)?.SetBackgroundColor(color);
            window.DecorView.SetBackgroundColor(color);

            // Dark icons on a light background and the other way round.
            var controller = WindowCompat.GetInsetsController(window, window.DecorView);
            if (controller is not null)
            {
                controller.AppearanceLightStatusBars = !darkTheme;
                controller.AppearanceLightNavigationBars = !darkTheme;
            }
        });
    }
}

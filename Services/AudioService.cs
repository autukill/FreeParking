namespace FreeParkingMaui.Services;

public class AudioService
{
    public void PlayNotificationSound()
    {
#if ANDROID
        PlayAndroidNotificationSound();
#endif
    }

#if ANDROID
    private void PlayAndroidNotificationSound()
    {
        var context = Android.App.Application.Context;
        var notification = new Android.Media.RingtoneManager(Android.App.Application.Context);
        var uri = Android.Media.RingtoneManager.GetDefaultUri(Android.Media.RingtoneType.Notification);
        var ringtone = Android.Media.RingtoneManager.GetRingtone(context, uri);
        ringtone?.Play();
    }
#endif
}

[assembly: Xamarin.Forms.Dependency(typeof(Http_Post.Droid.AndroidMethods))]
namespace Http_Post.Droid
{
    public class AndroidMethods : IAndroidMethods
    {
        public void CloseApp()
        {
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }
    }
}
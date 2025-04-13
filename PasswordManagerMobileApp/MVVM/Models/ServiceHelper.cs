
namespace PasswordManagerMobileApp.Models;


public static class ServiceHelper
{
    public static T GetService<T>() => 
        Current.GetService<T>();

    public static IServiceProvider Current =>
#if ANDROID
        MauiApplication.Current.Services;
#else
        App.Current.Services;
#endif
}

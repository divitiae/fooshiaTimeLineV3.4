using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Android.App;
using Android.Arch.Lifecycle;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App; 
using Firebase;
using Java.Lang;
using Newtonsoft.Json;
using Plugin.CurrentActivity;
using WoWonder.Activities;
using WoWonder.Activities.Communities.Groups;
using WoWonder.Activities.Communities.Pages;
using WoWonder.Activities.NativePost.Extra;
using WoWonder.Activities.NativePost.Services;
using WoWonder.Activities.SettingsPreferences;
using WoWonder.Activities.UserProfile;
using WoWonder.Helpers.Utils;
using WoWonder.Library.OneSignal; 
using WoWonderClient;
using WoWonderClient.Classes.Global;
using WoWonderClient.Classes.Posts;
using Xamarin.Android.Net;
using Exception = System.Exception;

namespace WoWonder
{
    //You can specify additional application information in this attribute
    [Application(UsesCleartextTraffic = true)]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        private static MainApplication Instance;
        public Activity Activity;

        public MainApplication(IntPtr handle, JniHandleOwnership transer) : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            try
            {
                base.OnCreate();
                //A great place to initialize Xamarin.Insights and Dependency Services!
                RegisterActivityLifecycleCallbacks(this);

                Instance = this;

                Client a = new Client(AppSettings.TripleDesAppServiceProvider);
                Console.WriteLine(a);

                //Bypass Web Errors 
                //======================================
                if (AppSettings.TurnSecurityProtocolType3072On)
                {
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    var client = new HttpClient(new AndroidClientHandler());
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls13;
                    Console.WriteLine(client);
                }


                if (AppSettings.TurnTrustFailureOnWebException)
                {
                    //If you are Getting this error >>> System.Net.WebException: Error: TrustFailure /// then Set it to true
                    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                    var b = new AesCryptoServiceProvider();
                    Console.WriteLine(b);
                }
                
                //OneSignal Notification  
                //======================================
                OneSignalNotification.RegisterNotificationDevice();

                // Check Created My Folder Or Not 
                //======================================
                Methods.Path.Chack_MyFolder();
                //======================================

                //Init Settings
                MainSettings.Init();

                ClassMapper.SetMappers();
                
                //App restarted after crash
                AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironmentOnUnhandledExceptionRaiser;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
                TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

                AppCompatDelegate.CompatVectorFromResourcesEnabled = true;
                FirebaseApp.InitializeApp(this);

                Methods.AppLifecycleObserver appLifecycleObserver = new Methods.AppLifecycleObserver();
                ProcessLifecycleOwner.Get().Lifecycle.AddObserver(appLifecycleObserver);
                 
                StartService(new Intent(this, typeof(ScheduledApiService))); 
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                Methods.DialogPopup.InvokeAndShowDialog(Activity, "ReportMode", exception.Message, "Close"); 
            }
        }


        private void AndroidEnvironmentOnUnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs e)
        {
            try
            {
                Intent intent = new Intent(Activity, typeof(SplashScreenActivity));
                intent.AddCategory(Intent.CategoryHome);
                intent.PutExtra("crash", true);
                intent.SetAction(Intent.ActionMain);
                intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);

                PendingIntent pendingIntent = PendingIntent.GetActivity(GetInstance().BaseContext, 0, intent, PendingIntentFlags.OneShot);
                AlarmManager mgr = (AlarmManager)GetInstance()?.BaseContext?.GetSystemService(AlarmService);
                mgr?.Set(AlarmType.Rtc, JavaSystem.CurrentTimeMillis() + 100, pendingIntent);

                Activity.Finish();
                JavaSystem.Exit(2);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                //var message = e.Exception.Message;
                var stackTrace = e.Exception.StackTrace;

                Methods.DisplayReportResult(Activity, stackTrace);
                Console.WriteLine(e.Exception);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                //var message = e;
                Methods.DisplayReportResult(Activity, e);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public static MainApplication GetInstance()
        {
            return Instance;
        }


        public override void OnTerminate() // on stop
        {
            try
            {
                base.OnTerminate();
                UnregisterActivityLifecycleCallbacks(this);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            try
            {
                Activity = activity;
                CrossCurrentActivity.Current.Activity = activity;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void OnActivityDestroyed(Activity activity)
        {
            Activity = activity;
        }

        public void OnActivityPaused(Activity activity)
        {
            Activity = activity;
        }

        public void OnActivityResumed(Activity activity)
        {
            try
            {
                Activity = activity;
                CrossCurrentActivity.Current.Activity = activity;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
            Activity = activity;
        }

        public void OnActivityStarted(Activity activity)
        {
            try
            {
                Activity = activity;
                CrossCurrentActivity.Current.Activity = activity;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void OnActivityStopped(Activity activity)
        {
            Activity = activity;
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            { 
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void NavigateTo(Activity fromContext, Type toContext, dynamic passData)
        {
            try
            {
                var intent = new Intent(this, toContext);

                if (passData != null)
                {
                    if (toContext == typeof(GroupProfileActivity))
                    {
                        if (passData is GroupClass groupClass)
                        { 
                            intent.PutExtra("GroupObject", JsonConvert.SerializeObject(groupClass));
                            intent.PutExtra("GroupId", groupClass.GroupId);
                        }
                    }
                    else if (toContext == typeof(PageProfileActivity))
                    {
                        if (passData is PageClass pageClass)
                        {
                            intent.PutExtra("PageObject", JsonConvert.SerializeObject(pageClass));
                            intent.PutExtra("PageId", pageClass.PageId);
                        } 
                    }
                    else if (toContext == typeof(YoutubePlayerActivity))
                    {
                        if (passData is PostDataObject postData)
                        {
                            intent.PutExtra("PostObject", JsonConvert.SerializeObject(postData));
                            intent.PutExtra("PostId", postData.PostId);
                        }
                         
                        fromContext.StartActivity(intent);
                        ((AppCompatActivity)fromContext).OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);
                        return;
                    }
                    else if (toContext == typeof(UserProfileActivity))
                    {
                        if (passData is UserDataObject userDataObject)
                        {
                            intent.PutExtra("UserObject", JsonConvert.SerializeObject(userDataObject));
                            intent.PutExtra("UserId", userDataObject.UserId);
                        }
                       
                        ((AppCompatActivity)fromContext).OverridePendingTransition(Resource.Animation.abc_popup_enter, Resource.Animation.popup_exit);
                        fromContext.StartActivity(intent);
                        return;
                    }

                    fromContext.StartActivity(intent);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AFollestad.MaterialDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Locations;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using TheArtOfDev.Edmodo.Cropper;
using Java.Lang;
using MeoNavLib.Com;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Plugin.Geolocator;
using WoWonder.Activities.AddPost;
using WoWonder.Activities.NativePost.Extra;
using WoWonder.Activities.NativePost.Post;
using WoWonder.Activities.NativePost.Services;
using WoWonder.Activities.Search;
using WoWonder.Activities.SettingsPreferences;
using WoWonder.Activities.Story;
using WoWonder.Activities.Story.Adapters;
using WoWonder.Activities.Tabbes.Adapters;
using WoWonder.Activities.Tabbes.Fragment;
using WoWonder.Adapters;
using WoWonder.Helpers.Ads;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils; 
using WoWonder.SQLite;
using WoWonderClient;
using WoWonderClient.Classes.Global;
using WoWonderClient.Classes.Posts;
using WoWonderClient.Classes.Product;
using WoWonderClient.Requests;
using Console = System.Console;
using Exception = System.Exception;
using File = Java.IO.File;
using SearchView = Android.Support.V7.Widget.SearchView;
using Uri = Android.Net.Uri;

namespace WoWonder.Activities.Tabbes
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges =ConfigChanges.Keyboard | ConfigChanges.Orientation | ConfigChanges.KeyboardHidden |ConfigChanges.ScreenLayout | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.UiMode | ConfigChanges.Locale)]
    public class TabbedMainActivity : AppCompatActivity, View.IOnClickListener, View.IOnFocusChangeListener, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        private static TabbedMainActivity Instance; 
        public ProUsersAdapter ProUsersAdapter;
        public HashtagUserAdapter HashTagUserAdapter;
        public ProPagesAdapter ProPagesAdapter;
        public ObservableCollection<UserDataObject> FriendRequestsList = new ObservableCollection<UserDataObject>();
        private CustomNavigationController FragmentBottomNavigator;
        public ViewPager ViewPager;
        private MainTabAdapter TabAdapter;
        public NewsFeedNative NewsFeedTab;
        private NotificationsFragment NotificationsTab;
        public TrendingFragment TrendingTab;
        private MoreFragment MoreTab;
        private MeowBottomNavigation NavigationTabBar;
        public FloatingActionButton FloatingActionButton;
        private SearchView SearchViewBar;
        private ImageButton BtnStory;
        private string ImageType = "";
        private static string CountNotificationsStatic = "0", CountMessagesStatic = "0", CountFriendStatic = "0";
        private static bool AddAnnouncement, RecentlyBackPressed;
        public static bool OnlineUsers = true;
        private readonly Handler ExitHandler = new Handler();
        
        private PowerManager.WakeLock Wl;
        public AdsGoogle.AdMobRewardedVideo RewardedVideoAd;
   
        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                AppCenter.Start("9f8cf987-8bfc-4895-a18b-bd4a86ef9ae3", typeof(Analytics), typeof(Crashes));

                Window?.SetSoftInputMode(SoftInput.StateAlwaysHidden);

                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base); base.OnCreate(savedInstanceState);

                AddFlagsWakeLock();
                 
                Methods.App.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.Tabbed_Main_Layout);

                Instance = this;

                //Get Value And Set Toolbar
                InitComponent(); 
                SetRecyclerViewAdapters();

                OnlineUsers = MainSettings.SharedData?.GetBoolean("onlineUser_key", true) ?? true;
                 
                AddFragmentsTabs();
                  
                GetGeneralAppData();

                SetService();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
         
        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                SearchViewBar?.ClearFocus();
                AddOrRemoveEvent(true);
                RewardedVideoAd?.OnResume(this);
                MoreTab?.MAdView?.Resume();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();
                AddOrRemoveEvent(false);
                RewardedVideoAd?.OnPause(this);
                MoreTab?.MAdView?.Pause();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnStop()
        {
            try
            {
                base.OnStop();
                NewsFeedTab?.MainRecyclerView?.StopVideo();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            } 
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
                Glide.Get(this).TrimMemory((int)level);
               
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect();
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                NewsFeedTab?.MainRecyclerView?.ReleasePlayer();
                NewsFeedTab?.MainRecyclerView?.InsertTheLatestPosts();

                RewardedVideoAd?.OnDestroy(this);
                MoreTab?.MAdView?.Destroy();

                OffWakeLock();
                base.OnDestroy(); 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            try
            {
                base.OnConfigurationChanged(newConfig);

                var currentNightMode = newConfig.UiMode & UiMode.NightMask;
                switch (currentNightMode)
                {
                    case UiMode.NightNo:
                        // Night mode is not active, we're using the light theme
                        AppSettings.SetTabDarkTheme = false;
                        break;
                    case UiMode.NightYes:
                        // Night mode is active, we're using dark theme
                        AppSettings.SetTabDarkTheme = true;
                        break;
                }

                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);

                NavigationTabBar.SetBackgroundBottomColor(AppSettings.SetTabDarkTheme ? Color.Black : Color.White);
                NavigationTabBar.SetCircleColor(AppSettings.SetTabDarkTheme ? Color.Black : Color.White); 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Functions
       
        private void InitComponent()
        {
            try
            {
                NavigationTabBar = FindViewById<MeowBottomNavigation>(Resource.Id.ntb_horizontal);
                FragmentBottomNavigator = new CustomNavigationController(this, NavigationTabBar);

                ViewPager = FindViewById<ViewPager>(Resource.Id.vp_horizontal_ntb);
                TabAdapter = new MainTabAdapter(SupportFragmentManager);
                SearchViewBar = FindViewById<SearchView>(Resource.Id.searchView);
                if (SearchViewBar != null)
                {
                    SearchViewBar.SetIconifiedByDefault(false);
                    SearchViewBar.SetOnClickListener(this);
                    SearchViewBar.SetOnSearchClickListener(this);
                    SearchViewBar.SetOnQueryTextFocusChangeListener(this);
                }

                if (AppSettings.FlowDirectionRightToLeft)
                    NavigationTabBar.LayoutDirection = LayoutDirection.Rtl;

                FloatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButtonView);
                FloatingActionButton.Visibility = AppSettings.ShowAddPostOnNewsFeed ? ViewStates.Visible : ViewStates.Gone;

                BtnStory = FindViewById<ImageButton>(Resource.Id.storybutton);

                if (!Directory.Exists(Methods.Path.FolderDcimStory))
                    Directory.CreateDirectory(Methods.Path.FolderDcimStory);

                BtnStory.Visibility = AppSettings.ShowStory ? ViewStates.Visible : ViewStates.Gone;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
         
        private void SetRecyclerViewAdapters()
        {
            try
            {
                //============================= Hash tag Users ================================== 
                HashTagUserAdapter = new HashtagUserAdapter(this)
                {
                    MHashtagList = new ObservableCollection<TrendingHashtag>()
                };
                ////============================= Promoted Pages ================================== 
                ProPagesAdapter = new ProPagesAdapter(this) {MProPagesList = new ObservableCollection<PageClass>()};
                ////============================= Pro Users ================================== 
                ProUsersAdapter = new ProUsersAdapter(this)
                {
                    MProUsersList = new ObservableCollection<UserDataObject>()
                };
                ////============================= Requests Users ================================== 
                FriendRequestsList = new ObservableCollection<UserDataObject>();
                //===============================================================
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                { 
                    if (AppSettings.ShowStory)
                        BtnStory.Click += CreateStories_OnClick;

                    FloatingActionButton.Click += Btn_AddPost_OnClick;
                }
                else
                {
                    //Close Event
                    if (AppSettings.ShowStory)
                        BtnStory.Click -= CreateStories_OnClick;

                    FloatingActionButton.Click -= Btn_AddPost_OnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public static TabbedMainActivity GetInstance()
        {
            try
            {
                return Instance;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null!;
            }
        }

        #endregion

        #region Events

        //Event Open page add post
        private void Btn_AddPost_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                var intent = new Intent(this, typeof(AddPostActivity));
                intent.PutExtra("Type", "Normal");
                intent.PutExtra("PostId", UserDetails.UserId);
                //intent.PutExtra("itemObject", JsonConvert.SerializeObject(PageData));
                StartActivityForResult(intent, 2500);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Permissions && Result

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                 
                //If its from Camera or Gallery
                if (requestCode == 503) // Add story using camera
                {
                    if (string.IsNullOrEmpty(IntentController.CurrentPhotoPath))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Failed_to_load), ToastLength.Short)?.Show();
                        return;
                    }
                     
                    if (Methods.MultiMedia.CheckFileIfExits(IntentController.CurrentPhotoPath) != "File Dont Exists")
                    {
                        //Do something with your Uri
                        Intent intent = new Intent(this, typeof(AddStoryActivity));
                        intent.PutExtra("Uri", IntentController.CurrentPhotoPath);
                        intent.PutExtra("Type", "image");
                        StartActivity(intent);
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Failed_to_load), ToastLength.Short)?.Show();
                    }
                }
                else if (requestCode == 501) // Add video story
                { 
                    var filepath = Methods.AttachmentFiles.GetActualPathFromFile(this, data.Data);
                    if (filepath != null)
                    {
                        var type = Methods.AttachmentFiles.Check_FileExtension(filepath);
                        if (type == "Video")
                        {
                            var fileName = filepath.Split('/').Last();
                            var fileNameWithoutExtension = fileName.Split('.').First();
                            var pathWithoutFilename = Methods.Path.FolderDcimImage;
                            var fullPathFile = new File(Methods.Path.FolderDcimImage, fileNameWithoutExtension + ".png");

                            var videoPlaceHolderImage = Methods.MultiMedia.GetMediaFrom_Gallery(pathWithoutFilename, fileNameWithoutExtension + ".png");
                            if (videoPlaceHolderImage == "File Dont Exists")
                            {
                                var bitmapImage = Methods.MultiMedia.Retrieve_VideoFrame_AsBitmap(this, data.Data.ToString());
                                Methods.MultiMedia.Export_Bitmap_As_Image(bitmapImage, fileNameWithoutExtension, pathWithoutFilename);
                            }

                            Intent intent = new Intent(this, typeof(AddStoryActivity));
                            intent.PutExtra("Uri", filepath);
                            intent.PutExtra("Thumbnail", fullPathFile);
                            intent.PutExtra("Type", "video");
                            StartActivity(intent);
                        }
                    }
                    else
                    {
                        Uri uri = data.Data;
                        var filepath2 = Methods.AttachmentFiles.GetActualPathFromFile(this, uri);
                        PickiTonCompleteListener(filepath2);
                    } 
                }
                else if (requestCode == 500) // Add image story
                {
                    Uri uri = data.Data;
                    var filepath = Methods.AttachmentFiles.GetActualPathFromFile(this, uri);
                    PickiTonCompleteListener(filepath);
                }
                else if (requestCode == CropImage.CropImageActivityRequestCode) //If its from Camera or Gallery
                {
                    var result = CropImage.GetActivityResult(data);

                    if (resultCode == Result.Ok)
                    {
                        if (result.IsSuccessful)
                        {
                            var resultUri = result.Uri;

                            if (!string.IsNullOrEmpty(resultUri.Path))
                            {
                                //Do something with your Uri
                                Intent intent = new Intent(this, typeof(AddStoryActivity));
                                intent.PutExtra("Uri", resultUri.Path);
                                intent.PutExtra("Type", "image");
                                StartActivity(intent);
                            }
                            else
                            {
                                Toast.MakeText(this, GetText(Resource.String.Lbl_something_went_wrong),ToastLength.Long)?.Show();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_something_went_wrong), ToastLength.Long)?.Show();
                        }
                    } 
                }
                else if (requestCode == 2 && resultCode == Result.Ok)
                {
                    SearchViewBar.Focusable = false;
                    SearchViewBar.FocusableInTouchMode = false;
                    SearchViewBar.ClearFocus();
                }
                else if (requestCode == 2500 && resultCode == Result.Ok) //add post
                {
                    if (!string.IsNullOrEmpty(data.GetStringExtra("itemObject")))
                    {
                        var postData = JsonConvert.DeserializeObject<PostDataObject>(data.GetStringExtra("itemObject"));
                        if (postData != null)
                        {
                            var countList = NewsFeedTab.PostFeedAdapter.ItemCount;

                            var combine = new FeedCombiner(postData, NewsFeedTab.PostFeedAdapter.ListDiffer, this);
                            combine.CombineDefaultPostSections("Top");

                            int countIndex = 1;
                            var model1 = NewsFeedTab.PostFeedAdapter.ListDiffer.FirstOrDefault(a => a.TypeView == PostModelType.Story);
                            var model2 = NewsFeedTab.PostFeedAdapter.ListDiffer.FirstOrDefault(a => a.TypeView == PostModelType.AddPostBox);
                            var model3 = NewsFeedTab.PostFeedAdapter.ListDiffer.FirstOrDefault(a => a.TypeView == PostModelType.AlertBox);
                            var model4 = NewsFeedTab.PostFeedAdapter.ListDiffer.FirstOrDefault(a => a.TypeView == PostModelType.SearchForPosts);

                            if (model4 != null)
                                countIndex += NewsFeedTab.PostFeedAdapter.ListDiffer.IndexOf(model4) + 1;
                            else if (model3 != null)
                                countIndex += NewsFeedTab.PostFeedAdapter.ListDiffer.IndexOf(model3) + 1;
                            else if (model2 != null)
                                countIndex += NewsFeedTab.PostFeedAdapter.ListDiffer.IndexOf(model2) + 1;
                            else if (model1 != null)
                                countIndex += NewsFeedTab.PostFeedAdapter.ListDiffer.IndexOf(model1) + 1;
                            else
                                countIndex = 0;

                            NewsFeedTab.PostFeedAdapter.NotifyItemRangeInserted(countIndex, NewsFeedTab.PostFeedAdapter.ListDiffer.Count - countList);
                        }
                    }
                    else
                    {
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => NewsFeedTab.MainRecyclerView.FetchNewsFeedApiPosts() });
                    }
                }
                else if (requestCode == 3950 && resultCode == Result.Ok) //Edit post
                {
                    var postId = data.GetStringExtra("PostId") ?? "";
                    var postText = data.GetStringExtra("PostText") ?? "";
                    var diff = NewsFeedTab.PostFeedAdapter.ListDiffer;
                    List<AdapterModelsClass> dataGlobal = diff.Where(a => a.PostData?.Id == postId).ToList();
                    if (dataGlobal.Count > 0)
                    {
                        foreach (var postData in dataGlobal)
                        {
                            postData.PostData.Orginaltext = postText;
                            var index = diff.IndexOf(postData);
                            if (index > -1)
                            {
                                NewsFeedTab.PostFeedAdapter.NotifyItemChanged(index);
                            }
                        }

                        var checkTextSection = dataGlobal.FirstOrDefault(w => w.TypeView == PostModelType.TextSectionPostPart);
                        if (checkTextSection == null)
                        {
                            var collection = dataGlobal.FirstOrDefault()?.PostData;
                            var item = new AdapterModelsClass
                            {
                                TypeView = PostModelType.TextSectionPostPart,
                                Id = Convert.ToInt32((int)PostModelType.TextSectionPostPart + collection?.Id),
                                PostData = collection,
                                IsDefaultFeedPost = true
                            };

                            var headerPostIndex = diff.IndexOf(dataGlobal.FirstOrDefault(w => w.TypeView == PostModelType.HeaderPost));
                            if (headerPostIndex > -1)
                            {
                                diff.Insert(headerPostIndex + 1, item);
                                NewsFeedTab.PostFeedAdapter.NotifyItemInserted(headerPostIndex + 1);
                            }
                        }
                    }
                } 
                else if (requestCode == 3500 && resultCode == Result.Ok) //Edit post product 
                { 
                    if (string.IsNullOrEmpty(data.GetStringExtra("itemData"))) return;
                    var item = JsonConvert.DeserializeObject<ProductDataObject>(data.GetStringExtra("itemData"));
                    if (item != null)
                    {
                        var diff = NewsFeedTab.PostFeedAdapter.ListDiffer;
                        var dataGlobal = diff.Where(a => a.PostData?.Id == item.PostId).ToList();
                        if (dataGlobal.Count > 0)
                        {
                            foreach (var postData in dataGlobal)
                            {
                                var index = diff.IndexOf(postData);
                                if (index > -1)
                                {
                                    var productUnion = postData.PostData.Product?.ProductClass;
                                    if (productUnion != null) productUnion.Id = item.Id;
                                    productUnion = item;
                                    Console.WriteLine(productUnion);
                                     
                                    NewsFeedTab.PostFeedAdapter.NotifyItemChanged(index);
                                }
                            }
                        } 
                    }
                } 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults); 
                if (requestCode == 108)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        switch (ImageType)
                        {
                            //requestCode >> 500 => Image Gallery
                            case "Image" when AppSettings.ImageCropping:
                                OpenDialogGallery("Image");
                                break;
                            case "Video":
                                //requestCode >> 501 => video Gallery
                                new IntentController(this).OpenIntentVideoGallery();
                                break;
                            case "Camera":
                                //requestCode >> 503 => Camera
                                new IntentController(this).OpenIntentCamera();
                                break;
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long)?.Show();
                    }
                } 
                else if (requestCode == 123)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { CheckAndGetLocation  });
                    }
                }  
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
         
        #endregion
            
        #region Search View

        public void OnClick(View v)
        {
            if (v.Id == SearchViewBar.Id)
            {
                //Hide keyboard programmatically in MonoDroid

                var inputManager = (InputMethodManager) GetSystemService(InputMethodService);
                inputManager.HideSoftInputFromWindow(SearchViewBar.WindowToken, HideSoftInputFlags.None);

                SearchViewBar.ClearFocus();

                var intent = new Intent(this, typeof(SearchTabbedActivity));
                intent.PutExtra("Key", "");
                StartActivity(intent);
            }
        }

        public void OnFocusChange(View v, bool hasFocus)
        {
            try
            {
                if (v.Id == SearchViewBar.Id && hasFocus)
                {
                    var intent = new Intent(this, typeof(SearchTabbedActivity));
                    intent.PutExtra("Key", "");
                    StartActivity(intent);
                }

                SearchViewBar.ClearFocus();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Set Tab

        private void AddFragmentsTabs()
        {
            try
            {
                TabAdapter.ClaerFragment();

                NewsFeedTab = new NewsFeedNative();
                NotificationsTab = new NotificationsFragment();
                TrendingTab = new TrendingFragment();
                MoreTab = new MoreFragment();

                if (TabAdapter != null && TabAdapter.Count <= 0)
                {
                    TabAdapter.AddFragment(NewsFeedTab, GetText(Resource.String.Lbl_News_Feed));
                    TabAdapter.AddFragment(NotificationsTab, GetText(Resource.String.Lbl_Notifications));

                    if (AppSettings.ShowTrendingPage)
                        TabAdapter.AddFragment(TrendingTab, GetText(Resource.String.Lbl_Trending));

                    TabAdapter.AddFragment(MoreTab, GetText(Resource.String.Lbl_More));
                    ViewPager.CurrentItem = 3;
                    ViewPager.OffscreenPageLimit = TabAdapter.Count;
                    ViewPager.Adapter = TabAdapter;
                    //ViewPager.PageScrolled += ViewPagerOnPageScrolled;
                    ViewPager.PageSelected += ViewPagerOnPageSelected;
                     
                    NavigationTabBar.SetZ(5);
                }
                 
                NavigationTabBar.Show(0, false);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void ViewPagerOnPageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            try
            {
                if (e.Position < 0) return;

                if (e.Position == 0) // News_Feed_Tab
                {
                    if (NavigationTabBar.GetSelectedId() != e.Position)
                        NavigationTabBar.Show(0, true);
                }
                else if (e.Position == 1) // Notifications_Tab
                {
                    if (NavigationTabBar.GetSelectedId() != e.Position)
                        NavigationTabBar.Show(1, true);

                    NewsFeedTab?.MainRecyclerView?.StopVideo();
                }
                else if (e.Position == 2 && AppSettings.ShowTrendingPage) // Trending_Tab
                {
                    if (NavigationTabBar.GetSelectedId() != e.Position)
                        NavigationTabBar.Show(2, true);

                    NewsFeedTab?.MainRecyclerView?.StopVideo();
                }
                else if (e.Position == 3) // More_Tab
                {
                    if (NavigationTabBar.GetSelectedId() != e.Position)
                        NavigationTabBar.Show(3, true);

                    NewsFeedTab?.MainRecyclerView?.StopVideo();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
         
        #endregion

        #region MaterialDialog

        private void ShowDialogAddStory()
        {
            try
            {
                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);

                arrayAdapter.Add(GetText(Resource.String.image));
                arrayAdapter.Add(GetText(Resource.String.video));
                arrayAdapter.Add(GetText(Resource.String.Lbl_Camera));

                dialogList.Title(GetText(Resource.String.Lbl_Addnewstory));
                dialogList.Items(arrayAdapter);
                dialogList.NegativeText(GetText(Resource.String.Lbl_Close)).OnNegative(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                if (itemString.ToString() == GetText(Resource.String.image))
                {
                    OpenDialogGallery("Image"); //requestCode >> 500 => Image Gallery
                }
                else if (itemString.ToString() == GetText(Resource.String.video))
                {
                    ImageType = "Video";

                    // Check if we're running on Android 5.0 or higher
                    if ((int)Build.VERSION.SdkInt < 23)
                    {
                        //requestCode >> 501 => video Gallery
                        new IntentController(this).OpenIntentVideoGallery();
                    }
                    else
                    {
                        if (CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted && CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted 
                            && CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                        {
                            //requestCode >> 501 => video Gallery
                            new IntentController(this).OpenIntentVideoGallery();
                        }
                        else
                        {
                            new PermissionsController(this).RequestPermission(108);
                        }
                    }
                }
                else if (itemString.ToString() == GetText(Resource.String.Lbl_Camera))
                {
                    ImageType = "Camera";

                    // Check if we're running on Android 5.0 or higher
                    if ((int)Build.VERSION.SdkInt < 23)
                    {
                        //requestCode >> 503 => Camera
                        new IntentController(this).OpenIntentCamera();
                    }
                    else
                    {
                        if (CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted
                            && CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted
                            && CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                        {
                            //requestCode >> 503 => Camera
                            new IntentController(this).OpenIntentCamera();
                        }
                        else
                        {
                            new PermissionsController(this).RequestPermission(108);
                        }
                    }
                } 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            }
        }

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (p1 == DialogAction.Positive)
                {
                }
                else if (p1 == DialogAction.Negative)
                {
                    p0.Dismiss();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
         
        #endregion

        #region Stories

        public void StoryAdapterOnItemClick(object sender, StoryAdapterClickEventArgs e)
        {
            try
            {
                var diff = NewsFeedTab.PostFeedAdapter.ListDiffer;
                var checkSection = diff.FirstOrDefault(a => a.TypeView == PostModelType.Story);
                if (checkSection != null)
                {
                    //Open View Story Or Create New Story
                    var item = NewsFeedTab.PostFeedAdapter?.HolderStory?.StoryAdapter.GetItem(e.Position);
                    if (item != null)
                    {
                        //var circleIndicator = e.View.FindViewById<CircleImageView>(Resource.Id.profile_indicator); 
                        //circleIndicator.BorderColor = Color.ParseColor(Settings.StoryReadColor);

                        if (item.Type == "Your")
                        {
                            ShowDialogAddStory();
                        }
                        else
                        { 
                            Intent intent = new Intent(this, typeof(ViewStoryActivity));
                            intent.PutExtra("UserId", item.UserId);
                            intent.PutExtra("DataItem", JsonConvert.SerializeObject(item));
                            intent.PutExtra("IndexItem", e.Position);
                            StartActivity(intent);

                            item.ProfileIndicator = AppSettings.StoryReadColor;
                            NewsFeedTab.PostFeedAdapter?.HolderStory?.StoryAdapter.NotifyItemChanged(e.Position);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Menu Create Stories >>  Image , Video
        private void CreateStories_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                ShowDialogAddStory();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Back Pressed 

        public override void OnBackPressed()
        {
            try
            {
                if (RecentlyBackPressed)
                {
                    ExitHandler.RemoveCallbacks(() => { RecentlyBackPressed = false; });
                    RecentlyBackPressed = false;
                    MoveTaskToBack(true);
                }
                else
                {
                    RecentlyBackPressed = true;
                    Toast.MakeText(this, GetString(Resource.String.press_again_exit), ToastLength.Long)?.Show();
                    ExitHandler.PostDelayed(() => { RecentlyBackPressed = false; }, 2000L);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region WakeLock System

        private void AddFlagsWakeLock()
        {
            try
            {
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    Window?.AddFlags(WindowManagerFlags.KeepScreenOn);
                }
                else
                {
                    if (CheckSelfPermission(Manifest.Permission.WakeLock) == Permission.Granted)
                    {
                        Window?.AddFlags(WindowManagerFlags.KeepScreenOn);
                    }
                    else
                    {
                        //request Code 110
                        new PermissionsController(this).RequestPermission(110);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetOnWakeLock()
        {
            try
            {
                PowerManager pm = (PowerManager)GetSystemService(PowerService);
                Wl = pm.NewWakeLock(WakeLockFlags.ScreenDim, "My Tag");
                Wl.Acquire();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetOffWakeLock()
        {
            try
            {
                PowerManager pm = (PowerManager)GetSystemService(PowerService);
                Wl = pm.NewWakeLock(WakeLockFlags.ScreenBright, "My Tag");
                Wl.Acquire();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void OffWakeLock()
        {
            try
            {
                // ..screen will stay on during this section..
                Wl?.Release();
                Wl = null!;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Service

        private void SetService()
        {
            try
            { 
                var intent = new Intent(this, typeof(ScheduledApiService));
                //intent.PutExtra("receiverTag", Receiver);
                StartService(intent);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
         

        #endregion

        private void GetGeneralAppData()
        {
            try
            {
                var sqlEntity = new SqLiteDatabase();

                if (string.IsNullOrEmpty(Current.AccessToken) || string.IsNullOrEmpty(UserDetails.UserId))
                    sqlEntity.Get_data_Login_Credentials();

                var data = ListUtils.DataUserLoginList.FirstOrDefault();
                if (data != null && data.Status != "Active")
                {
                    data.Status = "Active";
                    UserDetails.Status = "Active";
                    sqlEntity.InsertOrUpdateLogin_Credentials(data);
                }

                sqlEntity.Get_MyProfile();

                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt >= 23)
                {
                    if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.AccessMediaLocation) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.AccessFineLocation) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) == Permission.Granted)
                    {
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { CheckAndGetLocation });
                    }
                    else
                    {
                        // 100 >> Storage , 103 >> Camera , 105 >> Location
                        RequestPermissions(new[]
                        { 
                            Manifest.Permission.ReadExternalStorage,
                            Manifest.Permission.WriteExternalStorage,
                            Manifest.Permission.Camera,
                            Manifest.Permission.AccessFineLocation,
                            Manifest.Permission.AccessCoarseLocation,
                            Manifest.Permission.AccessMediaLocation,
                        }, 123);
                    }
                }
                else
                {
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { CheckAndGetLocation });
                }
                    
                Glide.With(this).Load(UserDetails.Avatar).Apply(new RequestOptions().SetDiskCacheStrategy(DiskCacheStrategy.All).CircleCrop()).Preload();

                if (Methods.CheckConnectivity())
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> {() => ApiRequest.Get_MyProfileData_Api(this), ApiRequest.LoadSuggestedUser, ApiRequest.LoadSuggestedGroup });

                sqlEntity.Dispose();
                
                LoadConfigSettings(); 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void LoadConfigSettings()
        {
            try
            {
                var dbDatabase = new SqLiteDatabase();  
                var settingsData = dbDatabase.GetSettings();
                if (settingsData != null)
                    ListUtils.SettingsSiteList = settingsData;

                if (Methods.CheckConnectivity())
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetSettings_Api(this) });

                dbDatabase.Dispose();
                 
                if (ListUtils.SettingsSiteList?.PostColors?.PostColorsList != null)
                {
                    foreach (var item in ListUtils.SettingsSiteList?.PostColors?.PostColorsList.Values)
                    {
                        if (!string.IsNullOrEmpty(item.Image))
                        {
                            Glide.With(this).Load(item.Image).Apply(new RequestOptions().SetDiskCacheStrategy(DiskCacheStrategy.All).CenterCrop()).Preload();
                        }
                        else
                        {
                            var colorsList = new List<int>();

                            if (!string.IsNullOrEmpty(item.Color1))
                                colorsList.Add(Color.ParseColor(item.Color1));

                            if (!string.IsNullOrEmpty(item.Color2))
                                colorsList.Add(Color.ParseColor(item.Color2));

                            GradientDrawable gd = new GradientDrawable(GradientDrawable.Orientation.TopBottom, colorsList.ToArray());
                            gd.SetCornerRadius(0f);
                            Glide.With(this).Load(gd).Apply(new RequestOptions().SetDiskCacheStrategy(DiskCacheStrategy.All).CenterCrop()).Preload();
                        } 
                    } 
                } 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private async Task CheckAndGetLocation()
        {
            try
            {
                var locationManager = (LocationManager)GetSystemService(LocationService);
                if (!locationManager.IsProviderEnabled(LocationManager.GpsProvider))
                {

                }
                else
                {
                    var locator = CrossGeolocator.Current;
                    locator.DesiredAccuracy = 50;
                    var position = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(10000));
                    Console.WriteLine("Position Status: {0}", position.Timestamp);
                    Console.WriteLine("Position Latitude: {0}", position.Latitude);
                    Console.WriteLine("Position Longitude: {0}", position.Longitude);

                    UserDetails.Lat = position.Latitude.ToString(CultureInfo.InvariantCulture);
                    UserDetails.Lng = position.Longitude.ToString(CultureInfo.InvariantCulture);

                   if (Methods.CheckConnectivity())
                   {
                       var dictionaryProfile = new Dictionary<string, string>
                       {
                           {"lat", UserDetails.Lat},
                           {"lng", UserDetails.Lng},
                       };

                       var dataUser = ListUtils.MyProfileList.FirstOrDefault();
                       if (dataUser != null)
                       {
                           dataUser.Lat = UserDetails.Lat;
                           dataUser.Lat = UserDetails.Lat;

                           var sqLiteDatabase = new SqLiteDatabase();
                           sqLiteDatabase.Insert_Or_Update_To_MyProfileTable(dataUser);
                           sqLiteDatabase.Dispose();
                       }

                       if (Methods.CheckConnectivity())
                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Update_User_Data(dictionaryProfile) });
                   }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public async Task Get_Notifications()
        {
            try
            {
                var (countNotifications, countFriend, countMessages, textAnnouncement) = await NotificationsTab.LoadGeneralData(false).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(countNotifications) && countNotifications != "0" && countNotifications != CountNotificationsStatic)
                    RunOnUiThread(() =>
                    {
                        try
                        {
                            FragmentBottomNavigator.ShowBadge(1, countNotifications, true);
                            CountNotificationsStatic = countNotifications;
                        }
                        catch (Exception e)
                        {
                            Methods.DisplayReportResultTrack(e);
                        }
                    });

                if (!string.IsNullOrEmpty(countFriend) && countFriend != "0" && countFriend != CountFriendStatic)
                    RunOnUiThread(() =>
                    {
                        try
                        {
                            if (AppSettings.ShowTrendingPage)
                            {
                                FragmentBottomNavigator.ShowBadge(2, countFriend, true);
                                CountFriendStatic = countFriend;
                            } 
                        }
                        catch (Exception e)
                        {
                            Methods.DisplayReportResultTrack(e);
                        }
                    });
                 
                if (AppSettings.MessengerIntegration)
                {
                    if (!string.IsNullOrEmpty(countMessages) && countMessages != "0" && countMessages != CountMessagesStatic)
                    {
                        RunOnUiThread(() =>
                        {
                            try
                            {
                                if (AppSettings.MoreTheme == MoreTheme.Default)
                                {
                                    var listMore = MoreTab.MoreSectionAdapter.SectionList;
                                    if (listMore?.Count > 0)
                                    {
                                        var dataTab = listMore.FirstOrDefault(a => a.Id == 2);
                                        if (dataTab != null)
                                        {
                                            CountMessagesStatic = countMessages;
                                            dataTab.BadgeCount = Convert.ToInt32(countMessages);
                                            dataTab.Badgevisibilty = true;

                                            MoreTab.MoreSectionAdapter.NotifyItemChanged(listMore.IndexOf(dataTab));
                                        }
                                    }
                                }
                                else
                                {
                                    var listMore = MoreTab.MoreSectionAdapterTheme2.SectionList;
                                    if (listMore?.Count > 0)
                                    {
                                        var dataTab = listMore.FirstOrDefault(a => a.Id == 2);
                                        if (dataTab != null)
                                        {
                                            CountMessagesStatic = countMessages;
                                            dataTab.BadgeCount = Convert.ToInt32(countMessages);
                                            dataTab.Badgevisibilty = true;
                                            MoreTab.MoreSectionAdapterTheme2.NotifyItemChanged(listMore.IndexOf(dataTab));
                                        }
                                    } 
                                } 
                            }
                            catch (Exception e)
                            {
                                Methods.DisplayReportResultTrack(e);
                            }
                        });
                    }
                    else
                    {
                        RunOnUiThread(() =>
                        {
                            try
                            {
                                if (AppSettings.MoreTheme == MoreTheme.Default)
                                {
                                    var listMore = MoreTab.MoreSectionAdapter?.SectionList;
                                    if (listMore?.Count > 0)
                                    {
                                        var dataTab = listMore.FirstOrDefault(a => a.Id == 2);
                                        if (dataTab != null)
                                        {
                                            CountMessagesStatic = "0";
                                            dataTab.BadgeCount = 0;
                                            dataTab.Badgevisibilty = false;
                                            dataTab.IconColor = Color.ParseColor("#03a9f4");

                                            MoreTab.MoreSectionAdapter.NotifyItemChanged(listMore.IndexOf(dataTab));
                                        }
                                    }
                                }
                                else
                                {
                                    var listMore = MoreTab.MoreSectionAdapterTheme2.SectionList;
                                    if (listMore?.Count > 0)
                                    {
                                        var dataTab = listMore.FirstOrDefault(a => a.Id == 2);
                                        if (dataTab != null)
                                        {
                                            CountMessagesStatic = "0";
                                            dataTab.BadgeCount = 0;
                                            dataTab.Badgevisibilty = false;
                                            MoreTab.MoreSectionAdapterTheme2.NotifyItemChanged(listMore.IndexOf(dataTab));
                                        }
                                    }
                                } 
                            }
                            catch (Exception e)
                            {
                                Methods.DisplayReportResultTrack(e);
                            }
                        });
                    }
                }

                if (!string.IsNullOrEmpty(textAnnouncement) && !AddAnnouncement)
                {
                    RunOnUiThread(() =>
                    {
                        try
                        {
                            AddAnnouncement = true;
                            OpenDialogAnnouncement(textAnnouncement);
                            //var combiner = new FeedCombiner(null, NewsFeedTab.PostFeedAdapter.ListDiffer, this);
                            //combiner.SetAnnouncementAlertView(textAnnouncement, "#3c763d");
                        }
                        catch (Exception e)
                        {
                            Methods.DisplayReportResultTrack(e);
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void OpenDialogAnnouncement(string textAnnouncement)
        {
            try
            { 
                var dialog = new MaterialDialog.Builder(this).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);
                dialog.Title(GetText(Resource.String.Lbl_Announcement));
                dialog.Content(Methods.FunString.DecodeString(textAnnouncement));
                dialog.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive((materialDialog, action) =>
                {
                    try
                    {
                        action.Dispose();
                    }
                    catch (Exception e)
                    {
                        Methods.DisplayReportResultTrack(e);
                    }
                });
                dialog.AlwaysCallSingleChoiceCallback();
                dialog.Build().Show();
            }
            catch (Exception ex)
            {
                Methods.DisplayReportResultTrack(ex);
            }
        }

        private void OpenDialogGallery(string typeImage)
        {
            try
            {
                ImageType = typeImage;
                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    Methods.Path.Chack_MyFolder();

                    //Open Image 
                    var myUri = Uri.FromFile(new File(Methods.Path.FolderDiskImage, Methods.GetTimestamp(DateTime.Now) + ".jpeg"));
                    CropImage.Activity()
                        .SetInitialCropWindowPaddingRatio(0)
                        .SetAutoZoomEnabled(true)
                        .SetMaxZoom(4)
                        .SetGuidelines(CropImageView.Guidelines.On)
                        .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Crop))
                        .SetOutputUri(myUri).Start(this);
                }
                else
                {
                    if (!CropImage.IsExplicitCameraPermissionRequired(this) && CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted && CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted)
                    {
                        Methods.Path.Chack_MyFolder();

                        //Open Image 
                        var myUri = Uri.FromFile(new File(Methods.Path.FolderDiskImage, Methods.GetTimestamp(DateTime.Now) + ".jpeg"));
                        CropImage.Activity()
                            .SetInitialCropWindowPaddingRatio(0)
                            .SetAutoZoomEnabled(true)
                            .SetMaxZoom(4)
                            .SetGuidelines(CropImageView.Guidelines.On)
                            .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Crop))
                            .SetOutputUri(myUri).Start(this);
                    }
                    else
                    {
                        new PermissionsController(this).RequestPermission(108);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
         
        public void GetOneSignalNotification()
        {
            try
            {
                string userId = Intent?.GetStringExtra("UserID") ?? "Don't have type";
                if (!string.IsNullOrEmpty(userId) && userId != "Don't have type")
                {
                    NavigationTabBar.Show(1, true);
                    ViewPager.SetCurrentItem(1, true);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        } 

        public void InAppReview()
        {
            try
            {
               bool inAppReview = MainSettings.InAppReview.GetBoolean(MainSettings.PrefKeyInAppReview, false);
               if (!inAppReview && AppSettings.ShowSettingsRateApp)
               {
                   var dialog = new MaterialDialog.Builder(this).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);
                   dialog.Title(GetText(Resource.String.Lbl_RateOurApp));
                   dialog.Content(GetText(Resource.String.Lbl_RateOurAppContent));
                   dialog.PositiveText(GetText(Resource.String.Lbl_Rate)).OnPositive((materialDialog, action) =>
                   {
                       try
                       {
                           StoreReviewApp store = new StoreReviewApp();
                           store.OpenStoreReviewPage(PackageName); 
                       }
                       catch (Exception e)
                       {
                           Methods.DisplayReportResultTrack(e);
                       }
                   });
                   dialog.NegativeText(GetText(Resource.String.Lbl_Close)).OnNegative(new WoWonderTools.MyMaterialDialog()); 
                   dialog.AlwaysCallSingleChoiceCallback();
                   dialog.Build().Show();

                   MainSettings.InAppReview?.Edit()?.PutBoolean(MainSettings.PrefKeyInAppReview, true)?.Commit();
               } 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #region PickiT >> Gert path file

        public void PickiTonCompleteListener(string path)
        {
            //Dismiss dialog and return the path
            try
            {
                //  Check if it was a Drive/local/unknown provider file and display a Toast
                //if (wasDriveFile) => "Drive file was selected" 
                //else if (wasUnknownProvider)  => "File was selected from unknown provider" 
                //else => "Local file was selected"

                //  Chick if it was successful
                var check = WoWonderTools.CheckMimeTypesWithServer(path);
                if (!check)
                {
                    //this file not supported on the server , please select another file 
                    Toast.MakeText(this, GetString(Resource.String.Lbl_ErrorFileNotSupported), ToastLength.Short)?.Show();
                    return;
                }

                var type = Methods.AttachmentFiles.Check_FileExtension(path);
                if (type == "Image")
                {
                    Intent intent = new Intent(this, typeof(AddStoryActivity));
                    intent.PutExtra("Uri", path);
                    intent.PutExtra("Type", "image");
                    StartActivity(intent);
                }
                else if (type == "Video")
                {
                    Intent intent = new Intent(this, typeof(AddStoryActivity));
                    intent.PutExtra("Uri", path);
                    intent.PutExtra("Type", "video");
                    StartActivity(intent);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion
         
    }
}
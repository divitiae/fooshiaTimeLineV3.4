﻿using System;
using System.Linq;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads.DoubleClick;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using TheArtOfDev.Edmodo.Cropper;
using Newtonsoft.Json;
using WoWonder.Activities.Base;
using WoWonder.Helpers.Ads;
using WoWonder.Helpers.CacheLoaders;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Fonts;
using WoWonder.Helpers.Utils;
using WoWonderClient.Classes.Event;
using WoWonderClient.Requests;
using static WoWonder.Helpers.Controller.PopupDialogController;
using File = Java.IO.File;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Uri = Android.Net.Uri;

namespace WoWonder.Activities.Events
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class EditEventActivity : BaseActivity, View.IOnClickListener, View.IOnFocusChangeListener
    {
        #region Variables Basic

        private TextView IconStartDate, IconEndDate, IconLocation, TxtAdd;
        private EditText TxtEventName, TxtStartDate, TxtStartTime, TxtEndDate, TxtEndTime, TxtLocation, TxtDescription;
        private ImageView ImageEvent;
        private Button BtnImage;
        private EventDataObject EventData;
        private string EventPathImage = "" , EventId = "";

        private PublisherAdView PublisherAdView;
        
        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);

                Methods.App.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.CreateEvent_Layout);

                EventId = Intent?.GetStringExtra("EventId") ?? string.Empty;

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();

                Get_Data_Event();
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
                AddOrRemoveEvent(true);
                PublisherAdView?.Resume();
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
                PublisherAdView?.Pause();
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
                DestroyBasic();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Menu

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                TxtEventName = FindViewById<EditText>(Resource.Id.eventname);
                IconStartDate = FindViewById<TextView>(Resource.Id.StartIcondate);
                TxtStartDate = FindViewById<EditText>(Resource.Id.StartDateTextview);
                TxtStartTime = FindViewById<EditText>(Resource.Id.StartTimeTextview);
                IconEndDate = FindViewById<TextView>(Resource.Id.EndIcondate);
                TxtEndDate = FindViewById<EditText>(Resource.Id.EndDateTextview);
                TxtEndTime = FindViewById<EditText>(Resource.Id.EndTimeTextview);
                IconLocation = FindViewById<TextView>(Resource.Id.IconLocation);
                TxtLocation = FindViewById<EditText>(Resource.Id.LocationTextview);
                TxtDescription = FindViewById<EditText>(Resource.Id.description);

                ImageEvent = FindViewById<ImageView>(Resource.Id.EventCover);
                BtnImage = FindViewById<Button>(Resource.Id.btn_selectimage);

                TxtAdd = FindViewById<TextView>(Resource.Id.toolbar_title);
                TxtAdd.Text = GetText(Resource.String.Lbl_Save);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconStartDate, IonIconsFonts.AndroidTime);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconEndDate, IonIconsFonts.AndroidTime);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconLocation, IonIconsFonts.Location);

                Methods.SetColorEditText(TxtEventName, AppSettings.SetTabDarkTheme ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtStartDate, AppSettings.SetTabDarkTheme ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtStartTime, AppSettings.SetTabDarkTheme ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtEndDate, AppSettings.SetTabDarkTheme ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtEndTime, AppSettings.SetTabDarkTheme ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtLocation, AppSettings.SetTabDarkTheme ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtDescription, AppSettings.SetTabDarkTheme ? Color.White : Color.Black);


                TxtStartTime.SetOnClickListener(this);
                TxtEndTime.SetOnClickListener(this);
                TxtStartDate.SetOnClickListener(this);
                TxtEndDate.SetOnClickListener(this);
           
                PublisherAdView = FindViewById<PublisherAdView>(Resource.Id.multiple_ad_sizes_view); 
                AdsGoogle.InitPublisherAdView(PublisherAdView);  
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void InitToolbar()
        {
            try
            {
                var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    toolbar.Title = GetText(Resource.String.Lbl_EditEvents);
                    toolbar.SetTitleTextColor(Color.White);
                    SetSupportActionBar(toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                    
                }
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
                    TxtAdd.Click += TxtAddOnClick;
                    TxtLocation.OnFocusChangeListener = this; 
                    BtnImage.Click += BtnImageOnClick;
                }
                else
                {
                    TxtAdd.Click -= TxtAddOnClick;
                    TxtLocation.OnFocusChangeListener = null!; 
                    BtnImage.Click -= BtnImageOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
        
        private void DestroyBasic()
        {
            try
            {
                PublisherAdView?.Destroy();

                IconStartDate = null!;
                IconEndDate = null!;
                IconLocation = null!;
                TxtAdd = null!;
                TxtEventName = null!;
                TxtStartDate = null!;
                TxtStartTime = null!;
                TxtEndDate = null!;
                TxtEndTime = null!;
                TxtLocation = null!;
                TxtDescription = null!;
                ImageEvent = null!;
                BtnImage = null!;
                EventData = null!;
                EventPathImage = null!;
                EventId = null!;

                PublisherAdView = null!;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
        #endregion

        #region Events

        private void BtnImageOnClick(object sender, EventArgs e)
        {
            try
            {
                OpenDialogGallery();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void TxtLocationOnFocusChange()
        {
            try
            {
                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    //Open intent Location when the request code of result is 502
                    new IntentController(this).OpenIntentLocation();
                }
                else
                {
                    if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) == Permission.Granted && CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) == Permission.Granted)
                    {
                        //Open intent Location when the request code of result is 502
                        new IntentController(this).OpenIntentLocation();
                    }
                    else
                    {
                        new PermissionsController(this).RequestPermission(105);
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private async void TxtAddOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
                else
                {
                    if (string.IsNullOrEmpty(TxtEventName.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_enter_name), ToastLength.Short)?.Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(TxtStartDate.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_select_start_date), ToastLength.Short)?.Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(TxtEndDate.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_select_end_date), ToastLength.Short)?.Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(TxtLocation.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_select_Location), ToastLength.Short)?.Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(TxtStartTime.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_select_start_time), ToastLength.Short)?.Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(TxtEndTime.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_select_end_time), ToastLength.Short)?.Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(TxtDescription.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_enter_Description), ToastLength.Short)?.Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(EventPathImage))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_select_Image), ToastLength.Short)?.Show();
                    }
                    else
                    {
                        //Show a progress
                        AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading) + "...");

                        var (apiStatus, respond) = await RequestsAsync.Event.Edit_Event(EventId, TxtEventName.Text, TxtLocation.Text, TxtDescription.Text, TxtStartDate.Text, TxtEndDate.Text, TxtStartTime.Text, TxtEndTime.Text, EventPathImage);
                        if (apiStatus == 200)
                        {
                            if (respond is EditEventObject result)
                            {
                                AndHUD.Shared.ShowSuccess(this); 
                                Toast.MakeText(this, GetString(Resource.String.Lbl_EventSuccessfullyEdited), ToastLength.Short)?.Show();

                                Console.WriteLine(result.MessageData);
                                //Add new item to my Event list
                                var user = ListUtils.MyProfileList?.FirstOrDefault();
                                 
                                if (EventMainActivity.GetInstance()?.MyEventTab?.MAdapter.EventList != null)
                                {
                                   var data = EventMainActivity.GetInstance()?.MyEventTab?.MAdapter?.EventList?.FirstOrDefault(a =>a.Id == EventId);
                                   if (data != null)
                                   {
                                       data.Id = EventId;
                                       data.Description = TxtDescription.Text;
                                       data.Cover = EventPathImage;
                                       data.EndDate = TxtEndDate.Text;
                                       data.EndTime = TxtEndTime.Text;
                                       data.IsOwner = true;
                                       data.Location = TxtLocation.Text;
                                       data.Name = TxtEventName.Text;
                                       data.StartDate = TxtStartDate.Text;
                                       data.StartTime = TxtStartTime.Text;
                                       data.UserData = user;

                                       EventMainActivity.GetInstance()?.MyEventTab?.MAdapter?.NotifyItemChanged(EventMainActivity.GetInstance().MyEventTab.MAdapter.EventList.IndexOf(data));
                                   } 
                                }

                                if (EventMainActivity.GetInstance()?.EventTab?.MAdapter.EventList != null)
                                {
                                    var data = EventMainActivity.GetInstance()?.EventTab?.MAdapter.EventList?.FirstOrDefault(a => a.Id == EventId);
                                    if (data != null)
                                    {
                                        data.Id = EventId;
                                        data.Description = TxtDescription.Text;
                                        data.Cover = EventPathImage;
                                        data.EndDate = TxtEndDate.Text;
                                        data.EndTime = TxtEndTime.Text;
                                        data.IsOwner = true;
                                        data.Location = TxtLocation.Text;
                                        data.Name = TxtEventName.Text;
                                        data.StartDate = TxtStartDate.Text;
                                        data.StartTime = TxtStartTime.Text;
                                        data.UserData = user;

                                        EventMainActivity.GetInstance()?.EventTab?.MAdapter?.NotifyItemChanged(EventMainActivity.GetInstance().EventTab.MAdapter.EventList.IndexOf(data));
                                    }
                                }
                                 
                                
                                

                                Finish();
                            }
                        }
                        else 
                        {
                            Methods.DisplayAndHudErrorResult(this, respond);
                        } 
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                AndHUD.Shared.Dismiss(this);
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
                if (requestCode == CropImage.CropImageActivityRequestCode)
                {
                    var result = CropImage.GetActivityResult(data);

                    if (resultCode == Result.Ok)
                    {
                        if (result.IsSuccessful)
                        {
                            var resultUri = result.Uri;

                            if (!string.IsNullOrEmpty(resultUri.Path))
                            {
                                EventPathImage = resultUri.Path;
                                File file2 = new File(resultUri.Path);
                                var photoUri = FileProvider.GetUriForFile(this, PackageName + ".fileprovider", file2);
                                Glide.With(this).Load(photoUri).Apply(new RequestOptions()).Into(ImageEvent);


                                //GlideImageLoader.LoadImage(this, resultUri.Path, ImageEvent, ImageStyle.RoundedCrop, ImagePlaceholders.Drawable);
                            }
                            else
                            {
                                Toast.MakeText(this, GetText(Resource.String.Lbl_something_went_wrong), ToastLength.Long)?.Show();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_something_went_wrong), ToastLength.Long)?.Show();
                        }
                    } 
                }
                else if (requestCode == 502 && resultCode == Result.Ok) // Location
                {
                    var placeAddress = data.GetStringExtra("Address") ?? "";
                    //var placeLatLng = data.GetStringExtra("latLng") ?? "";
                    if (!string.IsNullOrEmpty(placeAddress))
                        TxtLocation.Text = placeAddress;
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
                        OpenDialogGallery();
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long)?.Show();
                    }
                }
                else if (requestCode == 105)
                {
                    //Open intent Location when the request code of result is 502
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                        new IntentController(this).OpenIntentLocation();
                    else
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long)?.Show();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        public void OnClick(View v)
        {
            try
            {
                if (v.Id == TxtStartTime.Id)
                {
                    var frag = TimePickerFragment.NewInstance(delegate (DateTime time)
                    {
                        TxtStartTime.Text = time.ToShortTimeString();
                    });

                    frag.Show(SupportFragmentManager, TimePickerFragment.Tag);
                }
                else if (v.Id == TxtEndTime.Id)
                {
                    var frag = TimePickerFragment.NewInstance(delegate (DateTime time)
                    {
                        TxtEndTime.Text = time.ToShortTimeString();
                    });

                    frag.Show(SupportFragmentManager, TimePickerFragment.Tag);
                }
                else if (v.Id == TxtStartDate.Id)
                {
                    var frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                    {
                        TxtStartDate.Text = time.ToShortDateString();
                    });
                    frag.Show(SupportFragmentManager, DatePickerFragment.Tag);
                }
                else if (v.Id == TxtEndDate.Id)
                {
                    var frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                    {
                        TxtEndDate.Text = time.ToShortDateString();
                    });
                    frag.Show(SupportFragmentManager, DatePickerFragment.Tag);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void OpenDialogGallery()
        {
            try
            {
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
         
        private void Get_Data_Event()
        {
            try
            {
                EventData = JsonConvert.DeserializeObject<EventDataObject>(Intent?.GetStringExtra("EventData"));
                if (EventData != null)
                {
                    TxtEventName.Text = Methods.FunString.DecodeString(EventData.Name);
                    TxtStartDate.Text = EventData.StartDate;
                    TxtStartTime.Text = EventData.StartTime;
                    TxtEndDate.Text = EventData.EndDate;
                    TxtEndTime.Text = EventData.EndTime;
                    TxtLocation.Text = EventData.Location;
                    TxtDescription.Text = Methods.FunString.DecodeString(EventData.Description);
                     
                    EventPathImage = EventData.Cover;
                    GlideImageLoader.LoadImage(this, EventData.Cover, ImageEvent, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
         
        public void OnFocusChange(View v, bool hasFocus)
        {
            if (v?.Id == TxtLocation.Id && hasFocus)
            {
                TxtLocationOnFocusChange();
            }
        }
    }
}
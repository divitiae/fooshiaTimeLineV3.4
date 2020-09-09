//##############################################
//Cᴏᴘʏʀɪɢʜᴛ 2020 DᴏᴜɢʜᴏᴜᴢLɪɢʜᴛ Codecanyon Item 19703216
//Elin Doughouz >> https://www.facebook.com/Elindoughouz
//====================================================

//For the accuracy of the icon and logo, please use this website " http://nsimage.brosteins.com " and add images according to size in folders " mipmap " 

using WoWonder.Helpers.Model;

namespace WoWonder
{
    public static class AppSettings
    {
        public static string TripleDesAppServiceProvider = "iwjLdtDnEKUcMV+dROrgjJPQxvkqB+c4diAt5A8BKXWtsAheg8ptxT55oX6mEwg9rLNVGPh3fhjzJOT4vAoQY2LyLN7BkJL76XnJ33Ot7K/SrFwiTTCEc3dlu4PjSvvjtecfQZ3gSVjZW4Swya0DcW5LNU+iB6h6dwnMDkjgzcIVyfkYEVRGTYKa+juacyF1CapJ8i6zOj1ld4WKJBsdpdKH2mXgOh4c0/lJZ3A+VgE6ndyGxgkJMh+NEYm0WiLXsd/LHPor9LU6sLYSFPjbVXUAaCMmpuN8MXN5DkN2b8R8DKQaatE34/dyxk4eoQbS";
       
        //Main Settings >>>>>
        //*********************************************************
        public static string Version = "3.4";
        public static string ApplicationName = "Fooshia";
        public static string YoutubeKey = "AIzaSyAfcrBeoZ7Mb_TE0eFLpyFi24Ivqv1Z4ag";

        // Friend system = 0 , follow system = 1
        public static int ConnectivitySystem = 0;

        public static PostButtonSystem PostButton = PostButtonSystem.ReactionDefault;
        public static bool ShowTextShareButton = true;
        public static bool ShowShareButton = true; 

        //Main Colors >>
        //*********************************************************
        public static string MainColor = "#db67db";
        public static string StoryReadColor = "#808080";

        //Language Settings >> http://www.lingoes.net/en/translator/langcode.htm
        //*********************************************************
        public static bool FlowDirectionRightToLeft = false;
        public static string Lang = ""; //Default language ar_AE

        //Set Language User on site from phone 
        public static bool SetLangUser = true;  //#New

        //Notification Settings >>
        //*********************************************************
        public static bool ShowNotification = true;
        public static string OneSignalAppId = "862030dd-e2ab-4614-966b-2385616606ab";

        // WalkThrough Settings >>
        //*********************************************************
        public static bool ShowWalkTroutPage = false;
        public static bool WalkThroughSetFlowAnimation = true;
        public static bool WalkThroughSetZoomAnimation = false;
        public static bool WalkThroughSetSlideOverAnimation = false;
        public static bool WalkThroughSetDepthAnimation = false;
        public static bool WalkThroughSetFadeAnimation = false;

        //Main Messenger settings
        //*********************************************************
        public static bool MessengerIntegration = true;
        public static string MessengerPackageName = "com.divitiae.fooshiamessenger"; //APK name on Google Play

        //AdMob >> Please add the code ad in the Here and analytic.xml 
        //*********************************************************
        public static bool ShowAdMobBanner = true;
        public static bool ShowAdMobInterstitial = true;
        public static bool ShowAdMobRewardVideo = true;
        public static bool ShowAdMobNative = true;
        public static bool ShowAdMobNativePost = true; 

        public static string AdInterstitialKey = "ca-app-pub-9227865829432768/8754224930";
        public static string AdRewardVideoKey = "ca-app-pub-9227865829432768/5628549903";
        public static string AdAdMobNativeKey = "ca-app-pub-9227865829432768/1642021676";

        //Three times after entering the ad is displayed
        public static int ShowAdMobInterstitialCount = 3;
        public static int ShowAdMobRewardedVideoCount = 3;
        public static int ShowAdMobNativeCount = 40;

        //FaceBook Ads >> Please add the code ad in the Here and analytic.xml 
        //*********************************************************
        public static bool ShowFbBannerAds = false; 
        public static bool ShowFbInterstitialAds = false;  
        public static bool ShowFbRewardVideoAds = false; 
        public static bool ShowFbNativeAds = false; 
         
        //YOUR_PLACEMENT_ID
        public static string AdsFbBannerKey = "250485588986218_554026418632132"; 
        public static string AdsFbInterstitialKey = "250485588986218_554026125298828";  
        public static string AdsFbRewardVideoKey = "250485588986218_554072818627492"; 
        public static string AdsFbNativeKey = "250485588986218_554706301897477"; 

        //Three times after entering the ad is displayed
        public static int ShowFbNativeAdsCount = 40;  
         
        //********************************************************* 
        public static bool EnableRegisterSystem = true;  
        public static bool ShowGenderOnRegister = true; 

        //Set Theme Welcome Pages 
        //*********************************************************
        //Types >> Gradient or Video or Image
					  
																	   
																			
																			 
																		  
					  
        public static string BackgroundScreenWelcomeType = "Image";

        //Set Theme Full Screen App
        //*********************************************************
        public static bool EnableFullScreenApp = false;

        //Code Time Zone (true => Get from Internet , false => Get From #CodeTimeZone )
        //*********************************************************
        public static bool AutoCodeTimeZone = true;
        public static string CodeTimeZone = "UTC";

        //Error Report Mode
        //*********************************************************
        public static bool SetApisReportMode = false;

        //Social Logins >>
        //If you want login with facebook or google you should change id key in the analytic.xml file 
        //Facebook >> ../values/analytic.xml .. line 10-11 
        //Google >> ../values/analytic.xml .. line 15 
        //*********************************************************
        public static bool ShowFacebookLogin = true;
        public static bool ShowGoogleLogin = true;

        public static readonly string ClientId = "564781379986-kc4rs9l5484nv9ev4uutdiibgfi5tlv3.apps.googleusercontent.com";

        //########################### 
         
        public static bool ShowTrendingPage = true;  //#New
          
        //Main Slider settings
        //*********************************************************
        public static bool ShowAlbum = true;
        public static bool ShowArticles = true;
        public static bool ShowPokes = true;
        public static bool ShowCommunitiesGroups = true;
        public static bool ShowCommunitiesPages = true;
        public static bool ShowMarket = true;
        public static bool ShowPopularPosts = true;
        public static bool ShowMovies = true;
        public static bool ShowNearBy = true;
        public static bool ShowStory = true;
        public static bool ShowSavedPost = true;
        public static bool ShowUserContacts = true; 
        public static bool ShowJobs = true; 
        public static bool ShowCommonThings = true; 
        public static bool ShowFundings = true;
        public static bool ShowMyPhoto = true; 
        public static bool ShowMyVideo = true; 
        public static bool ShowGames = true;
        public static bool ShowMemories = true;  
        public static bool ShowOffers = true;  
        public static bool ShowNearbyShops = true;   

        public static bool ShowSuggestedGroup = true;
        public static bool ShowSuggestedUser = true;  
         
        //count times after entering the Suggestion is displayed
        public static int ShowSuggestedGroupCount = 70; 
        public static int ShowSuggestedUserCount = 50;

        //allow download or not when share
        public static bool AllowDownloadMedia = true; //New

        //Events settings
        //*********************************************************
        public static bool ShowEvents = true; 
        public static bool ShowEventGoing = true; 
        public static bool ShowEventInvited = true;  
        public static bool ShowEventInterested = true;  
        public static bool ShowEventPast = true; 

        //Set a story duration >> 7 Sec
        public static long StoryDuration = 7000L;
        //*********************************************************
        /// <summary>
        ///  Currency
        /// CurrencyStatic = true : get currency from app not api 
        /// CurrencyStatic = false : get currency from api (default)
        /// </summary>
        public static readonly bool CurrencyStatic = false;
        public static readonly string CurrencyIconStatic = "$";
        public static readonly string CurrencyCodeStatic = "USD";
        public static readonly string CurrencyFundingPriceStatic = "$";

        //Profile settings
        //*********************************************************
        public static bool ShowGift = true;
        public static bool ShowWallet = true; 
        public static bool ShowGoPro = true;  
        public static bool ShowWithdrawals = true;

        public static string WeeklyPrice = "3"; //#New
        public static string MonthlyPrice = "8"; //#New
        public static string YearlyPrice = "89"; //#New
        public static string LifetimePrice = "259"; //#New

        //Native Post settings
        //*********************************************************
        public static int AvatarPostSize = 60;
        public static int ImagePostSize = 200;
        public static string PostApiLimitOnScroll = "12";

        //Get post in background >> 1 Min = 60 Sec
        public static long RefreshPostSeconds = 60000; //#New 
        public static string PostApiLimitOnBackground = "12"; //#New

        public static bool AutoPlayVideo = true;
         
        public static bool EmbedPlayTubePostType = true;
        public static bool EmbedDeepSoundPostType = true;
        public static VideoPostTypeSystem EmbedFacebookVideoPostType = VideoPostTypeSystem.Link; //#New
        public static VideoPostTypeSystem EmbedVimeoVideoPostType = VideoPostTypeSystem.Link; //#New
        public static VideoPostTypeSystem EmbedPlayTubeVideoPostType = VideoPostTypeSystem.Link; //#New
        public static bool ShowSearchForPosts = true; 
        public static bool EmbedLivePostType = true;
         
        //new posts users have to scroll back to top
        public static bool ShowNewPostOnNewsFeed = true; 
        public static bool ShowAddPostOnNewsFeed = false; 
        public static bool ShowCountSharePost = true; 

        /// <summary>
        /// Post Privacy
        /// ShowPostPrivacyForAllUser = true : all posts user have icon Privacy 
        /// ShowPostPrivacyForAllUser = false : just my posts have icon Privacy (default)
        /// </summary>
        public static bool ShowPostPrivacyForAllUser = false; 

        public static bool ShowFullScreenVideoPost = true;
         
        //UsersPages
        public static bool ShowProUsersMembers = true;
        public static bool ShowPromotedPages = true;
        public static bool ShowTrendingHashTags = true;
        public static bool ShowLastActivities = true;

        public static bool ShowUserPoint = true;

        //Add Post
        public static bool ShowGalleryImage = true;
        public static bool ShowGalleryVideo = true;
        public static bool ShowMention = true;
        public static bool ShowLocation = true;
        public static bool ShowFeelingActivity = true;
        public static bool ShowFeeling = true;
        public static bool ShowListening = true;
        public static bool ShowPlaying = true;
        public static bool ShowWatching = true;
        public static bool ShowTraveling = true;
        public static bool ShowCamera = true;
        public static bool ShowGif = true;
        public static bool ShowFile = true;
        public static bool ShowMusic = true;
        public static bool ShowPolls = true;
        public static bool ShowColor = true;

        //Boost 
        public static bool ShowAdvertisingPost = true;  //#New

        //Settings Page >> General Account
        public static bool ShowSettingsGeneralAccount = true;
        public static bool ShowSettingsAccount = true;
        public static bool ShowSettingsSocialLinks = true;
        public static bool ShowSettingsPassword = true;
        public static bool ShowSettingsBlockedUsers = true;
        public static bool ShowSettingsDeleteAccount = true;
        public static bool ShowSettingsTwoFactor = true; 
        public static bool ShowSettingsManageSessions = true;  
        public static bool ShowSettingsVerification = true; 

        //Settings Page >> Privacy
        public static bool ShowSettingsPrivacy = true;
        public static bool ShowSettingsNotification = true;

        //Settings Page >> Tell a Friends (Earnings)
        public static bool ShowSettingsInviteFriends = true;

        public static bool ShowSettingsShare = true;
        public static bool ShowSettingsMyAffiliates = true;
         
        /// <summary>
        /// if you want this feature enabled go to Properties -> AndroidManefist.xml and remove comments from below code
        /// Just replace it with this 5 lines of code
        /// <uses-permission android:name="android.permission.READ_CONTACTS" />
        /// <uses-permission android:name="android.permission.READ_PHONE_NUMBERS" />
        /// <uses-permission android:name="android.permission.SEND_SMS" />
        /// </summary>
        public static bool InvitationSystem = false;

        public static int LimitGoProPlansCountsTo = 4;

        //Settings Page >> Help && Support
        public static bool ShowSettingsHelpSupport = true;

        public static bool ShowSettingsHelp = true;
        public static bool ShowSettingsReportProblem = true;
        public static bool ShowSettingsAbout = true;
        public static bool ShowSettingsPrivacyPolicy = true;
        public static bool ShowSettingsTermsOfUse = true;
        public static bool ShowSettingsRateApp = true; //#New

        public static bool ShowSettingsInvitationLinks = true; 
        public static bool ShowSettingsMyInformation = true; 

        public static bool ShowSuggestedUsersOnRegister = true;

        public static bool ImageCropping = true;

        //Set Theme Tab
        //*********************************************************
        public static bool SetTabDarkTheme = false;
        public static MoreTheme MoreTheme = MoreTheme.BeautyTheme; //#New
        public static bool SetTabOnButton = true;

        //Bypass Web Errors  
        //*********************************************************
        public static bool TurnTrustFailureOnWebException = true;
        public static bool TurnSecurityProtocolType3072On = true;

        //*********************************************************
        public static bool RenderPriorityFastPostLoad = false;

        /// <summary>
        /// if you want this feature enabled go to Properties -> AndroidManefist.xml and remove comments from below code
        /// <uses-permission android:name="com.android.vending.BILLING" />
        /// </summary>
        public static bool ShowInAppBilling = false; 

        public static bool ShowPaypal = true; 
        public static bool ShowBankTransfer = true; 
        public static bool ShowCreditCard = true;

        //********************************************************* 
        public static bool ShowCashFree = false;  //#New 

        /// <summary>
        /// Currencies : http://prntscr.com/u600ok
        /// </summary>
        public static string CashFreeCurrency = "INR";  //#New

        /// <summary>
        /// SandBox , Live
        /// </summary>
        public static string CashfreeMode = "SandBox"; 
        //********************************************************* 

					 
																				  
										   
					  
        public static bool ShowRazorPay = false;  //#New  

        /// <summary>
        /// Currencies : https://razorpay.com/accept-international-payments
        /// </summary>
        public static string RazorPayCurrency = "USD";  //#New
         
        public static bool ShowPayStack = true;  //#New
        public static bool ShowPaySera = false;  //#Next Version 
        //********************************************************* 
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Widget;
using Newtonsoft.Json;
using WoWonder.Activities.MyProfile;
using WoWonder.Activities.NativePost.Post;
using WoWonder.Activities.UserProfile;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Model;
using WoWonder.SQLite;
using WoWonderClient;
using WoWonderClient.Classes.Global;
using WoWonderClient.Classes.Group;
using WoWonderClient.Classes.Jobs;
using WoWonderClient.Requests;
using Exception = System.Exception;
using Path = System.IO.Path;

namespace WoWonder.Helpers.Utils
{
    public static class WoWonderTools  
    {
        public static string GetNameFinal(UserDataObject dataUser)
        {
            try
            {
                if (!string.IsNullOrEmpty(dataUser.Name) && !string.IsNullOrWhiteSpace(dataUser.Name))
                    return Methods.FunString.DecodeString(dataUser.Name);

                if (!string.IsNullOrEmpty(dataUser.Username) && !string.IsNullOrWhiteSpace(dataUser.Username))
                    return Methods.FunString.DecodeString(dataUser.Username);

                return Methods.FunString.DecodeString(dataUser.Username);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return Methods.FunString.DecodeString(dataUser.Username);
            }
        }

        public static string GetAboutFinal(UserDataObject dataUser)
        {
            try
            {
                if (!string.IsNullOrEmpty(dataUser.About) && !string.IsNullOrWhiteSpace(dataUser.About))
                    return Methods.FunString.DecodeString(dataUser.About);

                return Application.Context.Resources?.GetString(Resource.String.Lbl_DefaultAbout) + " " + AppSettings.ApplicationName;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return Application.Context.Resources?.GetString(Resource.String.Lbl_DefaultAbout) + " " + AppSettings.ApplicationName;
            }
        }

        public static (string, string) GetCurrency(string idCurrency)
        {
            try
            {
                if (AppSettings.CurrencyStatic) return (AppSettings.CurrencyCodeStatic, AppSettings.CurrencyIconStatic);

                string currency;
                bool success = int.TryParse(idCurrency, out var number);
                if (success)
                {
                    Console.WriteLine("Converted '{0}' to {1}.", idCurrency, number);
                    currency = ListUtils.SettingsSiteList?.CurrencyArray.CurrencyList[number] ?? "USD";
                }
                else
                {
                    Console.WriteLine("Attempted conversion of '{0}' failed.", idCurrency ?? "<null>");
                    currency = idCurrency;
                }

                if (ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList != null)
                {
                    string currencyIcon = currency?.ToUpper() switch
                    {
                        "USD" => !string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Usd)
                            ? ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Usd ?? "$"
                            : "$",
                        "Jpy" => !string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Jpy)
                            ? ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Jpy ?? "¥"
                            : "¥",
                        "EUR" => !string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Eur)
                            ? ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Eur ?? "€"
                            : "€",
                        "TRY" => !string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Try)
                            ? ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Try ?? "₺"
                            : "₺",
                        "GBP" => !string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Gbp)
                            ? ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Gbp ?? "£"
                            : "£",
                        "RUB" => !string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Rub)
                            ? ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Rub ?? "₽"
                            : "₽",
                        "PLN" => !string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Pln)
                            ? ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Pln ?? "zł"
                            : "zł",
                        "ILS" => !string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Ils)
                            ? ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Ils ?? "₪"
                            : "₪",
                        "BRL" => !string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Brl)
                            ? ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Brl ?? "R$"
                            : "R$",
                        "INR" => !string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Inr)
                            ? ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Inr ?? "₹"
                            : "₹",
                        _ => !string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Usd)
                            ? ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Usd ?? "$"
                            : "$"
                    };

                    return (currency, currencyIcon);
                }

                return (AppSettings.CurrencyCodeStatic, AppSettings.CurrencyIconStatic);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return (AppSettings.CurrencyCodeStatic, AppSettings.CurrencyIconStatic);
            }
        }

        public static List<string> GetCurrencySymbolList()
        {
            try
            {
                var arrayAdapter = new List<string>();

                if (AppSettings.CurrencyStatic)
                {
                    arrayAdapter.Add(AppSettings.CurrencyIconStatic);
                    return arrayAdapter;

                }

                if (ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList != null)
                {
                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Usd))
                        arrayAdapter.Add(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Usd ?? "$");

                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Jpy))
                        arrayAdapter.Add(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Jpy ?? "¥");

                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Eur))
                        arrayAdapter.Add(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Eur ?? "€");

                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Try))
                        arrayAdapter.Add(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Try ?? "₺");

                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Gbp))
                        arrayAdapter.Add(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Gbp ?? "£");

                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Rub))
                        arrayAdapter.Add(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Rub ?? "₽");

                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Pln))
                        arrayAdapter.Add(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Pln ?? "zł");

                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Ils))
                        arrayAdapter.Add(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Ils ?? "₪");

                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Brl))
                        arrayAdapter.Add(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Brl ?? "R$");

                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Inr))
                        arrayAdapter.Add(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Inr ?? "₹");
                }

                return arrayAdapter;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return new List<string>(); 
            }
        }

        public static void OpenProfile(Activity activity, string userId, UserDataObject item , string namePage)
        {
            try
            {
                if (userId != UserDetails.UserId)
                {
                    var intent = new Intent(activity, typeof(UserProfileActivity));
                    if (item != null) intent.PutExtra("UserObject", JsonConvert.SerializeObject(item));
                    intent.PutExtra("UserId", userId);
                    intent.PutExtra("NamePage", namePage);
                    activity.StartActivity(intent);
                }
                else
                {
                    var intent = new Intent(activity, typeof(MyProfileActivity));
                    activity.StartActivity(intent);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public static void OpenProfile(Activity activity, string userId, UserDataObject item)
        {
            try
            {
                if (userId != UserDetails.UserId)
                {
                    try
                    {
                        if (UserProfileActivity.SUserId != userId)
                        {
                            MainApplication.GetInstance()?.NavigateTo(activity, typeof(UserProfileActivity), item); 
                        } 
                    }
                    catch (Exception e)
                    {
                        Methods.DisplayReportResultTrack(e);
                        var intent = new Intent(activity, typeof(UserProfileActivity));
                        intent.PutExtra("UserObject", JsonConvert.SerializeObject(item));
                        intent.PutExtra("UserId", item.UserId);
                        activity.StartActivity(intent);
                    }
                }
                else
                {
                    if (PostClickListener.OpenMyProfile) return;
                    var intent = new Intent(activity, typeof(MyProfileActivity));
                    activity.StartActivity(intent);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public static bool GetStatusOnline(int lastSeen, string isShowOnline)
        {
            try
            {
                string time = Methods.Time.TimeAgo(lastSeen, false);
                bool status = isShowOnline == "on" && time == Methods.Time.LblJustNow;
                return status;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return false;
            }
        }

        public static JobInfoObject ListFilterJobs(JobInfoObject jobInfoObject)   
        {
            try
            {
                if (!jobInfoObject.Image.Contains(Client.WebsiteUrl))
                    jobInfoObject.Image = Client.WebsiteUrl + "/" + jobInfoObject.Image;

                jobInfoObject.IsOwner = jobInfoObject.UserId == UserDetails.UserId;

                return jobInfoObject;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return jobInfoObject;
            }
        }

        public static Dictionary<string, string> GetSalaryDateList(Activity activity)
        {
            try
            {
                var arrayAdapter = new Dictionary<string, string>
                {
                    {"per_hour", activity.GetString(Resource.String.Lbl_per_hour)},
                    {"per_day", activity.GetString(Resource.String.Lbl_per_day)},
                    {"per_week", activity.GetString(Resource.String.Lbl_per_week)},
                    {"per_month", activity.GetString(Resource.String.Lbl_per_month)},
                    {"per_year", activity.GetString(Resource.String.Lbl_per_year)}
                };
                 
                return arrayAdapter;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return new Dictionary<string, string>();
            }
        }
         
        public static Dictionary<string, string> GetJobTypeList(Activity activity)
        {
            try
            { 
                var arrayAdapter = new Dictionary<string, string>
                {
                    {"full_time", activity.GetString(Resource.String.Lbl_full_time)},
                    {"part_time", activity.GetString(Resource.String.Lbl_part_time)},
                    {"internship", activity.GetString(Resource.String.Lbl_internship)},
                    {"volunteer", activity.GetString(Resource.String.Lbl_volunteer)},
                    {"contract", activity.GetString(Resource.String.Lbl_contract)}
                };
                 
                return arrayAdapter;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return new Dictionary<string, string>();
            }
        }
         

        public static Dictionary<string, string> GetAddQuestionList(Activity activity)
        {
            try
            { 
                var arrayAdapter = new Dictionary<string, string>
                {
                    {"free_text_question", activity.GetString(Resource.String.Lbl_FreeTextQuestion)},
                    {"yes_no_question", activity.GetString(Resource.String.Lbl_YesNoQuestion)},
                    {"multiple_choice_question", activity.GetString(Resource.String.Lbl_MultipleChoiceQuestion)},
                };
                 
                return arrayAdapter;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return new Dictionary<string, string>();
            }
        }
        
        public static Dictionary<string, string> GetAddDiscountList(Activity activity)
        {
            try
            { 
                var arrayAdapter = new Dictionary<string, string>
                {
                    {"discount_percent", activity.GetString(Resource.String.Lbl_DiscountPercent)},
                    {"discount_amount", activity.GetString(Resource.String.Lbl_DiscountAmount)},
                    {"buy_get_discount", activity.GetString(Resource.String.Lbl_BuyGetDiscount)},
                    {"spend_get_off", activity.GetString(Resource.String.Lbl_SpendGetOff)},
                    {"free_shipping", activity.GetString(Resource.String.Lbl_FreeShipping)},
                };
                 
                return arrayAdapter;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return new Dictionary<string, string>();
            }
        }
         
        public static bool CheckAllowedFileSharingInServer(string type)
        {
            try
            {
                if (type == "File")
                {
                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.FileSharing) && ListUtils.SettingsSiteList?.FileSharing == "1")
                    {
                        // Allowed
                        return true;
                    }
                }
                else if (type == "Video")
                {
                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.VideoUpload) && ListUtils.SettingsSiteList?.VideoUpload == "1")
                    {
                        // Allowed
                        return true;
                    }
                }
                else if (type == "Audio")
                {
                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.AudioUpload) && ListUtils.SettingsSiteList?.AudioUpload == "1")
                    {
                        // Allowed
                        return true;
                    }
                }
                else if (type == "Image")
                {
                    // Allowed
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return false;
            }
        }

        public static bool CheckMimeTypesWithServer(string path)
        {
            try
            {
                var allowedExtenstionStatic = "jpg,png,jpeg,gif,mp4,m4v,webm,flv,mov,mpeg,mp3,wav";
                var fileName = path.Split('/').Last();
                var fileNameWithExtension = fileName.Split('.').Last();

                if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.MimeTypes))
                {
                    var allowedExtenstion = ListUtils.SettingsSiteList?.AllowedExtenstion; //jpg,png,jpeg,gif,mkv,docx,zip,rar,pdf,doc,mp3,mp4,flv,wav,txt,mov,avi,webm,wav,mpeg
                    var mimeTypes = ListUtils.SettingsSiteList?.MimeTypes; //video/mp4,video/mov,video/mpeg,video/flv,video/avi,video/webm,audio/wav,audio/mpeg,video/quicktime,audio/mp3,image/png,image/jpeg,image/gif,application/pdf,application/msword,application/zip,application/x-rar-compressed,text/pdf,application/x-pointplus,text/css

                    var getMimeType = MimeTypeMap.GetMimeType(fileNameWithExtension);

                    if (allowedExtenstion.Contains(fileNameWithExtension) && mimeTypes.Contains(getMimeType))
                    {
                        var type = Methods.AttachmentFiles.Check_FileExtension(path);

                        var check = CheckAllowedFileSharingInServer(type);
                        if (check)  // Allowed
                            return true;
                    }
                }

                //just this Allowed : >> jpg,png,jpeg,gif,mp4,m4v,webm,flv,mov,mpeg,mp3,wav
                if (allowedExtenstionStatic.Contains(fileNameWithExtension))
                    return true;

                return false;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return false;
            }
        }


        // Functions Save Images
        private static void SaveFile(string id, string folder, string fileName, string url)
        {
            try
            {
                if (url.Contains("http"))
                {
                    string folderDestination = folder + id + "/";

                    string filePath = Path.Combine(folderDestination);
                    string mediaFile = filePath + "/" + fileName;

                    if (File.Exists(mediaFile)) return;
                    WebClient webClient = new WebClient();

                    webClient.DownloadDataAsync(new Uri(url));

                    webClient.DownloadDataCompleted += (s, e) =>
                    {
                        try
                        {
                            File.WriteAllBytes(mediaFile, e.Result);
                        }
                        catch (Exception exception)
                        {
                            Methods.DisplayReportResultTrack(exception);
                        }
                    };
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        // Functions file from folder
        public static string GetFile(string id, string folder, string filename, string url)
        {
            try
            {
                string folderDestination = folder + id + "/";

                if (!Directory.Exists(folderDestination))
                {
                    if (folder == Methods.Path.FolderDiskStory)
                        Directory.Delete(folder, true);

                    Directory.CreateDirectory(folderDestination);
                }

                string imageFile = Methods.MultiMedia.GetMediaFrom_Gallery(folderDestination, filename);
                if (imageFile == "File Dont Exists")
                {
                    //This code runs on a new thread, control is returned to the caller on the UI thread.
                    Task.Factory.StartNew(() => { SaveFile(id, folder, filename, url); });
                    return url;
                }

                return imageFile;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return url;
            }
        }

        public static string GetDuration(string mediaFile)
        {
            try
            {
                string duration;
                MediaMetadataRetriever retriever;
                if (mediaFile.Contains("http"))
                {
                    retriever = new MediaMetadataRetriever();
                    if ((int)Build.VERSION.SdkInt >= 14)
                        retriever.SetDataSource(mediaFile, new Dictionary<string, string>());
                    else
                        retriever.SetDataSource(mediaFile);

                    duration = retriever.ExtractMetadata(MetadataKey.Duration); //time In Millisec 
                    retriever.Release();
                }
                else
                {
                    var file = Android.Net.Uri.FromFile(new Java.IO.File(mediaFile));
                    retriever = new MediaMetadataRetriever();
                    //if ((int)Build.VERSION.SdkInt >= 14)
                    //    retriever.SetDataSource(file.Path, new Dictionary<string, string>());
                    //else
                    retriever.SetDataSource(file?.Path);

                    duration = retriever.ExtractMetadata(MetadataKey.Duration); //time In Millisec 
                    retriever.Release();
                }

                return duration;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "0";
            }
        }

        private static readonly string[] RelationshipLocal = Application.Context.Resources?.GetStringArray(Resource.Array.RelationShipArray);
        public static string GetRelationship(int index)
        {
            try
            { 
                if (index > -1)
                {
                    string name = RelationshipLocal[index];
                    return name;
                }
                return RelationshipLocal?.First() ?? "";
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "";
            }
        }

        public static bool IsValidPassword(string password)
        {
            try
            {
                bool flag;
                if (password.Length >= 8 && password.Any(char.IsUpper) && password.Any(char.IsLower) && password.Any(char.IsNumber) && password.Any(char.IsSymbol))
                {
                    Console.WriteLine("valid");
                    flag = true;
                }
                else
                {
                    Console.WriteLine("invalid");
                    flag = false; 
                }

                return flag;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return false;
            }
        }
          
        public static bool IsLikedPage(PageClass pageData)
        {
            try
            {
                if (!string.IsNullOrEmpty(pageData?.IsLiked.String))
                {
                    return pageData.IsLiked.String.ToLower() switch
                    {
                        //no
                        "no" => false,
                        "yes" => true, 
                    };
                }
                 
                return pageData?.IsLiked.Bool != null && pageData.IsLiked.Bool.Value;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return false;
            }
        }
        
        public static bool IsJoinedGroup(GroupClass pageData)
        {
            try
            {
                if (!string.IsNullOrEmpty(pageData?.IsJoined.String))
                {
                    return pageData.IsJoined.String.ToLower() switch
                    {
                        //no
                        "no" => false,
                        "yes" => true, 
                    };
                }

                return pageData?.IsJoined.Bool != null && pageData.IsJoined.Bool.Value; 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return false;
            }
        }
         
        /// <summary>
        /// ['amazone_s3'] == 1   >> $wo['config']['s3_site_url'] . '/' . $media;
        /// ['spaces'] == 1       >> 'https://' . $wo['config']['space_name'] . '.' . $wo['config']['space_region'] . '.digitaloceanspaces.com/' . $media;
        /// ['ftp_upload'] == 1   >> "http://".$wo['config']['ftp_endpoint'] . '/' . $media;
        /// ['cloud_upload'] == 1 >> "'https://storage.cloud.google.com/'. $wo['config']['cloud_bucket_name'] . '/' . $media;
        /// </summary>
        /// <param name="media"></param>
        /// <returns></returns>
        public static string GetTheFinalLink(string media)
        {
            try
            {
                var path = media; 
                var config = ListUtils.SettingsSiteList;
                if (!string.IsNullOrEmpty(config?.AmazoneS3) && config?.AmazoneS3 == "1")
                { 
                    path = config.S3SiteUrl + "/" + media;
                    return path;
                }

                if (!string.IsNullOrEmpty(config?.Spaces) && config?.Spaces == "1")
                {
                    path = "https://" + config.SpaceName + "." + config.SpaceRegion + ".digitaloceanspaces.com/" + media;
                    return path;
                }
               
                if (!string.IsNullOrEmpty(config?.FtpUpload) && config?.FtpUpload == "1")
                {
                    path = "http://" + config.FtpEndpoint + "/" + media;
                    return path;
                }
                
                if (!string.IsNullOrEmpty(config?.CloudUpload) && config?.CloudUpload == "1")
                {
                    path = "https://storage.cloud.google.com/" + config.BucketName + "/" + media;
                    return path;
                }

                if (!media.Contains(Client.WebsiteUrl))
                    path = Client.WebsiteUrl + "/" + media;
            
                return path;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return media;
            }
        }
         
        public static void SetAddFriend(Activity activity ,UserDataObject item , Button btnAddUser)
        {
            try
            {
                if (item.IsFollowing == null)
                    item.IsFollowing = "0";

                var dbDatabase = new SqLiteDatabase();
                string isFollowing;
                switch (item.IsFollowing)
                {
                    case "0": // Add Or request friends
                    case "no":
                    case "No":
                        if (item.ConfirmFollowers == "1" || AppSettings.ConnectivitySystem == 0)
                        {
                            item.IsFollowing = isFollowing = "2";
                            btnAddUser.Tag = "request";

                            dbDatabase.Insert_Or_Replace_OR_Delete_UsersContact(item, "Update");
                        }
                        else
                        {
                            item.IsFollowing = isFollowing = "1";
                            btnAddUser.Tag = "friends";

                            dbDatabase.Insert_Or_Replace_OR_Delete_UsersContact(item, "Insert");
                        }

                        SetAddFriendCondition(isFollowing, btnAddUser);

                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Follow_User(item.UserId) });

                        break;
                    case "1": // Remove friends
                    case "yes": 
                    case "Yes":  
                        item.IsFollowing = isFollowing = "0";
                        btnAddUser.Tag = "Add";

                        dbDatabase.Insert_Or_Replace_OR_Delete_UsersContact(item, "Delete");

                        SetAddFriendCondition(isFollowing, btnAddUser);

                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Follow_User(item.UserId) });

                        break;
                    case "2": // Remove request friends

                        var dialog = new MaterialDialog.Builder(activity).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);
                        dialog.Content(activity.GetText(Resource.String.Lbl_confirmationUnFriend));
                        dialog.PositiveText(activity.GetText(Resource.String.Lbl_Confirm)).OnPositive((materialDialog, action) =>
                        {
                            try
                            {
                                item.IsFollowing = isFollowing = "0";
                                btnAddUser.Tag = "Add";

                                dbDatabase = new SqLiteDatabase();
                                dbDatabase.Insert_Or_Replace_OR_Delete_UsersContact(item, "Delete");

                                SetAddFriendCondition(isFollowing, btnAddUser);

                                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Follow_User(item.UserId) }); 
                            }
                            catch (Exception e)
                            {
                                Methods.DisplayReportResultTrack(e);
                            }
                        });
                        dialog.NegativeText(activity.GetText(Resource.String.Lbl_Close)).OnNegative(new MyMaterialDialog());
                        dialog.AlwaysCallSingleChoiceCallback();
                        dialog.Build().Show();
                       
                        break;
                }
                dbDatabase.Dispose(); 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public static void SetAddFriendCondition(string isFollowing, Button btnAddUser)
        {
            try
            {
                switch (isFollowing)
                {
                    //>> Not Friend
                    case "0":
                        btnAddUser.SetTextColor(Color.ParseColor(AppSettings.MainColor));
                        btnAddUser.Text = Application.Context.GetText(AppSettings.ConnectivitySystem == 1 ? Resource.String.Lbl_Follow : Resource.String.Lbl_AddFriends);
                        btnAddUser.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                        btnAddUser.Tag = "Add";
                        break;
                    //>> Friend
                    case "1":
                        btnAddUser.SetTextColor(Color.White);
                        btnAddUser.Text = Application.Context.GetText(AppSettings.ConnectivitySystem == 1 ? Resource.String.Lbl_Following : Resource.String.Lbl_Friends);
                        btnAddUser.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                        btnAddUser.Tag = "friends";
                        break;
                    //>> Request
                    case "2":
                        btnAddUser.SetTextColor(Color.ParseColor("#444444"));
                        btnAddUser.Text = Application.Context.GetText(Resource.String.Lbl_Request);
                        btnAddUser.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                        btnAddUser.Tag = "request";
                        break;
                    default:
                        btnAddUser.SetTextColor(Color.ParseColor(AppSettings.MainColor));
                        btnAddUser.Text = Application.Context.GetText(AppSettings.ConnectivitySystem == 1 ? Resource.String.Lbl_Follow : Resource.String.Lbl_AddFriends);
                        btnAddUser.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                        btnAddUser.Tag = "Add";
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public static async void SetJoinGroup(Activity activity, string groupId, Button button)
        {
            try 
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(activity, activity.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    return;
                }

                var (apiStatus, respond) = await RequestsAsync.Group.Join_Group(groupId);
                if (apiStatus == 200)
                {
                    if (respond is JoinGroupObject result)
                    {
                        if (result.JoinStatus == "requested")
                        {
                            button.SetTextColor(Color.ParseColor("#444444"));
                            button.Text = Application.Context.GetText(Resource.String.Lbl_Request);
                            button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                        }
                        else
                        {
                            var isJoined = result.JoinStatus == "left" ? "false" : "true";
                            button.Text = activity.GetText(isJoined == "yes" || isJoined == "true" ? Resource.String.Btn_Joined : Resource.String.Btn_Join_Group);

                            if (isJoined == "yes" || isJoined == "true")
                            {
                                button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                                button.SetTextColor(Color.White);
                            }
                            else
                            {
                                button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                                button.SetTextColor(Color.ParseColor(AppSettings.MainColor));
                            }
                        }
                    }
                }
                else Methods.DisplayReportResult(activity, respond);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
        
        public static void SetLikePage(Activity activity, string pageId, Button button)
        {
            try
            { 
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(activity, activity.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    return;
                }

                if (button?.Tag?.ToString() == "false")
                {
                    button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                    button.SetTextColor(Color.ParseColor("#ffffff"));
                    button.Text = activity.GetText(Resource.String.Btn_Unlike);
                    button.Tag = "true";
                }
                else
                {
                    button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                    button.SetTextColor(Color.ParseColor(AppSettings.MainColor));
                    button.Text = activity.GetText(Resource.String.Btn_Like);
                    button.Tag = "false";
                }

                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Page.Like_Page(pageId) });
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            } 
        }


        #region MaterialDialog

        public class MyMaterialDialog : Java.Lang.Object, MaterialDialog.ISingleButtonCallback
        {
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
        }
         
        #endregion

    }
}
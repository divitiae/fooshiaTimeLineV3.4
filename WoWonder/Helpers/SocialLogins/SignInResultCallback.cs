﻿using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using WoWonder.Activities.Default;
using WoWonder.Helpers.Utils;
using Exception = Java.Lang.Exception;
using Object = Java.Lang.Object;

namespace WoWonder.Helpers.SocialLogins
{
    public class SignInResultCallback : Object, IResultCallback
    {
        public LoginActivity Activity { get; set; }

        public void OnResult(Object result)
        {
            try
            {
                var googleSignInResult = result as GoogleSignInResult;
                Activity.HandleSignInResult(googleSignInResult);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        } 
    }
}
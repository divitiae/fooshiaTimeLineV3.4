﻿using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Webkit;
using WoWonder.Helpers.Utils;

namespace WoWonder.Library.UI
{
    public class WoWebView : WebView
    { 
        protected WoWebView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public WoWebView(Context context) : base(context)
        {
            Init();
        }

        public WoWebView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init();
        }

        public WoWebView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init();
        }

#pragma warning disable 618
        public WoWebView(Context context, IAttributeSet attrs, int defStyleAttr, bool privateBrowsing) : base(context, attrs, defStyleAttr, privateBrowsing)
#pragma warning restore 618
        {
            Init();
        }

        public WoWebView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Init();
        }

        private void Init()
        {
            try
            {
                Settings.JavaScriptEnabled = true;
                Settings.LoadsImagesAutomatically = true;
                Settings.SetAppCacheEnabled(true);
                Settings.JavaScriptCanOpenWindowsAutomatically = true;
                Settings.DomStorageEnabled = true;
                Settings.AllowFileAccess = true;
                 
                Settings.UseWideViewPort = true;
                Settings.LoadWithOverviewMode = true;
               
                Settings.SetSupportZoom(false);
                Settings.BuiltInZoomControls = false;
                Settings.DisplayZoomControls = false;

                Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.TextAutosizing);

            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

    }
}
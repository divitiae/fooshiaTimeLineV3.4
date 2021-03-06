﻿using System;
using System.Collections.Generic;
using Android.App;
using WoWonder.Activities.Wallet;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;

namespace WoWonder.Helpers.Utils
{
    public class FragmentBottomNavigationView
    {
        private readonly TabbedWalletActivity Context; 
        public readonly List<Fragment> FragmentListTab0 = new List<Fragment>();
        public readonly List<Fragment> FragmentListTab1 = new List<Fragment>();
        private int PageNumber;

        public FragmentBottomNavigationView(Activity context)
        {
            Context = (TabbedWalletActivity)context;
        }
          
        public void NavigationTabBarOnStartTabSelected(int index)
        {
            try
            {
                switch (index)
                {
                    case 0:
                        PageNumber = 0;
                        ShowFragment0();
                        break;

                    case 1:
                        PageNumber = 1;
                        ShowFragment1();
                        break;

                    default:
                        PageNumber = 0;
                        ShowFragment0();
                        break;
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public int GetCountFragment()
        {
            try
            {
                switch (PageNumber)
                {
                    case 0:
                        return FragmentListTab0.Count > 1 ? FragmentListTab0.Count : 0;
                    case 1:
                        return FragmentListTab1.Count > 1 ? FragmentListTab1.Count : 0;
                    default:
                        return FragmentListTab0.Count > 1 ? FragmentListTab0.Count : 0;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return 0;
            }
        }

        public static void HideFragmentFromList(List<Fragment> fragmentList, FragmentTransaction ft)
        {
            try
            {
                if (fragmentList.Count > 0)
                {
                    foreach (var fra in fragmentList)
                    {
                        if (fra.IsAdded && fra.IsVisible)
                            ft.Hide(fra);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public Fragment GetSelectedTabBackStackFragment()
        {
            switch (PageNumber)
            {
                case 0:
                    {
                        var currentFragment = FragmentListTab0[FragmentListTab0.Count - 2];
                        if (currentFragment != null)
                            return currentFragment;
                        break;
                    }
                case 1:
                    {
                        var currentFragment = FragmentListTab1[FragmentListTab1.Count - 2];
                        if (currentFragment != null)
                            return currentFragment;
                        break;
                    }

                default:
                    return null!;

            }

            return null!;
        }

        public Fragment GetSelectedTabLastStackFragment()
        {
            switch (PageNumber)
            {
                case 0:
                    {
                        var currentFragment = FragmentListTab0[FragmentListTab0.Count - 1];
                        if (currentFragment != null)
                            return currentFragment;
                        break;
                    }
                case 1:
                    {
                        var currentFragment = FragmentListTab1[FragmentListTab1.Count - 1];
                        if (currentFragment != null)
                            return currentFragment;
                        break;
                    }

                default:
                    return null!;

            }

            return null!;
        }
         
        public void DisplayFragment(Fragment newFragment)
        {
            try
            {
                FragmentTransaction ft = Context.SupportFragmentManager.BeginTransaction();

                HideFragmentFromList(FragmentListTab0, ft);
                HideFragmentFromList(FragmentListTab1, ft);

                if (PageNumber == 0)
                    if (!FragmentListTab0.Contains(newFragment))
                        FragmentListTab0.Add(newFragment);

                if (PageNumber == 1)
                    if (!FragmentListTab1.Contains(newFragment))
                        FragmentListTab1.Add(newFragment);

                if (!newFragment.IsAdded)
                    ft.Add(Resource.Id.mainFragment, newFragment, PageNumber + newFragment.Id.ToString());
                else
                    ft.Show(newFragment);

                ft.CommitNow();
                Context.SupportFragmentManager.ExecutePendingTransactions();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e); 
            } 
        }
         
        public void RemoveFragment(Fragment oldFragment)
        {
            FragmentTransaction ft = Context.SupportFragmentManager.BeginTransaction();

            if (PageNumber == 0)
                if (FragmentListTab0.Contains(oldFragment))
                    FragmentListTab0.Remove(oldFragment);

            if (PageNumber == 1)
                if (FragmentListTab1.Contains(oldFragment))
                    FragmentListTab1.Remove(oldFragment);

            HideFragmentFromList(FragmentListTab0, ft);
            HideFragmentFromList(FragmentListTab1, ft);

            if (oldFragment.IsAdded)
                ft.Remove(oldFragment);

            if (PageNumber == 0)
            {
                var currentFragment = FragmentListTab0[FragmentListTab0.Count - 1];
                ft.Show(currentFragment).Commit();
            }
            else if (PageNumber == 1)
            {
                var currentFragment = FragmentListTab1[FragmentListTab1.Count - 1];
                ft.Show(currentFragment).Commit();
            }
        }

        public void OnBackStackClickFragment()
        {
            if (PageNumber == 0)
            {
                if (FragmentListTab0.Count > 1)
                {
                    var currentFragment = FragmentListTab0[FragmentListTab0.Count - 1];
                    if (currentFragment != null)
                        RemoveFragment(currentFragment);
                }
                else
                {
                    Context.Finish();
                }
            }
            else if (PageNumber == 1)
            {
                if (FragmentListTab1.Count > 1)
                {
                    var currentFragment = FragmentListTab1[FragmentListTab1.Count - 1];
                    if (currentFragment != null)
                        RemoveFragment(currentFragment);
                }
                else
                {
                    Context.Finish();
                }
            }
        }

        public void ShowFragment0()
        {
            try
            {
                if (FragmentListTab0.Count < 0) return;

                // If user presses it while still on that tab it removes all fragments from the list
                if (FragmentListTab0.Count > 1)
                {
                    FragmentTransaction ft = Context.SupportFragmentManager.BeginTransaction();

                    for (var index = FragmentListTab0.Count - 1; index > 0; index--)
                    {
                        var oldFragment = FragmentListTab0[index];
                        if (FragmentListTab0.Contains(oldFragment))
                            FragmentListTab0.Remove(oldFragment);

                        if (oldFragment.IsAdded)
                            ft.Remove(oldFragment);

                        HideFragmentFromList(FragmentListTab0, ft);
                    }

                    var currentFragment = FragmentListTab0[FragmentListTab0.Count - 1];
                    ft.Show(currentFragment).Commit();
                }
                else
                {
                    var currentFragment = FragmentListTab0[FragmentListTab0.Count - 1];
                    if (currentFragment != null)
                        DisplayFragment(currentFragment);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void ShowFragment1()
        {
            if (FragmentListTab1.Count < 0) return;
            var currentFragment = FragmentListTab1[FragmentListTab1.Count - 1];
            if (currentFragment != null)
                DisplayFragment(currentFragment);
        }

    }
}
using System;
using System.Collections.ObjectModel;
using Android.App;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;

namespace WoWonder.Helpers.Controller
{
    public class CategoriesController
    { 
        //Categories Communities Local Custom
        public string[] CategoriesCreateComunities = Application.Context.Resources?.GetStringArray(Resource.Array.Categories_Communities_array);
        public static ObservableCollection<Classes.Categories> ListCategoriesPage = new ObservableCollection<Classes.Categories>();
        public static ObservableCollection<Classes.Categories> ListCategoriesGroup = new ObservableCollection<Classes.Categories>();
        public static ObservableCollection<Classes.Categories> ListCategoriesBlog = new ObservableCollection<Classes.Categories>();
        public static ObservableCollection<Classes.Categories> ListCategoriesProducts = new ObservableCollection<Classes.Categories>();
        public static ObservableCollection<Classes.Categories> ListCategoriesJob = new ObservableCollection<Classes.Categories>();
        public static ObservableCollection<Classes.Categories> ListCategoriesMovies = new ObservableCollection<Classes.Categories>();

        public string Get_Translate_Categories_Communities(string idCategory, string textCategory)
        { 
            try
            { 
                if (CategoriesCreateComunities.Length <= 0) return textCategory;

                if (string.IsNullOrEmpty(textCategory))
                    return Application.Context.GetText(Resource.String.Lbl_Unknown);
                
                string categoryName = Convert.ToInt32(idCategory) - 1 >= 0 ? CategoriesCreateComunities[Convert.ToInt32(idCategory) - 1] : textCategory;
                return categoryName;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return textCategory;
            }
        } 
    }
}
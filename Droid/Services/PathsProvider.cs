using System;
using Xamarin.Forms;
using BasketTracker.Mobile.Core;

[assembly: Dependency(typeof(BasketTracker.Droid.PathsProvider))]
namespace BasketTracker.Droid
{
	public class PathsProvider: IPathsProvider
	{
		public PathsProvider() {}

        public string ApplicationData
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
        }
    }
}


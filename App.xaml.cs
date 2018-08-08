using System;
using Xamarin.Forms;
using xamarinrest.Views;
using Xamarin.Forms.Xaml;
using xamarinrest.Database;
using xamarinrest.Services;
using xamarinrest.Services.Rest;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace xamarinrest
{
	public partial class App : Application
	{
		
		public App ()
		{

            SQLiteRepository.Init();
            RestService.Init();
            SyncService.Init();
            InitializeComponent();


			MainPage = new MainPage();
		}

		protected override void OnStart ()
		{
            // Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

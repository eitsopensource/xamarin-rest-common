using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinrest.Database;
using xamarinrest.Models;

namespace xamarinrest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmpresasPage : ContentPage
    {
        private Subscription<Empresa> listEmpresasSubscription;

        public EmpresasPage()
        {
            InitializeComponent();

            //Cria um callback (Watcher, Observable) que será executado após um Sync de success (automático)
            listEmpresasSubscription = new Subscription<Empresa>(() => {
                Device.BeginInvokeOnMainThread( async () => {
                    MyListView.ItemsSource = await SQLiteRepository.Query<Empresa>("SELECT * FROM " + typeof(Empresa).Name);
                });
            });

            //Chama o callback criado acima (para listar e não precisar esperar que o App faça uma requisição)
            listEmpresasSubscription.Callback.Invoke();
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}

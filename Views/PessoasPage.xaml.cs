using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using xamarinrest.Models;
using xamarinrest.Database;

namespace xamarinrest.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PessoasPage : ContentPage
	{
        private Subscription<Pessoa> listPessoaSubscription;

        public PessoasPage()
        {
            InitializeComponent();

            //Cria um callback (Watcher, Observable) que será executado após um Sync de success (automático)
            listPessoaSubscription = new Subscription<Pessoa>( () => {
                Device.BeginInvokeOnMainThread( async () => {
                    MyListView.ItemsSource = await SQLiteRepository.Query<Pessoa>("SELECT * FROM " + typeof(Pessoa).Name);
                });
            });

            //Chama o callback criado acima (para listar e não precisar esperar que o App faça uma requisição)
            listPessoaSubscription.Callback.Invoke();
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
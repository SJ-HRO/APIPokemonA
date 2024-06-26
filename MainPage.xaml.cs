using RestSharp;
using System.Collections.ObjectModel;
using System.Linq;

namespace APIPokemonA
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Pokemon> Pokemons { get; set; }

        public MainPage()
        {
            InitializeComponent();
            Pokemons = new ObservableCollection<Pokemon>();
            LoadPokemonData();
            PokemonListView.ItemsSource = Pokemons;
        }

        private async void LoadPokemonData()
        {
            var client = new RestClient("https://pokeapi.co/api/v2/");
            var request = new RestRequest("pokemon?limit=151", Method.Get);
            var response = await client.ExecuteAsync<PokemonResponse>(request);

            if (response.IsSuccessful && response.Data != null)
            {
                foreach (var result in response.Data.Results)
                {
                    Pokemons.Add(new Pokemon { Name = result.Name, Url = result.Url });
                }
            }
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedPokemon = e.CurrentSelection.FirstOrDefault() as Pokemon;

            if (selectedPokemon != null)
            {
                var client = new RestClient(selectedPokemon.Url);
                var request = new RestRequest(selectedPokemon.Url, Method.Get);
                var response = await client.ExecuteAsync<PokemonDetail>(request);

                if (response.IsSuccessful && response.Data != null)
                {
                    PokemonDetail.IsVisible = true;
                    PokemonName.Text = response.Data.Name ?? "No Name";
                    PokemonImage.Source = response.Data.Sprites?.FrontDefault ?? "";
                    PokemonAbilities.Text = response.Data.Abilities != null
                        ? string.Join(", ", response.Data.Abilities.Select(a => a.Ability.Name))
                        : "No Abilities";
                }
            }
        }
    }

    public class Pokemon
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class PokemonResponse
    {
        public List<Pokemon> Results { get; set; }
    }

    public class PokemonDetail
    {
        public string Name { get; set; }
        public Sprites Sprites { get; set; }
        public List<AbilityInfo> Abilities { get; set; }
    }

    public class Sprites
    {
        public string FrontDefault { get; set; }
    }

    public class AbilityInfo
    {
        public Ability Ability { get; set; }
    }

    public class Ability
    {
        public string Name { get; set; }
    }
}

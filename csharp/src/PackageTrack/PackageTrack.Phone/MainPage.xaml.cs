using Algolia.Search;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace PackageTrack.Phone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private AlgoliaClient algoliaClient = null;
        private Index algoliaIndex = null;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            algoliaClient = new AlgoliaClient("<APPLICATION_ID>", "<SEARCH_ONLY_API_KEY>");
            algoliaIndex = algoliaClient.InitIndex("packages");
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var searchTerm = sender.Text;
                var query = new Query(searchTerm);
                var result = algoliaIndex.SearchAsync(query).Result;
                var packagesResult = result.ToObject<PackagesResult>();

                sender.ItemsSource = packagesResult.hits;
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var hit = (PackagesResult.Hit)args.SelectedItem;
            SearchWebView.Navigate(new Uri("http://localhost:8671/api/packages/" + hit.objectID));
        }

        private class PackagesResult
        {
            public class Hit
            {
                public string Name { get; set; }
                public string Link { get; set; }
                public string Count { get; set; }
                public string objectID { get; set; }

                public override string ToString()
                {
                    return Name + Environment.NewLine +
                        "  Link:\t" + Link + Environment.NewLine +
                        "  Count:\t" + Count;
                }
            }

            public List<Hit> hits { get; set; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Net.Http;
using HtmlAgilityPack;

namespace KeyWordChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public string website { get; set; }
        public string keyword { get; set; }
        public bool startUpWebsite = true;
        public bool startUpKeyword = true;
        public bool comboChange = true;
        public List<string> historyWebsiteList = new List<string>();
        public List<string> historyKeywordList = new List<string>();        
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            Initiate(null, null);
        }

        static string URLRequest(string url)
        {
            // This section will extract / get the data / information of the "url" website if connection is succesfull / approved, then send it back to the method
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "GET";
            request.Timeout = 6000;
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows Phone OS 7.5; Trident/5.0; IEMobile/9.0)";

            string responseContent = null;

            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        responseContent = streamReader.ReadToEnd();
                    }
                }
            }
            return (responseContent);
        }

        static int WebsiteNodesSection(HtmlDocument htmlDoc, string keyword, string node)
        {
            // This section will try to match the keyword entered by the user, with the current "node" used by the main program. If it matches will will add 1 to the "result" int, then return it when its done.
            var result = 0;
            var Matchnodes = htmlDoc.DocumentNode.SelectNodes(node);
            if (Matchnodes != null)
            {
                foreach (var item in Matchnodes)
                {
                    if (item.InnerText.Contains(keyword))
                    {
                        result++;
                    }
                }
            }
            return result;
        }

        public void Website(string url, string keyword)
        {
            //This section will be sending a request to WebsiteNodesSection, asking for every time the "Keyword" gets mentioned in the requested nodes / tags, in this case its "a", "p", "li" and "sup" tags
            // After that it will be adding the input to the combobox "comboBoxHistory"
            HtmlDocument htmlDoc = new HtmlDocument();
            string urlResponse = URLRequest(url);

            htmlDoc.LoadHtml(urlResponse);

            int keywordResult = WebsiteNodesSection(htmlDoc, keyword, "//p") + WebsiteNodesSection(htmlDoc, keyword, "//a") + WebsiteNodesSection(htmlDoc, keyword, "//li") + WebsiteNodesSection(htmlDoc, keyword, "//sup");                                  

            lbResult.Content = keywordResult;
            comboboxHistory.Items.Add($"Website: {url} - Keyword: {keyword} - Matches: {keywordResult}");

        }

        public void Initiate(string websiteBlank, string keyword)
        {
            // checks if the input is either null or default when clikced "Search" by the user
            if (websiteBlank == null && keyword == null)
            {
                websiteBlank = txtWebsite.Text;
                keyword = txtKeyword.Text;               
            }
            /*
            if (checkboxCaseSentitive.IsChecked == false)
            {
                // do something to combine them into lowercases
                isLowerCasing();
            }
            */
            if (websiteBlank.Contains("Enter Website") || websiteBlank == string.Empty || keyword.Contains("Enter Keyword") || keyword == string.Empty)
            {
                MessageBox.Show("Error! Invalid input entered!");
                return;
            }
            else
            {
                // if the user enters a "blank" website address with missing the "https://" or the "http://" then it would autumatically assign the missing to the url link for useage
                if (!websiteBlank.Contains("https://") && !websiteBlank.Contains("http://"))
                {
                    websiteBlank = $"https://{websiteBlank}";
                }

                //Method 2, works mostly - Using HTMLAgilityPackage by Nuget
                Website(websiteBlank, keyword);


                /* Method 1 with issues
                 * Using regex to separate the main, full website string, to only keep in the input requested in the regex rules
                 * 
                website = ReadTextFromUrl(websiteBlank);

                //Regex rule is to separate only paragraphs
                Regex Reg = new Regex(@"<[^>]*>");

                string newWebsite = Reg.Replace(website, "").Trim();
                                               
                

                //Counts the matches of the keyword with the newWebsite string then sends the info to comboboxHistory and changes the value of lbResults
                var containsWord = Regex.Matches(newWebsite, keyword).Count;
                lbResult.Content = containsWord;
                comboboxHistory.Items.Add($"Website: {websiteBlank} - Keyword: {keyword} - Matches: {containsWord}");
                */

                // Makes it possible to double click on the two boxes to instant clear them after each search
                txtKeyword.Text = keyword;
                txtWebsite.Text = websiteBlank;
                startUpWebsite = true;
                startUpKeyword = true;

                //Checks if the word has already been searched for and if the match is the same, if true, it will remove it from the search history.
                if (historyWebsiteList.Contains(websiteBlank)&& historyKeywordList.Contains(keyword))
                {
                    var i = historyWebsiteList.IndexOf(websiteBlank);
                    historyWebsiteList.RemoveAt(i);
                    historyKeywordList.RemoveAt(i);
                    comboChange = false;
                    comboboxHistory.Items.RemoveAt(i + 1);

                    comboboxHistory.SelectedIndex = 0;
                    comboChange = true;

                }
                //adds the former elements to the list for the search history
                historyWebsiteList.Add(websiteBlank);
                historyKeywordList.Add(keyword);
            }
        }

        public void isLowerCasing()
        {
            //  do the lower casing converter
        }

            private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            //A method to exit the application onclick and confirmation
            var result = MessageBox.Show("Are you sure you want to exit the application?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                System.Environment.Exit(1);
            }

        }


        private void TxtWebsite_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Double click to clear the textbox
            if (startUpWebsite == true)
            {
                txtWebsite.Text = "";
                startUpWebsite = false;
            }

        }

        private void TxtKeyword_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Double click to clear the textbox
            if (startUpKeyword == true)
            {
                txtKeyword.Text = "";
                startUpKeyword = false;
            }
        }

        private void ComboboxHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // if clicked on an item in the combobox, it will redo the whole process of the search for a keyword, by using the values in the combobox's selected item/value and then 
            if (comboboxHistory.SelectedIndex != 0 && comboChange == true)
            {
                Initiate(historyWebsiteList[comboboxHistory.SelectedIndex - 1], historyKeywordList[comboboxHistory.SelectedIndex - 1]);
            }
        }
               

        /* Used in method 1
         * Used to recieve the data from an URL and then convert it to readable chacters (utf8) in case of special characters, then return it to the caller
        string ReadTextFromUrl(string url)
        {
            using (var client = new WebClient())
            using (var stream = client.OpenRead(url))
            using (var textReader = new StreamReader(stream, Encoding.UTF8, true))
            {
                return textReader.ReadToEnd();
            }
        }
        */
    }
}

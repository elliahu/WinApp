using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using WinApp.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private async void GenerateDataBtn_Click(object sender, RoutedEventArgs e)
        {
            if(dataCount.Text == String.Empty)
            {
                return;
            }
            progress.Visibility = Visibility.Visible;
            progress.IsActive = true;

            for(int i = 0; i < Int32.Parse(dataCount.Text); i++)
            {
                using HttpClient http = new HttpClient();
                string name = await http.GetStringAsync("https://baconipsum.com/api/?type=meat-and-filler&paras=5&format=text");
                string desc = await http.GetStringAsync("https://baconipsum.com/api/?type=meat-and-filler&paras=5&format=text");
                string content = await http.GetStringAsync("https://baconipsum.com/api/?type=meat-and-filler&paras=5&format=text");

                var document = new Document()
                {
                    Name = name.Substring(0, 15),
                    Description = desc.Substring(0, 30),
                    Content = content,
                    Author = new Author()
                    {
                        Name = "System"
                    }
                };
                await StorageManager.Save(document);
            }
            dataCount.Text = String.Empty;
            progress.Visibility = Visibility.Collapsed;
            progress.IsActive = false;
        }
    }
}

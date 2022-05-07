using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using MimeKit;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using WinApp.Model;
using Windows.Storage;
using MailKit.Net.Smtp;
using MailKit;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public ObservableCollection<Document> Documents { get; set; } = new ObservableCollection<Document> { };
        public Author User { get; set; }

        public static event EventHandler NavigationRequested;

        public HomePage()
        {
            this.InitializeComponent();
            DocumentListView.DoubleTapped += DocumentListView_DoubleTapped;
        }

        private async void DocumentListView_Loaded(object sender, RoutedEventArgs e)
        {
            var documents = await StorageManager.LoadAll();

            if(documents == null || documents.Count == 0)
            {
                // Create dummy data
                var document = new Document()
                {
                    Name = "Dummy document",
                    Description = "Description",
                    Content = "Content",
                    Author = new Author()
                    {
                        Name = "Matej Elias",
                        Email = "matej.elias@vsb.ct"
                    }
                };
                await StorageManager.Save(document);
            }

            foreach (var document in documents)
            {
                Documents.Add(document);
            }

            // Hide loading
            progressRing.IsActive = false;
            progressRing.Visibility = Visibility.Collapsed;
        }

        private void DocumentListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(DocumentListView.SelectedItems.Count > 0)
            {
                editBtn.IsEnabled = true;
                deleteBtn.IsEnabled = true;
                shareBtn.IsEnabled = true;
            }
        }

        private void CreateNew_Click(object sender, RoutedEventArgs e)
        {
            NavigationRequested?.Invoke(new Document() { Author = User}, EventArgs.Empty);
        }

        private void DocumentListView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            // Edit document
            if (DocumentListView.SelectedItems.Count > 0)
            {
                NavigationRequested?.Invoke((Document)DocumentListView.SelectedItem, EventArgs.Empty);
            }
        }

        private void EditDocument_Click(object sender, RoutedEventArgs e)
        {
            // Edit document
            if (DocumentListView.SelectedItems.Count > 0)
            {
                NavigationRequested?.Invoke((Document)DocumentListView.SelectedItem, EventArgs.Empty);
            }
        }

        private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            DisplayDeleteDocumentDialog();
        }

        private async void DisplayDeleteDocumentDialog()
        {
            ContentDialog deleteFileDialog = new ContentDialog
            {
                Title = "Delete document permanently?",
                Content = "If you delete this document, you won't be able to recover it. Do you want to delete it?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel"
            };

            deleteFileDialog.XamlRoot = this.Content.XamlRoot;
            ContentDialogResult result = await deleteFileDialog.ShowAsync();

            // Delete the file if the user clicked the primary button.
            /// Otherwise, do nothing.
            if (result == ContentDialogResult.Primary)
            {
                await StorageManager.Delete((DocumentListView.SelectedItem as Document).DocumentId);
                Documents.Remove((Document) DocumentListView.SelectedItem);
            }
        }

        private void ShareBtn_Click(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void autobox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = Documents.Where(d => d.Name.Contains(sender.Text) || d.Description.Contains(sender.Text));
            }
        }

        private void autobox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                // User selected an item from the suggestion list, take an action on it here.
                NavigationRequested?.Invoke((Document)args.ChosenSuggestion, EventArgs.Empty);
            }
            else
            {
                // Use args.QueryText to determine what to do.
            }
        }

        private void autobox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = (args.SelectedItem as Document).Name;
        }

        private async void DownloadJSON_Click(object sender, RoutedEventArgs e)
        {
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;

            TextBox pathBox = new TextBox();
            pathBox.Header = "Filename";
            pathBox.Text = (DocumentListView.SelectedItem as Document).Name + ".json";
            pathBox.HorizontalAlignment = HorizontalAlignment.Stretch;

            panel.Children.Add(pathBox);

            ContentDialog saveDialog = new ContentDialog()
            {
                Title = "Save document as",
                Content = panel,
                CloseButtonText = "Cancle",
                PrimaryButtonText = "Save"
            };

            saveDialog.XamlRoot = this.Content.XamlRoot;
            ContentDialogResult result = await saveDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                if (!pathBox.Text.EndsWith(".json"))
                {
                    pathBox.Text += ".json";
                }

                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                    WriteIndented = true,
                };

                string data = JsonSerializer.Serialize((DocumentListView.SelectedItem as Document));
                StorageFile file = await DownloadsFolder.CreateFileAsync(pathBox.Text, CreationCollisionOption.GenerateUniqueName);
                await FileIO.WriteTextAsync(file, JsonSerializer.Serialize(data, options));
            }
        }

        private async void SendEmail_Click(object sender, RoutedEventArgs e)
        {
            Document document = (DocumentListView.SelectedItem as Document);
            string smtpServer = "wes1-smtp.wedos.net";
            string user = "testing@matejelias.cz";
            string password = "Qaywsx123@";
            string subject = "Document sharing";

            StackPanel panel = new StackPanel();

            TextBox emailBox = new TextBox();
            emailBox.Header = "To";
            emailBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            emailBox.Width = 300;
            emailBox.Margin = new Thickness(0, 15, 0, 0);

            TextBox nameBox = new TextBox();
            nameBox.Header = "Name";
            nameBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            nameBox.Width = 300;

            panel.Children.Add(nameBox);
            panel.Children.Add(emailBox);

            ContentDialog sendEmailDialog = new ContentDialog()
            {
                Title = "Send as email to",
                Content = panel,
                CloseButtonText = "Cancle",
                PrimaryButtonText = "Send"
            };

            sendEmailDialog.XamlRoot = this.Content.XamlRoot;
            ContentDialogResult result = await sendEmailDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Header
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Document Manager", user));
                message.To.Add(new MailboxAddress(nameBox.Text, emailBox.Text));
                message.Subject = subject;

                // Body
                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = @$"
                <h1>{document.Name}</h1>
                <h3>{document.Description}</h3>
                <p>{document.Content}</p>
                <p>Created by: {document.Author.Name}</p>
                ";
                bodyBuilder.TextBody = @$"
                {document.Name}
                {document.Description}
                {document.Content}
                Created by: {document.Author.Name}
                ";
                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(smtpServer, 587, false);

                    // Note: only needed if the SMTP server requires authentication
                    await client.AuthenticateAsync(user, password);

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                };
            }

            
        }
    }
}

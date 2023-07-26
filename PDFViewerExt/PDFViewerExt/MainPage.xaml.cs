using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PDFViewerExt
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void LauncherButtonOnClicked(object sender, EventArgs e)
        {
            var filePath = await DownloadPdfFileAsync();

            if (filePath != null)
            {
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });
            }
        }

        private async void OpenButtonOnClicked(object sender, EventArgs e)
        {
            var filePath = await GeneratePDFFile();

            if (File.Exists(filePath))
            {
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });
            }
        }

        private async Task<string> DownloadPdfFileAsync()
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, "test.pdf");

            if (File.Exists(filePath))
                return filePath;

            var httpClient = new HttpClient();
            var pdfBytes = await httpClient.GetByteArrayAsync("https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf");

            try
            {
                File.WriteAllBytes(filePath, pdfBytes);

                return filePath;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
            }

            return null;
        }

        private async Task<string> GeneratePDFFile()
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, "PDFSample.pdf");

            if (File.Exists(filePath))
                return filePath;

            Assembly assembly = GetType().GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream("PDFViewerExt.Content.PDFSample.pdf"))
            {
                if (stream != null)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        try
                        {

                            stream.CopyTo(memoryStream);
                            var pdfBytes = memoryStream.ToArray();
                            File.WriteAllBytes(filePath, pdfBytes);

                            return filePath;
                        }
                        catch (Exception ex)
                        {
                            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
                        }
                    }
                }
            }

            return null;
        }
    }
}

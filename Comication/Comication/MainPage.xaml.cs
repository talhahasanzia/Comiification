using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Lumia.Imaging;
using System.Threading.Tasks;
using Lumia.Imaging.Artistic;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Comication
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {


        private FilterEffect _cartoonEffect = null;

        // The following  WriteableBitmap contains 
        // the filtered and thumbnail images.
        private WriteableBitmap _cartoonImageBitmap = null;

        FileOpenPicker openPicker;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker();
            openPicker.ContinuationData["Action"] = "SelectPicture";
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.PickSingleFileAndContinue();
            
        }
 
        public async void Continue(IContinuationActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.PickFileContinuation)
            {
                var openPickerContinuationArgs = args as FileOpenPickerContinuationEventArgs;

                // Recover the "Action" info we stored in ContinuationData
                string action = (string)openPickerContinuationArgs.ContinuationData["Action"];

                if (openPickerContinuationArgs.Files.Count > 0)
                {
                    FileNameText.Text = openPickerContinuationArgs.Files[0].Name;


                    IRandomAccessStream selectedFileStream = await openPickerContinuationArgs.Files[0].OpenAsync(FileAccessMode.Read);
                    BitmapImage selectedImage = new BitmapImage();
                    selectedImage.SetSource(selectedFileStream);
                    Preview.Source = selectedImage;

                    _cartoonImageBitmap = new WriteableBitmap(selectedImage.PixelHeight,selectedImage.PixelWidth);
                    ApplyFilterAsync(openPickerContinuationArgs.Files[0]);
                
                }
                else
                {
                    // TODO: Write code here to handle picker cancellation.
                }
            }
        }
        private async Task<bool> ApplyFilterAsync(StorageFile file)
        {
            // Open a stream for the selected file. 
            IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);

            string errorMessage = null;

            try
            {
                // Show the thumbnail of original image. 
                
                // Rewind the stream to start. 
                fileStream.Seek(0);

                // A cartoon effect is initialized with the selected image stream as source.  
                var imageSource = new RandomAccessStreamImageSource(fileStream);
                _cartoonEffect = new FilterEffect(imageSource);

                // Add the cartoon filter as the only filter for the effect. 
                var cartoonFilter = new CartoonFilter();
                _cartoonEffect.Filters = new[] { cartoonFilter };

                // Render the image to a WriteableBitmap. 
                var renderer = new WriteableBitmapRenderer(_cartoonEffect, _cartoonImageBitmap);
                _cartoonImageBitmap = await renderer.RenderAsync();
                _cartoonImageBitmap.Invalidate();

                // Set the rendered image as source for the cartoon image control. 
               FilteredView.Source = _cartoonImageBitmap;
            }
            catch (Exception exception)
            {
                errorMessage = exception.Message;
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
               
                return false;
            }

            return true;
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
    }
}

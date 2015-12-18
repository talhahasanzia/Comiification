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

using Lumia.Imaging.Transforms;
using Lumia.Imaging.Compositing;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Comication
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Chroma Key Filter Variables
        private WriteableBitmap _bitmap = null;
        private DispatcherTimer _timer = null;
        private Color _color = new Color();
        private bool _initialized = false;
        private bool _rendering = false;
        private BitmapImage _source = null;
        private WriteableBitmapRenderer _renderer = null;
        private FilterEffect _effect = null;
        private IList<IFilter> _filters = null;
        private ChromaKeyFilter _chromaKeyFilter = null;
        private RotationFilter _rotationFilter = null;


        StorageFile SelectedImageFile;
        StorageFile SelectedImageFile2;

        // Cartoon Filter Variables
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
            openPicker.PickMultipleFilesAndContinue();
            
        }
        private void Chroma_Click(object sender, RoutedEventArgs e) {



            ApplyChromaFilterAsync();
        
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

                    SelectedImageFile = openPickerContinuationArgs.Files[0];
                    SelectedImageFile2 = openPickerContinuationArgs.Files[1];
                   
                    
                    IRandomAccessStream selectedFileStream = await openPickerContinuationArgs.Files[0].OpenAsync(FileAccessMode.Read);
                    BitmapImage selectedImage = new BitmapImage();
                    selectedImage.SetSource(selectedFileStream);


                    IRandomAccessStream selectedFileStream2 = await openPickerContinuationArgs.Files[1].OpenAsync(FileAccessMode.Read);
                    BitmapImage selectedImage2 = new BitmapImage();
                    selectedImage2.SetSource(selectedFileStream2);

                    Preview.Source = selectedImage;
                    Preview2.Source = selectedImage2;
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

       private async Task<bool> ApplyChromaFilterAsync()
        {

            IRandomAccessStream fileStream = await SelectedImageFile.OpenAsync(FileAccessMode.Read);
            IRandomAccessStream fileStream2 = await SelectedImageFile2.OpenAsync(FileAccessMode.Read);

            string errorMessage = null;

            try
            {
                

                // Rewind the stream to start. 
                fileStream.Seek(0);
                fileStream2.Seek(0);

                // Image 1 
                var imageSource = new RandomAccessStreamImageSource(fileStream);
                var OtherImage = new RandomAccessStreamImageSource(fileStream2);

                _effect = new FilterEffect(imageSource);




                // Add the cartoon filter as the only filter for the effect. 
                var blendFilter = new BlendEffect(OtherImage,_effect,BlendFunction.Normal,1.0);
                

               _effect.Filters = new IFilter[] { new ChromaKeyFilter(Windows.UI.Color.FromArgb(255, 36, 219, 49),0.2,0.3,false) };
                // Create a target where the filtered image will be rendered to
                var target = new WriteableBitmap(_cartoonImageBitmap.PixelWidth, _cartoonImageBitmap.PixelHeight);

                // Create a new renderer which outputs WriteableBitmaps
                using (var renderer = new WriteableBitmapRenderer(blendFilter, target))
                {
                    await renderer.RenderAsync();

                    // Set the output image to Image control as a source
                   FilteredView.Source = target;

                    
                }
            }
            catch (Exception exception)
            {
                errorMessage = exception.Message;
            }

            return true;
        
        
        }

        
    }
}

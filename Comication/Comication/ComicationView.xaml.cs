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
using Windows.UI.Xaml.Shapes;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Comication
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ComicationView : Page
    {
        private FilterEffect _effect = null;
        Color BackgroundColor;
        private StorageFile SelectedImageFile;
        
        BitmapImage selectedImage;
        // Cartoon Filter Variables
        private FilterEffect _cartoonEffect = null;

        private WriteableBitmap transparentImage;
        bool isFilterBusy = false;

        //
        private WriteableBitmap _cartoonImageBitmap = null;

        public ComicationView()
        {
            this.InitializeComponent();
            Step1.Visibility = Visibility.Visible;
            FrontView.Stretch = Stretch.Fill;
            
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
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
                   

                    SelectedImageFile = openPickerContinuationArgs.Files[0];





                    IRandomAccessStream selectedFileStream = await openPickerContinuationArgs.Files[0].OpenAsync(FileAccessMode.Read);
                    selectedImage = new BitmapImage();
                    selectedImage.SetSource(selectedFileStream);
                    


                    FrontView.Source = selectedImage;
                    




                  



                }
                else
                {
                    // TODO: Write code here to handle picker cancellation.
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Step1.Visibility = Visibility.Collapsed;
            Step2.Visibility = Visibility.Visible;
            ColorFocus.Visibility = Visibility.Visible;

        }

       

        private async void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var rect = sender as Rectangle;


            SolidColorBrush c = new SolidColorBrush();
            c =(SolidColorBrush) rect.Fill;

            BackgroundColor = c.Color;

            await ApplyChromaFilterAsync();
            
            
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            BackgroundColor = Color.FromArgb(255, (byte)R_Value.Value, (byte)G_Value.Value, (byte)B_Value.Value);
            
            
            await ApplyChromaFilterAsync();
        }
      
        
        private async Task<bool> ApplyChromaFilterAsync()
        {
            try
            {

                // await getFile();

                //FileNameText.Text = filesample.Name;


                //BitmapImage bitmapImageBack = new BitmapImage();


                //StorageFolder installFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;

                //StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///colorWheel.png"));





                IRandomAccessStream fileStream = await SelectedImageFile.OpenAsync(FileAccessMode.Read);




                BitmapImage BackgroundImage =new BitmapImage() ;
                BackgroundImage.UriSource=new Uri("ms-appx:///comication.png");


                BackView.Source = BackgroundImage;


                // Rewind the stream to start. 
                fileStream.Seek(0);
                // backgroundStream.Seek(0);

                // Image 1 
                var imageSource = new RandomAccessStreamImageSource(fileStream);


                // Image 2
                // var BackgroundImage = new RandomAccessStreamImageSource(backgroundStream);


                // Effect
                _effect = new FilterEffect(imageSource);




                // blend effect. 
                //var blendFilter = new BlendEffect(BackgroundImage, _effect, BlendFunction.Normal, 1.0);

                // BackgroundColor = Color.FromArgb(255, 217, 204, 185);
                _effect.Filters = new IFilter[] { new ChromaKeyFilter(BackgroundColor, 0.2, 0.3, false) };
                // Create a target where the filtered image will be rendered to
                 transparentImage = new WriteableBitmap(selectedImage.PixelWidth, selectedImage.PixelHeight);

                // Create a new renderer which outputs WriteableBitmaps
                using (var renderer = new WriteableBitmapRenderer(_effect, transparentImage))
                {
                    await renderer.RenderAsync();


                    FrontView.Source = transparentImage;
                    // Set the output image to Image control as a source



                }
            }
            catch (FileNotFoundException ex)
            {

            }
            catch (Exception e)
            {

            }
            finally
            {

                isFilterBusy = false;
                FrontView.Source = transparentImage;
            }

            return true;


        }

        private void ImageModes_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                // TODO: Add event handler implementation here.
                if (((ComboBoxItem)ImageModes.SelectedItem).Content.ToString() == "Fill")
                {
                    FrontView.Stretch = Stretch.Fill;
                }
                if (((ComboBoxItem)ImageModes.SelectedItem).Content.ToString() == "Uniform")
                {
                    FrontView.Stretch = Stretch.Uniform;
                }
                if (((ComboBoxItem)ImageModes.SelectedItem).Content.ToString() == "Stretch")
                {
                    FrontView.Stretch = Stretch.UniformToFill;
                }
            }
            catch (Exception ex)
            { 
            
            }
        }

        private async void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BackgroundColor = Color.FromArgb(255, (byte)R_Value.Value, (byte)G_Value.Value, (byte)B_Value.Value);
            ColorOutput.Fill = new SolidColorBrush(BackgroundColor);
            ColorFocus.Stroke = new SolidColorBrush(BackgroundColor);

            await ApplyChromaFilterAsync();
        }

        private void R_Value_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            BackgroundColor = Color.FromArgb(255, (byte)R_Value.Value, (byte)G_Value.Value, (byte)B_Value.Value);
            ColorOutput.Fill = new SolidColorBrush(BackgroundColor);
            ColorFocus.Stroke = new SolidColorBrush(BackgroundColor);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Step2.Visibility = Visibility.Collapsed;
            Step3.Visibility = Visibility.Visible;
            ColorFocus.Visibility = Visibility.Collapsed;

        }





    }
}

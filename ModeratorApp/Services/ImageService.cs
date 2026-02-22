using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeratorApp.Services
{
    static class ImageService
    {
        public static async Task<byte[]> ImageSourceToBytes(ImageSource imageSource) {
            if (imageSource == null)
                return null;

            Stream stream = null;

            switch (imageSource) {
                case StreamImageSource streamImageSource:
                stream = await streamImageSource.Stream(CancellationToken.None);
                break;

                case FileImageSource fileImageSource:
                stream = await FileSystem.OpenAppPackageFileAsync(fileImageSource.File);
                break;

                case UriImageSource uriImageSource:
                var httpClient = new HttpClient();
                stream = await httpClient.GetStreamAsync(uriImageSource.Uri);
                break;
            }

            if (stream == null)
                return null;

            using (var memoryStream = new MemoryStream()) {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static async Task<byte[]> FileResultToBytesAsync(FileResult file) {
            if (file == null)
                return null;

            using var stream = await file.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
        public static ImageSource BytesToImageSource(byte[] imageData) {
            if (imageData == null || imageData.Length == 0)
                return null;

            return ImageSource.FromStream(() => new MemoryStream(imageData));
        }
    }
}

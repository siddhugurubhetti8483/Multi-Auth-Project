using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace MultiAuthAPI.Helpers
{
    public static class FaceRecognitionHelper
    {
        public static float CompareImages(string base64Img1, string base64Img2)
        {
            byte[] imgBytes1 = Convert.FromBase64String(base64Img1);
            byte[] imgBytes2 = Convert.FromBase64String(base64Img2);

            using var image1 = Image.Load<Rgba32>(imgBytes1);
            using var image2 = Image.Load<Rgba32>(imgBytes2);

            if (image1.Width != image2.Width || image1.Height != image2.Height)
                return 0f;

            float diff = 0f;
            for (int y = 0; y < image1.Height; y++)
            {
                for (int x = 0; x < image1.Width; x++)
                {
                    var pixel1 = image1[x, y];
                    var pixel2 = image2[x, y];

                    diff += Math.Abs(pixel1.R - pixel2.R)
                          + Math.Abs(pixel1.G - pixel2.G)
                          + Math.Abs(pixel1.B - pixel2.B);
                }
            }

            float maxDiff = 255f * 3f * image1.Width * image1.Height;
            return 1f - (diff / maxDiff); // 1 = perfect match
        }
    }
}

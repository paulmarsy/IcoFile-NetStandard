using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageSharp;
using ImageSharp.PixelFormats;

namespace Icons.NetStandard
{
    public class IconFile
    {
        private readonly SortedList<int, Image<Bgra32>> _images = new SortedList<int, Image<Bgra32>>();

        private readonly ILog _log;
        public IconFile(ILog log)
        {
            _log = log;
        }
        

        public void AddImageFile(string path, int width, int height)
        {
            var image = Resize(Image.Load<Bgra32>(path), width, height);
            _images.Add(image.Width, image);
        }

        public void Save(Stream stream)
        {
            var writer = new BinaryWriter(stream);

            var imageData = new Dictionary<int, byte[]>();

            var offset = _images.Count * 16 + 6;

            // Write the icon file header.
            writer.Write((ushort)0); // must be 0
            writer.Write((ushort)1); // 1 = ico file
            writer.Write((ushort)_images.Count); // number of sizes

            foreach (var image in _images)
            {
                var data = GetImageData(image.Value);
                imageData.Add(image.Value.Width, data);

                writer.Write((byte)image.Value.Width); // width
                writer.Write((byte)image.Value.Height); // height
                writer.Write((byte)0); // colors, 0 = more than 256
                writer.Write((byte)0); // must be 0
                writer.Write((ushort)1); // color planes, should be 0 or 1
                writer.Write((ushort)32); // bits per pixel
                writer.Write(data.Length); // size of bitmap data in bytes
                writer.Write(offset); // bitmap data offset in file

                offset += data.Length;
            }

            var sortedData = from i in imageData
                orderby i.Key
                select i.Value;

            foreach (var data in sortedData)
                writer.Write(data);
        }

        public void Save(string fileName)
        {
            _log.WriteLine($"Writing new icon file {fileName}...");
            using (var stream = File.OpenWrite(fileName))
            {
                Save(stream);
            }
            _log.WriteLine($"Icon file created successfully.");
        }

        private Image<Bgra32> Resize(Image<Bgra32> image, int maxWidth, int maxHeight)
        {
            var width = image.Width;
            var height = image.Height;
            _log.WriteLine($"Current dimensions: {width}x{height}");

            if (width > maxWidth)
            {
                height = (int)(height * (maxWidth / (float)width));
                width = maxWidth;
            }

            if (height > maxHeight)
            {
                width = (int)(width * (maxHeight / (float)height));
                height = maxHeight;
            }

            _log.WriteLine($"Resized dimensions: {width}x{height}");
            if (width == image.Width && height == image.Height)
            {
                _log.WriteLine("No need to resize image");
                return image;
            }
            _log.WriteLine("Resizing... ");
            var newImage = image.Resize(width, height);
            _log.WriteLine("Done");
            return newImage;
        }

        private byte[] GetImageData(Image<Bgra32> image)
        {
            using (var memoryStream = new MemoryStream())
            using (var writer = new BinaryWriter(memoryStream))
            {
                var width = image.Width;
                var pixelCount = width * width;
                var maskWidth = width / 8;
                if (maskWidth % 4 != 0)
                    maskWidth += 3 - maskWidth % 4;

                writer.Write(40); // size of BITMAPINFOHEADER
                writer.Write(width); // icon width/height
                writer.Write(width * 2); // icon height * 2 (AND plane)
                writer.Write((short)1); // must be 1
                writer.Write((short)32); // bits per pixel
                writer.Write(0); // must be 0
                writer.Write(pixelCount * 4 + maskWidth * width); // size of bitmap data
                writer.Write(new byte[4 * 4]); // must be 0

                for (var y = width - 1; y >= 0; y--)
                for (var x = 0; x < width; x++)
                {
                    var srcPixel = image.Pixels[y * width + x].Bgra;
                    if (srcPixel >> 24 != 0)
                        writer.Write(srcPixel);
                    else
                        writer.Write((uint)0);
                }

                for (var y = width - 1; y >= 0; y--)
                {
                    for (var x = 0; x < width / 8; x++)
                    {
                        byte maskValue = 0;

                        for (var bit = 0; bit < 8; bit++)
                        {
                            var srcPixel = image.Pixels[y * width + x * 8 + bit].Bgra;
                            if (srcPixel >> 24 < 128)
                                maskValue |= (byte)(1 << (7 - bit));
                        }

                        writer.Write(maskValue);
                    }

                    for (var padding = 0; padding < width / 8 % 4; padding++)
                        writer.Write((byte)0);
                }

                return memoryStream.ToArray();
            }
        }
    }
}
using System;
using System.Diagnostics;
using System.IO;
using CommandLine;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace TilePacker
{
    public class Options
    {
        [Option(Required = true, HelpText = "The input folder containing a collection of sprite images to be packed")]
        public string Input { get; set; }

        [Option(Required = true, HelpText = "The output path to the packed tileset image (.png format)")]
        public string Output { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args);

            result.WithParsed(options => PackSprites(options.Input, options.Output));

            if (Debugger.IsAttached)
                Console.ReadKey();
        }

        private static void PackSprites(string inputPath, string outputPath)
        {
            var maxWidth = 0;
            var maxHeight = 0;
            var count = 0;

            foreach (var filePath in Directory.GetFiles(inputPath))
            {
                using (var image = Image.Load(filePath))
                {
                    if (image.Width > maxWidth)
                        maxWidth = image.Width;

                    if (image.Height > maxHeight)
                        maxHeight = image.Height;
                }

                count++;
            }

            var columns = (int)Math.Sqrt(count) + 1;
            var rows = count / columns + 1;
            var packedWidth = maxWidth * columns;
            var packedHeight = maxHeight * rows;
            var xOffset = 0;
            var yOffset = 0;

            using (var packedImage = new Image<Rgba32>(packedWidth, packedHeight))
            {
                foreach (var filePath in Directory.GetFiles(inputPath))
                {
                    using (var image = Image.Load(filePath))
                    {
                        var xAlignmentOffset = (maxWidth - image.Width) / 2;
                        var yAlignmentOffset = maxHeight - image.Height;

                        for (var x = 0; x < image.Width; x++)
                        {
                            for (var y = 0; y < image.Height; y++)
                            {
                                var px = xOffset + x + xAlignmentOffset;
                                var py = yOffset + y + yAlignmentOffset;

                                packedImage[px, py] = image[x, y];
                            }
                        }
                    }

                    xOffset += maxWidth;

                    if (xOffset > packedWidth - maxWidth)
                    {
                        xOffset = 0;
                        yOffset += maxHeight;
                    }
                }

                using (var fileStream = File.OpenWrite(outputPath))
                    packedImage.SaveAsPng(fileStream);
            }

            Console.WriteLine($"tile size: {maxWidth} x {maxHeight}");
        }
    }
}

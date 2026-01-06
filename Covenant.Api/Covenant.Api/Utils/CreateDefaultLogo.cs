using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;


namespace Covenant.Api.Utils
{
    public class CreateDefaultLogo
    {
        private static readonly Rgba32 Red = new Rgba32(244, 67, 54);
        private static readonly Rgba32 Pink = new Rgba32(233, 30, 99);
        private static readonly Rgba32 Purple = new Rgba32(156, 39, 176);
        private static readonly Rgba32 DeepPurple = new Rgba32(103, 58, 183);
        private static readonly Rgba32 Indigo = new Rgba32(63, 81, 181);
        private static readonly Rgba32 Blue = new Rgba32(33, 150, 243);
        private static readonly Rgba32 LightBlue = new Rgba32(3, 169, 244);
        private static readonly Rgba32 Cyan = new Rgba32(0, 188, 212);
        private static readonly Rgba32 Teal = new Rgba32(0, 150, 136);
        private static readonly Rgba32 Green = new Rgba32(76, 175, 80);
        private static readonly Rgba32 LightGreen = new Rgba32(139, 195, 74);
        private static readonly Rgba32 Lime = new Rgba32(205, 220, 57);
        private static readonly Rgba32 Yellow = new Rgba32(255, 235, 59);
        private static readonly Rgba32 Amber = new Rgba32(255, 193, 7);
        private static readonly Rgba32 Orange = new Rgba32(255, 152, 0);
        private static readonly Rgba32 DeepOrange = new Rgba32(255, 87, 34);
        private static readonly Rgba32 Brown = new Rgba32(121, 85, 72);
        private static readonly Rgba32 Grey = new Rgba32(158, 158, 158);
        private static readonly Rgba32 BlueGrey = new Rgba32(96, 125, 139);
        private static readonly Rgba32[] _colors =
        {
            Red,Pink,Purple,DeepPurple,Indigo,Blue,LightBlue,Cyan,Teal,
            Green,LightGreen,Lime,Yellow,Amber,Orange,DeepOrange,Brown,Grey,BlueGrey
        };
        public string FontPath { get; } = "wwwroot/assets/fonts/RobotoBold.ttf";
        public CreateDefaultLogo()
        {
        }
        public CreateDefaultLogo(string fontPath) => FontPath = fontPath;

        public IFileInfo Create(string name1, string name2 = null)
        {
            try
            {
                var letters = "AB";
                letters = name2 is null
                    ? string.Concat(name1?.Take(2) ?? letters)
                    : string.Concat(name1?.Take(1) ?? "A", name2.Take(1) ?? "B");
                string fileName = $"default{Guid.NewGuid()}{letters}.png";
                string fullFilePath = Path.Combine(Path.GetTempPath(), fileName);

                Font font;
                if (File.Exists(FontPath))
                {
                    var fontCollection = new FontCollection();
                    font = fontCollection.Add(FontPath).CreateFont(50);
                }
                else
                {
                    font = SystemFonts.CreateFont("Arial", 50);
                }
                using (var img = new Image<Rgba32>(200, 200))
                {
                    var random = new Random();
                    img.Mutate(ctx => ctx
                        .Fill(_colors[random.Next(0, 18)])
                        .DrawText(letters.ToUpper(), font, Color.White, new PointF(68, 80)));
                    img.Save(fullFilePath);
                }
                return new PhysicalFileInfo(new FileInfo(fullFilePath));
            }
            catch (Exception)
            {
                return new NotFoundFileInfo(name1);
            }
        }
    }
}
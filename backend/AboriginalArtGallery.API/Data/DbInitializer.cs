using Microsoft.EntityFrameworkCore;
using AboriginalArtGallery.API.Data;
using AboriginalArtGallery.API.Models;

namespace AboriginalArtGallery.API.Data;

public static class DbInitializer
{
    public static void Seed(IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            // 1. Ensure Local Images Directory exists and is populated
            var webRootPath = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
            var imagesDir = Path.Combine(webRootPath, "images");
            Directory.CreateDirectory(imagesDir);

            var imagesToDownload = new Dictionary<string, string>
            {
                { "awelye.jpg", "https://picsum.photos/seed/awelye-kngwarreye/800/600" },
                { "ghost_gums.jpg", "https://picsum.photos/seed/ghost-gums-namatjira/800/600" },
                { "cyclone_tracy.jpg", "https://picsum.photos/seed/cyclone-tracy-rover/800/600" },
                { "sandhills.jpg", "https://picsum.photos/seed/sandhills-napangardi/800/600" },
                { "namorrorddo.jpg", "https://picsum.photos/seed/namorrorddo-bark/800/600" },
                { "mimi_spirits.jpg", "https://picsum.photos/seed/mimi-spirits-nourlangie/800/600" },
                { "yam_dreaming.jpg", "https://picsum.photos/seed/yam-dreaming-gallery/800/600" },
                { "rain_dreaming.jpg", "https://picsum.photos/seed/rain-dreaming-warlpiri/800/600" },
                { "macdonnell_ranges.jpg", "https://picsum.photos/seed/macdonnell-ranges-dusk/800/600" },
                { "kimberley_country.jpg", "https://picsum.photos/seed/kimberley-country-rover/800/600" }
            };

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "AboriginalArtGalleryAPI/1.0");

            foreach (var (filename, url) in imagesToDownload)
            {
                var filePath = Path.Combine(imagesDir, filename);
                if (!File.Exists(filePath))
                {
                    try
                    {
                        var bytes = client.GetByteArrayAsync(url).GetAwaiter().GetResult();
                        File.WriteAllBytes(filePath, bytes);
                        Console.WriteLine($"Downloaded local image: {filename}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error downloading {filename} from {url}: {ex.Message}");
                    }
                }
            }

            // 2. Ensure Seeded Artists
            void EnsureArtist(string name, string tribe, string country, int birthYear, string bio)
            {
                if (!db.Artists.Any(a => a.Name == name))
                    db.Artists.Add(new Artist
                    { 
                        Name = name, Tribe = tribe, Country = country, BirthYear = birthYear,
                        Biography = bio, CreatedAt = DateTime.UtcNow 
                    });
            }

            EnsureArtist("Emily Kame Kngwarreye", "Anmatyerr", "Australia", 1910,
                "One of Australia's most celebrated Aboriginal painters, known for her vibrant dot and line works depicting Alhalkere — her Country in the Sandover River region of the Northern Territory.");
            EnsureArtist("Albert Namatjira", "Arrernte", "Australia", 1902,
                "Pioneer of the Western Aranda watercolour movement. His landscapes of Central Australia introduced Indigenous art to mainstream Australia in the mid-20th century.");
            EnsureArtist("Rover Thomas", "Kukatja/Wangkajunga", "Australia", 1926,
                "A founding figure of the Warmun (Turkey Creek) art movement in the Kimberley. His works depict Country, law and ceremony using ochres on canvas and board.");
            EnsureArtist("Dorothy Napangardi", "Warlpiri", "Australia", 1956,
                "Known for her intricate depictions of Mina Mina, a sacred women's ceremonial site in the Tanami Desert, rendered in flowing white lines over a black ground.");
            EnsureArtist("Yirawala", "Kuninjku", "Australia", 1903,
                "Often called the 'Picasso of Arnhem Land', Yirawala was a master bark painter whose works depicted ancestral beings from Western Arnhem Land traditions.");
            db.SaveChanges();

            int ArtistId(string name) => db.Artists.First(a => a.Name == name).Id;

            // 3. Ensure Seeded Artifacts (pointing to local files)
            if (db.Artifacts.Count() < 10)
            {
                var emilyId = ArtistId("Emily Kame Kngwarreye");
                var albertId = ArtistId("Albert Namatjira");
                var roverId = ArtistId("Rover Thomas");
                var dorothyId = ArtistId("Dorothy Napangardi");
                var yirawalaId = ArtistId("Yirawala");

                var seed = new List<Artifact>
                {
                    new() { Title="Awelye (Body Paint Dreaming)",
                        Description="A luminous field of purple, gold and white created from the artist's memory of women's ceremonial body-paint designs performed on Alhalkere Country.",
                        ArtType="Painting", YearCreated=1994, ArtistId=emilyId,
                        ImageUrl="/images/awelye.jpg",
                        Price=0, IsAvailableForPurchase=false, CreatedAt=DateTime.UtcNow },
                    new() { Title="Ghost Gums, MacDonnell Ranges",
                        Description="One of Namatjira's iconic watercolours depicting the striking white-barked ghost gums against the red ochre ranges of Central Australia.",
                        ArtType="Painting", YearCreated=1957, ArtistId=albertId,
                        ImageUrl="/images/ghost_gums.jpg",
                        Price=0, IsAvailableForPurchase=false, CreatedAt=DateTime.UtcNow },
                    new() { Title="Cyclone Tracy",
                        Description="A powerful aerial-perspective work depicting the destruction wrought by Cyclone Tracy on Darwin in 1974, rendered in ochres and charcoal.",
                        ArtType="Painting", YearCreated=1991, ArtistId=roverId,
                        ImageUrl="/images/cyclone_tracy.jpg",
                        Price=0, IsAvailableForPurchase=false, CreatedAt=DateTime.UtcNow },
                    new() { Title="Sandhills, Mina Mina",
                        Description="Napangardi's signature flowing white lines trace the topography of Mina Mina, a sacred women's ceremonial site in the Tanami Desert.",
                        ArtType="Painting", YearCreated=2004, ArtistId=dorothyId,
                        ImageUrl="/images/sandhills.jpg",
                        Price=0, IsAvailableForPurchase=false, CreatedAt=DateTime.UtcNow },
                    new() { Title="Namorrorddo (Lightning Spirit)",
                        Description="A bark painting depicting Namorrorddo, a dangerous skeletal spirit of the night sky associated with lightning. Rendered in natural ochres on stringybark.",
                        ArtType="Bark Painting", YearCreated=1968, ArtistId=yirawalaId,
                        ImageUrl="/images/namorrorddo.jpg",
                        Price=0, IsAvailableForPurchase=false, CreatedAt=DateTime.UtcNow },
                    new() { Title="Kakadu Rock Art — Mimi Spirits",
                        Description="Study of Mimi spirit figures from the rock art galleries of Kakadu National Park, estimated 10,000+ years old.",
                        ArtType="Rock Art", YearCreated=1989, ArtistId=yirawalaId,
                        ImageUrl="/images/mimi_spirits.jpg",
                        Price=0, IsAvailableForPurchase=false, CreatedAt=DateTime.UtcNow },
                    new() { Title="Yam Dreaming",
                        Description="A striking contemporary work in red ochre depicting the underground network of yam roots — a vital food source and women's Dreaming subject.",
                        ArtType="Contemporary", YearCreated=2009, ArtistId=emilyId,
                        ImageUrl="/images/yam_dreaming.jpg",
                        Price=480, IsAvailableForPurchase=true, CreatedAt=DateTime.UtcNow },
                    new() { Title="Rain Dreaming",
                        Description="Concentric circles and flowing lines represent waterholes and the paths of the Rain Ancestor across the desert.",
                        ArtType="Painting", YearCreated=2001, ArtistId=dorothyId,
                        ImageUrl="/images/rain_dreaming.jpg",
                        Price=320, IsAvailableForPurchase=true, CreatedAt=DateTime.UtcNow },
                    new() { Title="MacDonnell Ranges at Dusk",
                        Description="A panoramic watercolour in deep purple and terracotta, capturing the dramatic light of sunset over the West MacDonnell Ranges.",
                        ArtType="Painting", YearCreated=1955, ArtistId=albertId,
                        ImageUrl="/images/macdonnell_ranges.jpg",
                        Price=750, IsAvailableForPurchase=true, CreatedAt=DateTime.UtcNow },
                    new() { Title="Kimberley Country",
                        Description="An aerial view of Rover Thomas's Country in the East Kimberley. Black and raw sienna forms represent ancient escarpments and ceremony sites.",
                        ArtType="Painting", YearCreated=1995, ArtistId=roverId,
                        ImageUrl="/images/kimberley_country.jpg",
                        Price=920, IsAvailableForPurchase=true, CreatedAt=DateTime.UtcNow },
                };
                db.Artifacts.AddRange(seed);
                db.SaveChanges();
            }

            // 4. Backfill existing database rows to point to local images
            var localImagesMap = new Dictionary<string, string>
            {
                { "Awelye (Body Paint Dreaming)", "/images/awelye.jpg" },
                { "Ghost Gums, MacDonnell Ranges", "/images/ghost_gums.jpg" },
                { "Cyclone Tracy", "/images/cyclone_tracy.jpg" },
                { "Sandhills, Mina Mina", "/images/sandhills.jpg" },
                { "Namorrorddo (Lightning Spirit)", "/images/namorrorddo.jpg" },
                { "Kakadu Rock Art — Mimi Spirits", "/images/mimi_spirits.jpg" },
                { "Yam Dreaming", "/images/yam_dreaming.jpg" },
                { "Rain Dreaming", "/images/rain_dreaming.jpg" },
                { "MacDonnell Ranges at Dusk", "/images/macdonnell_ranges.jpg" },
                { "Kimberley Country", "/images/kimberley_country.jpg" }
            };

            foreach (var (title, localPath) in localImagesMap)
            {
                var art = db.Artifacts.FirstOrDefault(a => a.Title == title);
                if (art != null && art.ImageUrl != localPath)
                {
                    art.ImageUrl = localPath;
                }
            }
            db.SaveChanges();

            // Ensure the last 4 artworks are available for purchase with correct prices
            var purchasableMap = new Dictionary<string, decimal>
            {
                ["Yam Dreaming"] = 480m,
                ["Rain Dreaming"] = 320m,
                ["MacDonnell Ranges at Dusk"] = 750m,
                ["Kimberley Country"] = 920m,
            };
            foreach (var (title, price) in purchasableMap)
            {
                var art = db.Artifacts.FirstOrDefault(a => a.Title == title);
                if (art != null && (!art.IsAvailableForPurchase || art.Price == 0))
                {
                    art.IsAvailableForPurchase = true;
                    art.Price = price;
                }
            }
            db.SaveChanges();
        }
    }
}

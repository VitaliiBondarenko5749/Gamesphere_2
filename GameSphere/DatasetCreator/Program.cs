using HtmlAgilityPack;
using System.Data.SqlClient;

class Program
{
    static async Task Main(string[] args)
    {
        // Налаштування виведення кирилиці у консоль
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // Строка підключення
        string connectionString = "Server=.;Initial Catalog=CatalogOfGamesDb;Trusted_Connection=True;TrustServerCertificate=True;";

        Console.WriteLine("Getting games from the database...\n");

        // Отримання ігор з бази даних
        List<string> gameNames = GetGameNamesFromDatabase(connectionString);

        Console.WriteLine(new string('-', 50));

        foreach (string gameName in gameNames)
        {
            Console.WriteLine(gameName);
        }

        Console.WriteLine(new string('-', 50));

        Console.WriteLine("\nCreating dataset...\n");

        foreach (string gameName in gameNames) 
        {
            // Створюємо папку для кожної гри
            string gameDirectory = Path.Combine(@"D:\Dataset", gameName);
            Directory.CreateDirectory(gameDirectory);

            string queryText = gameName + " screenshots";

            // Формуємо URL пошукового запиту (в даному випадку Google Images або інший сайт)
            string searchUrl = $"https://www.google.com/search?q={Uri.EscapeDataString(queryText)}&tbm=isch";

            // Парсимо зображення зі сторінки
            string[] imageUrls = await GetImageUrlsFromPage(searchUrl);

            // Завантажуємо і зберігаємо зображення
            int imageIndex = 1;
            foreach (var imageUrl in imageUrls)
            {
                await DownloadAndSaveImage(imageUrl, gameDirectory, $"{gameName}_{imageIndex}");
                imageIndex++;
            }
        }

        Console.WriteLine(new string('-', 50));

        Console.WriteLine("\n DATASET HAS BEEN CREATED!");
    }

    // Метод для отримання назв ігор з бази даних
    static List<string> GetGameNamesFromDatabase(string connectionString)
    {
        List<string> gameNames = new List<string>();

        // SQL-запит для отримання назв ігор
        string query = "SELECT Name FROM gamecatalog.Games;";

        // Створення підключення до бази даних
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                // Відкриття підключення
                connection.Open();

                // Створення команди SQL
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Виконання запиту і читання результатів
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Перевіряємо, чи є записи
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                // Додаємо назву гри до списку
                                string gameName = reader.GetString(0); // 0 - індекс колонки "GameName"
                                gameNames.Add(gameName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при підключенні до бази даних: {ex.Message}");
            }

            return gameNames;
        }
    }

    static async Task<string[]> GetImageUrlsFromPage(string url)
    {
        using HttpClient client = new HttpClient();
        string response = await client.GetStringAsync(url);

        // Парсимо HTML
        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(response);

         // Знаходимо всі теги <img> або інші теги з атрибутом data-iurl або data-src (URL великих зображень)
    var imageNodes = htmlDoc.DocumentNode.SelectNodes("//img")
                       ?.Select(img => img.GetAttributeValue("data-iurl", null) // шукаємо атрибут data-iurl
                           ?? img.GetAttributeValue("data-src", null) // якщо data-iurl немає, беремо data-src
                           ?? img.GetAttributeValue("src", null)) // якщо немає, то використовуємо src
                       .Where(src => src != null)
                       .Take(100) // обмежуємо кількість зображень
                       .ToArray();

        return imageNodes ?? new string[] { };
    }

    static async Task DownloadAndSaveImage(string imageUrl, string directory, string fileName)
    {
        using HttpClient client = new HttpClient();

        try
        {
            // Завантажуємо зображення
            byte[] imageBytes = await client.GetByteArrayAsync(imageUrl);

            // Формуємо шлях до файлу
            string filePath = Path.Combine(directory, $"{fileName}.jpg");

            // Зберігаємо зображення у файл
            await File.WriteAllBytesAsync(filePath, imageBytes);

            Console.WriteLine($"Зображення збережене: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при завантаженні зображення: {ex.Message}");
        }
    }
}
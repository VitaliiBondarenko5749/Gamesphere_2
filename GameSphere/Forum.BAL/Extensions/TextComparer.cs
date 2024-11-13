using System.Text.RegularExpressions;

namespace Forum.BAL.Extensions;

public static class TextComparer
{
    // Метод для обчислення косинусної подібності між двома векторами
    public static double CosineSimilarity(Dictionary<string, int> vectorA, Dictionary<string, int> vectorB)
    {
        var commonTerms = vectorA.Keys.Intersect(vectorB.Keys);
        double dotProduct = commonTerms.Sum(term => vectorA[term] * vectorB[term]);

        double magnitudeA = Math.Sqrt(vectorA.Values.Sum(x => x * x));
        double magnitudeB = Math.Sqrt(vectorB.Values.Sum(x => x * x));

        if (magnitudeA == 0 || magnitudeB == 0)
            return 0;

        return dotProduct / (magnitudeA * magnitudeB);
    }

    // Метод для побудови вектора частоти термів (TF) для кожного поста
    public static Dictionary<string, int> GetTermFrequencyVector(string postText)
    {
        // Видаляємо HTML теги та непотрібні символи
        postText = Regex.Replace(postText.ToLower(), "<.*?>", ""); // Видалення HTML-тегів
        postText = Regex.Replace(postText, @"[^\w\s]", ""); // Видалення пунктуації

        // Розбиваємо текст на слова
        string[] words = postText.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        // Рахуємо частоту кожного терма
        Dictionary<string, int> termFrequency = new();
        foreach (var word in words)
        {
            if (termFrequency.ContainsKey(word))
                termFrequency[word]++;
            else
                termFrequency[word] = 1;
        }

        return termFrequency;
    }

    public static string RemoveHtmlTags(string text)
    {
        return Regex.Replace(text, "<[^>]*>", "");
    }
}
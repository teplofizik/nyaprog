using System.Collections.Generic;

namespace Programmer.Platform
{
    /// <summary>
    /// Получение данных по сети. Абстракция позволяет подменять реализацию
    /// и не тянуть сетевые зависимости в ядро команд напрямую.
    /// </summary>
    public interface IHttpFetcher
    {
        /// <summary>Получить тело ответа как текст (UTF-8). null — при ошибке.</summary>
        string GetText(string Url, string Method = "GET", IReadOnlyDictionary<string, string> Headers = null);

        /// <summary>Получить тело ответа как байты. null — при ошибке.</summary>
        byte[] GetBytes(string Url, string Method = "GET", IReadOnlyDictionary<string, string> Headers = null);
    }
}

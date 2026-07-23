using System.Text;

namespace Shos.Normalize;

public static class StringExtensions
{
    static readonly Dictionary<char, char> characterTable = new() {
        ['￥'] = '\\',
        ['“'] = '"',
        ['”'] = '"',
        ['’'] = '\'',
        ['ﾞ'] = '゛',
        ['ﾟ'] = '゜'
    };
 
    public static string Normalize(this string text)
        => text.Normalize(NormalizationForm.FormKC)
               .NormalizeWithCharacterTable();

    static string NormalizeWithCharacterTable(this string text)
    {
        foreach (var pair in characterTable)
            text = text.Replace(pair.Key, pair.Value);
        return text;
    }
}

static class Program
{
    const string ApplicationName = "Shos.Normalize";

    [STAThread]
    static void Main()
    {
        try {
            Show($"{ApplicationName}");

            var data = Clipboard.GetDataObject();
            var formats = data?.GetFormats(autoConvert: false) ?? [];
            string[] textFormats = [
                DataFormats.Text        ,
                DataFormats.UnicodeText ,
                DataFormats.OemText     ,
                DataFormats.StringFormat,
            ];
            var allowedFormats = textFormats.Append(DataFormats.Locale);

            if (formats.Length == 0 || formats.Except(allowedFormats).Any())
                return;

            var text = textFormats.Where(format => formats.Contains(format))
                                  .Select(format => data!.GetData(format, autoConvert: false))
                                  .OfType<string>()
                                  .FirstOrDefault();

            if (string.IsNullOrEmpty(text))
                return;

            Clipboard.SetText(text.Normalize());
        } catch (Exception exception) {
            ShowError(exception.Message);
        }
    }

    static void ShowError(string message)
    {
        (Console.ForegroundColor, Console.BackgroundColor) = (ConsoleColor.Red, ConsoleColor.Black);
        Show($"An error occurred: {message}");
        Console.ResetColor();
    }

    static void Show(string message) => Console.WriteLine(message);
}

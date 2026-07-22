using System.Text;

namespace Shos.Normalize;

static class Program
{
    const string ApplicationName = "Shos.Normalize";

    static readonly Dictionary<char, char> characterTable = new() {
        ['“'] = '"',
        ['”'] = '"',
        ['’'] = '\''
    };

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

            text = Normalize(text);

            Clipboard.SetText(text);
        } catch (Exception exception) {
            ShowError(exception.Message);
        }
    }

    static string Normalize(string text)
    {
        text = text.Normalize(NormalizationForm.FormKC);
        foreach (var pair in characterTable)
            text = text.Replace(pair.Key, pair.Value);
        return text;
    }

    static void ShowError(string message)
    {
        (Console.ForegroundColor, Console.BackgroundColor) = (ConsoleColor.Red, ConsoleColor.Black);
        Show($"An error occurred: {message}");
        Console.ResetColor();
    }

    static void Show(string message) => Console.WriteLine(message);
}

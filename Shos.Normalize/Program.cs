using System.Runtime.InteropServices;
using System.Text;

namespace Shos.Normalize;

static class Program
{
    const string ApplicationName = nameof(Shos.Normalize);

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

            var normalizedText = text.Normalize(NormalizationForm.FormKC)
                                     .Replace('“', '"')
                                     .Replace('”', '"')
                                     .Replace('’', '\'');
            Clipboard.SetText(normalizedText);
        } catch (ExternalException exception) {
            ShowError($"An error occurred: {exception.Message}");
        } catch (Exception exception) {
            ShowError($"An error occurred: {exception.Message}");
        }
    }

    static void ShowError(string message)
    {
        (Console.ForegroundColor, Console.BackgroundColor) = (ConsoleColor.Red, ConsoleColor.Black);
        Show(message);
        Console.ResetColor();
    }

    static void Show(string message) => Console.WriteLine(message);
}

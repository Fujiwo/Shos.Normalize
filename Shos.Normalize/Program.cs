using System.Text;

namespace Shos.Normalize;

/// <summary>Provides an extension method that normalizes text copied to the clipboard,
/// converting compatibility characters and typographic punctuation to their
/// plain ASCII / standard Unicode equivalents.</summary>
public static class StringExtensions
{
    // Maps individual characters that are not covered (or not covered the way we want)
    // by Unicode NFKC normalization to their preferred replacement character.
    // This runs after NFKC normalization, so it can also "undo" or adjust
    // some of the results produced by NormalizationForm.FormKC.
    static readonly Dictionary<char, char> characterTable = new() {
        ['¥'] = '\\', // NFKC fullwidth yen sign -> ASCII backslash.
        ['“'] = '"' , // Left double quotation mark -> ASCII double quote.
        ['”'] = '"' , // Right double quotation mark -> ASCII double quote.
        ['’'] = '\'', // Right single quotation mark -> ASCII apostrophe.
        ['゙'] = '゛', // NFKC halfwidth voiced sound mark -> fullwidth voiced sound mark.
        ['゚'] = '゜'  // NFKC halfwidth semi-voiced sound mark -> fullwidth semi-voiced sound mark.
    };

    /// <summary>Normalizes the given text: first applies Unicode NFKC (compatibility)
    /// normalization, then replaces any remaining characters listed in
    /// <see cref="characterTable"/> with their preferred equivalents.</summary>
    /// <param name="text">The text to normalize.</param>
    /// <returns>The normalized text.</returns>
    public static string NormalizeEx(this string text)
        => text.Normalize(NormalizationForm.FormKC)
               .NormalizeWithCharacterTable();

    /// <summary>Replaces every character found in <see cref="characterTable"/> with its
    /// mapped replacement character.</summary>
    static string NormalizeWithCharacterTable(this string text)
    {
        foreach (var pair in characterTable)
            text = text.Replace(pair.Key, pair.Value);
        return text;
    }
}

/// <summary>Console entry point that reads text from the clipboard, normalizes it,
/// and writes the result back to the clipboard.</summary>
static class Program
{
    const string ApplicationName = "Shos.Normalize";
    static readonly string[] textFormats = [
        DataFormats.Text        ,
        DataFormats.UnicodeText ,
        DataFormats.OemText     ,
        DataFormats.StringFormat
    ];
    static readonly HashSet<string> allowedFormats = [.. textFormats, DataFormats.Locale];

    // STAThread is required because Clipboard access uses COM, which needs
    // the calling thread to run in a single-threaded apartment.
    [STAThread]
    static void Main()
    {
        try {
            // Show application title.
            Show($"{ApplicationName}");

            // Get a text from the clipboard.
            var text = GetClipboardText();
            if (string.IsNullOrEmpty(text))
                return;

            var normalized = text.NormalizeEx();
            if (normalized != text)
                Clipboard.SetText(normalized);
        } catch (Exception exception) {
            ShowError(exception.Message);
        }
    }

    static string GetClipboardText()
    {
        // Retrieve the raw clipboard data object without any automatic format conversion,
        // so we can inspect exactly which formats are present.
        var data = Clipboard.GetDataObject();
        var formats = data?.GetFormats(autoConvert: false) ?? [];

        if (!HasOnlySupportedTextFormats(formats))
            return string.Empty;

        // Pick the first supported text format present on the clipboard and read its content.
        var text = textFormats.Where(format => formats.Contains(format))
                              .Select(format => data!.GetData(format, autoConvert: false))
                              .OfType<string>()
                              .FirstOrDefault();
        return text ?? string.Empty;
    }

    static bool HasOnlySupportedTextFormats(string[] formats)
        => formats.Length > 0                                  &&
           formats.Any(format => textFormats.Contains(format)) &&
           formats.All(allowedFormats.Contains);

    /// <summary>Writes an error message to the console using a red-on-black color scheme.</summary>
    static void ShowError(string message)
    {
        (Console.ForegroundColor, Console.BackgroundColor) = (ConsoleColor.Red, ConsoleColor.Black);
        Show($"An error occurred: {message}");
        Console.ResetColor();
    }

    /// <summary>Writes a message to the console.</summary>
    static void Show(string message) => Console.WriteLine(message);
}

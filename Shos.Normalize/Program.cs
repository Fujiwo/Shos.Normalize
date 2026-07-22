using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        try
        {
            var data = Clipboard.GetDataObject();
            var formats = data?.GetFormats(autoConvert: false) ?? [];
            string[] textFormats =
            [
                DataFormats.Text,
                DataFormats.UnicodeText,
                DataFormats.OemText,
                DataFormats.StringFormat,
            ];
            var allowedFormats = textFormats.Append(DataFormats.Locale);

            if (formats.Length == 0 || formats.Except(allowedFormats).Any())
            {
                return;
            }

            var text = textFormats
                .Where(format => formats.Contains(format))
                .Select(format => data!.GetData(format, autoConvert: false))
                .OfType<string>()
                .FirstOrDefault();

            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            Clipboard.SetText(text.Normalize(NormalizationForm.FormKC));
        }
        catch (ExternalException)
        {
        }
    }
}

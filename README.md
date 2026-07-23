# Shos.Normalize

Shos.Normalize is a Windows utility that normalizes text in the clipboard. It is useful when text copied from different applications contains compatibility characters or typographic quotation marks and needs to be made more consistent before use.

## Features

- Applies Unicode NFKC normalization through `NormalizationForm.FormKC`.
- Converts the yen sign (`￥`) to the ASCII backslash (`\`).
- Converts left and right double quotation marks (`“` and `”`) to the ASCII double quotation mark (`\"`).
- Converts the right single quotation mark (`’`) to the ASCII apostrophe (`'`).
- Converts the half-width voiced sound mark (`ﾞ`) to the full-width voiced sound mark (`゛`).
- Converts the half-width semi-voiced sound mark (`ﾟ`) to the full-width semi-voiced sound mark (`゜`).
- Processes clipboard data only when it contains supported text formats, optionally accompanied by locale information.
- Writes the normalized text back to the clipboard only when it differs from the original text.

## Requirements

- Windows
- .NET 10

## Usage

1. Copy text to the clipboard.
2. Run `Shos.Normalize.exe`.
3. Paste the normalized text wherever it is needed.

The application exits without changing the clipboard when no supported text format is available, when the text is empty, or when any unsupported format such as HTML, rich text, an image, or a file list is present. It writes an error message to the console if an exception occurs during processing.

## Build

### Visual Studio

Open `Shos.Normalize.slnx` in Visual Studio, then build the solution using the standard Build command.

### .NET CLI

From the repository root, run:

```powershell
dotnet build Shos.Normalize.slnx
```

The project targets `net10.0-windows` and uses Windows Forms for clipboard access.

## License

This project is licensed under the MIT License. See [LICENSE.txt](LICENSE.txt) for the complete license text.

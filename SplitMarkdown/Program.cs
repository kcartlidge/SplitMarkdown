using System.Text;

namespace SplitMarkdown;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            // Information.
            Console.WriteLine();
            Console.WriteLine("SPLIT MARKDOWN");
            Console.WriteLine();
            Console.WriteLine("Reads a Markdown file and splits it into multiple files.");
            Console.WriteLine("Headers are treated as split points and the heading becomes");
            Console.WriteLine("the filename. Files are written to a new `_split` subfolder.");
            Console.WriteLine("Headings must only use characters supported as filenames.");
            Console.WriteLine("The heading level for the split is specified when run.");
            Console.WriteLine("There is a maximum of 999 headers allowed.");
            Console.WriteLine("If numbered files are requested they are ordered as per");
            Console.WriteLine("their appearance within the main document.");
            Console.WriteLine();
            Console.WriteLine("Arguments:");
            Console.WriteLine("  <source>     The combined Markdown content");
            Console.WriteLine("  <heading>    Eg 1 for `#` or 2 for `##`");
            Console.WriteLine("  <numbered?>  Eg Y, true, Yes, false etc");
            Console.WriteLine();
            Console.WriteLine();

            // Validation.
            if (args.Length != 3) throw new ApplicationException("Invalid number of arguments.");
            if (!File.Exists(args[0])) throw new ApplicationException($"File does not exist.");
            if (!int.TryParse(args[1], out var level)) throw new ApplicationException("The heading level should be a whole number.");
            if (level < 1 || level > 6) throw new ApplicationException("The heading level should be a number from 1 to 6.");
            var numbered = ParseBool(args[2], "numbered-files");

            // Preparing.
            var folder = Path.Join(Path.GetDirectoryName(args[0]), "_split");
            if (Directory.Exists(folder))
            {
                Console.WriteLine();
                Console.WriteLine("WARNING: existing destination folder will be overwritten!");
                Console.Write("Press any key to continue or Ctrl+C to exit ... ");
                Console.ReadKey(true);
                Directory.Delete(folder, true);
                Console.WriteLine();
            }
            Directory.CreateDirectory(folder!);

            // Pre-processing.
            var markdown = File.ReadAllLines(args[0]);
            var heading = "######".Substring(0, level) + " ";
            if (markdown.Length == 0) throw new ApplicationException("No markdown found.");

            // Processing.
            Console.WriteLine();
            var filename = Path.Join(folder, "_untitled.md");
            var structureFilename = Path.Join(folder, "_list.yaml");
            var content = new StringBuilder();
            var lineNumber = 0;
            var headingCount = 0;
            File.AppendAllText(structureFilename, "Content:\n");
            foreach (var line in markdown)
            {
                lineNumber++;

                if (line.StartsWith(heading))
                {
                    // New file.
                    headingCount++;
                    if (content.Length > 0) File.WriteAllText(filename, content.ToString());
                    if (headingCount > 999) throw new ApplicationException("Too many headings (999 max).");
                    content.Clear();
                    filename = line.Substring(heading.Length).Trim();
                    if (filename.Length == 0) throw new ApplicationException($"{lineNumber} Cannot extract filename from heading.");
                    content.AppendLine($"# {filename}");
                    Console.WriteLine($"- {filename}");
                    if (numbered) filename = headingCount.ToString("000") + " " + filename;
                    File.AppendAllText(structureFilename, $"    - {filename}\n");
                    filename = Path.Join(folder, filename + ".md");
                }
                else
                {
                    // Continue building content.
                    content.AppendLine(line);
                }
            }

            // Write the trailing content.
            if (content.Length > 0) File.WriteAllText(filename, content.ToString());

            // Finished.
            Console.WriteLine();
            Console.WriteLine($"Done: {headingCount}");
            Console.WriteLine();
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            // Errored.
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("ERROR");
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Parse a Y/N, Yes/No, T/F, or True/False.
    /// Anything else throws.
    /// </summary>
    private static bool ParseBool(string text, string reason)
    {
        if (string.IsNullOrWhiteSpace(text)) throw new ApplicationException("Invalid value for " + reason);
        switch (text.Trim().ToLowerInvariant())
        {
            case "y":
            case "yes":
            case "t":
            case "true":
                return true;
            case "n":
            case "no":
            case "f":
            case "false":
                return false;
            default:
                throw new ApplicationException("Invalid value for " + reason);
        }
    }
}

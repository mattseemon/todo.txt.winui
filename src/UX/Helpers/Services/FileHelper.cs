using Seemon.Todo.Exceptions;

namespace Seemon.Todo.Helpers.Services;

public static class FileHelper
{
    public static IList<string> ReadLinesFromFile(string path)
    {
        var lines = new List<string>();

        if (File.Exists(path))
        {
            try
            {
                var stream = File.OpenRead(path);

                using var reader = new StreamReader(stream);
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        lines.Add(line.Trim());
                    }
                }
            }
            catch (IOException ex) { throw new TaskException("An error occurred while trying to read the todo file.", ex); }
            catch (Exception) { throw; }
        }
        return lines;
    }

    public static void WriteLinesToFile(string path, IList<string> lines)
    {
        try
        {
            using StreamWriter writer = new(path);
            foreach (var task in lines)
            {
                writer.WriteLine(task);
            }
            writer.Close();
        }
        catch (IOException ex)
        {
            var message = "An erorr occurred when trying to write to the todo.txt file.";
            throw new TaskException(message, ex);
        }
        catch (Exception)
        {
            throw;
        }
    }
}

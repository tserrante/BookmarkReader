using BookmarkReader.Model;
using System.IO;
using System.Text.Json;

namespace BookmarkReader;
class Program
{
    static void Main(string[] args)
    {
        string jsonInputFile = File.ReadAllText("InputFiles//Bookmarks");
        var options = new JsonSerializerOptions
        {
            Converters = { new BookmarkElementConverter() },
            PropertyNameCaseInsensitive = true, // If you want case-insensitive property name matching
            WriteIndented = true
        };
        BookmarkModel bookmarkModel = JsonSerializer.Deserialize<BookmarkModel>(jsonInputFile, options);
        string jsonOutputString = JsonSerializer.Serialize(bookmarkModel, options);
        
        foreach(var rootElem in bookmarkModel.Roots.Values)
        {
            Console.WriteLine("Name: {0}\tType: {1}",rootElem.Name, rootElem.Type);

            foreach(var childElem in rootElem.Children)
            {
                Console.Write("\t\tName: {0}\tType: {1}", childElem.Name, childElem.Type);
                if(childElem.GetType() == typeof(Folder))
                {
                    foreach(var folderElem in childElem.Children)
                    {
                        Console.Write("\t\t\tName: {0}\tType: {1}", folderElem.Name, folderElem.Type);
                        if(childElem.GetType() == typeof(Bookmark))
                        {
                            Console.Write("\tURL: {0}", folderElem.Url);
                        }          
                    }
                    if(childElem.GetType() == typeof(Bookmark))
                    {
                        Console.Write("\tURL: {0}", childElem.Url);
                    }                
                }

                if(childElem.GetType() == typeof(Bookmark))
                {
                    Console.Write("\tURL: {0}", childElem.Url);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        // try
        // {
        //     TestClass.Test();
        // }
        // catch (Exception ex)
        // {
        //     Console.WriteLine("Failed with unhandled exception: ");
        //     Console.WriteLine(ex);
        //     throw;
        // }
        File.WriteAllText("OutputFiles//reserialized-output.json", jsonOutputString);

        Console.WriteLine("Environment version: {0} ({1}), {2}", System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription , Environment.Version, Environment.OSVersion);
        Console.WriteLine("System.Text.Json version: " + typeof(JsonSerializer).Assembly.FullName);
        Console.WriteLine();
    }
}


// class TestClass
// {
//     public static void Test()
//     {
//         //var json = GetJson();
//         var json = File.ReadAllText("Bookmarks");
//         var options = new JsonSerializerOptions
//         {
//             Converters = { new BookmarkElementConverter() },
//             // Add any additional required options here:
//             WriteIndented = true,
//         };
        
//         var model = JsonSerializer.Deserialize<BookmarkModel>(json, options);

//         var json2 = JsonSerializer.Serialize(model, options);
        
//         Console.WriteLine("Re-serialized {0}", model);
        
//         Console.WriteLine(json2);
//     }
// }

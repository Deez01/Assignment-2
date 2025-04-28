// Program.cs
//
// CECS 342 Assignment 2
// File Type Report
// Solution Template

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace FileTypeReport {
  internal static class Program {
    // 1. Enumerate all files in a folder recursively
    private static IEnumerable<string> EnumerateFilesRecursively(string path) {
      // TODO: Fill in your code here.
      // Retrieve files at top level
      foreach (string file in System.IO.Directory.EnumerateFiles(path)) {
        yield return file;
      }

      // Recursively retrieve all nested files
      foreach (string directory in System.IO.Directory.EnumerateDirectories(path)) {
        foreach (string file in EnumerateFilesRecursively(directory)) {
          yield return file;
        }
      }
      // END TODO
    }

    // Human readable byte size
    private static string FormatByteSize(long byteSize) {
        // TODO: Fill in your code here.
        string[] units = {"B", "kB", "MB", "GB", "TB", "PB", "EB", "ZB"};
        double size = byteSize;
        int idx = 0;

        while (size >= 1000 && idx < units.Length - 1) {
            size /= 1000;
            idx++;
        }
        return $"{size:0.00} {units[idx]}";
        // END TODO
    }

    // Create an HTML report file
    private static XDocument CreateReport(IEnumerable<string> files) {
      // 2. Process data
      var query =
        from file in files
        // TODO: Fill in your code here.
        let extension = Path.GetExtension(file) ?? ""
        let size = new FileInfo(file).Length
            group size by extension into g
        let count = g.Count()
        let totalSize = g.Sum()
        orderby totalSize descending
        select new {
          Type =      g.Key, // TODO: Fill in your code here.
          Count =     count, // TODO: Fill in your code here.
          TotalSize = totalSize // TODO: Fill in your code here.
        };

        // END TODOS (4)

      // 3. Functionally construct XML
      var alignment = new XAttribute("align", "right");
      var style = "table, th, td { border: 1px solid black; }";

      // TODO: Fill in your code here.
      var tableRows = query.Select(item =>
                        new XElement("tr",
                            new XElement("td", item.Type),
                            new XElement("td", item.Count),
                            new XElement("td", FormatByteSize(item.TotalSize))
                        )
                    );
      // END TODO

      var table = new XElement("table",
        new XElement("thead",
          new XElement("tr",
            new XElement("th", "Type"),
            new XElement("th", "Count"),
            new XElement("th", "Total Size"))),
        new XElement("tbody", tableRows));

      return new XDocument(
        new XDocumentType("html", null, null, null),
          new XElement("html",
            new XElement("head",
              new XElement("title", "File Report"),
              new XElement("style", style)),
            new XElement("body", table)));
    }

    // Console application with two arguments
    public static void Main(string[] args) {
      try {
        string inputFolder = args[0];
        string reportFile  = args[1];
        CreateReport(EnumerateFilesRecursively(inputFolder)).Save(reportFile);
      } catch {
        Console.WriteLine("Usage: FileTypeReport <folder> <report file>");
      }
    }
  }
}

<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

const string filesPath = @"C:\repos\";
const bool includeTestsFiles = false;
const string textToSearch = "double.MinValue";

var cSharpFiles = Directory.GetFiles(filesPath, "*.cs", SearchOption.AllDirectories);
var filesToSearch = cSharpFiles.Where(f => !f.Contains("obj") && !f.Contains("Assembly") && (includeTestsFiles || !f.Contains("Tests")));
var matches = await FindInFiles(filesToSearch);

matches.Dump();

async Task<Dictionary<string, string>> FindInFiles(IEnumerable<string> files)
{
	Dictionary<string, string> linesMatches = new Dictionary<string, string>();
	foreach (var file in filesToSearch)
	{
		var lines = await File.ReadAllLinesAsync(file);
		var linesContent = Enumerable.Range(0, lines.Length)
									 .Select(e => new { Key = e, Value = lines[e] })
									 .ToDictionary(v => v.Key, v => v.Value);
		var matches = linesContent.Where(l => l.Value.ToLowerInvariant().Contains(textToSearch.ToLowerInvariant()));
		foreach (var match in matches)
		{
			linesMatches.Add($"{file}:{match.Key}", match.Value.Trim());
		}
	}
	return linesMatches;
}
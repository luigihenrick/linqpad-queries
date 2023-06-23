<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

const bool 	INCLUDE_TESTS_FILES = false;
const string REPOSITORIES_PATH 	= @"C:\repos\";
const string SEARCH_TERM 		= "PlatformRunnableGovernor";

var matches = await Task.WhenAll(Directory.GetFiles(REPOSITORIES_PATH, "*.cs", SearchOption.AllDirectories)
									      .Where(f => !f.Contains("obj") &&
									      			  !f.Contains("Assembly") &&
									      			  (INCLUDE_TESTS_FILES || !f.Contains("Tests")))
									      .Select(filePath => FindInFile(filePath)));

matches.Where(m => m.Any()).SelectMany(m => m).GroupBy(m => m.File.Split(@"\")[2]).Select(g => new Group(g.Key, g.Select(g => g))).Dump();

async Task<IEnumerable<Match>> FindInFile(string path)
{
	var lines = await File.ReadAllLinesAsync(path);
	return Enumerable.Range(0, lines.Length)
					 .Select(e => new { Key = e, Value = lines[e] })
					 .ToDictionary(v => v.Key, v => v.Value)
					 .Where(c => c.Value.Contains(SEARCH_TERM, StringComparison.InvariantCultureIgnoreCase))
					 .Select(match => new Match($"{path}:{match.Key}", match.Value.Trim()));
}

record Group(string Project, IEnumerable<Match> Matches);
record Match(string File, string Content);
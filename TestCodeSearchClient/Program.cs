namespace SearchClient
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.Services.Client;
    using Microsoft.VisualStudio.Services.Search.WebApi;
    using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code;
    using Microsoft.VisualStudio.Services.WebApi;

    internal static class Program
    {
        public static void Main(string[] args)
        {
            const string url = @"https://account.visualstudio.com";

            Uri uri = new Uri(url);
            VssConnection connection = new VssConnection(uri, new VssClientCredentials());
            SearchHttpClient searchClient = connection.GetClient<SearchHttpClient>();

            IDictionary<string, IEnumerable<string>> filters = new Dictionary<string, IEnumerable<string>>
            {
                ["Project"] = new[]
                {
                    "<repo project>"
                },
                ["Repository"] = new[]
                {
                    "<repo name>"
                },
                ["Path"] = new[]
                {
                    ""
                },
                ["Branch"] = new[]
                {
                    "master"
                },
                // ["CodeElement"] = new[] { "def" }
            };

            CodeSearchRequest searchRequest = new CodeSearchRequest
            {
                SearchText = "def:getconfig* OR basetype:getconfig*",
                Skip = 0,
                Top = 10,
                Filters = filters,
                IncludeFacets = true
            };

            CodeSearchResponse searchResults = searchClient.FetchAdvancedCodeSearchResultsAsync(searchRequest).GetAwaiter().GetResult();

            Console.WriteLine($"Total matches = {searchResults.Count}.");
            Console.WriteLine("Results fetched:");
            int rank = 1;
            foreach (CodeResult result in searchResults.Results)
            {
                var f = Path.GetFileNameWithoutExtension(result.Filename);
                Console.WriteLine($"Rank = {rank++}, Project = {result.Project.Name}, Repository = {result.Repository.Name}, Branch = {result.Versions.FirstOrDefault()?.BranchName}, " +
                    $"FilePath = {result.Path}, HitsInFile = {result.Matches.Count}");
            }
        }
    }
}

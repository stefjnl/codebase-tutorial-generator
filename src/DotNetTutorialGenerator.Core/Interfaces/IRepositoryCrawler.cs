using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetTutorialGenerator.Core.Models;

namespace DotNetTutorialGenerator.Core.Interfaces
{
    public interface IRepositoryCrawler
    {
        Task<List<CodeFile>> CrawlRepositoryAsync(string repositoryPath, CrawlOptions options);
        Task<ProjectMetadata> ExtractProjectMetadataAsync(string repositoryPath);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetTutorialGenerator.Core.Models;

namespace DotNetTutorialGenerator.Core.Interfaces
{
    public interface IAbstractionIdentifier
    {
        Task<List<Abstraction>> IdentifyAbstractionsAsync(List<CodeFile> codeFiles);
        Task<Abstraction> AnalyzeCodeFileAsync(CodeFile codeFile);
    }
}

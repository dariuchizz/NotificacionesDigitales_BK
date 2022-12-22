using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Processor
{
    public interface ICompressUrl
    {
        Task<string> CompressAsync(IConfiguration configuration, string url);
    }
}
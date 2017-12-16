using System.Threading.Tasks;

namespace TaskSharper.Domain.RestClient
{
    public interface IStatusRestClient
    {
        Task<bool> IsAliveAsync();
    }
}

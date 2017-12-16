using System.Collections.Generic;
using System.Threading.Tasks;
using TaskSharper.Domain.Models;

namespace TaskSharper.Domain.BusinessLayer
{
    public interface ICategoryManager
    {
        Task<IList<EventCategory>> GetCategoriesAsync();
    }
}

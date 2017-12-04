using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Models;

namespace TaskSharper.Domain.DataAccessLayer
{
    public interface ICategoryRepository
    {
        List<EventCategory> GetCategories();

        Task<List<EventCategory>> GetCategoriesAsync();
    }
}

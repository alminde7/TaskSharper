using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSharper.Shared.Configuration
{
    public class Config
    {
        public static readonly TimeSpan ElasticsearchConnectionTimeout = TimeSpan.FromMilliseconds(500);
    }
}

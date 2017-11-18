using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSharper.Domain.Configuration
{
    public interface ILoggingConfiguration
    {
        bool LogLocal { get; set; }
        bool LogElasticsearch { get; set; }
        ElasticsearchConfig ElasticsearchConfiguration { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSharper.Domain.RestDTO
{
    public class ReportDto
    {
        public DateTime TaskStartTime { get; set; }
        public DateTime TaskEndTime { get; set; }
        public string Comment { get; set; }
        public bool TaskCompleted { get; set; }
        public string TaskRelatedToUserId { get; set; }
        public string TaskPerformedByUserId { get; set; }
        public string TaskPerformedByAutomatedDeviceId { get; set; }
    }
}

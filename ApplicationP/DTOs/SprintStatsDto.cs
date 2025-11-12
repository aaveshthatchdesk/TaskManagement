using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Application.DTOs
{
    public class SprintStatsDto
    {
        public int ActiveSprints { get; set; }
        public int CompletedSprints {  get; set; }
        public int TaskCompleted { get; set; }
        public int AverageVelocity { get; set; }
    }
}

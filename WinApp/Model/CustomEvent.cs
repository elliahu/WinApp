using Microsoft.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace WinApp.Model
{
    public class CustomEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string Description { get; set; }
        public Color Color { get; set; } = ColorGenerator.GetColor();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WinApp.Model
{
    public class Document
    {
        public Guid DocumentId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public Author Author { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;

    }
}

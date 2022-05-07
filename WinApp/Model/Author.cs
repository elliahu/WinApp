using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Model
{
    public class Author
    {
        // Active record
        public Guid AuthorId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Document> Documents { get; set; }

    }
}

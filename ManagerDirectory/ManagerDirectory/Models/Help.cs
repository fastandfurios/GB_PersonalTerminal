using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerDirectory.Models
{
    public class Help
    {
        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}

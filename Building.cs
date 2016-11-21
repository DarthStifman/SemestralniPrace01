using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemestralniPrace01
{
    class Building
    {

        private string Name { get; set; }
        private string Code { get; set; }
        private string Description { get; set; }

        public Building(string name, string code, string description)
        {
            Name = name;
            Code = code;
            Description = description;
        }
    }
}

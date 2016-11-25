using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemestralniPrace01
{
    public class MeetingCenter
    {


        private string Name { get; set; }
        private string Code { get; set; }
        private string Description { get; set; }

        public MeetingCenter(string name, string code, string description)
        {
            Name = name;
            Code = code;
            Description = description;
        }
    }
}

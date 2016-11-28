using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemestralniPrace01
{
    public class MeetingCenter 
    {


        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        

        public MeetingCenter(string name, string code, string description)
        {
            Name = name;
            Code = code;
            Description = description;            
        }

        //Prepsani metody ToString() tak, aby v listboxu vypsala Name a Code
        public override string ToString()
        {
            return Name + "     " + Code;
        }
    }
}

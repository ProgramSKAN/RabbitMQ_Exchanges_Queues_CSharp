using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Id.ToString() + " " + Name;
        }
    }
}

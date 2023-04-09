using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorFileUpload.Shared
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null;
        public string LastName { get; set; } = null;    
        public int Score { get; set; }
        public bool Active { get; set; }

        public DateTime DateOfBirth { get; set; }

    }
}

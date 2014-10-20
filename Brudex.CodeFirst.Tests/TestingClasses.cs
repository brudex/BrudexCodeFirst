using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brudex.CodeFirst.Tests
{

    public class Movie
    {
        public string Id;
        public string Name { get; set; }
        public string Author;
        private int AgeLimit;
        private string Director { get; set; }
        public int DirectorId;
        public Director director { get; set; }
        public List<string> actors { get; set; }
    }
    
    public class Director
    {
        public string Id;
        public string FirstName { get; set; }
        public string LastName;
        private string Unreleased = "Superman";
    }

    public class Actor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }

    public class Project
    {
        public Project()
        {
            languages = new List<string>();
        }
        
        public string id { set; get; }
        public string project_name { get; set; }
        public int project_type { get; set; }
        public bool compiled { get; set; }
        public int status { get; set; }
        public string compiled_file { get; set; }
        public DateTime date_created { get; set; }
        public bool spss_exported { get; set; }
        public bool excel_exported { get; set; }
        public bool receive_data { get; set; }
        public bool cloud_exported { get; set; }
        public string datafile_path { get; set; }
        public List<string> languages { get; set; }
        public string unique_name { get; set; }
        public string email { get; set; }
        public string companyfolder { get; set; }
    }
}

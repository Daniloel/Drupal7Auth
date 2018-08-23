using System;
using System.Collections.Generic;
using System.Text;

namespace Drupal7Auth.Models
{
    public class Roles
    {
        public string __invalid_name__2 { get; set; }
    }

    public class Name
    {
        public List<string> predicates { get; set; }
    }

    public class Homepage
    {
        public List<string> predicates { get; set; }
        public string type { get; set; }
    }

    public class RdfMapping
    {
        public List<string> rdftype { get; set; }
        public Name name { get; set; }
        public Homepage homepage { get; set; }
    }

    public class User
    {
        public string uid { get; set; }
        public string name { get; set; }
        public string mail { get; set; }
        public string theme { get; set; }
        public string signature { get; set; }
        public string signature_format { get; set; }
        public string created { get; set; }
        public string access { get; set; }
        public int login { get; set; }
        public string status { get; set; }
        public string timezone { get; set; }
        public string language { get; set; }
        public object picture { get; set; }
        public bool data { get; set; }
        public Roles roles { get; set; }
        public RdfMapping rdf_mapping { get; set; }
    }

    public class LoginModel
    {
        public string sessid { get; set; }
        public string session_name { get; set; }
        public string token { get; set; }
        public User user { get; set; }
    }

    public class Token
    {
        public string token { get; set; }
    }

    public class Connect
    {
        public string sessid { get; set; }
        public string session_name { get; set; }
        public User user { get; set; }
    }
}

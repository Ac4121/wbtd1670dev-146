using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FullStackApp.Server.Models
{
    public class SessionExport
    {
        public string Title { get; set; }
        public string StartDatetime { get; set; }

        public string EndDatetime { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LiarsDiceApi.Models
{
    public class Rules
    {
        [Key]
        public int Id { get; set; }
        public string RulesText { get; set; }

    }
}

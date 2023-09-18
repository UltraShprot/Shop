using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Data
{
    public class AppIdentityUser : IdentityUser 
    {
        public AppIdentityUser()
        {
            Messages = new HashSet<Message>();
        }
        public string ImagePath { get; set; }
        [Range(0.01f, float.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal Money { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}
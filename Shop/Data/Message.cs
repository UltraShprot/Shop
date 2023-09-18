using Microsoft.Build.Framework;

namespace Shop.Data
{
    public class Message
    {
        public int Id { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
		[Required]
		public string UserName { get; set; }
        [Required]
        public string Text { get; set; }
        public DateTime When { get; set; }  = DateTime.Now;
        public virtual AppIdentityUser Sender { get; set; }

    }
}

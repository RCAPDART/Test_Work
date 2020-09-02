using System;
using System.ComponentModel;

namespace Test_Work.Models
{
    public class Url : DbEntity
    {
        [DisplayName("Long URL")]
        public virtual string LongUrl { get; set; }
        [DisplayName("Short URL")]
        public virtual string ShortUrl { get; set; }
        [DisplayName("Creation date")]
        public virtual DateTime CreatedOn { get; set; }
        [DisplayName("Redirection count")]
        public virtual int RedirectCount { get; set; }
    }
}

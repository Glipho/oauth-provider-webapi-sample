using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Glipho.OAuth.Providers.WebApiSample.Models
{
    public class Consumer
    {
        public Consumer()
        {
        }

        public Consumer(Providers.Consumer consumer)
        {
            this.Callback = consumer.Callback;
            this.Key = consumer.Key;
            this.Name = consumer.Name;
            this.Secret = consumer.Secret;
        }

        [Required, Display(Name = "Name"), MinLength(5)]
        public string Name { get; set; }

        [Url]
        public Uri Callback { get; set; }

        [Editable(false)]
        public string Key { get; set; }

        [Editable(false)]
        public string Secret { get; set; }
    }
}
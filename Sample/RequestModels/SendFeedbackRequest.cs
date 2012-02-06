using System;
using System.ComponentModel.DataAnnotations;

namespace Sample
{
    public class SendFeedbackRequest
    {
        [Required, DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        
        [Required]
        public string Feedback { get; set; }
    }
}
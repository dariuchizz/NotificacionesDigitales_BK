using System;
using System.ComponentModel.DataAnnotations;

namespace Common.Model.Response
{
    public class TokenResponseWeb
    {
        [Display(Name = "Module which the permission is ask for")]
        [DataType(DataType.Text)]
        [Required]
        public string Module { get; set; }

        [Display(Name = "Expiration Time")]
        [DataType(DataType.DateTime)]
        [Required]
        public DateTime Expiration { get; set; }

        [Display(Name = "JWT Token")]
        [DataType(DataType.Text)]
        [Required]
        public string Token { get; set; }

        [Display(Name = "Audience which the token is for")]
        [DataType(DataType.Text)]
        [Required]
        public string Audience { get; set; }
    }
}
using Microsoft.EntityFrameworkCore;
using SlimeStoreWeb.Data;
using System.ComponentModel.DataAnnotations;

namespace SlimeStoreWeb.Models
{
    public class OrderViewModel : OrderCreateViewModel 
    {
        public int Id { get; set; }
       

    }
}

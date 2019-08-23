using System;
using Notification.Core.Enums;

namespace Notification.Core.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Cpf { get; set; }
        public RegisterStatus RegisterStatus { get; set; }
    }
}
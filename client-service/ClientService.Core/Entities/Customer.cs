using System;
using ClientService.Core.Enums;

namespace ClientService.Core.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }

        public String Name { get; set; }

        public String Cpf { get; set; }

        public RegisterStatus RegisterStatus { get; set; }
    }
}
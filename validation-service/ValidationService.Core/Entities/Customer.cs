using System;
using ValidationService.Core.Enums;

namespace ValidationService.Core.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Cpf { get; set; }

        public RegisterStatus RegisterStatus { get; set; } 
    }
}
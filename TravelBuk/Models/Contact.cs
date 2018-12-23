﻿using System.ComponentModel.DataAnnotations;

namespace TravelBuk.Models {
    public class Contact {
        public long ContactId { get; set; }

        // user ID from AspNetUser table.
        public string OwnerID { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public ContactStatus Status { get; set; }
    }

    public enum ContactStatus {
        Submitted,
        Approved,
        Rejected
    }
}

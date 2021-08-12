using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MythicNights.DataContext
{
    public class MythicNight
    {
        [Key]
        public ulong Id { get; set; }
        public DateTimeOffset EventTime { get; set; }

        public List<ulong> Attending { get; set; }

        //[NotMapped]
        //public List<ulong> Attending {
        //    get
        //    {
        //        return AttendingBlob.Split(',').Where(u=>!string.IsNullOrEmpty(u)).Select(u=>ulong.Parse(u)).ToList();
        //    }
        //    set
        //    {
        //        AttendingBlob = string.Join(",", value.ToArray());
        //    } 
        //}
        //public string AttendingBlob { get; set; }
    }
} 

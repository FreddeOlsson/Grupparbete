﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KalmarBSK.DataAccess
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class KlubbdatabasEntities2 : DbContext
    {
        public KlubbdatabasEntities2()
            : base("name=KlubbdatabasEntities2")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<GameLocation> GameLocations { get; set; }
        public virtual DbSet<MeetingParticipant> MeetingParticipants { get; set; }
        public virtual DbSet<Personer> Personers { get; set; }
    }
}

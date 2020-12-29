﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Backlogger.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class BackloggerEntities : DbContext
    {
        public BackloggerEntities()
            : base("name=BackloggerEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<Hobby> Hobbies { get; set; }
        public virtual DbSet<MaterialFormat> MaterialFormats { get; set; }
        public virtual DbSet<Material> Materials { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<StatusUpdate> StatusUpdates { get; set; }
        public virtual DbSet<Subscription> Subscriptions { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<BooksSubscription> BooksSubscriptions { get; set; }
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<GamesSubscription> GamesSubscriptions { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<MoviesSubscription> MoviesSubscriptions { get; set; }
    
        [DbFunction("BackloggerEntities", "ConcatenateAuthors")]
        public virtual IQueryable<ConcatenateAuthors_Result> ConcatenateAuthors(Nullable<int> materialID)
        {
            var materialIDParameter = materialID.HasValue ?
                new ObjectParameter("MaterialID", materialID) :
                new ObjectParameter("MaterialID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<ConcatenateAuthors_Result>("[BackloggerEntities].[ConcatenateAuthors](@MaterialID)", materialIDParameter);
        }
    
        [DbFunction("BackloggerEntities", "ConcatenateGenres")]
        public virtual IQueryable<ConcatenateGenres_Result> ConcatenateGenres(Nullable<int> materialID)
        {
            var materialIDParameter = materialID.HasValue ?
                new ObjectParameter("MaterialID", materialID) :
                new ObjectParameter("MaterialID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<ConcatenateGenres_Result>("[BackloggerEntities].[ConcatenateGenres](@MaterialID)", materialIDParameter);
        }
    
        [DbFunction("BackloggerEntities", "LastStatusUpdate")]
        public virtual IQueryable<LastStatusUpdate_Result> LastStatusUpdate(Nullable<int> materialID)
        {
            var materialIDParameter = materialID.HasValue ?
                new ObjectParameter("MaterialID", materialID) :
                new ObjectParameter("MaterialID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<LastStatusUpdate_Result>("[BackloggerEntities].[LastStatusUpdate](@MaterialID)", materialIDParameter);
        }
    }
}

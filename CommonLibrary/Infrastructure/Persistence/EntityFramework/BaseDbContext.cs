using CommonLibrary.Core.Domain;
using CommonLibrary.Core.Domain.Entities;
using CommonLibrary.Core.Domain.Enum;
using CommonLibrary.Core.Domain.Interfaces;
using CommonLibrary.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Reflection;

namespace CommonLibrary.Infrastructure.Persistence.EntityFramework
{
    public class BaseDbContext : DbContext, IUnitOfWork
    {
        public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options)
        {
        }
        public new ResultObject<int> SaveChanges()
        {
            var result = new ResultObject<int>();
            var changedEntities = ChangeTracker.Entries().Where(_ => _.State
                              == EntityState.Added || _.State == EntityState.Modified);

            var errors = validateEntity(changedEntities);
            if (errors.Count > 0)
            {
                errors.ForEach(p => result.ServerErrors.Add(
                        new ServerError()
                        {
                            Hint = p.ErrorMessage,
                            Type = (int)ErrorTypeEnum.Validation,
                            Code = (int)HttpStatusCode.BadRequest
                        }));
                return result;
            }
            SetRegMomentOnAdd(ChangeTracker.Entries());
            var res = base.SaveChanges();
            return result;
        }
        public async Task<ResultObject<int>> SaveChangesAsync()
        {
            try
            {
                var result = new ResultObject<int>();
                var changedEntities = ChangeTracker.Entries().Where(_ => _.State
                     == EntityState.Added || _.State == EntityState.Modified);

                var errors = ValidateEntity(changedEntities);
                if (errors.Count > 0)
                {
                    errors.ForEach(p => result.ServerErrors.Add(
                        new ServerError()
                        {
                            Hint = p.ErrorMessage,
                            Type = (int)ErrorTypeEnum.Validation,
                            Code = (int)HttpStatusCode.BadRequest
                        }));
                    return result;
                }

                SetRegMomentOnAdd(ChangeTracker.Entries());
                //SetCreatorOnAdd(ChangeTracker.Entries());
                var res = await base.SaveChangesAsync();
                result.Data = res;
                return result;

            }
            catch (Exception e)
            {

                throw;
            }
        }

        public virtual async Task AddOrUpdateAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            try
            {
                TrimAllEntityStrings(entity);
                if (entity.IsItNew)
                    await this.AddAsync(entity);
                else
                {
                    var dbEntity = await this.Set<TEntity>().FindAsync(entity.ID);
                    this.Entry(dbEntity).CurrentValues.SetValues(entity);
                    this.Update(dbEntity);
                }

            }
            catch (Exception e)
            {
                throw;
            }
        }
        private void TrimAllEntityStrings<TEntity>(TEntity entity)
        {

            //foreach (var property in entity.GetType().GetProperties(BindingFlags.Public).Where(x =>
            //  x.PropertyType == typeof(string) &&
            //  x.GetCustomAttributes(typeof(TrimAttribute), true).Any()))
            //{
            //    property.SetValue(entity, (property.GetValue(entity) as string).Trim());
            //}

            foreach (var property in entity.GetType().GetProperties(BindingFlags.Public).Where(x => x.PropertyType == typeof(string)))
            {
                property.SetValue(entity, (property.GetValue(entity) as string)?.Trim());
            }
        }
        private List<ValidationResult> validateEntity(IEnumerable<EntityEntry> changedEntities)
        {
            var errors = new List<ValidationResult>();
            foreach (var e in changedEntities)
            {
                var vc = new ValidationContext(e.Entity, null, null);
                Validator.TryValidateObject(
                    e.Entity, vc, errors, validateAllProperties: true);
            }
            return errors;
        }
        public override DbSet<TEntity> Set<TEntity>() where TEntity : class => base.Set<TEntity>();

        public override DatabaseFacade Database { get => base.Database; }

        private void SetRegMomentOnAdd(IEnumerable<EntityEntry> entries)
        {
            entries = entries.Where(e => e.Entity is IRegMoment &&
                e.State == EntityState.Added);
            foreach (var entityEntry in entries)
            {
                ((IRegMoment)entityEntry.Entity).RegMoment = DateTime.Now;
            }
        }
        private void SetCreatorOnAdd(IEnumerable<EntityEntry> entries, int userID = 0)
        {
            //entries = entries.Where(e => e.Entity is ICreator &&
            //    e.State == EntityState.Added);
            //foreach (var entityEntry in entries)
            //{
            //    ((ICreator)entityEntry.Entity).CreationDate = DateTime.Now;
            //    ((ICreator)entityEntry.Entity).CreatorUserID = userID;
            //}
        }
        private List<ValidationResult> ValidateEntity(IEnumerable<EntityEntry> changedEntities)
        {
            var errors = new List<ValidationResult>();
            foreach (var e in changedEntities)
            {
                var vc = new ValidationContext(e.Entity, null, null);
                Validator.TryValidateObject(
                    e.Entity, vc, errors, validateAllProperties: true);
            }
            return errors;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Oqtane.Extensions;
using Oqtane.Infrastructure;
using Oqtane.Interfaces;
using Oqtane.Migrations.Framework;
using Oqtane.Models;
using Oqtane.Shared;

// ReSharper disable BuiltInTypeReferenceStyleForMemberAccess

namespace Oqtane.Repository
{
    public class DBContextBase :  IdentityUserContext<IdentityUser>
    {
        private readonly ITenantResolver _tenantResolver;
        private readonly ITenantManager _tenantManager;
        private readonly IHttpContextAccessor _accessor;
        private string _connectionString;
        private string _databaseType;

        public DBContextBase(ITenantManager tenantManager, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = String.Empty;
            _tenantManager = tenantManager;
            _accessor = httpContextAccessor;
        }

        public IOqtaneDatabase ActiveDatabase { get; private set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ReplaceService<IMigrationsAssembly, MultiDatabaseMigrationsAssembly>();

            if (string.IsNullOrEmpty(_connectionString))
            {

                Tenant tenant;
                if (_tenantResolver != null)
                {
                    tenant = _tenantResolver.GetTenant();
                }
                else
                {
                    tenant = _tenantManager.GetTenant();
                }

                if (tenant != null)
                {
                    _connectionString = tenant.DBConnectionString
                        .Replace("|DataDirectory|", AppDomain.CurrentDomain.GetData("DataDirectory")?.ToString());
                    _databaseType = tenant.DBType;
                }
            }

            if (!String.IsNullOrEmpty(_databaseType))
            {
                var type = Type.GetType(_databaseType);
                ActiveDatabase = Activator.CreateInstance(type) as IOqtaneDatabase;
            }

            if (!string.IsNullOrEmpty(_connectionString) && ActiveDatabase != null)
            {
                optionsBuilder.UseOqtaneDatabase(ActiveDatabase, _connectionString);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ActiveDatabase.UpdateIdentityStoreTableNames(builder);
        }

        public override int SaveChanges()
        {
            DbContextUtils.SaveChanges(this, _accessor);

            return base.SaveChanges();
        }

        [Obsolete("This constructor is obsolete. Use DBContextBase(ITenantManager tenantManager, IHttpContextAccessor httpContextAccessor) instead.", false)]
        public DBContextBase(ITenantResolver tenantResolver, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = String.Empty;
            _tenantResolver = tenantResolver;
            _accessor = httpContextAccessor;
        }
    }
}

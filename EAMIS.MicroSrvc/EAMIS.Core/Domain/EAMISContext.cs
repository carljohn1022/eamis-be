using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Domain.Entities.AIS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain
{
    public class EAMISContext : DbContext
    {
        public EAMISContext(DbContextOptions<EAMISContext> options) : base(options)
        {

        }

        public DbSet<EAMISREGION> EAMIS_REGION { get; set; }
        public DbSet<EAMISPROVINCE> EAMIS_PROVINCE { get; set; }
        public DbSet<EAMISMUNICIPALITY> EAMIS_MUNICIPALITY { get; set; }
        public DbSet<EAMISBARANGAY> EAMIS_BARANGAY { get; set; }
        public DbSet<EAMISUSERS> EAMIS_USERS { get; set; }
        public DbSet<EAMISROLES> EAMIS_ROLES { get; set; }
        public DbSet<EAMISUSERROLES> EAMIS_USER_ROLES { get; set; }
        public DbSet<EAMISPERSONNELINFO> EAMIS_APP_USER_INFO { get; set; }
        public DbSet<EAMISUSERLOGIN> EAMIS_USER_LOGIN { get; set; }
        public DbSet<EAMISATTACHMENTS> EAMIS_ATTACHMENTS { get; set; }
        public DbSet<EAMISATTACHMENTTYPE> EAMIS_ATTACHMENT_TYPE { get; set; }
        public DbSet<EAMISPROPERTYDETAILS> EAMIS_PROPERTY_DETAILS { get; set; }
        public DbSet<EAMISPRECONDITIONS> EAMIS_PPECONDITIONS { get; set; }
        public DbSet<EAMISUNITOFMEASURE> EAMIS_UNITOFMEASURE { get; set; }
        public DbSet<EAMISWAREHOUSE> EAMIS_WAREHOUSE { get;set; }
        public DbSet<EAMISPROCUREMENTCATEGORY> EAMIS_PROCUREMENTCATEGORY { get; set; }
        public DbSet<EAMISPROPERTYITEMS> EAMIS_PROPERTYITEMS { get; set; }
        public DbSet<EAMISENVIRONMENTALIMPACTS> EAMIS_ENVIRONMENTALIMPACTS { get; set; }
        public DbSet<EAMISSIGNIFICANTENVIRONMENTALASPECTS> EAMIS_ENVIRONMENTALASPECTS { get; set; }
        public DbSet<EAMISPROPERTYTYPE> EAMIS_PROPERTY_TYPE { get; set; }
        public DbSet<EAMISCHARTOFACCOUNTS> EAMIS_CHART_OF_ACCOUNTS { get; set; }
        public DbSet<EAMISPROPERTYDEPRECIATION> EAMIS_PROPERTY_DEPRECIATION { get; set; }
        public DbSet<EAMISPROPERTYTRANSFERDETAILS> EAMIS_PROPERTY_TRANSFER_DETAILS { get; set; }
        public DbSet<EAMISNOTICEOFDELIVERY> EAMIS_NOTICE_OF_DELIVERY { get; set; }
        public DbSet<EAMISSUPPLIER> EAMIS_SUPPLIER { get; set; }
        public DbSet<EAMISITEMSUBCATEGORY> EAMIS_ITEMS_SUB_CATEGORY { get; set; }
        public DbSet<EAMISITEMCATEGORY> EAMIS_ITEM_CATEGORY { get; set; }
        public DbSet<EAMISFUNDSOURCE> EAMIS_FUND_SOURCE { get; set; }
        public DbSet<EAMISGENERALFUNDSOURCE> EAMIS_GENERAL_FUND_SOURCE { get; set; }

        public DbSet<EAMISCLASSIFICATION> EAMIS_CLASSIFICATION { get; set; }
        public DbSet<EAMISSUBCLASSIFICATION> EAMIS_SUB_CLASSIFICATION { get; set; }
        public DbSet<EAMISGROUPCLASSIFICATION> EAMIS_GROUP_CLASSIFICATION { get; set; }
        
        public DbSet<EAMISGENERALCHARTOFACCOUNTS> EAMIS_GENERAL_CHART_OF_ACCOUNTS { get; set; }
        public DbSet<EAMISFINANCINGSOURCE> EAMIS_FINANCING_SOURCE { get; set; }
        public DbSet<EAMISAUTHORIZATION> EAMIS_AUTHORIZATION { get; set; }
        public DbSet<EAMISRESPONSIBILITYCODE> EAMIS_RESPONSIBILITY_CODE { get; set; }
        public DbSet<EAMISRESPONSIBILITYCENTER> EAMIS_RESPONSIBILITY_CENTER { get; set; }

        //Transaction
      
        public DbSet<EAMISDELIVERYRECEIPT> EAMIS_DELIVERY_RECEIPT { get; set; }
        public DbSet<EAMISDELIVERYRECEIPTDETAILS> EAMIS_DELIVERY_RECEIPT_DETAILS { get; set; }
        public DbSet<EAMISPROPERTYTRANSACTION> EAMIS_PROPERTY_TRANSACTION { get; set; }
        public DbSet<EAMISPROPERTYTRANSACTIONDETAILS> EAMIS_PROPERTY_TRANSACTION_DETAILS { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<EAMISATTACHMENTTYPE>()
            //    .HasOne(x => x.ATTACHMENTS)
            //    .WithMany(x => x.ATTACHMENTTYPE)
            //    .HasForeignKey(x => x.ID);

            modelBuilder.Entity<EAMISPROPERTYTRANSACTIONDETAILS>()
                .HasOne(x => x.DELIVERYRECEIPT_GROUP)
                .WithMany(x => x.PROPERTYTRANSACTIONDETAILS)
                .HasForeignKey(x => x.DELIVERY_RECEIPT_ID);

            modelBuilder.Entity<EAMISPROPERTYTRANSACTIONDETAILS>()
               .HasOne(x => x.PROPERTYTRANSACTION)
               .WithMany(x => x.PROPERTYTRANSACTIONDETAILS)
               .HasForeignKey(x => x.PROPERTY_TRANS_ID);



            modelBuilder.Entity<EAMISFUNDSOURCE>()
                .HasOne(x => x.FINANCING_SOURCE)
                .WithMany(x => x.FUND_SOURCE)
                .HasForeignKey(x => x.FINANCING_SOURCE_ID);

            modelBuilder.Entity<EAMISFUNDSOURCE>()
                .HasOne(x => x.AUTHORIZATION)
                .WithMany(x => x.FUND_SOURCE)
                .HasForeignKey(x => x.AUTHORIZATION_ID);

            modelBuilder.Entity<EAMISPROPERTYITEMS>()
                .HasOne(x => x.ITEM_CATEGORY)
                .WithMany(x => x.PROPERTY_ITEMS)
                .HasForeignKey(x => x.CATEGORY_ID);

            modelBuilder.Entity<EAMISPROPERTYITEMS>()
                .HasOne(x => x.UOM_GROUP)
                .WithMany(x => x.PROPERTY_ITEM)
                .HasForeignKey(x => x.UOM_ID);

            modelBuilder.Entity<EAMISPROPERTYITEMS>()
                .HasOne(x => x.WAREHOUSE_GROUP)
                .WithMany(x => x.PROPERTY_ITEMS)
                .HasForeignKey(x => x.WAREHOUSE_ID);

            modelBuilder.Entity<EAMISPROPERTYITEMS>()
                .HasOne(x => x.SUPPLIER_GROUP)
                .WithMany(x => x.PROPERTY_ITEMS)
                .HasForeignKey(x => x.SUPPLIER_ID);

            modelBuilder.Entity<EAMISITEMCATEGORY>()
                .HasOne(x => x.CHART_OF_ACCOUNTS)
                .WithMany(x => x.ITEM_CATEGORY)
                .HasForeignKey(x => x.CHART_OF_ACCOUNT_ID);

            modelBuilder.Entity<EAMISDELIVERYRECEIPT>()
                .HasOne(x => x.WAREHOUSE_GROUP)
                .WithMany(x => x.DELIVERY_RECEIPT)
                .HasForeignKey(x => x.WAREHOUSE_ID);

            modelBuilder.Entity<EAMISDELIVERYRECEIPT>()
                .HasOne(x => x.SUPPLIER_GROUP)
                .WithMany(x => x.DELIVERY_RECEIPT)
                .HasForeignKey(x => x.SUPPLIER_ID);

            modelBuilder.Entity<EAMISDELIVERYRECEIPTDETAILS>()
                .HasOne(x => x.ITEMS_GROUP)
                .WithMany(x => x.DELIVERY_RECEIPT_DETAILS)
                .HasForeignKey(x => x.ITEM_ID);
            //modelBuilder.Entity<EAMISITEMCATEGORY>()
            //    .HasOne(x => x.CHART_OF_ACCOUNTS)
            //    .WithOne(x => x.ITEM_CATEGORY)
            //    .HasPrincipalKey<EAMISCHARTOFACCOUNTS>(x => x.ACCOUNT_CODE)
            //    .HasForeignKey<EAMISITEMCATEGORY>(x => x.ACCOUNT_CODE);

            //modelBuilder.Entity<EAMISCHARTOFACCOUNTS>()
            //    .HasIndex(x => x.ACCOUNT_CODE);

            //modelBuilder.Entity<EAMISITEMCATEGORY>()
            //    .HasIndex(x => x.ACCOUNT_CODE);

            modelBuilder.Entity<EAMISREGION>()
                .HasIndex(x => x.REGION_CODE)
                .IsUnique();
            modelBuilder.Entity<EAMISPROVINCE>()
                .HasIndex(x => x.PROVINCE_CODE)
                .IsUnique();
            modelBuilder.Entity<EAMISMUNICIPALITY>()
                .HasIndex(x => x.MUNICIPALITY_CODE)
                .IsUnique();
            modelBuilder.Entity<EAMISBARANGAY>()
                .HasIndex(x => x.BRGY_CODE) 
                .IsUnique();       

            modelBuilder.Entity<EAMISUNITOFMEASURE>()
                .HasIndex(a => a.SHORT_DESCRIPTION)
                .IsUnique();
            modelBuilder.Entity<EAMISUNITOFMEASURE>()
                .HasIndex(a => a.UOM_DESCRIPTION)
                .IsUnique();
            modelBuilder.Entity<EAMISMUNICIPALITY>()
                .HasIndex(x => x.MUNICIPALITY_CODE)
                .IsUnique();
            modelBuilder.Entity<EAMISREGION>()
                .HasIndex(x => x.REGION_CODE)
                .IsUnique();
            modelBuilder.Entity<EAMISBARANGAY>()
                .HasIndex(x => x.BRGY_CODE);
            modelBuilder.Entity<EAMISPROVINCE>()
                .HasIndex(x => x.PROVINCE_CODE);

            //modelBuilder.Entity<EAMISUSERS>()
            //    .HasOne(x => x.PERSONNEL_INFO)
            //    .WithOne(x => x.User)
            //    .HasForeignKey<EAMISUSERS>(x => x.PERSONNEL_INFO_ID);


          

            modelBuilder.Entity<EAMISGROUPCLASSIFICATION>()
                .HasOne(x => x.CLASSIFICATION)
                .WithMany(x => x.GROUPCLASSIFICATION)
                .HasForeignKey(x => x.CLASSIFICATION_ID);


            modelBuilder.Entity<EAMISSUBCLASSIFICATION>()
                .HasOne(a => a.CLASSIFICATION)
                .WithMany(b => b.SUBCLASSIFICIATION)
                .HasForeignKey(c => c.CLASSIFICATION_ID);

            modelBuilder.Entity<EAMISGROUPCLASSIFICATION>()
                .HasOne(a => a.SUBCLASSIFICATION)
                .WithMany(b => b.GROUPCLASSIFICATION)
                .HasForeignKey(c => c.SUB_CLASSIFICATION_ID);
            
            modelBuilder.Entity<EAMISFUNDSOURCE>()
                 .HasOne(q => q.GENERALFUNDSOURCE)
                 .WithMany(q => q.FUNDSOURCE)
                 .HasForeignKey(z => z.GENERAL_FUND_SOURCE_ID)
                 .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EAMISCHARTOFACCOUNTS>()
                .HasOne(a => a.GROUPCLASSIFICATION)
                .WithMany(b => b.CHARTOFACCOUNTS)
                .HasForeignKey(x => x.GROUP_ID);
           

            modelBuilder.Entity<EAMISITEMSUBCATEGORY>()
                 .HasOne(q => q.ITEM_CATEGORY)
                 .WithMany(x => x.ITEM_SUB_CATEGORY)
                 .HasForeignKey(z => z.CATEGORY_ID);

            modelBuilder.Entity<EAMISUSERROLES>()
                .HasOne(b => b.USERS)
                .WithMany(x => x.USER_ROLES)
                .HasForeignKey(c => c.USER_ID);


            //modelBuilder.Entity<EAMISPROPERTYDETAILS>()
            //    .HasOne(x => x.UOM)
            //    .WithOne(x => x.PROPERTY_DETAILS)
            //    .HasForeignKey<EAMISPROPERTYDETAILS>(c => c.PROPERTY_TYPE_ID);

            modelBuilder.Entity<EAMISUSERS>()
                .HasIndex(u => u.AGENCY_EMPLOYEE_NUMBER)
                .IsUnique();

            modelBuilder.Entity<EAMISUSERROLES>()
               .HasOne(x => x.ROLES)
               .WithMany(b => b.USERROLES)
               .HasForeignKey(z => z.ROLE_ID);

            modelBuilder.Entity<EAMISUSERLOGIN>()
                .HasOne(x => x.EAMIS_USERS)
                .WithMany(b => b.EAMISUSER_LOGIN)
                .HasForeignKey(b => b.USER_ID);

    



        }
    }
}

﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using asagiv.datapush.ui.Utilities;

namespace asagiv.datapush.ui.Migrations
{
    [DbContext(typeof(WinUiDataPushDbContext))]
    partial class WinUiDataPushDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.9");

            modelBuilder.Entity("asagiv.datapush.common.Models.ClientConnectionSettings", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConnectionName")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConnectionString")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsPullNode")
                        .HasColumnType("INTEGER");

                    b.Property<string>("NodeName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ConnectionSettingsSet");
                });
#pragma warning restore 612, 618
        }
    }
}
﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Squil;

#nullable disable

namespace Squil.Migrations
{
    [DbContext(typeof(Db))]
    [Migration("20221112195029_ModifiedAt")]
    partial class ModifiedAt
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.10");

            modelBuilder.Entity("Squil.SqlServerHostConfiguration", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Catalog")
                        .HasMaxLength(60)
                        .HasColumnType("TEXT");

                    b.Property<string>("Host")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .HasMaxLength(60)
                        .HasColumnType("TEXT");

                    b.Property<bool>("UseWindowsAuthentication")
                        .HasColumnType("INTEGER");

                    b.Property<string>("User")
                        .HasMaxLength(60)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("SqlServerHostConfigurations");
                });
#pragma warning restore 612, 618
        }
    }
}

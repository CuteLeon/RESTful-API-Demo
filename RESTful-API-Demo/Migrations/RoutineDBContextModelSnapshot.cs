﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RESTful_API_Demo.Data;

namespace RESTful_API_Demo.Migrations
{
    [DbContext(typeof(RoutineDBContext))]
    partial class RoutineDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2");

            modelBuilder.Entity("RESTful_API_Demo.Entities.Company", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Introduction")
                        .HasColumnType("TEXT")
                        .HasMaxLength(500);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("Companies");

                    b.HasData(
                        new
                        {
                            Id = new Guid("bbdee09c-089b-4d30-bece-44df5923716c"),
                            Introduction = "Great Company",
                            Name = "Microsoft"
                        },
                        new
                        {
                            Id = new Guid("6fb600c1-9011-4fd7-9234-881379716440"),
                            Introduction = "Don't be evil",
                            Name = "Google"
                        },
                        new
                        {
                            Id = new Guid("5efc910b-2f45-43df-afae-620d40542853"),
                            Introduction = "Fubao Company",
                            Name = "Alibaba"
                        });
                });

            modelBuilder.Entity("RESTful_API_Demo.Entities.Employee", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("CompanyId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("TEXT");

                    b.Property<string>("EmployeeNo")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(10);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<int>("Gender")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("Employees");

                    b.HasData(
                        new
                        {
                            Id = new Guid("4b501cb3-d168-4cc0-b375-48fb33f318a4"),
                            CompanyId = new Guid("6fb600c1-9011-4fd7-9234-881379716440"),
                            DateOfBirth = new DateTime(1986, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EmployeeNo = "G003",
                            FirstName = "Mary",
                            Gender = 0,
                            LastName = "King"
                        },
                        new
                        {
                            Id = new Guid("7eaa532c-1be5-472c-a738-94fd26e5fad6"),
                            CompanyId = new Guid("6fb600c1-9011-4fd7-9234-881379716440"),
                            DateOfBirth = new DateTime(1977, 4, 6, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EmployeeNo = "G097",
                            FirstName = "Kevin",
                            Gender = 1,
                            LastName = "Richardson"
                        });
                });

            modelBuilder.Entity("RESTful_API_Demo.Entities.Employee", b =>
                {
                    b.HasOne("RESTful_API_Demo.Entities.Company", "Company")
                        .WithMany("Employees")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}

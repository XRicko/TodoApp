﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using ToDoList.Infrastructure.Data;

namespace ToDoList.Infrastructure.Migrations
{
    [DbContext(typeof(TodoListContext))]
    [Migration("20210319124754_UniqueConstraintsForTodoItem")]
    partial class UniqueConstraintsForTodoItem
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ToDoList.Core.Entities.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(75)
                        .HasColumnType("nvarchar(75)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Category");
                });

            modelBuilder.Entity("ToDoList.Core.Entities.Checklist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasMaxLength(75)
                        .HasColumnType("nvarchar(75)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("Name", "UserId")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.ToTable("Checklist");
                });

            modelBuilder.Entity("ToDoList.Core.Entities.Image", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasMaxLength(260)
                        .HasColumnType("nvarchar(260)");

                    b.HasKey("Id");

                    b.HasIndex("Path")
                        .IsUnique();

                    b.ToTable("Image");
                });

            modelBuilder.Entity("ToDoList.Core.Entities.Status", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsDone")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(75)
                        .HasColumnType("nvarchar(75)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Status");
                });

            modelBuilder.Entity("ToDoList.Core.Entities.TodoItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<int>("ChecklistId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DueDate")
                        .HasColumnType("datetime2");

                    b.Property<Point>("GeoPoint")
                        .HasColumnType("geography");

                    b.Property<int?>("ImageId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("StatusId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ChecklistId");

                    b.HasIndex("ImageId");

                    b.HasIndex("ParentId");

                    b.HasIndex("StatusId");

                    b.HasIndex("Name", "ChecklistId")
                        .IsUnique();

                    b.ToTable("Task");
                });

            modelBuilder.Entity("ToDoList.Core.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasMaxLength(125)
                        .HasColumnType("nvarchar(125)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Name", "Password")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL AND [Password] IS NOT NULL");

                    b.ToTable("User");
                });

            modelBuilder.Entity("ToDoList.Core.Entities.Checklist", b =>
                {
                    b.HasOne("ToDoList.Core.Entities.User", "User")
                        .WithMany("Checklists")
                        .HasForeignKey("UserId")
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ToDoList.Core.Entities.TodoItem", b =>
                {
                    b.HasOne("ToDoList.Core.Entities.Category", "Category")
                        .WithMany("TodoItems")
                        .HasForeignKey("CategoryId");

                    b.HasOne("ToDoList.Core.Entities.Checklist", "Checklist")
                        .WithMany("TodoItems")
                        .HasForeignKey("ChecklistId")
                        .IsRequired();

                    b.HasOne("ToDoList.Core.Entities.Image", "Image")
                        .WithMany("TodoItems")
                        .HasForeignKey("ImageId");

                    b.HasOne("ToDoList.Core.Entities.TodoItem", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.HasOne("ToDoList.Core.Entities.Status", "Status")
                        .WithMany("TodoItems")
                        .HasForeignKey("StatusId");

                    b.Navigation("Category");

                    b.Navigation("Checklist");

                    b.Navigation("Image");

                    b.Navigation("Parent");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("ToDoList.Core.Entities.Category", b =>
                {
                    b.Navigation("TodoItems");
                });

            modelBuilder.Entity("ToDoList.Core.Entities.Checklist", b =>
                {
                    b.Navigation("TodoItems");
                });

            modelBuilder.Entity("ToDoList.Core.Entities.Image", b =>
                {
                    b.Navigation("TodoItems");
                });

            modelBuilder.Entity("ToDoList.Core.Entities.Status", b =>
                {
                    b.Navigation("TodoItems");
                });

            modelBuilder.Entity("ToDoList.Core.Entities.TodoItem", b =>
                {
                    b.Navigation("Children");
                });

            modelBuilder.Entity("ToDoList.Core.Entities.User", b =>
                {
                    b.Navigation("Checklists");
                });
#pragma warning restore 612, 618
        }
    }
}
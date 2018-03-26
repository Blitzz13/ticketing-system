﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;
using TicketingSystem.Data;

namespace TicketingSystem.Data.Migrations
{
    [DbContext(typeof(TicketingSystemDbContext))]
    partial class TicketingSystemDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TicketingSystem.Data.File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("Content");

                    b.Property<int?>("MessageId");

                    b.Property<string>("Name");

                    b.Property<int?>("TicketId");

                    b.HasKey("Id");

                    b.HasIndex("MessageId");

                    b.HasIndex("TicketId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("TicketingSystem.Data.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<DateTime>("PublishingDate");

                    b.Property<int>("State");

                    b.Property<int?>("TicketId");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("TicketId");

                    b.HasIndex("UserId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("TicketingSystem.Data.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAlternateKey("Name")
                        .HasName("AlternateKey_ProjectName");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("TicketingSystem.Data.Ticket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<int>("ProjectId");

                    b.Property<int>("State");

                    b.Property<DateTime>("SubmissionDate");

                    b.Property<int?>("SubmitterId");

                    b.Property<string>("Title");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("SubmitterId");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("TicketingSystem.Data.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccountState");

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<string>("Password");

                    b.Property<int>("Role");

                    b.Property<string>("Username")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAlternateKey("Username")
                        .HasName("AlternateKey_Username");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TicketingSystem.Data.File", b =>
                {
                    b.HasOne("TicketingSystem.Data.Message", "Message")
                        .WithMany("Files")
                        .HasForeignKey("MessageId");

                    b.HasOne("TicketingSystem.Data.Ticket", "Ticket")
                        .WithMany("Files")
                        .HasForeignKey("TicketId");
                });

            modelBuilder.Entity("TicketingSystem.Data.Message", b =>
                {
                    b.HasOne("TicketingSystem.Data.Ticket")
                        .WithMany("Messages")
                        .HasForeignKey("TicketId");

                    b.HasOne("TicketingSystem.Data.User", "User")
                        .WithMany("Messages")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TicketingSystem.Data.Ticket", b =>
                {
                    b.HasOne("TicketingSystem.Data.Project", "Project")
                        .WithMany("Tickets")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TicketingSystem.Data.User", "Submitter")
                        .WithMany()
                        .HasForeignKey("SubmitterId");
                });
#pragma warning restore 612, 618
        }
    }
}

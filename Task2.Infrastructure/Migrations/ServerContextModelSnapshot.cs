﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Task2.Infrastructure.Context;

#nullable disable

namespace Task2.Infrastructure.Migrations
{
    [DbContext(typeof(ServerContext))]
    partial class ServerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("public")
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Task2.Infrastructure.Context.FileModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ServerId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.ToTable("Files", "public");
                });

            modelBuilder.Entity("Task2.Infrastructure.Context.ServeModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Port")
                        .HasColumnType("integer");

                    b.Property<int>("ServerType")
                        .HasColumnType("integer");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Servers", "public");

                    b.HasData(
                        new
                        {
                            Id = "62CD1744-DD07-4BB4-8EE0-FC1359E2C278",
                            Name = "LocalFile1Server",
                            Password = "",
                            Port = 0,
                            ServerType = 0,
                            Url = "D:\\testFTP",
                            UserName = ""
                        },
                        new
                        {
                            Id = "62CD1744-DD07-4BB4-8EE0-FC1359E2C279",
                            Name = "LocalFile2Server",
                            Password = "",
                            Port = 0,
                            ServerType = 0,
                            Url = "D:\\output",
                            UserName = ""
                        },
                        new
                        {
                            Id = "62CD1744-DD07-4BB4-8EE0-FC1359E2C280",
                            Name = "FTPFileServer",
                            Password = "Test123",
                            Port = 21,
                            ServerType = 1,
                            Url = "192.168.50.11",
                            UserName = "TestFtp"
                        },
                        new
                        {
                            Id = "62CD1744-DD07-4BB4-8EE0-FC1359E2C281",
                            Name = "SFTPFileServer",
                            Password = "password",
                            Port = 22,
                            ServerType = 2,
                            Url = "test.rebex.net",
                            UserName = "demo"
                        });
                });

            modelBuilder.Entity("Task2.Infrastructure.Context.FileModel", b =>
                {
                    b.HasOne("Task2.Infrastructure.Context.ServeModel", "Server")
                        .WithMany()
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });
#pragma warning restore 612, 618
        }
    }
}

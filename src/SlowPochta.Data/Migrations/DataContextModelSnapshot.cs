﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SlowPochta.Data.Repository;

namespace SlowPochta.Data.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("SlowPochta.Data.Model.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationDate");

                    b.Property<DateTime?>("DeliveryDate");

                    b.Property<string>("MessageText");

                    b.Property<int>("Status");

                    b.Property<string>("StatusDescription");

                    b.HasKey("Id");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("SlowPochta.Data.Model.MessageDeliveryStatusVariant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DeliveryStatusDescription");

                    b.HasKey("Id");

                    b.ToTable("MessageDeliveryStatusVariants");
                });

            modelBuilder.Entity("SlowPochta.Data.Model.MessageFromUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("MessageId");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.ToTable("MessagesFromUsers");
                });

            modelBuilder.Entity("SlowPochta.Data.Model.MessagePassedDeliveryStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("DeliveryStatusVariantId");

                    b.Property<int>("MessageId");

                    b.HasKey("Id");

                    b.ToTable("MessagePassedDeliveryStatuses");
                });

            modelBuilder.Entity("SlowPochta.Data.Model.MessageToUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("MessageId");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.ToTable("MessagesToUsers");
                });

            modelBuilder.Entity("SlowPochta.Data.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Login");

                    b.Property<string>("Password");

                    b.Property<int>("Role");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}

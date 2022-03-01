﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ThinkOrSwimAlerts.Data;

namespace ThinkOrSwimAlerts.Migrations
{
    [DbContext(typeof(PositionDb))]
    partial class PositionDbModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.14")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ThinkOrSwimAlerts.Data.Models.Position", b =>
                {
                    b.Property<long>("PositionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTimeOffset>("FinalSell")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("FirstBuy")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("Indicator")
                        .HasColumnType("int");

                    b.Property<string>("IndicatorVersion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PutOrCall")
                        .HasColumnType("int");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Underlying")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PositionId");

                    b.ToTable("Positions");
                });

            modelBuilder.Entity("ThinkOrSwimAlerts.Data.Models.PositionUpdate", b =>
                {
                    b.Property<long>("PositionUpdateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<float>("GainOrLossPct")
                        .HasColumnType("real");

                    b.Property<bool>("IsNewHigh")
                        .HasColumnType("bit");

                    b.Property<bool>("IsNewLow")
                        .HasColumnType("bit");

                    b.Property<float>("Mark")
                        .HasColumnType("real");

                    b.Property<long?>("PositionId")
                        .HasColumnType("bigint");

                    b.Property<int>("SecondsAfterPurchase")
                        .HasColumnType("int");

                    b.HasKey("PositionUpdateId");

                    b.HasIndex("PositionId");

                    b.ToTable("PositionUpdates");
                });

            modelBuilder.Entity("ThinkOrSwimAlerts.Data.Models.Purchase", b =>
                {
                    b.Property<long>("PurhaseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTimeOffset>("Bought")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("Bought15MinuteInterval")
                        .HasColumnType("int");

                    b.Property<float>("BuyPrice")
                        .HasColumnType("real");

                    b.Property<int>("Day")
                        .HasColumnType("int");

                    b.Property<long?>("PositionId")
                        .HasColumnType("bigint");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("PurhaseId");

                    b.HasIndex("PositionId");

                    b.ToTable("Purchases");
                });

            modelBuilder.Entity("ThinkOrSwimAlerts.Data.Models.PositionUpdate", b =>
                {
                    b.HasOne("ThinkOrSwimAlerts.Data.Models.Position", "Position")
                        .WithMany()
                        .HasForeignKey("PositionId");

                    b.Navigation("Position");
                });

            modelBuilder.Entity("ThinkOrSwimAlerts.Data.Models.Purchase", b =>
                {
                    b.HasOne("ThinkOrSwimAlerts.Data.Models.Position", "Position")
                        .WithMany()
                        .HasForeignKey("PositionId");

                    b.Navigation("Position");
                });
#pragma warning restore 612, 618
        }
    }
}

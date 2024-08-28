﻿// <auto-generated />
using System;
using DiscordButBetter.Server.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DiscordButBetter.Server.Database.Migrations
{
    [DbContext(typeof(DbbContext))]
    partial class DbbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("DiscordButBetter.Server.Database.Models.ChatMessageModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<Guid>("ConversationId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Metadata")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("SenderId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("SentAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("ConversationId");

                    b.HasIndex("SenderId");

                    b.HasIndex("SentAt", "Content");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("DiscordButBetter.Server.Database.Models.ConnectionModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<Guid>("ServerId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.HasIndex("UserId");

                    b.ToTable("Connections");
                });

            modelBuilder.Entity("DiscordButBetter.Server.Database.Models.ConversationModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("ConversationName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ConversationPicture")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<byte>("ConversationType")
                        .HasColumnType("tinyint unsigned");

                    b.Property<DateTime>("LastMessageTime")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("OwnerId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Conversations");
                });

            modelBuilder.Entity("DiscordButBetter.Server.Database.Models.FriendRequestModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("ReceiverId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("SenderId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("ReceiverId");

                    b.HasIndex("SenderId");

                    b.ToTable("FriendRequests");
                });

            modelBuilder.Entity("DiscordButBetter.Server.Database.Models.ServerModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("LastPing")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("Servers");
                });

            modelBuilder.Entity("DiscordButBetter.Server.Database.Models.SessionModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("UserAgent")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("token")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("userId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("userId");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("DiscordButBetter.Server.Database.Models.UploadedFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("FileType")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("OriginalFileName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("UploadedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("UploaderId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("FileName");

                    b.HasIndex("Hash");

                    b.HasIndex("UploaderId");

                    b.ToTable("UploadedFiles");
                });

            modelBuilder.Entity("DiscordButBetter.Server.Database.Models.UserModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Biography")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("Online")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ProfilePicture")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<byte>("Status")
                        .HasColumnType("tinyint unsigned");

                    b.Property<string>("StatusMessage")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("Username");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Friends", b =>
                {
                    b.Property<Guid>("FriendId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("FriendId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("Friends");
                });

            modelBuilder.Entity("conversationParticipants", b =>
                {
                    b.Property<Guid>("ConversationId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("ConversationId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("conversationParticipants");
                });

            modelBuilder.Entity("conversationParticipantsVisible", b =>
                {
                    b.Property<Guid>("ConversationId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("ConversationId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("conversationParticipantsVisible");
                });

            modelBuilder.Entity("DiscordButBetter.Server.Database.Models.ChatMessageModel", b =>
                {
                    b.HasOne("DiscordButBetter.Server.Database.Models.ConversationModel", "Conversation")
                        .WithMany("ChatMessages")
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DiscordButBetter.Server.Database.Models.UserModel", "Sender")
                        .WithMany("ChatMessages")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Conversation");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("DiscordButBetter.Server.Database.Models.ConnectionModel", b =>
                {
                    b.HasOne("DiscordButBetter.Server.Database.Models.ServerModel", "Server")
                        .WithMany("Connections")
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DiscordButBetter.Server.Database.Models.UserModel", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DiscordButBetter.Server.Database.Models.ConversationModel", b =>
                {
                    b.HasOne("DiscordButBetter.Server.Database.Models.UserModel", "Owner")
                        .WithMany("OwnedConversations")
                        .HasForeignKey("OwnerId");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("DiscordButBetter.Server.Database.Models.FriendRequestModel", b =>
                {
                    b.HasOne("DiscordButBetter.Server.Database.Models.UserModel", "Receiver")
                        .WithMany("ReceivedFriendRequests")
                        .HasForeignKey("ReceiverId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DiscordButBetter.Server.Database.Models.UserModel", "Sender")
                        .WithMany("SentFriendRequests")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Receiver");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("DiscordButBetter.Server.Database.Models.SessionModel", b =>
                {
                    b.HasOne("DiscordButBetter.Server.Database.Models.UserModel", "user")
                        .WithMany("Sessions")
                        .HasForeignKey("userId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("user");
                });

            modelBuilder.Entity("DiscordButBetter.Server.Database.Models.UploadedFile", b =>
                {
                    b.HasOne("DiscordButBetter.Server.Database.Models.UserModel", "Uploader")
                        .WithMany("UploadedFiles")
                        .HasForeignKey("UploaderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Uploader");
                });

            modelBuilder.Entity("Friends", b =>
                {
                    b.HasOne("DiscordButBetter.Server.Database.Models.UserModel", null)
                        .WithMany()
                        .HasForeignKey("FriendId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DiscordButBetter.Server.Database.Models.UserModel", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("conversationParticipants", b =>
                {
                    b.HasOne("DiscordButBetter.Server.Database.Models.ConversationModel", null)
                        .WithMany()
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DiscordButBetter.Server.Database.Models.UserModel", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("conversationParticipantsVisible", b =>
                {
                    b.HasOne("DiscordButBetter.Server.Database.Models.ConversationModel", null)
                        .WithMany()
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DiscordButBetter.Server.Database.Models.UserModel", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DiscordButBetter.Server.Database.Models.ConversationModel", b =>
                {
                    b.Navigation("ChatMessages");
                });

            modelBuilder.Entity("DiscordButBetter.Server.Database.Models.ServerModel", b =>
                {
                    b.Navigation("Connections");
                });

            modelBuilder.Entity("DiscordButBetter.Server.Database.Models.UserModel", b =>
                {
                    b.Navigation("ChatMessages");

                    b.Navigation("OwnedConversations");

                    b.Navigation("ReceivedFriendRequests");

                    b.Navigation("SentFriendRequests");

                    b.Navigation("Sessions");

                    b.Navigation("UploadedFiles");
                });
#pragma warning restore 612, 618
        }
    }
}

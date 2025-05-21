using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PersonalAssistant.Models;

namespace PersonalAssistant.Context;

public partial class User8Context : DbContext
{
    public User8Context()
    {
    }

    public User8Context(DbContextOptions<User8Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Emotion> Emotions { get; set; }

    public virtual DbSet<Feeling> Feelings { get; set; }

    public virtual DbSet<List> Lists { get; set; }

    public virtual DbSet<Pfp> Pfps { get; set; }

    public virtual DbSet<Priority> Priorities { get; set; }

    public virtual DbSet<PriorityTable> PriorityTables { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=45.67.56.214; Port=5666; Username=user8; Password=i9ehyuJ3; Database=user8");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Emotion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("emotions_pk");

            entity.ToTable("emotions");

            entity.HasIndex(e => e.Name, "emotions_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        });

        modelBuilder.Entity<Feeling>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("feelings_pk");

            entity.ToTable("feelings");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("date");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Level)
                .HasDefaultValue(50)
                .HasColumnName("level");

            entity.HasMany(d => d.Emotions).WithMany(p => p.Feelings)
                .UsingEntity<Dictionary<string, object>>(
                    "FeelingsEmotion",
                    r => r.HasOne<Emotion>().WithMany()
                        .HasForeignKey("Emotion")
                        .OnDelete(DeleteBehavior.SetNull)
                        .HasConstraintName("feelings_emotions_emotions_fk"),
                    l => l.HasOne<Feeling>().WithMany()
                        .HasForeignKey("Feeling")
                        .HasConstraintName("feelings_emotions_feelings_fk"),
                    j =>
                    {
                        j.HasKey("Feeling", "Emotion").HasName("feelings_emotions_pk");
                        j.ToTable("feelings_emotions");
                        j.IndexerProperty<int>("Feeling").HasColumnName("feeling");
                        j.IndexerProperty<int>("Emotion").HasColumnName("emotion");
                    });
        });

        modelBuilder.Entity<List>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("lists_pk");

            entity.ToTable("lists");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        });

        modelBuilder.Entity<Pfp>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pfps_pk");

            entity.ToTable("pfps");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Path)
                .HasColumnType("character varying")
                .HasColumnName("path");
        });

        modelBuilder.Entity<Priority>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("priorities_pk");

            entity.ToTable("priorities");

            entity.HasIndex(e => e.Name, "priorities_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Strength).HasColumnName("strength");
        });

        modelBuilder.Entity<PriorityTable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("priority_table_pk");

            entity.ToTable("priority_table");

            entity.HasIndex(e => e.Name, "priority_table_unique").IsUnique();

            entity.HasIndex(e => e.Strength, "priority_table_unique_1").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Strength).HasColumnName("strength");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("statuses_pk");

            entity.ToTable("statuses");

            entity.HasIndex(e => e.Name, "statuses_unique").IsUnique();

            entity.HasIndex(e => e.Type, "statuses_unique_1").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Type).HasColumnName("type");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tasks_pk");

            entity.ToTable("tasks");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreationDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("creation_date");
            entity.Property(e => e.Deadline)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deadline");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("end_date");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Priority).HasColumnName("priority");
            entity.Property(e => e.PriorityTable).HasColumnName("priority_table");
            entity.Property(e => e.StartDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");

            entity.HasOne(d => d.PriorityNavigation).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.Priority)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("tasks_priorities_fk");

            entity.HasOne(d => d.PriorityTableNavigation).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.PriorityTable)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("tasks_priority_table_fk");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.Status)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("tasks_statuses_fk");

            entity.HasMany(d => d.Lists).WithMany(p => p.Tasks)
                .UsingEntity<Dictionary<string, object>>(
                    "TasksList",
                    r => r.HasOne<List>().WithMany()
                        .HasForeignKey("List")
                        .HasConstraintName("tasks_lists_lists_fk"),
                    l => l.HasOne<Task>().WithMany()
                        .HasForeignKey("Task")
                        .HasConstraintName("tasks_lists_tasks_fk"),
                    j =>
                    {
                        j.HasKey("Task", "List").HasName("tasks_lists_pk");
                        j.ToTable("tasks_lists");
                        j.IndexerProperty<int>("Task").HasColumnName("task");
                        j.IndexerProperty<int>("List").HasColumnName("list");
                    });

            entity.HasMany(d => d.SubTasks).WithMany(p => p.Tasks)
                .UsingEntity<Dictionary<string, object>>(
                    "TasksSub",
                    r => r.HasOne<Task>().WithMany()
                        .HasForeignKey("SubTask")
                        .HasConstraintName("tasks_subs_tasks_fk_1"),
                    l => l.HasOne<Task>().WithMany()
                        .HasForeignKey("Task")
                        .HasConstraintName("tasks_subs_tasks_fk"),
                    j =>
                    {
                        j.HasKey("Task", "SubTask").HasName("tasks_subs_pk");
                        j.ToTable("tasks_subs");
                        j.IndexerProperty<int>("Task").HasColumnName("task");
                        j.IndexerProperty<int>("SubTask").HasColumnName("sub_task");
                    });

            entity.HasMany(d => d.Tasks).WithMany(p => p.SubTasks)
                .UsingEntity<Dictionary<string, object>>(
                    "TasksSub",
                    r => r.HasOne<Task>().WithMany()
                        .HasForeignKey("Task")
                        .HasConstraintName("tasks_subs_tasks_fk"),
                    l => l.HasOne<Task>().WithMany()
                        .HasForeignKey("SubTask")
                        .HasConstraintName("tasks_subs_tasks_fk_1"),
                    j =>
                    {
                        j.HasKey("Task", "SubTask").HasName("tasks_subs_pk");
                        j.ToTable("tasks_subs");
                        j.IndexerProperty<int>("Task").HasColumnName("task");
                        j.IndexerProperty<int>("SubTask").HasColumnName("sub_task");
                    });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pk");

            entity.ToTable("users");

            entity.HasIndex(e => e.Login, "users_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.Login)
                .HasColumnType("character varying")
                .HasColumnName("login");
            entity.Property(e => e.Lvl)
                .HasDefaultValue(0)
                .HasColumnName("lvl");
            entity.Property(e => e.MainPfp).HasColumnName("main_pfp");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasColumnType("character varying")
                .HasColumnName("password");
            entity.Property(e => e.Xp)
                .HasDefaultValue(0L)
                .HasColumnName("xp");

            entity.HasOne(d => d.MainPfpNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.MainPfp)
                .HasConstraintName("users_pfps_fk");

            entity.HasMany(d => d.Feelings).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UsersFeeling",
                    r => r.HasOne<Feeling>().WithMany()
                        .HasForeignKey("Feeling")
                        .HasConstraintName("users_feelings_feelings_fk"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("User")
                        .HasConstraintName("users_feelings_users_fk"),
                    j =>
                    {
                        j.HasKey("User", "Feeling").HasName("users_feelings_pk");
                        j.ToTable("users_feelings");
                        j.IndexerProperty<int>("User").HasColumnName("user");
                        j.IndexerProperty<int>("Feeling").HasColumnName("feeling");
                    });

            entity.HasMany(d => d.Lists).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UsersList",
                    r => r.HasOne<List>().WithMany()
                        .HasForeignKey("List")
                        .HasConstraintName("users_lists_lists_fk"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("User")
                        .HasConstraintName("users_lists_users_fk"),
                    j =>
                    {
                        j.HasKey("User", "List").HasName("users_lists_pk");
                        j.ToTable("users_lists");
                        j.IndexerProperty<int>("User").HasColumnName("user");
                        j.IndexerProperty<int>("List").HasColumnName("list");
                    });

            entity.HasMany(d => d.Pfps).WithMany(p => p.UsersNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "UsersPfp",
                    r => r.HasOne<Pfp>().WithMany()
                        .HasForeignKey("Pfp")
                        .HasConstraintName("users_pfps_pfps_fk"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("User")
                        .HasConstraintName("users_pfps_users_fk"),
                    j =>
                    {
                        j.HasKey("User", "Pfp").HasName("users_pfps_pk");
                        j.ToTable("users_pfps");
                        j.IndexerProperty<int>("User").HasColumnName("user");
                        j.IndexerProperty<int>("Pfp").HasColumnName("pfp");
                    });

            entity.HasMany(d => d.Tasks).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UsersTask",
                    r => r.HasOne<Task>().WithMany()
                        .HasForeignKey("Task")
                        .HasConstraintName("users_tasks_tasks_fk"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("User")
                        .HasConstraintName("users_tasks_users_fk"),
                    j =>
                    {
                        j.HasKey("User", "Task").HasName("users_tasks_pk");
                        j.ToTable("users_tasks");
                        j.IndexerProperty<int>("User").HasColumnName("user");
                        j.IndexerProperty<int>("Task").HasColumnName("task");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

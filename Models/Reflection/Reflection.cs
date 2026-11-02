using static System.String;

namespace Nafs.Api.Models;

public class Reflection
{
    public int Id { get; private set; }

    public string Text { get; private set; }

    public DateTime CreatedAt { get; private set; }
    
    public DateTimeOffset? ScheduledTime { get; private set; }
    public ReflectionStatus Status { get; private set; }

    public Reflection(int id, string text, DateTime createdAt)
    {
        if (id <= 0)
        {
            throw new ArgumentException(
                "Id must be greater than zero.",
                nameof(id));
        }

        if (IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException(
                "Reflection text cannot be empty.",
                nameof(text));
        }

        Id = id;
        Text = text;
        Status = ReflectionStatus.Draft;
        CreatedAt = createdAt;
    }


    public void Schedule(DateTimeOffset scheduledTime, DateTimeOffset currentTime)
    {
        if (Status != ReflectionStatus.Draft)
        {
            throw new InvalidOperationException("Only Draft reflection can be scheduled");
        }

        if (scheduledTime <= currentTime)
        {
            throw new InvalidOperationException("Can't schedule past date time");
        }
        

        Status = ReflectionStatus.Scheduled;
        ScheduledTime = scheduledTime;
    }

    public void CancelSchedule()
    {
        if (Status != ReflectionStatus.Scheduled)
        {
            throw new InvalidOperationException("Please cancel schedule for Scheduled Notes only");
        }

        Status = ReflectionStatus.Draft;
        ScheduledTime = null;

    }

    public void Publish(DateTimeOffset currentTime)
    {
        if (Status != ReflectionStatus.Draft && Status != ReflectionStatus.Scheduled)
        {
            throw new InvalidOperationException("Only Draft or Scheduled reflection can be published");
        }

        if (Status == ReflectionStatus.Scheduled && ScheduledTime >currentTime)
        {
            throw new InvalidOperationException("Cant publish scheduled notes before its time");

        }
        Status = ReflectionStatus.Published;
        ScheduledTime = null;
    }

    public void Archive()
    {
        if (Status != ReflectionStatus.Published)
        {
            throw new InvalidOperationException("Only Published notes can be archived");
        }

        Status = ReflectionStatus.Archived;
    }

    public void UpdateText(string text)
    {
        if (IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException(
                "Reflection text cannot be empty.",
                nameof(text));
        }

        if (Status == ReflectionStatus.Archived)
        {
            throw new InvalidOperationException("Archived notes can't be updated");
        }

        Text = text;
    }

    public void Restore()
    {
        if (Status != ReflectionStatus.Archived)
        {
            throw new InvalidOperationException(
                "Only archived reflections can be restored.");
        }

        Status = ReflectionStatus.Published;
    }
}
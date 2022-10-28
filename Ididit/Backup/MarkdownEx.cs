using Ididit.Data.Models;
using System;
using System.Globalization;
using System.Text;

namespace Ididit.Backup;

public static class MarkdownEx
{
    public static bool AddDetail(this DetailsModel details, string detail)
    {
        if (detail.StartsWith("- Date: "))
        {
            if (DateTime.TryParse(detail.Replace("- Date: ", string.Empty), out DateTime date))
            {
                details.Date = date;
            }

            return true;
        }
        else if (detail.StartsWith("- Address: "))
        {
            details.Address = detail.Replace("- Address: ", string.Empty);

            return true;
        }
        else if (detail.StartsWith("- Phone: "))
        {
            details.Phone = detail.Replace("- Phone: ", string.Empty);

            return true;
        }
        else if (detail.StartsWith("- Email: "))
        {
            if (Uri.TryCreate(detail.Replace("- Email: ", string.Empty), UriKind.Absolute, out Uri? uri))
            {
                details.Email = uri;
            }

            return true;
        }
        else if (detail.StartsWith("- Website: "))
        {
            if (Uri.TryCreate(detail.Replace("- Website: ", string.Empty), UriKind.Absolute, out Uri? uri))
            {
                details.Website = uri;
            }

            return true;
        }
        else if (detail.StartsWith("- Open: "))
        {
            string open = detail.Replace("- Open: ", string.Empty);

            string[] fromTill = open.Split('-');

            if (fromTill.Length == 2)
            {
                if (TimeOnly.TryParse(fromTill[0].Trim(), out TimeOnly from))
                {
                    details.OpenFrom = from;
                }

                if (TimeOnly.TryParse(fromTill[1].Trim(), out TimeOnly till))
                {
                    details.OpenTill = till;
                }
            }

            return true;
        }

        return false;
    }

    public static void AppendToStringBuilder(this DetailsModel details, StringBuilder stringBuilder)
    {
        if (details.Date is not null)
            stringBuilder.AppendLine("- Date: " + details.Date);

        if (details.Address is not null)
            stringBuilder.AppendLine("- Address: " + details.Address);

        if (details.Phone is not null)
            stringBuilder.AppendLine("- Phone: " + details.Phone);

        if (details.Email is not null)
            stringBuilder.AppendLine("- Email: " + details.Email);

        if (details.Website is not null)
            stringBuilder.AppendLine("- Website: " + details.Website);

        if (details.OpenFrom is not null && details.OpenTill is not null)
            stringBuilder.AppendLine($"- Open: {details.OpenFrom} - {details.OpenTill}");
    }

    public static bool AddDetail(this TaskModel task, string detail)
    {
        task.Details ??= new();

        bool added = task.Details.AddDetail(detail);

        if (added)
            return true;

        if (detail.StartsWith("- Priority: "))
        {
            if (Enum.TryParse(detail.Replace("- Priority: ", string.Empty), out Priority priority))
            {
                task.Priority = priority;
            }

            return false;
        }
        else if (detail.StartsWith("- Interval: "))
        {
            if (double.TryParse(detail.Replace("- Interval: ", string.Empty), NumberStyles.Any, CultureInfo.InvariantCulture, out double days))
            {
                task.DesiredInterval = TimeSpan.FromDays(days);
                task.TaskKind = TaskKind.RepeatingTask;
            }
            else if (string.Equals(detail.Replace("- Interval: ", string.Empty), "ASAP", StringComparison.OrdinalIgnoreCase))
            {
                task.DesiredInterval = TimeSpan.Zero;
                task.TaskKind = TaskKind.Task;
            }

            return false;
        }
        else if (detail.StartsWith("- Duration: "))
        {
            if (double.TryParse(detail.Replace("- Duration: ", string.Empty), NumberStyles.Any, CultureInfo.InvariantCulture, out double minutes))
            {
                task.DesiredDuration = TimeSpan.FromMinutes(minutes);
            }

            return false;
        }

        return true;
    }
}

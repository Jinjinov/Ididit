using System;
using System.Text;

namespace Ididit.Data.Models;

public class DetailsModel
{
    public DateTime? Date { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public Uri? Email { get; set; }
    public Uri? Website { get; set; }
    public TimeOnly? OpenFrom { get; set; }
    public TimeOnly? OpenTill { get; set; }

    public void AddDetail(string detail)
    {
        if (detail.StartsWith("- Date: "))
        {
            if (DateTime.TryParse(detail.Replace("- Date: ", string.Empty), out DateTime date))
            {
                Date = date;
            }
        }
        else if (detail.StartsWith("- Address: "))
        {
            Address = detail.Replace("- Address: ", string.Empty);
        }
        else if (detail.StartsWith("- Phone: "))
        {
            Phone = detail.Replace("- Phone: ", string.Empty);
        }
        else if (detail.StartsWith("- Email: "))
        {
            if (Uri.TryCreate(detail.Replace("- Email: ", string.Empty), UriKind.Absolute, out Uri? uri))
            {
                Email = uri;
            }
        }
        else if (detail.StartsWith("- Website: "))
        {
            if (Uri.TryCreate(detail.Replace("- Website: ", string.Empty), UriKind.Absolute, out Uri? uri))
            {
                Website = uri;
            }
        }
        else if (detail.StartsWith("- Open: "))
        {
            string open = detail.Replace("- Open: ", string.Empty);

            string[] fromTill = open.Split('-');

            if (fromTill.Length == 2)
            {
                if (TimeOnly.TryParse(fromTill[0].Trim(), out TimeOnly from))
                {
                    OpenFrom = from;
                }

                if (TimeOnly.TryParse(fromTill[1].Trim(), out TimeOnly till))
                {
                    OpenTill = till;
                }
            }
        }
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new();

        if (Date is not null)
            stringBuilder.AppendLine("- Date: " + Date);

        if (Address is not null)
            stringBuilder.AppendLine("- Address: " + Address);

        if (Phone is not null)
            stringBuilder.AppendLine("- Phone: " + Phone);

        if (Email is not null)
            stringBuilder.AppendLine("- Email: " + Email);

        if (Website is not null)
            stringBuilder.AppendLine("- Website: " + Website);

        if (OpenFrom is not null && OpenTill is not null)
            stringBuilder.AppendLine($"- Open: {OpenFrom} - {OpenTill}");

        return stringBuilder.ToString();
    }
}

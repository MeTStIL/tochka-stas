using System.Globalization;
using System.Text.RegularExpressions;


class HotelCapacity
{
    static bool CheckCapacity(int maxCapacity, List<Guest> guests)
    {
        var queue = new Queue<Guest>();

        var guestSort = guests.OrderBy(e => e.CheckOut).ToList();

        foreach (var guest in guestSort)
        {
            if (queue.Count < maxCapacity)
                queue.Enqueue(guest);
            else
            {
                var curr = queue.Dequeue();
                if (guest.CheckIn >= curr.CheckOut)
                    queue.Enqueue(guest);
                else
                    return false;
            }
        }

        return true;
    }


    class Guest
    {
        public string Name { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }


    static void Main()
    {
        using var input = new StreamReader(Console.OpenStandardInput());
        using var output = new StreamWriter(Console.OpenStandardOutput());

        var maxCapacity = int.Parse(input.ReadLine());
        var cn = input.ReadLine();
        var n = 0;
        if (string.Empty == cn)
            n = int.Parse(input.ReadLine());
        else
            n = int.Parse(cn);

        var guests = new List<Guest>();
        
        for (var i = 0; i < n; i++)
        {
            var line = input.ReadLine();
            var guest = ParseGuest(line);
            guests.Add(guest);
        }

        var result = CheckCapacity(maxCapacity, guests);
        
        output.WriteLine(result ? "True" : "False");
    }


    // Простой парсер JSON-строки для объекта Guest
    static Guest ParseGuest(string json)
    {
        var guest = new Guest();
        
        // Извлекаем имя
        Match nameMatch = Regex.Match(json, "\"name\"\\s*:\\s*\"([^\"]+)\"");
        if (nameMatch.Success)
            guest.Name = nameMatch.Groups[1].Value;
        
        // Извлекаем дату заезда
        Match checkInMatch = Regex.Match(json, "\"check-in\"\\s*:\\s*\"([^\"]+)\"");
        if (checkInMatch.Success)
        {
            var dateString = checkInMatch.Groups[1].Value;
            var checkInMatchDate = DateTime.ParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            guest.CheckIn = checkInMatchDate;
        }
        
        // Извлекаем дату выезда
        Match checkOutMatch = Regex.Match(json, "\"check-out\"\\s*:\\s*\"([^\"]+)\"");
        if (checkOutMatch.Success)
        {
            var dateString = checkOutMatch.Groups[1].Value;
            var checkOutDate = DateTime.ParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            guest.CheckOut = checkOutDate;
        }
        
        return guest;
    }
}
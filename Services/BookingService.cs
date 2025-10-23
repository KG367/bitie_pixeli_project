using System.Numerics;

namespace Server // мой сервис с реализацией системы брони. Пока что сырой и не обкатанный, но компилируется!
{

    public enum BookingField
    {
        ClientId,
        ClientName,
        PhoneNumber,
        Comment,
    }
    public struct Booking
    {
        public int ClientId;
        public string ClientName;
        public string PhoneNumber;
        public DateTime StartTime;
        public DateTime EndTime;
        public string Comment;
        public int AssignedTableId;

        public Booking(int clientId, string clientName, string phoneNumber, DateTime startTime, DateTime endTime, string comment, int assignedTableId)
        {
            ClientId = clientId;
            ClientName = clientName;
            PhoneNumber = phoneNumber;
            StartTime = startTime;
            EndTime = endTime;
            Comment = comment;
            AssignedTableId = assignedTableId;
        }

        public override string ToString()
        {
            return $"Клиент: {this.ClientId}\nИмя: {this.ClientName}\nТелефон: {this.PhoneNumber}\nНачало: {this.StartTime}\nКонец: {this.EndTime}\nКомментарий: {this.Comment}\n";
        } 

        public bool TryModify(params (BookingField, string)[] fields)
        {
            foreach ((var field, var value) in fields)
            {
                switch (field)
                {
                    case BookingField.ClientId:
                        int clearId = 0;
                        if (int.TryParse(value, out clearId) && clearId >= 0)
                        {
                            this.ClientId = clearId;
                            break;
                        }
                        return false;

                    case BookingField.ClientName:
                        this.ClientName = value;
                        break;

                    case BookingField.PhoneNumber:
                        this.PhoneNumber = value;
                        break;

                    case BookingField.Comment:
                        this.Comment = value;
                        break;

                }

            }
            return true;
        }
    }

    public class BookingService
    {
        public Dictionary<int, List<Booking>> bookings;

        public bool AddNewTable(int tableId)
        {
            if (!bookings.ContainsKey(tableId))
            {
                bookings.Add(tableId, new List<Booking>());
                return true;
            }
            return false;
        }

        public bool GetEmptyBookPosition(in List<Booking> bookings, in DateTime start, in DateTime end, out int index)
        {
            index = -1;
    
            if (bookings == null || bookings.Count == 0)
            {
                index = 0;
                return true;
            }
    
            if (end <= bookings[0].StartTime)
            {
                index = 0;
                return true;
            }
    
            if (start >= bookings[^1].EndTime)  
            {
                index = bookings.Count;
                return true;
            }
    
            int left = 0;
            int right = bookings.Count - 1;
    
            while (left <= right)
            {
                int mid = left + (right - left) / 2;  
        
                Booking current = bookings[mid];
                if (current.EndTime <= start)
                {
                    if (mid + 1 < bookings.Count)
                    {
                        Booking next = bookings[mid + 1];
                        if (next.StartTime >= end)
                        {
                            index = mid + 1;
                            return true;
                        }
                        else
                        {
                            left = mid + 1;
                        }   
                    }
                    else
                    {
                        index = mid + 1;
                        return true;
                    }
                }
                else
                {
                    right = mid - 1;
                }   
            }
    
            return false;
        }

        public bool BookTable(int tableId, in DateTime startTime, in DateTime endTime, int clientId, string clientName, string phoneNumber, string comment)
        {
            List<Booking> tableBookings = bookings[tableId];
            if (tableBookings.Count == 0)
            {
                tableBookings.Add(new Booking(clientId, clientName, phoneNumber, startTime, endTime, comment, tableId));
                return true;
            }
            else
            {
                int targetIndex = -1;
                if (!(this.GetEmptyBookPosition(in tableBookings, in startTime, in endTime, out targetIndex)))
                {
                    return false;
                }
                else
                {
                    tableBookings.Insert(targetIndex, new Booking(clientId, clientName, phoneNumber, startTime, endTime, comment, tableId));
                }
            }
            return true;
        }

        public bool BookTable(in Booking book)
        {
            List<Booking> tableBookings = bookings[book.AssignedTableId];
            if (tableBookings.Count == 0)
            {
                tableBookings.Add(book);
                return true;
            }
            else
            {
                int targetIndex = -1;
                if (!(this.GetEmptyBookPosition(in tableBookings,  book.StartTime, book.EndTime, out targetIndex)))
                {
                    return false;
                }
                else
                {
                    tableBookings.Insert(targetIndex, book);
                }
            }
            return true;
        }

        public bool CancelBooking(in Booking booking)
        {
            List<Booking> tableBookings = this.bookings[booking.AssignedTableId];
            return tableBookings.Remove(booking);
        }

        public bool CancelBooking(int tableId, in DateTime time, bool start)
        {
            List<Booking> tableBookings = this.bookings[tableId];

            if (start)
            {
                for (int i = 0; i < tableBookings.Count; i++)
                {
                    if (tableBookings[i].StartTime == time)
                    {
                        tableBookings.RemoveAt(i);
                        return true;
                    }
                }
                return false;
            }
            else
            {
                for (int i = 0; i < tableBookings.Count; i++)
                {
                    if (tableBookings[i].EndTime == time)
                    {
                        tableBookings.RemoveAt(i);
                        return true;
                    }
                }
                return false;
            }
        }

        public bool TryGetBooking(int tableId, in DateTime time, bool start, out Booking? outB)
        {
            List<Booking> bookings = this.bookings[tableId];



            if (start)
            {
                foreach (Booking booking in bookings)
                {
                    if (booking.StartTime == time)
                    {
                        outB = booking;
                        return true;
                    }
                }
            }
            else
            {
                foreach (Booking booking in bookings)
                {
                    if (booking.EndTime == time)
                    {
                        outB = booking;
                        return true;
                    }
                }
            }

            outB = null;
            return false;
        }
    }
}
using System;

namespace EFConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new MSSQLContext()) 
            {
                db.Database.EnsureCreated();
            }
        }
    }
}

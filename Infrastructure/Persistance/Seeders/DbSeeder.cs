using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace Infrastructure.Persistance.Seeders
{
    public class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            CategorySeeder.Seed(context);
            ChannelSeeder.Seed(context);
            UserSeeder.Seed(context);
        }
    }
}

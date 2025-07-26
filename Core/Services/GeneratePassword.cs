using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class GeneratePassword
    {
        public async Task <string> GenPasswordAsync(int length = 12)
        {
            const string chars = "zsrextdrcyftvugybnhumijpokyt12345678!@#$%^&*()_";
            Random random = new Random();
            StringBuilder password = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                password.Append(chars[random.Next(chars.Length)]);
            }
            return password.ToString();
        }
    }
}

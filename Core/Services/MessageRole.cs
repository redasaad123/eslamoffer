using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class MessageRole
    {
        public async Task<string> messageRole(string Role)
        {
            string message;

            if (Role == "editor")
            {
                message = "\r\n- Can add and edit stores and coupons.\r\n- Cannot delete stores or coupons.\r\n- Can write and edit blog posts.\r\n- Can review blog comments.\r\n- No access to site settings or SEO tools.\r\n- Cannot add or edit users.";

            }
            else if (Role == "author")
            {
                message = "\r\n- Can add and edit their own blog posts.\r\n- Cannot edit or delete blog posts written by others.\r\n- Can review blog comments.\r\n- No access to stores or coupons.\r\n- No access to site settings or SEO tools.\r\n- Cannot add or edit users.";
            }
            else
            {
                message = "\r\n- Full access to stores and coupons (add, edit, delete).\r\n- Full access to all blog posts (add, edit, delete).\r\n- Can manage and moderate blog comments.\r\n- Full access to site settings and SEO tools.\r\n- Can add, edit, and delete users.";
            }



            return message;
        }
    }
}

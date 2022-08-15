using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Itami
{
   public class Userinfo
    {
        private string _Username;
        private string _UUID;
        public string Username
        {
            get { return _Username; }
            set { _Username = value; }
        }
        public string UUID
        {
            get { return _UUID; }
            set { _UUID = value; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskManager.Utils
{
    public static class ThreadPoolTimer
    {

        public static Timer Create(Action callback, TimeSpan intervall)
        {
            return Task.Run(() => new Timer(state => callback(), null, TimeSpan.Zero, intervall)).Result;
        }

    }
}

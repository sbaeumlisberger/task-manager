using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.ViewModel
{
    public interface ITabModel
    {

        void Update();

        void Filter(string searchQuery);

    }
}

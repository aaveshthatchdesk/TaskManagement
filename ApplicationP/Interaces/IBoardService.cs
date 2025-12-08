using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Application.Interaces
{
    public interface IBoardService
    {
      Task<bool> RenameBoardAsync(int boardId,string Name);
    }
}

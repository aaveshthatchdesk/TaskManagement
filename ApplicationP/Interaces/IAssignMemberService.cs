using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Application.Interaces
{
    public interface IAssignMemberService
    {
        Task<bool> AssignedMemberAsync(int taskId, int userId, int createdUserId );
        Task<bool> RemoveMemberAsync(int taskId, int userId, int createdUserId);
    }
}

using MinimalAPI.Models;

namespace MinimalAPI.IRepository
{
    public interface ICommandRepository
    {
        Task SaveChanges();
        Task<Command?> GetCommandById(int id);
        Task<IEnumerable<Command>> GetAllCommands();
        Task CreateCommand(Command cmd);

        void DeleteCommand(Command cmd);
    }
}

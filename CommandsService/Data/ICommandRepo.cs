using System.Collections.Generic;
using CommandsService.Models;

namespace CommandsService.Data
{
    // Repository pattrern: a repository is a class defined for an entity, with all the operations possible on that specific entity
    // Unit Of Work: a single transaction that involves multiple operations
    public interface ICommandRepo
    {
        bool SaveChanges();

        // Platforms
        IEnumerable<Platform> GetAllPlatforms();
        void CreatePlatform(Platform plat);
        bool PlaformExits(int platformId);
        bool ExternalPlatformExists(int externalPlatformId);

        // Commands
        IEnumerable<Command> GetCommandsForPlatform(int platformId);
        Command GetCommand(int platformId, int commandId);
        void CreateCommand(int platformId, Command command);
    }
}
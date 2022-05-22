using System.Collections;
namespace Atavism
{

    public interface IGameWorld
    {
        void Initialize();

        void SetupMessageHandlers();

       
        /// <summary>
        ///   This is used so that the client can send messages to the user
        ///   in whatever system the given game world uses.
        ///
        ///   This is the equivalent of the ClientAPI.Write, but is more 
        ///   easily available to the C# code.
        /// </summary>
        /// <param name="msg"></param>
        void Write(string msg);

        /// <summary>
        ///   Get or set the WorldManager object that keeps track of objects 
        ///   in the world.
        /// </summary>
        AtavismWorldManager WorldManager
        {
            get;
            set;
        }
    }
}
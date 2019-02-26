using InstaSharper.API;
using System;
using System.Threading.Tasks;

namespace CodeAThoneInstaBot.Actions
{
    public class FollowUser : IAction
    {
        private IInstaApi _instaApi;

        public FollowUser(IInstaApi instaApi)
        {
            _instaApi = instaApi;
        }

        /// <summary>
        /// follow user from username
        /// </summary>
        /// <returns></returns>
        public async Task Do()
        {
            // follow user using username
            Console.WriteLine("follow user using username ..");
            Console.WriteLine("Enter User Name to follow");

            // Read user name
            string userToFollow = Console.ReadLine();
            var userinfo = await _instaApi.GetUserAsync(userToFollow);
            var result = await _instaApi.FollowUserAsync(userinfo.Value.Pk);

            Console.WriteLine($"Follow User : [{userToFollow}] : {result.Succeeded}");
        }
    }
}

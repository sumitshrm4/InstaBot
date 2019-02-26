using System;
using System.Linq;
using System.Threading.Tasks;
using InstaSharper.API;
using InstaSharper.Classes;

namespace CodeAThoneInstaBot.Actions
{
    public class GetUserFollowers : IAction
    {
        private IInstaApi _instaApi;
        public GetUserFollowers(IInstaApi instaApi)
        {
            _instaApi = instaApi;
        }

        /// <summary>
        /// Get user Follower count
        /// </summary>
        /// <returns></returns>
        public async Task Do()
        {
            // get user follow count
            Console.WriteLine("Get User followers...");
            Console.WriteLine("Enter User Name to get follower count.");

            // Read user name
            string userToGetCount = Console.ReadLine();

            var followers = await _instaApi.GetUserFollowersAsync(userToGetCount, PaginationParameters.MaxPagesToLoad(6));
            Console.WriteLine($"Count of followers [{userToGetCount}]:{followers.Value.Count}");
        }

    }
}

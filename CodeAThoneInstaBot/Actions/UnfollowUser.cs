using InstaSharper.API;
using InstaSharper.Classes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeAThoneInstaBot.Actions
{
    public class UnfollowUser : IAction
    {
        private IInstaApi _instaApi;

        public UnfollowUser(IInstaApi instaApi)
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
            Console.WriteLine("Unfollow following users in once starting ...");

            var currentUser = await _instaApi.GetCurrentUserAsync();
            var followers = await _instaApi.GetUserFollowingAsync(currentUser.Value.UserName, PaginationParameters.MaxPagesToLoad(10));
            int count = 0;
            foreach (var follower in followers.Value)
            {
                count++;
                var result = await _instaApi.UnFollowUserAsync(follower.Pk);
                Console.WriteLine($"UnFollow User : [{follower.UserName}] : {result.Succeeded}");
                Random rnd = new Random();
                int sleepTime = rnd.Next(10000, 15000);
                Console.WriteLine($"Sleep Time is : {sleepTime }");
                Thread.Sleep(sleepTime);
                
                if (count >= 50)
                {
                    Console.WriteLine($"[{DateTime.Now}] Thread is in sleep and will start after 45 min..");
                    Thread.Sleep(2700000);
                    count = 0;
                    continue;                   
                }
            } 
        }
    }
}

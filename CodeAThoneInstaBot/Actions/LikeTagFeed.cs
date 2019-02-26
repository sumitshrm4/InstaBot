using InstaSharper.API;
using InstaSharper.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeAThoneInstaBot.Actions
{
    public class LikeTagFeed : IAction
    {
        private IInstaApi _instaApi;
        public LikeTagFeed(IInstaApi instaApi)
        {
            _instaApi = instaApi;
        }

        /// <summary>
        /// Like user post from tag feed
        /// </summary>
        /// <returns></returns>
        public async Task Do()
        {
            // Like tag feed
            Console.WriteLine("Auto Like tag feed started ..");
            Console.WriteLine("Enter tag name.");

            // read tag name
            string tagNameToLike = Console.ReadLine();


            // get tag feed 
            var tagFeed = await _instaApi.GetTagFeedAsync(tagNameToLike, PaginationParameters.MaxPagesToLoad(5));

            if (tagFeed.Succeeded)
            {
                foreach (var media in tagFeed.Value.Medias.Take(10))
                {
                    // like media
                    var likeResult = _instaApi.LikeMediaAsync(media.InstaIdentifier);
                    if (likeResult.Result.Succeeded)
                    {
                        Console.WriteLine($"feed Liked for User : [{media.User.FullName}] ");
                    }

                }
            }
        }

    }


}

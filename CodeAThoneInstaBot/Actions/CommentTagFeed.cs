using InstaSharper.API;
using InstaSharper.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAThoneInstaBot.Actions
{
    public class CommentTagFeed : IAction
    {
        private IInstaApi _instaApi;
        public CommentTagFeed(IInstaApi instaApi)
        {
            _instaApi = instaApi;
        }

        /// <summary>
        /// Comment on user feed from tag
        /// </summary>
        /// <returns></returns>
        public async Task Do()
        {//Comment tag feed
            Console.WriteLine("Auto comment tag feed started ..");
            Console.WriteLine("Enter tag name.");

            // read tag name
            string tagNameToComment = Console.ReadLine();

            // get tag feed 
            var tagFeed = await _instaApi.GetTagFeedAsync(tagNameToComment, PaginationParameters.MaxPagesToLoad(5));

            if (tagFeed.Succeeded)
            {
                Console.WriteLine("What comment you want to post ?");
                // read tag name
                string comment = Console.ReadLine();

                foreach (var media in tagFeed.Value.Medias.Take(10))
                {
                    // Comment on media
                    var result = _instaApi.CommentMediaAsync(media.InstaIdentifier, comment);
                    if (result.Result.Succeeded)
                    {
                        Console.WriteLine($"Commented on User : [{media.User.FullName}] Feed");
                    }

                }
            }
        }
    }
}

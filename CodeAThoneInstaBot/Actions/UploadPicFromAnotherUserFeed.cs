using InstaSharper.API;
using InstaSharper.Classes;
using InstaSharper.Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CodeAThoneInstaBot.Actions
{
    public class UploadPicFromAnotherUserFeed : IAction
    {

        private IInstaApi _instaApi;

        public UploadPicFromAnotherUserFeed(IInstaApi instaApi)
        {
            _instaApi = instaApi;
        }

        public async Task Do()
        {
            Console.WriteLine("Enter the UserName From Where you want to download images ...");
            string userName = Console.ReadLine();
           int imageUplodedCount = 0;
            var userMedia = _instaApi.GetUserMediaAsync(userName, PaginationParameters.MaxPagesToLoad(1)).Result;

            foreach (var media in userMedia.Value.Take(10))
            {
                // video upload
                if (imageUplodedCount >= 3)
                    break;

                 if(media.HasAudio)
                  {
                    //var image = media.Images[0];
                    //var video = media.Videos[2];

                    //using (WebClient client = new WebClient())
                    //{
                    //    client.DownloadFile(new Uri(video.Url), @"c:\temp\video35.mp4");
                    //    client.DownloadFile(new Uri(media.Videos[1].Url), @"c:\temp\video351.mp4");
                    //    client.DownloadFile(new Uri(media.Videos[2].Url), @"c:\temp\video352.mp4");
                    //    //client.DownloadFile(new Uri(media.Videos[3].Url), @"c:\temp\video353.mp4");

                    //    client.DownloadFile(new Uri(image.URI), @"c:\temp\image35.png");
                    //    Console.WriteLine($"Image downloaded from User - {userName}");
                    //}

                    //video.Url = @"c:\temp\video352.mp4";

                    //string my_String = Regex.Replace(media.Caption.Text, @"[^0-9a-zA-Z]+", ",");
                    //var result = await _instaApi.UploadVideoAsync(video, image, "Hello");

                    
                    //var shareResult = await _instaApi.ShareMedia(result.Value.Pk, InstaSharper.Classes.Models.InstaMediaType.Video, new string[] { "hello"});

                    //if (result.Succeeded)
                    //    Console.WriteLine($"Media uploaded successfully");

                    //if (shareResult.Succeeded)
                    //    Console.WriteLine($"media has been shared on facebook");
                   
                }
                 // Image upload
                else
                {
                    if (media.Images.Count > 0)
                    {
                        var image = media.Images[0];
                        using (WebClient client = new WebClient())
                        {
                            client.DownloadFile(new Uri(image.URI), @"c:\temp\image35.png");
                            Console.WriteLine($"Image downloaded from User - {userName}");
                        }

                        image.URI = @"c:\temp\image35.png";
                        var result = await _instaApi.UploadPhotoAsync(image, media.Caption?.Text);

                        var currentUser = _instaApi.GetDirectInboxAsync();
                        var shareResult = await _instaApi.ShareMedia(result.Value.Pk, InstaSharper.Classes.Models.InstaMediaType.Image, currentUser.Result.Value.Inbox.Threads.ToString());

                        if (result.Succeeded)
                        {
                            imageUplodedCount++;
                            Console.WriteLine("Media uploaded successfully");
                        }
                            

                        if (shareResult.Succeeded)
                        {
                            Console.WriteLine("media has been shared on facebook");
                        }                          

                    }
                    else if(media.MediaType == InstaMediaType.Carousel)
                    {
                        if(media.Carousel.Count > 0)
                        {
                            int imageIndex = 0;
                            InstaImage[] imageArray = new InstaImage[media.Carousel.Count];
                            foreach(var currentCarousel in media.Carousel)
                            {
                                
                                if (currentCarousel.Images.Count > 0)
                                {
                                    InstaImage image = currentCarousel.Images[0];
                                    using (WebClient client = new WebClient())
                                    {
                                        client.DownloadFile(new Uri(image.URI), $@"c:\temp\image{imageIndex}.png");
                                        Console.WriteLine($"Image downloaded from User - {userName}");
                                    }
                                    image.URI = $@"c:\temp\image{imageIndex}.png";
                                    imageArray[imageIndex] = image;
                                        imageIndex++;
                                }

                            }

                           var result =  await _instaApi.UploadPhotosAlbumAsync(imageArray, "");

                           
                                if (result.Succeeded)
                                {
                                    imageUplodedCount++;
                                    Console.WriteLine("Media uploaded successfully");
                                }
                        }
                    }
                }
                    

                Thread.Sleep(10000);
            }
        }
    }
}

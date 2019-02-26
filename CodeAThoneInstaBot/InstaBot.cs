using CodeAThoneInstaBot.Actions;
using InstaSharper.API;
using InstaSharper.API.Builder;
using InstaSharper.Classes;
using InstaSharper.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeAThoneInstaBot
{
    public class InstaBot
    {
        /// <summary>
        ///     Api instance (one instance per Instagram user)
        /// </summary>
        private static IInstaApi _instaApi;
        private static string username;
        private static string password;
        const string stateFile = "state.bin";

        public InstaBot()
        {

        }
        public static async Task<bool> MainAsync()
        {
            try
            {
                Console.WriteLine("Welcome to The Insta Bot ... Made By ---Sumit Sharma----");
                Console.WriteLine("---Sumit Sharma----");
                Console.WriteLine("---Contact via Email : sumitshrm45@gmail.com----");
                Console.WriteLine("");

                if (!File.Exists(stateFile))
                {
                    Console.WriteLine("Enter User Name .. ");
                    username = Console.ReadLine();
                    Console.WriteLine("Enter Password .. ");
                    password = Console.ReadLine();
                }


                // create user session data and provide login details
                var userSession = new UserSessionData
                {
                    UserName = username,
                    Password = password
                };

                
                // create new InstaApi instance using Builder
                _instaApi = InstaApiBuilder.CreateBuilder()
                    .SetUser(userSession)
                    .UseLogger(new DebugLogger(LogLevel.Exceptions)) // use logger for requests and debug messages
                    .SetRequestDelay(RequestDelay.FromSeconds(0, 1))
                    .Build();

                Login(userSession);



                var currentUser = await _instaApi.GetCurrentUserAsync();

                if (currentUser.Succeeded)
                    Console.WriteLine(
                        $"Logged in: username - {currentUser.Value.UserName}, full name - {currentUser.Value.FullName}");
                else
                {
                    if (File.Exists(stateFile))
                    {
                        Console.WriteLine($"Login Failed from file, deleting state file");
                        File.Delete(stateFile);
                        Console.WriteLine("State file deleted.");
                    }
                    if (!File.Exists(stateFile))
                    {
                        Console.WriteLine("Enter User Name .. ");
                        username = Console.ReadLine();
                        Console.WriteLine("Enter Password .. ");
                        password = Console.ReadLine();
                    }

                    // create user session data and provide login details
                    var userSessionNew = new UserSessionData
                    {
                        UserName = username,
                        Password = password
                    };


                    Login(userSessionNew);

                    var currentUser1 = await _instaApi.GetCurrentUserAsync();
                    if (currentUser1.Succeeded)
                        Console.WriteLine(
                            $"Logged in: username - {currentUser.Value.UserName}, full name - {currentUser.Value.FullName}");
                }

                bool isContinue = true;

                Console.WriteLine("Press 1 to Get User followers");
                Console.WriteLine("Press 2 to follow user using username");
                Console.WriteLine("Press 3 to Auto Like tag feed");
                Console.WriteLine("Press 4 to Auto comment tag feed");
                Console.WriteLine("Press 5 to Auto UnFollow multiple users in once.");
                Console.WriteLine("Press 6 to Upload images from other page.");
                Console.WriteLine("Press esc to exit.");

                while (isContinue)
                {
                    var samplesMap = new Dictionary<ConsoleKey, IAction>
                    {
                        [ConsoleKey.D1] = new GetUserFollowers(_instaApi),
                        [ConsoleKey.D2] = new FollowUser(_instaApi),
                        [ConsoleKey.D3] = new LikeTagFeed(_instaApi),
                        [ConsoleKey.D4] = new CommentTagFeed(_instaApi),
                        [ConsoleKey.D5] = new UnfollowUser(_instaApi),
                        [ConsoleKey.D6] = new UploadPicFromAnotherUserFeed(_instaApi),

                        // handling NumPad keys

                        [ConsoleKey.NumPad1] = new GetUserFollowers(_instaApi),
                        [ConsoleKey.NumPad2] = new FollowUser(_instaApi),
                        [ConsoleKey.NumPad3] = new LikeTagFeed(_instaApi),
                        [ConsoleKey.NumPad4] = new CommentTagFeed(_instaApi),
                        [ConsoleKey.NumPad5] = new UnfollowUser(_instaApi),
                        [ConsoleKey.NumPad6] = new UploadPicFromAnotherUserFeed(_instaApi),
                    };
                    var key = Console.ReadKey();
                    Console.WriteLine(Environment.NewLine);

                    if (samplesMap.ContainsKey(key.Key))
                        await samplesMap[key.Key].Do();

                    if (key.Key == ConsoleKey.Escape)
                    {
                        isContinue = false;
                    }
                    else
                    {
                        Console.WriteLine("Done.  Chosse another option.");
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                // perform that if user needs to logged out
                //var logoutResult = Task.Run(() => _instaApi.LogoutAsync()).GetAwaiter().GetResult();
                // if (logoutResult.Succeeded) Console.WriteLine("Logout succeed");
            }
            return false;
        }

        public static void Login(UserSessionData userSession)
        {
            var delay = RequestDelay.FromSeconds(2, 2);
            try
            {
                if (File.Exists(stateFile))
                {
                    Console.WriteLine("Loading state from file");
                    using (var fs = File.OpenRead(stateFile))
                    {
                        _instaApi.LoadStateDataFromStream(fs);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if (!_instaApi.IsUserAuthenticated)
            {
                // login
                Console.WriteLine($"Logging in as {userSession.UserName}");
                delay.Disable();
                var logInResult =  _instaApi.LoginAsync().Result;
                delay.Enable();
                if (!logInResult.Succeeded)
                {
                    if (logInResult.Value == InstaLoginResult.ChallengeRequired)
                    {
                        Console.WriteLine("Instagram want you to verify your account.");

                        var challenge = _instaApi.ChooseVerifyMethod(0).Result;

                        if (challenge.Succeeded)
                        {
                            if (challenge.Value.StepData != null)
                            {
                                var email = _instaApi.GetVerifyStep().Result;

                                if (email.Succeeded)
                                {
                                    Console.WriteLine("We have sent you a code on your regiesterd phone number.");
                                    Console.WriteLine("Enter the verification code here.. and Press Enter");
                                    var verficationCode = Console.ReadLine();
                                    verficationCode = verficationCode.Replace(" ", "");
                                    var regex = new Regex(@"^-*[0-9,\.]+$");
                                    if (!regex.IsMatch(verficationCode))
                                    {
                                        Console.WriteLine("Verification code is numeric!!!");
                                    }
                                    if (verficationCode.Length != 6)
                                    {
                                        Console.WriteLine($@"Verification code must be 6 digits!!!");
                                    }

                                    try
                                    {
                                        var verifyLogin = _instaApi.SendVerifyCode(verficationCode).Result;
                                        if (verifyLogin.Succeeded)
                                        {
                                            // you are logged in sucessfully.
                                            Console.WriteLine("Logged In successfully");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Log In failed. verifyLogin.Value : -" + verifyLogin.Value);
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                                else
                                {
                                    //sending code failed
                                    Console.WriteLine("sending code failed" + email.Value + email.Info);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine(challenge.Info + "challange method selection failed.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Unable to login: {logInResult.Info.Message}");
                    }
                }
            }

            var state = _instaApi.GetStateDataAsStream();
            using (var fileStream = File.Create(stateFile))
            {
                state.Seek(0, SeekOrigin.Begin);
                state.CopyTo(fileStream);
            }

        }
    }


}

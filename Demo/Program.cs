﻿using Demo.HttpClients;
using Demo.HttpServices;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Parameterables;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Init();
            RequestAsync().Wait();
            Console.ReadLine();
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        static void Init()
        {
            HttpServer.Start(9999);

            //  注册IUserApi 配置其工厂
            HttpApi.Register<IUserApi>().ConfigureHttpApiConfig(c =>
            {
                c.HttpHost = new Uri("http://localhost:9999/");
                c.FormatOptions.DateTimeFormat = DateTimeFormats.ISO8601_WithMillisecond;
            });
        }

        /// <summary>
        /// 请求接口
        /// </summary>
        /// <returns></returns>
        private static async Task RequestAsync()
        {
            var userApi = HttpApi.Resolve<IUserApi>();

            var user = new UserInfo
            {
                Account = "laojiu",
                Password = "123456",
                BirthDay = DateTime.Parse("2018-01-01 12:30:30"),
                Email = "laojiu@webapiclient.com",
                Gender = Gender.Male
            };

            var about = await userApi
                .GetAboutAsync("webapi/user/about", user, "somevalue");

            var aboutCache = await userApi
                .GetAboutAsync("webapi/user/about", user, "somevalue");

            var user1 = await userApi
                .GetByIdAsync("id001", CancellationToken.None);

            var user2 = await userApi
                .GetByAccountAsync("laojiu", CancellationToken.None);

            var user3 = await userApi
                .UpdateWithFormAsync(user, nickName: "老九", age: 18)
                .Retry(3, i => TimeSpan.FromSeconds(i))
                .WhenCatch<HttpStatusFailureException>();

            var user4 = await userApi
                .UpdateWithJsonAsync(user);

            var user5 = await userApi
                .UpdateWithXmlAsync(user).HandleAsDefaultWhenException();

            var file = new MulitpartFile("file.data");
            file.UploadProgressChanged += (s, e) =>
            {
                // Console.WriteLine(e.Progress);
            };

            var user6 = await userApi
                .UpdateWithMulitpartAsync(user, "老九", 18, file);

        }
    }
}

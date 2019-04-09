//using Amazon.DynamoDBv2.DataModel;
//using Common.Dynamo.Contracts;
//using Common.Dynamo.Models;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;

//namespace RecipeBookApi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class DataImportController : ControllerBase
//    {
//        private readonly IDynamoDBContext _context;
//        private readonly IDynamoStorageRepository<AppUser> _appUserStorage;

//        public DataImportController(IDynamoDBContext context, IDynamoStorageRepository<AppUser> appUserStorage)
//        {
//            _context = context;
//            _appUserStorage = appUserStorage;
//        }

//        [HttpPost]
//        [Route("[action]")]
//        public async Task<IActionResult> ImportDataFromOldSite()
//        {
//            throw new Exception("Not allowed to do this right now.");

//            var httpClient = new HttpClient();
//            var response = await httpClient.GetAsync("http://aladd04.com/services/api/recipe/");
//            var content = await response.Content.ReadAsStringAsync();

//            var oldData = content.JsonToObject<List<OldRecipeModel>>();
//            var appUsers = await _appUserStorage.ReadAll();

//            var whoMadeIt = oldData
//                .Join(appUsers, r => r.OwnerName, u => u.FullName, (r, u) => new { RecipeId = r.Id, UserId = u.Id })
//                .ToDictionary((x) => x.RecipeId, (x) => x.UserId);

//            var whenWasItMade = GetWhenItWasCreated();
//            var whenWasItUpdated = GetWhenItWasUpdated();

//            var newData = new List<Recipe>();
//            foreach (var recipe in oldData)
//            {
//                newData.Add(new Recipe
//                {
//                    Id = Guid.NewGuid().ToString(),
//                    Description = recipe.Description,
//                    Ingredients = recipe.Ingredients,
//                    Instructions = recipe.Instructions,
//                    Name = recipe.Name,
//                    CreatedById = whoMadeIt[recipe.Id],
//                    UpdatedById = whoMadeIt[recipe.Id],
//                    CreateDate = whenWasItMade[recipe.Id],
//                    UpdateDate = whenWasItUpdated[recipe.Id]
//                });
//            }

//            var tonsOfTasks = new List<Task>();
//            foreach (var recipe in newData)
//            {
//                tonsOfTasks.Add(_context.SaveAsync(recipe));
//            }

//            await Task.WhenAll(tonsOfTasks);

//            return Ok();
//        }

//        [HttpPost]
//        [Route("[action]")]
//        public async Task<IActionResult> CreateUsers()
//        {
//            throw new Exception("Not allowed to do this right now.");

//            await _appUserStorage.Create(new AppUser
//            {
//                EmailAddress = "system@katiemaeskitchen.com",
//                FirstName = "System",
//                LastName = "System"
//            }, null);

//            await _appUserStorage.Create(new AppUser
//            {
//                EmailAddress = "kathleen.m.wells92@gmail.com",
//                FirstName = "Kathleen",
//                LastName = "Ladd"
//            }, null);

//            await _appUserStorage.Create(new AppUser
//            {
//                EmailAddress = "aladd04@gmail.com",
//                FirstName = "Andy",
//                LastName = "Ladd"
//            }, null);

//            await _appUserStorage.Create(new AppUser
//            {
//                EmailAddress = "shantom2k@gmail.com",
//                FirstName = "Shannon",
//                LastName = "Wells"
//            }, null);

//            return Ok();
//        }

//        private static Dictionary<int, DateTime> GetWhenItWasCreated()
//        {
//            return new Dictionary<int, DateTime>
//            {
//                { 141, Convert.ToDateTime("2015-09-06 23:32:53.3316833") },
//                { 142, Convert.ToDateTime("2015-09-06 23:39:52.3768392") },
//                { 143, Convert.ToDateTime("2015-09-06 23:49:11.0811824") },
//                { 144, Convert.ToDateTime("2015-09-08 01:46:26.1615111") },
//                { 145, Convert.ToDateTime("2015-09-08 01:50:49.6391572") },
//                { 146, Convert.ToDateTime("2015-09-08 01:58:02.2690793") },
//                { 147, Convert.ToDateTime("2015-09-08 02:02:57.1595570") },
//                { 148, Convert.ToDateTime("2015-09-08 02:06:55.5571594") },
//                { 149, Convert.ToDateTime("2015-09-08 02:11:04.3216735") },
//                { 150, Convert.ToDateTime("2015-09-08 02:16:48.7201522") },
//                { 151, Convert.ToDateTime("2015-09-08 02:25:23.3657069") },
//                { 152, Convert.ToDateTime("2015-09-08 02:29:31.9286729") },
//                { 153, Convert.ToDateTime("2015-09-08 02:32:49.8902640") },
//                { 154, Convert.ToDateTime("2015-09-08 02:39:08.8282808") },
//                { 155, Convert.ToDateTime("2015-09-08 02:44:43.0287994") },
//                { 156, Convert.ToDateTime("2015-09-08 02:47:36.8527189") },
//                { 157, Convert.ToDateTime("2015-09-08 02:56:33.7938667") },
//                { 158, Convert.ToDateTime("2015-09-08 13:07:12.4192459") },
//                { 159, Convert.ToDateTime("2015-09-10 12:18:26.8252245") },
//                { 160, Convert.ToDateTime("2015-09-10 12:27:52.7310663") },
//                { 161, Convert.ToDateTime("2015-09-10 12:33:06.0613213") },
//                { 162, Convert.ToDateTime("2015-09-10 12:40:32.5888779") },
//                { 163, Convert.ToDateTime("2015-09-10 12:49:48.2578133") },
//                { 164, Convert.ToDateTime("2015-09-10 12:58:03.3519233") },
//                { 165, Convert.ToDateTime("2015-09-10 13:03:22.9195504") },
//                { 166, Convert.ToDateTime("2015-09-10 13:10:07.6818025") },
//                { 167, Convert.ToDateTime("2015-09-10 13:15:22.0750629") },
//                { 168, Convert.ToDateTime("2015-09-10 13:18:24.3712697") },
//                { 169, Convert.ToDateTime("2015-09-27 21:21:15.7477825") },
//                { 170, Convert.ToDateTime("2015-09-27 21:31:03.5944580") },
//                { 171, Convert.ToDateTime("2015-09-27 21:37:12.4115173") },
//                { 172, Convert.ToDateTime("2015-09-27 21:42:42.5558395") },
//                { 173, Convert.ToDateTime("2015-09-27 22:09:22.4157961") },
//                { 174, Convert.ToDateTime("2015-09-27 22:17:04.7328568") },
//                { 175, Convert.ToDateTime("2015-09-27 22:23:40.6957166") },
//                { 176, Convert.ToDateTime("2015-09-27 22:28:45.3772132") },
//                { 177, Convert.ToDateTime("2015-09-27 22:34:01.7934142") },
//                { 178, Convert.ToDateTime("2015-10-30 16:07:36.7442850") },
//                { 179, Convert.ToDateTime("2015-11-11 16:25:23.4452115") },
//                { 180, Convert.ToDateTime("2015-12-20 16:42:11.6776374") },
//                { 181, Convert.ToDateTime("2015-12-20 17:34:07.4014962") },
//                { 182, Convert.ToDateTime("2015-12-20 19:20:28.0650511") },
//                { 183, Convert.ToDateTime("2016-03-21 18:25:17.1491396") },
//                { 184, Convert.ToDateTime("2016-04-04 12:02:34.3744565") },
//                { 185, Convert.ToDateTime("2016-04-10 12:43:50.0292840") },
//                { 186, Convert.ToDateTime("2016-04-10 15:06:29.4603881") },
//                { 187, Convert.ToDateTime("2016-05-04 12:27:08.4780432") },
//                { 188, Convert.ToDateTime("2016-05-04 12:54:27.2288075") },
//                { 189, Convert.ToDateTime("2016-05-04 13:04:17.3405946") },
//                { 190, Convert.ToDateTime("2016-05-14 09:14:58.8038932") },
//                { 191, Convert.ToDateTime("2016-05-29 18:01:26.1803818") },
//                { 192, Convert.ToDateTime("2016-11-15 16:39:45.4551838") },
//                { 193, Convert.ToDateTime("2016-11-15 16:42:53.5966169") },
//                { 194, Convert.ToDateTime("2016-11-15 16:46:31.9986650") },
//                { 195, Convert.ToDateTime("2016-11-15 16:49:58.3330973") },
//                { 196, Convert.ToDateTime("2016-11-15 16:53:38.3889447") },
//                { 197, Convert.ToDateTime("2016-11-15 17:02:18.1702773") },
//                { 199, Convert.ToDateTime("2016-11-15 17:11:42.2273664") },
//                { 200, Convert.ToDateTime("2016-11-15 17:16:38.5651566") },
//                { 201, Convert.ToDateTime("2016-11-15 17:18:49.1660333") },
//                { 202, Convert.ToDateTime("2016-11-21 16:13:16.6668299") },
//                { 203, Convert.ToDateTime("2016-12-26 14:18:36.0166972") },
//                { 204, Convert.ToDateTime("2017-01-23 21:47:10.2790313") },
//                { 205, Convert.ToDateTime("2017-02-04 21:35:19.9954980") },
//                { 206, Convert.ToDateTime("2017-03-29 16:51:56.5152800") },
//                { 207, Convert.ToDateTime("2017-07-09 15:14:27.8705268") },
//                { 208, Convert.ToDateTime("2017-07-11 13:42:22.3154762") },
//                { 209, Convert.ToDateTime("2017-08-14 17:17:55.3567940") },
//                { 210, Convert.ToDateTime("2017-10-13 10:04:23.0717749") },
//                { 211, Convert.ToDateTime("2017-11-07 11:12:52.4289125") },
//                { 212, Convert.ToDateTime("2017-11-07 11:20:05.0275282") },
//                { 213, Convert.ToDateTime("2017-11-07 11:27:55.6217482") },
//                { 214, Convert.ToDateTime("2017-11-07 11:35:07.1582366") },
//                { 215, Convert.ToDateTime("2017-11-07 11:39:09.1911723") },
//                { 216, Convert.ToDateTime("2017-11-07 11:47:30.7048536") },
//                { 217, Convert.ToDateTime("2018-03-21 12:44:16.1395825") },
//                { 218, Convert.ToDateTime("2018-03-21 12:53:00.3239767") },
//                { 219, Convert.ToDateTime("2018-03-21 12:58:03.6359304") },
//                { 220, Convert.ToDateTime("2018-03-21 13:06:53.3513693") },
//                { 221, Convert.ToDateTime("2018-03-21 13:13:09.9418342") },
//                { 222, Convert.ToDateTime("2018-03-21 13:24:09.3520746") },
//                { 224, Convert.ToDateTime("2018-03-21 13:39:38.5673101") },
//                { 225, Convert.ToDateTime("2018-03-21 13:43:13.6741235") },
//                { 226, Convert.ToDateTime("2018-11-15 15:10:31.8166584") }
//            };
//        }

//        private static Dictionary<int, DateTime> GetWhenItWasUpdated()
//        {
//            return new Dictionary<int, DateTime>
//            {
//                { 141, Convert.ToDateTime("2017-07-11 00:00:12.9654950") },
//                { 142, Convert.ToDateTime("2015-09-06 23:39:52.3768392") },
//                { 143, Convert.ToDateTime("2015-09-06 23:58:42.5916017") },
//                { 144, Convert.ToDateTime("2015-09-08 01:46:26.1615111") },
//                { 145, Convert.ToDateTime("2015-09-08 01:50:49.6391572") },
//                { 146, Convert.ToDateTime("2015-09-10 13:22:20.3617030") },
//                { 147, Convert.ToDateTime("2015-09-08 02:02:57.1595570") },
//                { 148, Convert.ToDateTime("2015-09-08 02:06:55.5571594") },
//                { 149, Convert.ToDateTime("2015-09-08 02:11:04.3216735") },
//                { 150, Convert.ToDateTime("2015-09-08 02:17:11.7199533") },
//                { 151, Convert.ToDateTime("2015-09-08 02:25:23.3657069") },
//                { 152, Convert.ToDateTime("2015-09-08 02:30:04.3029459") },
//                { 153, Convert.ToDateTime("2017-11-07 10:28:36.6503417") },
//                { 154, Convert.ToDateTime("2015-09-08 02:39:08.8282808") },
//                { 155, Convert.ToDateTime("2015-09-08 02:44:43.0287994") },
//                { 156, Convert.ToDateTime("2015-09-08 02:47:36.8527189") },
//                { 157, Convert.ToDateTime("2016-05-04 12:41:51.4649811") },
//                { 158, Convert.ToDateTime("2015-09-08 13:07:45.8032503") },
//                { 159, Convert.ToDateTime("2015-09-10 12:18:38.2577873") },
//                { 160, Convert.ToDateTime("2015-09-10 12:30:05.6631480") },
//                { 161, Convert.ToDateTime("2015-09-10 12:33:06.0613213") },
//                { 162, Convert.ToDateTime("2015-09-10 12:40:32.5888779") },
//                { 163, Convert.ToDateTime("2015-09-10 12:50:16.3513481") },
//                { 164, Convert.ToDateTime("2015-09-10 12:58:26.2970808") },
//                { 165, Convert.ToDateTime("2016-05-04 13:06:51.9724866") },
//                { 166, Convert.ToDateTime("2015-09-10 13:10:54.3428048") },
//                { 167, Convert.ToDateTime("2015-09-10 13:15:22.0750629") },
//                { 168, Convert.ToDateTime("2015-09-10 13:18:59.9414547") },
//                { 169, Convert.ToDateTime("2016-06-06 18:41:48.2814913") },
//                { 170, Convert.ToDateTime("2015-09-27 21:31:03.5944580") },
//                { 171, Convert.ToDateTime("2015-09-27 21:37:12.4115173") },
//                { 172, Convert.ToDateTime("2015-09-27 21:42:42.5558395") },
//                { 173, Convert.ToDateTime("2015-09-27 22:09:22.4157961") },
//                { 174, Convert.ToDateTime("2015-09-27 22:19:20.0831805") },
//                { 175, Convert.ToDateTime("2015-09-27 22:23:40.6957166") },
//                { 176, Convert.ToDateTime("2015-09-27 22:28:45.3772132") },
//                { 177, Convert.ToDateTime("2016-06-05 17:59:10.9624915") },
//                { 178, Convert.ToDateTime("2018-10-03 16:49:05.5991896") },
//                { 179, Convert.ToDateTime("2015-11-11 17:20:24.2837651") },
//                { 180, Convert.ToDateTime("2015-12-20 16:42:11.6776374") },
//                { 181, Convert.ToDateTime("2015-12-20 17:34:07.4014962") },
//                { 182, Convert.ToDateTime("2015-12-20 19:21:17.2243782") },
//                { 183, Convert.ToDateTime("2017-01-23 21:48:22.2720437") },
//                { 184, Convert.ToDateTime("2016-04-04 12:26:48.0861746") },
//                { 185, Convert.ToDateTime("2016-04-10 12:45:00.9161531") },
//                { 186, Convert.ToDateTime("2016-04-10 15:06:29.4603881") },
//                { 187, Convert.ToDateTime("2016-05-04 12:42:18.5589103") },
//                { 188, Convert.ToDateTime("2016-05-04 13:05:02.9053686") },
//                { 189, Convert.ToDateTime("2016-05-04 13:04:17.3405946") },
//                { 190, Convert.ToDateTime("2016-05-14 09:30:29.1672930") },
//                { 191, Convert.ToDateTime("2018-06-11 15:02:09.2032861") },
//                { 192, Convert.ToDateTime("2016-11-15 16:39:45.4551838") },
//                { 193, Convert.ToDateTime("2016-11-15 16:42:53.5966169") },
//                { 194, Convert.ToDateTime("2016-11-15 17:20:36.2965255") },
//                { 195, Convert.ToDateTime("2016-11-15 16:49:58.3330973") },
//                { 196, Convert.ToDateTime("2016-11-15 16:53:38.3889447") },
//                { 197, Convert.ToDateTime("2018-12-14 17:01:33.7737896") },
//                { 199, Convert.ToDateTime("2016-11-15 17:11:42.2273664") },
//                { 200, Convert.ToDateTime("2016-11-15 17:16:38.5651566") },
//                { 201, Convert.ToDateTime("2016-11-15 17:18:49.1660333") },
//                { 202, Convert.ToDateTime("2016-11-21 16:13:16.6668299") },
//                { 203, Convert.ToDateTime("2016-12-26 14:18:36.0166972") },
//                { 204, Convert.ToDateTime("2017-01-23 21:47:10.2790313") },
//                { 205, Convert.ToDateTime("2017-02-09 12:17:41.9717746") },
//                { 206, Convert.ToDateTime("2017-03-29 16:51:56.5152800") },
//                { 207, Convert.ToDateTime("2018-10-03 16:47:32.6328361") },
//                { 208, Convert.ToDateTime("2017-07-11 13:42:22.3154762") },
//                { 209, Convert.ToDateTime("2017-08-14 17:17:55.3567940") },
//                { 210, Convert.ToDateTime("2017-10-13 10:04:23.0717749") },
//                { 211, Convert.ToDateTime("2017-11-07 11:12:52.4289125") },
//                { 212, Convert.ToDateTime("2017-11-07 11:20:05.0275282") },
//                { 213, Convert.ToDateTime("2017-11-07 11:27:55.6217482") },
//                { 214, Convert.ToDateTime("2017-11-07 11:35:07.1582366") },
//                { 215, Convert.ToDateTime("2017-11-07 11:39:40.5495481") },
//                { 216, Convert.ToDateTime("2017-11-07 11:47:30.7048536") },
//                { 217, Convert.ToDateTime("2018-03-21 12:44:16.1395825") },
//                { 218, Convert.ToDateTime("2018-03-21 12:53:00.3239767") },
//                { 219, Convert.ToDateTime("2018-03-21 12:58:03.6359304") },
//                { 220, Convert.ToDateTime("2018-03-21 13:06:53.3513693") },
//                { 221, Convert.ToDateTime("2018-03-21 13:13:09.9418342") },
//                { 222, Convert.ToDateTime("2018-03-21 13:24:09.3520746") },
//                { 224, Convert.ToDateTime("2018-03-21 13:40:22.9414691") },
//                { 225, Convert.ToDateTime("2018-03-21 13:43:13.6741235") },
//                { 226, Convert.ToDateTime("2018-11-15 15:10:31.8166584") }
//            };
//        }
//    }

//    internal class OldRecipeModel
//    {
//        public int Id { set; get; }

//        public string Name { set; get; }

//        public string Description { set; get; }

//        public string Ingredients { set; get; }

//        public string Instructions { set; get; }

//        public string OwnerName { set; get; }

//        public DateTime UpdateDate { set; get; }
//    }
//}
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PlayFab.Samples;



using PlayFab.ServerModels;
using PlayFab.Json;
using System.Collections.Generic;
using PlayFab.DataModels;
using System.Net.Http;
using System.Net.Http.Headers;



namespace Company.Function
{
    public static class HttpTrigger1
    {
        [FunctionName("HttpTrigger1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // log.LogInformation("C# HTTP trigger function processed a request.");



            //  string player1_playfabID = req.Query["player1_playfabID"];
            //  string player2_playfabID = req.Query["player2_playfabID"];

            //  log.LogInformation(player1_playfabID + " and " + player2_playfabID);

            //  string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //  dynamic data = JsonConvert.DeserializeObject(requestBody);
            //  string test_player1_playfabID = player1_playfabID ?? data?.player1_playfabID;

            //  log.LogInformation(test_player1_playfabID + " test_player1_playfabID ");




            // string responseMessage = string.IsNullOrEmpty(name)
            //     ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //     : $"Hello, {name}. This HTTP triggered function executed successfully.";


            // return new OkObjectResult(responseMessage);

            PlayFab.PlayFabSettings.staticSettings.TitleId = "80664";          
            PlayFab.PlayFabSettings.staticSettings.DeveloperSecretKey = "QXYHDFCZP16WESAE5YRE54IW4ECQB5HDETX863G4491141RPQY";

             FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());

             dynamic args = context.FunctionArgument;

             var message = $"Hello {context.CallerEntityProfile.Lineage.MasterPlayerAccountId}!";
             log.LogInformation(message);



            string player1_playfabID = null;
            if (args != null && args["player1_playfabID"] != null)
            {
                player1_playfabID = args["player1_playfabID"];
            }
            string player2_playfabID = null;
            if (args != null && args["player2_playfabID"] != null)
            {
                player2_playfabID = args["player2_playfabID"];
            }
              log.LogInformation(player1_playfabID + " and " + player2_playfabID);


            //PlayFab.PlayFabClientAPI.GetTitleDataAsync.GetSharedGroupDataAsync()
            //var getRequest = PlayFab.PlayFabServerAPI.GetSharedGroupDataAsync()




            // var userInternalDataRequest = new GetUserDataRequest
            // {
            //     PlayFabId = context.CallerEntityProfile.Lineage.MasterPlayerAccountId,
            //     Keys = new List<string>
            //     {
            //         "Ranking",
            //         "Picture"
                    
            //     }
            // };
            // var playerDataResponse = await PlayFab.PlayFabServerAPI.GetUserDataAsync(userInternalDataRequest);
            // var playerData = playerDataResponse.Result.Data;

            // message+= " "+playerData["Ranking"].Value+ " "+ playerData["Picture"].Value;
            //  log.LogInformation(message);










            var userInternalDataRequest = new AddSharedGroupMembersRequest
            {
                SharedGroupId = player1_playfabID,
                PlayFabIds = new List<string>(){player2_playfabID}
            };
            var playerDataResponse = await PlayFab.PlayFabServerAPI.AddSharedGroupMembersAsync(userInternalDataRequest);

          log.LogDebug(playerDataResponse.Result.ToString());

            if( playerDataResponse.Error != null)
                log.LogDebug(playerDataResponse.Error.ErrorMessage);
            else
            {
                log.LogDebug("Added user :" +player2_playfabID + " to shared group: "+ player1_playfabID);
            }




            // dynamic inputValue = null;
            // if (args != null && args["inputValue"] != null)
            // {
            //     inputValue = args["inputValue"];
            // }

            // log.LogDebug($"HelloWorld: {new { input = inputValue} }");

            // return new { messageValue = message };
        
            return new OkObjectResult(message);
        }
        
    }
}




namespace PlayFab.Samples
{
    using System;
    using System.Collections.Generic;

    // Shared models
    public class TitleAuthenticationContext
    {
        public string Id { get; set; }
        public string EntityToken { get; set; }
    }

    // Models  via ExecuteFunction API
    public class FunctionExecutionContext<T>
    {
        public PlayFab.ProfilesModels.EntityProfileBody CallerEntityProfile { get; set; }
        public TitleAuthenticationContext TitleAuthenticationContext { get; set; }
        public bool? GeneratePlayStreamEvent { get; set; }
        public T FunctionArgument { get; set; }
    }

    public class FunctionExecutionContext : FunctionExecutionContext<object>
    {
    }

    // Models via Player PlayStream event, entering or leaving a 
    // player segment or as part of a player segment based scheduled task.
    public class PlayerPlayStreamFunctionExecutionContext<T>
    {
        public PlayFab.CloudScriptModels.PlayerProfileModel PlayerProfile { get; set; }
        public bool PlayerProfileTruncated { get; set; }
        public PlayFab.CloudScriptModels.PlayStreamEventEnvelopeModel PlayStreamEventEnvelope { get; set; }
        public TitleAuthenticationContext TitleAuthenticationContext { get; set; }
        public bool? GeneratePlayStreamEvent { get; set; }
        public T FunctionArgument { get; set; }
    }

    public class PlayerPlayStreamFunctionExecutionContext : PlayerPlayStreamFunctionExecutionContext<object>
    {
    }

    // Models via Scheduled task
    public class PlayStreamEventHistory
    {
        public string ParentTriggerId { get; set; }
        public string ParentEventId { get; set; }
        public bool TriggeredEvents { get; set; }
    }

    public class ScheduledTaskFunctionExecutionContext<T>
    {
        public PlayFab.CloudScriptModels.NameIdentifier ScheduledTaskNameId { get; set; }
        public Stack<PlayStreamEventHistory> EventHistory { get; set; }
        public TitleAuthenticationContext TitleAuthenticationContext { get; set; }
        public bool? GeneratePlayStreamEvent { get; set; }
        public T FunctionArgument { get; set; }
    }

    public class ScheduledTaskFunctionExecutionContext : ScheduledTaskFunctionExecutionContext<object>
    {
    }

    // Models via entity PlayStream event, entering or leaving an 
    // entity segment or as part of an entity segment based scheduled task.
    public class EventFullName
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
    }

    public class OriginInfo
    {
        public string Id { get; set; }
        public DateTime? Timestamp { get; set; }
    }

    public class EntityPlayStreamEvent<T>
    {
        public string SchemaVersion { get; set; }
        public EventFullName FullName { get; set; }
        public string Id { get; set; }
        public DateTime Timestamp { get; set; }
        public PlayFab.CloudScriptModels.EntityKey Entity { get; set; }
        public PlayFab.CloudScriptModels.EntityKey Originator { get; set; }
        public OriginInfo OriginInfo { get; set; }
        public T Payload { get; set; }
        public PlayFab.ProfilesModels.EntityLineage EntityLineage { get; set; }
    }

    public class EntityPlayStreamEvent : EntityPlayStreamEvent<object>
    {
    }

    public class EntityPlayStreamFunctionExecutionContext<TPayload, TArg>
    {
        public PlayFab.ProfilesModels.EntityProfileBody CallerEntityProfile { get; set; }
        public EntityPlayStreamEvent<TPayload> PlayStreamEvent { get; set; }
        public TitleAuthenticationContext TitleAuthenticationContext { get; set; }
        public bool? GeneratePlayStreamEvent { get; set; }
        public TArg FunctionArgument { get; set; }
    }

    public class EntityPlayStreamFunctionExecutionContext : EntityPlayStreamFunctionExecutionContext<object, object>
    {
    }
}

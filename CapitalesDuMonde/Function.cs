using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace CapitalesDuMonde
{
    public class Function
    {
        //Constantes 
        private const string BUCKET_NAME = "listepaysfr";
        private const string KEY_NAME = "monde.json";
        private const string SESSION_STATE_KEY = "skill-state";

        private static readonly RegionEndpoint BucketRegion = RegionEndpoint.EUWest1;
        public static IAmazonS3 S3Client;
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            S3Client = new AmazonS3Client(BucketRegion);
            string source;
            //Construct response
            var response = new SkillResponse
            {
                Response = new ResponseBody()
            };
            response.Response.ShouldEndSession = false;
            IOutputSpeech innerResponse = null;
            string reponse = "";

            //Récupérer la session
            var playerSession = input.Session;
            int playerScore = RestoreState(playerSession);

            //Récupérer le fichier xml 
            source = await GetS3File(S3Client);
            Monde monde = new Monde();
            if (source != null)
            {
                LambdaLogger.Log("source : "+source.ToString());
                monde = JsonConvert.DeserializeObject<Rootobject>(source).monde;
            }
            else
                LambdaLogger.Log("memory is null");


            // Traiter la demande du joueur
            if (input.Request.GetType() == typeof(LaunchRequest))
            {
                LambdaLogger.Log("Skill Request Type : Lunch Request");

                reponse = "Bienvenue dans Capitale du monde, je vais vous dire un nom de pays et vous devez trouver la capitale.";
                // Si c'est un premier lancement sans fichier de sauvegarde
                //if (state.Status == AdventureStatus.New)
                // {
                //   state.Status = AdventureStatus.InProgress;
                //}
            }
            else if (input.Request.GetType() == typeof(IntentRequest))
            {
                Random rnd = new Random();
                int index = rnd.Next(100);
                string question = "C'est quoi la capitale de : ";
                var inputRequest = (IntentRequest)input.Request;
                LambdaLogger.Log($"Skill Request Type : IntentRequest {inputRequest.Intent.Name}");

                else
                {
                    switch (inputRequest.Intent.Name)
                    {
                        case "RepeatQuestion":
                            reponse = question;
                            break;

                        case "ProposeChocies":
                            reponse = "choix";
                            break;

                        case "GetHint":
                            reponse = "Elle commence par 'A'";
                            break;

                        case "AMAZON.FallbackIntent":
                            reponse = "Can't understant your request";
                            break;
                    }
                }
            }

            innerResponse = new SsmlOutputSpeech();
            ((SsmlOutputSpeech)innerResponse).Ssml = $"<speak>{}</speak>";
            response.Version = "1.0";
            response.Response.OutputSpeech = innerResponse;
            response.SessionAttributes = new Dictionary<string, object> { [SESSION_STATE_KEY] = playerScore };

            return response;
        }

        /// <summary>
        /// Récupérer le score stoquer dans la sessions
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public int RestoreState(Session session)
        {
            int score = 0;
            if (session != null && session.New)
            {
                LambdaLogger.Log("New Session");
            }
            else
            {
                LambdaLogger.Log("Exesting Session");
                //Récupérer l'objet stocké en session
                var playerStateValue = session.Attributes.GetValueOrDefault(SESSION_STATE_KEY) ?? null;
                try
                {
                    score = (int)playerStateValue;
                }
                catch (Exception)
                {
                    LambdaLogger.Log("Converstion Exception");
                    return 0;
                }
            }
            return score;

        }

        /// <summary>
        /// récuperer le fichier stocker dans S3
        /// </summary>
        /// <param name="S3Client"></param>
        public async Task<string> GetS3File(IAmazonS3 S3Client)
        {
            string source;
            var request = new GetObjectRequest
            {
                BucketName = BUCKET_NAME,
                Key = KEY_NAME
            }; 

            try
            {
                using (var s3Response = await S3Client.GetObjectAsync(request))
                {
                    var memory = new MemoryStream();
                    await s3Response.ResponseStream.CopyToAsync(memory);
                    source = Encoding.UTF8.GetString(memory.ToArray());
                    return source;
                }
            }
            catch (AmazonS3Exception e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                LambdaLogger.Log($"Error encountered ***. Message:'{0}' when writing an object {e.Message}");
                throw new Exception("unable to load file from s3");
            }
            catch (Exception e)
            {
                LambdaLogger.Log($"Unknown encountered on server. Message:'{0}' when writing an object {e.Message}");
                throw new Exception("Unknown encountered on server");
            }

        }

        public T ConvertJSonToObject<T>(string jsonString)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)serializer.ReadObject(ms);
            return obj;
        }

    }

}

